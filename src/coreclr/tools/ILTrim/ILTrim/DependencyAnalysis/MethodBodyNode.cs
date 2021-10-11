// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ILCompiler.DependencyAnalysisFramework;
using Internal.IL;

namespace ILTrim.DependencyAnalysis
{
    /// <summary>
    /// Represents method body bytes emitted into the executable.
    /// </summary>
    public class MethodBodyNode : DependencyNodeCore<NodeFactory>
    {
        private readonly EcmaModule _module;
        private readonly MethodDefinitionHandle _methodHandle;

        public MethodBodyNode(EcmaModule module, MethodDefinitionHandle methodHandle)
        {
            _module = module;
            _methodHandle = methodHandle;
        }

        private DependencyListEntry GetDependencyForToken(NodeFactory factory, int token, string reason)
            => new DependencyListEntry(factory.GetNodeForToken(_module, MetadataTokens.EntityHandle(token)), reason);

        public override IEnumerable<DependencyListEntry> GetStaticDependencies(NodeFactory factory)
        {
            // RVA = 0 is an extern method, such as a DllImport
            int rva = _module.MetadataReader.GetMethodDefinition(_methodHandle).RelativeVirtualAddress;
            if (rva == 0)
                yield break;

            MethodBodyBlock bodyBlock = _module.PEReader.GetMethodBody(rva);

            if (!bodyBlock.LocalSignature.IsNil)
                yield return new DependencyListEntry(factory.StandaloneSignature(_module, bodyBlock.LocalSignature), "Signatures of local variables");

            if (bodyBlock.GetILBytes() is { } bodyBytes)
            {
                ILReader ilReader = new(bodyBytes);

                while (ilReader.HasNext)
                {
                    ILOpcode opcode = ilReader.ReadILOpcode();

                    switch (opcode)
                    {
                        case ILOpcode.sizeof_:
                        case ILOpcode.newarr:
                        case ILOpcode.stsfld:
                        case ILOpcode.ldsfld:
                        case ILOpcode.ldsflda:
                        case ILOpcode.stfld:
                        case ILOpcode.ldfld:
                        case ILOpcode.ldflda:
                        case ILOpcode.call:
                        case ILOpcode.newobj:
                        case ILOpcode.ldtoken:
                        case ILOpcode.ldftn:
                        case ILOpcode.initobj:
                        case ILOpcode.stelem:
                        case ILOpcode.ldelem:
                        case ILOpcode.box:
                        case ILOpcode.unbox_any:
                            yield return GetDependencyForToken(factory, ilReader.ReadILToken(), $"Instruction {opcode.ToString()} operand");
                            break;

                        default:
                            ilReader.Skip(opcode);
                            break;
                    }
                }
            }
        }

        public int Write(ModuleWritingContext writeContext)
        {
            int rva = _module.MetadataReader.GetMethodDefinition(_methodHandle).RelativeVirtualAddress;
            if (rva == 0)
                return -1;

            writeContext.ILStream.Align(4);

            MethodBodyBlock bodyBlock = _module.PEReader.GetMethodBody(rva);

            // TODO: need to rewrite token references in the method body and exception regions

            byte[] bodyBytes = bodyBlock.GetILBytes();

            writeContext.ILStream.Align(4);
            MethodBodyStreamEncoder bodyStreamEncoder = new MethodBodyStreamEncoder(writeContext.ILStream);
            var bodyEncoder = bodyStreamEncoder.AddMethodBody(
                bodyBytes.Length,
                bodyBlock.MaxStack,
                exceptionRegionCount: 0,
                hasSmallExceptionRegions: false,
                (StandaloneSignatureHandle)writeContext.TokenMap.MapToken(bodyBlock.LocalSignature),
                bodyBlock.LocalVariablesInitialized ? MethodBodyAttributes.InitLocals : MethodBodyAttributes.None);
            new BlobWriter(bodyEncoder.Instructions).WriteBytes(bodyBytes);

            return bodyEncoder.Offset;
        }

        protected override string GetName(NodeFactory factory)
        {
            // TODO: would be nice to have a common formatter we can call into that also includes owning type
            MetadataReader reader = _module.MetadataReader;
            return "Method body for " + reader.GetString(reader.GetMethodDefinition(_methodHandle).Name);
        }

        public override bool InterestingForDynamicDependencyAnalysis => false;
        public override bool HasDynamicDependencies => false;
        public override bool HasConditionalStaticDependencies => false;
        public override bool StaticDependenciesAreComputed => true;
        public override IEnumerable<CombinedDependencyListEntry> GetConditionalStaticDependencies(NodeFactory factory) => null;
        public override IEnumerable<CombinedDependencyListEntry> SearchDynamicDependencies(List<DependencyNodeCore<NodeFactory>> markedNodes, int firstNode, NodeFactory factory) => null;
    }
}
