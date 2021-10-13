// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Internal.TypeSystem.Ecma;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents a row in the Property table.
    /// </summary>
    public sealed class PropertyDefinitionNode : TokenBasedNode
    {
        public PropertyDefinitionNode(EcmaModule module, PropertyDefinitionHandle handle)
            : base(module, handle)
        {
        }

        private PropertyDefinitionHandle Handle => (PropertyDefinitionHandle)_handle;

        // TODO: this could be done when reflection-marking a type for better performance.
        TypeDefinitionHandle GetDeclaringType()
        {
            MetadataReader reader = _module.MetadataReader;

            foreach (var typeHandle in reader.TypeDefinitions) {
                TypeDefinition typeDef = reader.GetTypeDefinition(typeHandle);
                foreach (var propertyHandle in typeDef.GetProperties()) {
                    if (propertyHandle == Handle)
                        return typeHandle;
                }
            }
            throw new BadImageFormatException();
        }

        private bool IsMatchingAccessorMethod(MethodDefinitionHandle methodDefHandle, out bool isSetter)
        {
            MetadataReader reader = _module.MetadataReader;
            PropertyDefinition property = reader.GetPropertyDefinition(Handle);
            isSetter = false;
            MethodDefinition methodDef = reader.GetMethodDefinition(methodDefHandle);
            bool isSpecialName = methodDef.Attributes.HasFlag(MethodAttributes.SpecialName);
            string methodName = reader.GetString(methodDef.Name);
            return isSpecialName
                && (methodName.StartsWith("get_") || (isSetter = methodName.StartsWith("set_")))
                && methodName.Substring(4) == reader.GetString(property.Name);
        }

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            MetadataReader reader = _module.MetadataReader;

            PropertyDefinition property = reader.GetPropertyDefinition(Handle);

            TypeDefinitionHandle declaringTypeHandle = GetDeclaringType();

            DependencyList dependencies = new DependencyList();

            EcmaSignatureAnalyzer.AnalyzePropertySignature(
                _module,
                reader.GetBlobReader(property.Signature),
                factory,
                dependencies);

            dependencies.Add(factory.TypeDefinition(_module, declaringTypeHandle), "Property owning type");

            foreach (CustomAttributeHandle customAttribute in property.GetCustomAttributes())
            {
                dependencies.Add(factory.CustomAttribute(_module, customAttribute), "Custom attribute of a property");
            }

            // Property accessors
            TypeDefinition declaringType = reader.GetTypeDefinition(declaringTypeHandle);
            foreach (MethodDefinitionHandle methodDefHandle in declaringType.GetMethods())
            {
                if (IsMatchingAccessorMethod(methodDefHandle, out _))
                {
                    dependencies.Add(factory.MethodDefinition(_module, methodDefHandle), "Property accessor");
                }
            }

            return dependencies;
        }

        protected override EntityHandle WriteInternal(ModuleWritingContext writeContext)
        {
            MetadataReader reader = _module.MetadataReader;

            PropertyDefinition property = reader.GetPropertyDefinition(Handle);

            var builder = writeContext.MetadataBuilder;

            TypeDefinitionHandle declaringTypeHandle = GetDeclaringType();

            // Add MethodSemantics rows to link properties with accessor methods.
            TypeDefinition declaringType = reader.GetTypeDefinition(declaringTypeHandle);
            foreach (MethodDefinitionHandle methodDefHandle in declaringType.GetMethods())
            {
                if (IsMatchingAccessorMethod(methodDefHandle, out bool isSetter))
                {
                    MethodSemanticsAttributes semantics = isSetter
                        ? MethodSemanticsAttributes.Setter
                        : MethodSemanticsAttributes.Getter;
                    // MethodSemantics rows may be added in any order.
                    builder.AddMethodSemantics(Handle, semantics, methodDefHandle);
                }
            }

            BlobBuilder signatureBlob = writeContext.GetSharedBlobBuilder();
            EcmaSignatureRewriter.RewritePropertySignature(
                reader.GetBlobReader(property.Signature),
                writeContext.TokenMap,
                signatureBlob);

            return builder.AddProperty(
                property.Attributes,
                builder.GetOrAddString(reader.GetString(property.Name)),
                builder.GetOrAddBlob(signatureBlob));
        }

        public override string ToString()
        {
            // TODO: would be nice to have a common formatter we can call into that also includes owning type
            MetadataReader reader = _module.MetadataReader;
            return reader.GetString(reader.GetPropertyDefinition(Handle).Name);
        }
    }
}
