// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Linq;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents an entry in the StandaloneSignature table.
    /// </summary>
    public sealed class StandaloneSignatureNode : TokenBasedNode
    {
        public StandaloneSignatureNode(EcmaModule module, StandaloneSignatureHandle handle)
            : base(module, handle)
        {
        }

        private StandaloneSignatureHandle Handle => (StandaloneSignatureHandle)_handle;


        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            MetadataReader reader = _module.MetadataReader;
            StandaloneSignature standaloneSig = reader.GetStandaloneSignature(Handle);

            //TODO: These need to go a different structure, similar to src\coreclr\tools\Common\TypeSystem\Ecma\EcmaSignatureParser.cs
            //Adding a small prototype for local variable signature parser to evaluate

            BlobReader signatureReader = reader.GetBlobReader(standaloneSig.Signature);

            SignatureHeader header = signatureReader.ReadSignatureHeader();
            int count = signatureReader.ReadCompressedInteger();

            for (int i = 0; i < count; i++)
            {
                SignatureTypeCode typeCode = signatureReader.ReadSignatureTypeCode();
                switch (typeCode)
                {
                    case SignatureTypeCode.TypeHandle:
                        TypeDefinitionHandle typeDefHandle = (TypeDefinitionHandle)signatureReader.ReadTypeHandle();
                        yield return new DependencyListEntry(factory.TypeDefinition(_module, typeDefHandle), "Local variable type");
                        break;

                    case SignatureTypeCode.Int32:
                        yield break;
                }
            }

            yield break;
        }

        protected override EntityHandle WriteInternal(ModuleWritingContext writeContext)
        {
            MetadataReader reader = _module.MetadataReader;
            StandaloneSignature standaloneSig = reader.GetStandaloneSignature(Handle);


            BlobReader signatureReader = reader.GetBlobReader(standaloneSig.Signature);
            SignatureHeader header = signatureReader.ReadSignatureHeader();
            int varCount = signatureReader.ReadCompressedInteger();
            var blobBuilder = new BlobBuilder();
            var encoder = new BlobEncoder(blobBuilder);
            var localEncoder = encoder.LocalVariableSignature(varCount);

            for (int i = 0; i < varCount; i++)
            {
                SignatureTypeCode typeCode = signatureReader.ReadSignatureTypeCode();
                switch (typeCode)
                {
                    case SignatureTypeCode.TypeHandle:
                        {
                            var localVarTypeEncoder = localEncoder.AddVariable();

                            var signatureTypeEncoder = localVarTypeEncoder.Type();
                            signatureTypeEncoder.Type(writeContext.TokenMap.MapToken((TypeDefinitionHandle)signatureReader.ReadTypeHandle()), isValueType: false);
                            break;
                        }

                    case SignatureTypeCode.Int32:
                        {
                            var localVarTypeEncoder = localEncoder.AddVariable();

                            var signatureTypeEncoder = localVarTypeEncoder.Type();
                            signatureTypeEncoder.Int32();
                            break;
                        }

                    default:
                        break;
                }
            }

            byte[] blobBytes = blobBuilder.ToArray();
            var builder = writeContext.MetadataBuilder;
            return builder.AddStandaloneSignature(
                builder.GetOrAddBlob(blobBytes));

        }

        public override string ToString()
        {
            return "Standalone signature";
        }
    }
}
