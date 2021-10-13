﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents a row in the Field table.
    /// </summary>
    public sealed class FieldDefinitionNode : TokenBasedNode
    {
        public FieldDefinitionNode(EcmaModule module, FieldDefinitionHandle handle)
            : base(module, handle)
        {
        }

        private FieldDefinitionHandle Handle => (FieldDefinitionHandle)_handle;

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            FieldDefinition fieldDef = _module.MetadataReader.GetFieldDefinition(Handle);
            TypeDefinitionHandle declaringType = fieldDef.GetDeclaringType();

            // TODO: Check if FieldDefinition has other references that needed to be added
            yield return new DependencyListEntry(factory.TypeDefinition(_module, declaringType), "Field owning type");

            if((fieldDef.Attributes & FieldAttributes.Literal) == FieldAttributes.Literal)
            {
                yield return new DependencyListEntry(factory.GetNodeForToken(_module, fieldDef.GetDefaultValue()), "Constant in field definition");
            }

            foreach (CustomAttributeHandle customAttribute in fieldDef.GetCustomAttributes())
            {
                yield return new(factory.CustomAttribute(_module, customAttribute), "Custom attribute of a field");
            }

        }

        protected override EntityHandle WriteInternal(ModuleWritingContext writeContext)
        {
            MetadataReader reader = _module.MetadataReader;

            FieldDefinition fieldDef = reader.GetFieldDefinition(Handle);

            var builder = writeContext.MetadataBuilder;

            // TODO: the signature blob might contain references to tokens we need to rewrite
            var signatureBlob = reader.GetBlobBytes(fieldDef.Signature);

            if ((fieldDef.Attributes & FieldAttributes.HasFieldRVA) == FieldAttributes.HasFieldRVA)
            {
                WriteMagicField(fieldDef, writeContext);
            }

            return builder.AddFieldDefinition(
                fieldDef.Attributes,
                builder.GetOrAddString(reader.GetString(fieldDef.Name)),
                builder.GetOrAddBlob(signatureBlob));
        }

        unsafe internal void WriteMagicField(FieldDefinition fieldDef, ModuleWritingContext writeContext)
        {
            MetadataReader reader = _module.MetadataReader;

            int rva = fieldDef.GetRelativeVirtualAddress();
            if (rva != 0)
            {
                var rvaBlobReader = _module.PEReader.GetSectionData(rva).Pointer;
                var classLayoutSize = reader.GetTypeDefinition((TypeDefinitionHandle)writeContext.TokenMap.MapToken(fieldDef.GetDeclaringType())).GetLayout().Size;
                BlobBuilder outputBodyBuilder = writeContext.fieldBlobBuilder;
                outputBodyBuilder.WriteBytes(rvaBlobReader, classLayoutSize);
            }
        } 

        public override string ToString()
        {
            // TODO: would be nice to have a common formatter we can call into that also includes owning type
            MetadataReader reader = _module.MetadataReader;
            return reader.GetString(reader.GetFieldDefinition(Handle).Name);
        }
    }
}
