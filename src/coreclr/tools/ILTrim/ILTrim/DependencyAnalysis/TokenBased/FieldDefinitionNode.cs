// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Internal.TypeSystem.Ecma;

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

            if ((fieldDef.Attributes & FieldAttributes.Literal) == FieldAttributes.Literal)
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
                WriteMagicField(writeContext, fieldDef);
            }

            return builder.AddFieldDefinition(
                fieldDef.Attributes,
                builder.GetOrAddString(reader.GetString(fieldDef.Name)),
                builder.GetOrAddBlob(signatureBlob));
        }

        unsafe internal void WriteMagicField(ModuleWritingContext writeContext, FieldDefinition fieldDef)
        {
            var fieldDesc = _module.GetField(Handle);
            int rva = fieldDef.GetRelativeVirtualAddress();

            if (fieldDesc.FieldType is EcmaType typeDesc && rva != 0)
            {
                var rvaBlobReader = _module.PEReader.GetSectionData(rva).Pointer;
                int fieldSize = typeDesc.Category switch
                {
                    Internal.TypeSystem.TypeFlags.Byte => 1,
                    Internal.TypeSystem.TypeFlags.Int16 => 2,
                    Internal.TypeSystem.TypeFlags.Int32 => 4,
                    Internal.TypeSystem.TypeFlags.Int64 => 8,
                    _ => typeDesc.EcmaModule.MetadataReader.GetTypeDefinition(typeDesc.Handle).GetLayout().Size
                };
                BlobBuilder outputBodyBuilder = writeContext.fieldBuilder;
                int newRVA = outputBodyBuilder.Count;
                outputBodyBuilder.WriteBytes(rvaBlobReader, fieldSize);
                var fieldDefHandle = (FieldDefinitionHandle)writeContext.TokenMap.MapToken(Handle);
                writeContext.MetadataBuilder.AddFieldRelativeVirtualAddress(
                    fieldDefHandle,
                    newRVA);
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
