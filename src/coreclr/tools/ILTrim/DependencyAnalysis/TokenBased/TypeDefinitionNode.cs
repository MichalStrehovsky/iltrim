﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents an entry in the TypeDef metadata table.
    /// </summary>
    public sealed class TypeDefinitionNode : TokenBasedNode
    {
        public TypeDefinitionNode(EcmaModule module, TypeDefinitionHandle handle)
            : base(module, handle)
        {
        }

        private TypeDefinitionHandle Handle => (TypeDefinitionHandle)_handle;

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            yield return new DependencyListEntry(factory.ModuleDefinition(_module), "Owning module");

            TypeDefinition typeDef = _module.MetadataReader.GetTypeDefinition(Handle);
            if (!typeDef.BaseType.IsNil)
            {
                yield return new(factory.GetNodeForToken(_module, typeDef.BaseType), "Base type of a type");
            }

            if (!typeDef.GetDeclaringType().IsNil)
            {
                yield return new DependencyListEntry(factory.TypeDefinition(_module, typeDef.GetDeclaringType()), "Declaring type of a type");
            }
        }

        protected override EntityHandle WriteInternal(ModuleWritingContext writeContext)
        {
            MetadataReader reader = _module.MetadataReader;
            TypeDefinition typeDef = reader.GetTypeDefinition(Handle);

            var builder = writeContext.MetadataBuilder;

            if (!typeDef.GetDeclaringType().IsNil)
                builder.AddNestedType(Handle, typeDef.GetDeclaringType());

            return builder.AddTypeDefinition(typeDef.Attributes,
                builder.GetOrAddString(reader.GetString(typeDef.Namespace)),
                builder.GetOrAddString(reader.GetString(typeDef.Name)),
                (TypeDefinitionHandle)writeContext.TokenMap.MapToken(typeDef.BaseType),
                writeContext.TokenMap.MapTypeFieldList(Handle),
                writeContext.TokenMap.MapTypeMethodList(Handle));
        }

        public override string ToString()
        {
            MetadataReader reader = _module.MetadataReader;
            return reader.GetString(reader.GetTypeDefinition(Handle).Name);
        }
    }
}
