﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;

using Internal.TypeSystem.Ecma;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents an entry in the MemberRef metadata table.
    /// </summary>
    public sealed class MemberReferenceNode : TokenBasedNode
    {
        public MemberReferenceNode(EcmaModule module, MemberReferenceHandle handle)
            : base(module, handle)
        {
        }

        private MemberReferenceHandle Handle => (MemberReferenceHandle)_handle;

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            MemberReference memberRef = _module.MetadataReader.GetMemberReference(Handle);

            DependencyList dependencies = new DependencyList();

            BlobReader signatureBlob = _module.MetadataReader.GetBlobReader(memberRef.Signature);
            EcmaSignatureAnalyzer.AnalyzeMemberReferenceSignature(
                _module,
                signatureBlob,
                factory,
                dependencies);

            if (!memberRef.Parent.IsNil)
                dependencies.Add(factory.GetNodeForToken(_module, memberRef.Parent), "Parent of member reference");

            return dependencies;
        }

        protected override EntityHandle WriteInternal(ModuleWritingContext writeContext)
        {
            MetadataReader reader = _module.MetadataReader;
            MemberReference memberRef = reader.GetMemberReference(Handle);

            var builder = writeContext.MetadataBuilder;

            var signatureBlob = writeContext.GetSharedBlobBuilder();
            EcmaSignatureRewriter.RewriteMemberReferenceSignature(
                reader.GetBlobReader(memberRef.Signature),
                writeContext.TokenMap,
                signatureBlob);

            return builder.AddMemberReference(writeContext.TokenMap.MapToken(memberRef.Parent),
                builder.GetOrAddString(reader.GetString(memberRef.Name)),
                builder.GetOrAddBlob(signatureBlob));
        }

        public override string ToString()
        {
            MetadataReader reader = _module.MetadataReader;
            return reader.GetString(reader.GetMemberReference(Handle).Name);
        }
    }
}
