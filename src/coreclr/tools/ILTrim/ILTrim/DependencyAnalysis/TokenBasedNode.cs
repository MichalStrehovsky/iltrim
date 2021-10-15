// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Internal.TypeSystem.Ecma;

using ILCompiler.DependencyAnalysisFramework;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Base class for all nodes that have an associated row in one of the metadata tables
    /// of the input module.
    /// </summary>
    public abstract class TokenBasedNode : TokenWriterNode
    {
        protected readonly EntityHandle _handle;

        public override TableIndex TableIndex {
            get {
                bool gotIndex = MetadataTokens.TryGetTableIndex(_handle.Kind, out TableIndex index);
                Debug.Assert(gotIndex);
                return index;
            }
        }

        /// <summary>
        /// Gets the module associated with this node.
        /// </summary>
        public EcmaModule Module => _module;

        public TokenBasedNode(EcmaModule module, EntityHandle handle)
            : base(module)
        {
            _handle = handle;
        }

        public sealed override void Write(ModuleWritingContext writeContext)
        {
            EntityHandle writtenHandle = WriteInternal(writeContext);
            // TODO: clean? kinda shared with base.
            Debug.Assert(writeContext.TokenMap.MapToken(_handle) == writtenHandle);
        }

        public sealed override void BuildTokens(TokenMap.Builder builder)
        {
            builder.AddTokenMapping(_handle);
        }

        public override int CompareTo(TokenWriterNode other)
        {
            int baseResult = base.CompareTo(other);
            if (baseResult != 0)
                return baseResult;

            // Nodes with the same table index must be the same node type.
            var otherTokenBasedNode = (TokenBasedNode)other;
            int result = MetadataTokens.GetToken(_handle).CompareTo(MetadataTokens.GetToken(otherTokenBasedNode._handle));

            // It's only valid to compare these within the same module
            Debug.Assert(result != 0 || this == other);

            return result;
        }

        protected sealed override string GetName(NodeFactory context)
        {
            MetadataReader reader = _module.MetadataReader;
            int tokenRaw = MetadataTokens.GetToken(_handle);
            string moduleName = reader.GetString(reader.GetModuleDefinition().Name);
            return $"{this.ToString()} ({moduleName}:{tokenRaw:X8})";
        }
    }
}
