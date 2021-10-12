// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;

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
            // TODO: These need to go a different structure, similar to src\coreclr\tools\Common\TypeSystem\Ecma\EcmaSignatureParser.cs
            // Adding a small prototype for local variable signature parser to evaluate
            StandaloneSignature signature = _module.MetadataReader.GetStandaloneSignature(Handle);
            BlobReader signatureReader = _module.MetadataReader.GetBlobReader(signature.Signature);

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

            // TODO: the signature might have tokens we need to rewrite
            var builder = writeContext.MetadataBuilder;
            return builder.AddStandaloneSignature(
                builder.GetOrAddBlob(reader.GetBlobBytes(standaloneSig.Signature))
                );
        }

        public override string ToString()
        {
            return "Standalone signature";
        }
    }
}
