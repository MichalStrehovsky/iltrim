﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Internal.TypeSystem.Ecma;

using DependencyList = ILCompiler.DependencyAnalysisFramework.DependencyNodeCore<ILTrim.DependencyAnalysis.NodeFactory>.DependencyList;

namespace ILTrim.DependencyAnalysis
{
    public struct EcmaSignatureAnalyzer
    {
        private readonly EcmaModule _module;
        private BlobReader _blobReader;
        private readonly NodeFactory _factory;
        private DependencyList _dependenciesOrNull;

        private DependencyList Dependencies
        {
            get
            {
                return _dependenciesOrNull ??= new DependencyList();
            }
        }

        private EcmaSignatureAnalyzer(EcmaModule module, BlobReader blobReader, NodeFactory factory, DependencyList dependencies)
        {
            _module = module;
            _blobReader = blobReader;
            _factory = factory;
            _dependenciesOrNull = dependencies;
        }

        private void AnalyzeCustomModifier(SignatureTypeCode typeCode)
        {
            Dependencies.Add(_factory.GetNodeForToken(_module, _blobReader.ReadTypeHandle()), "Custom modifier");
        }

        private void AnalyzeType()
        {
            AnalyzeType(_blobReader.ReadSignatureTypeCode());
        }

        private void AnalyzeType(SignatureTypeCode typeCode)
        {
            switch (typeCode)
            {
                case SignatureTypeCode.Void:
                case SignatureTypeCode.Boolean:
                case SignatureTypeCode.SByte:
                case SignatureTypeCode.Byte:
                case SignatureTypeCode.Int16:
                case SignatureTypeCode.UInt16:
                case SignatureTypeCode.Int32:
                case SignatureTypeCode.UInt32:
                case SignatureTypeCode.Int64:
                case SignatureTypeCode.UInt64:
                case SignatureTypeCode.Single:
                case SignatureTypeCode.Double:
                case SignatureTypeCode.Char:
                case SignatureTypeCode.String:
                case SignatureTypeCode.IntPtr:
                case SignatureTypeCode.UIntPtr:
                case SignatureTypeCode.Object:
                case SignatureTypeCode.TypedReference:
                case SignatureTypeCode.GenericTypeParameter:
                case SignatureTypeCode.GenericMethodParameter:
                    break;
                case SignatureTypeCode.TypeHandle:
                    Dependencies.Add(_factory.GetNodeForToken(_module, _blobReader.ReadTypeHandle()), "Signature reference");
                    break;
                case SignatureTypeCode.SZArray:
                case SignatureTypeCode.Pointer:
                case SignatureTypeCode.ByReference:
                    AnalyzeType();
                    break;
                case SignatureTypeCode.Array:
                    throw new NotImplementedException();
                
                case SignatureTypeCode.RequiredModifier:
                case SignatureTypeCode.OptionalModifier:
                    AnalyzeCustomModifier(typeCode);
                    break;
                case SignatureTypeCode.GenericTypeInstance:
                    _blobReader.ReadCompressedInteger();
                    Dependencies.Add(_factory.GetNodeForToken(_module, _blobReader.ReadTypeHandle()), "Signature reference");
                    int numGenericArgs = _blobReader.ReadCompressedInteger();
                    for (int i = 0; i < numGenericArgs; i++)
                    {
                        AnalyzeType();
                    }
                    break;
                case SignatureTypeCode.FunctionPointer:
                    throw new NotImplementedException();
                default:
                    throw new BadImageFormatException();
            }
        }

        public static DependencyList AnalyzeLocalVariableBlob(EcmaModule module, BlobReader blobReader, NodeFactory factory, DependencyList dependencies = null)
        {
            return new EcmaSignatureAnalyzer(module, blobReader, factory, dependencies).AnalyzeLocalVariableBlob();
        }

        private DependencyList AnalyzeLocalVariableBlob()
        {
            SignatureHeader header = _blobReader.ReadSignatureHeader();
            int varCount = _blobReader.ReadCompressedInteger();
            for (int i = 0; i < varCount; i++)
            {
            again:
                SignatureTypeCode typeCode = _blobReader.ReadSignatureTypeCode();
                if (typeCode == SignatureTypeCode.RequiredModifier || typeCode == SignatureTypeCode.OptionalModifier)
                {
                    AnalyzeCustomModifier(typeCode);
                    goto again;
                }
                if (typeCode == SignatureTypeCode.Pinned)
                {
                    goto again;
                }
                if (typeCode == SignatureTypeCode.ByReference)
                {
                    goto again;
                }
                AnalyzeType(typeCode);
            }

            return _dependenciesOrNull;
        }

        public static DependencyList AnalyzeMethodSignature(EcmaModule module, BlobReader blobReader, NodeFactory factory, DependencyList dependencies = null)
        {
            return new EcmaSignatureAnalyzer(module, blobReader, factory, dependencies).AnalyzeMethodSignature();
        }

        private DependencyList AnalyzeMethodSignature()
        {
            SignatureHeader header = _blobReader.ReadSignatureHeader();
            return AnalyzeMethodSignature(header);
        }

        private DependencyList AnalyzeMethodSignature(SignatureHeader header)
        {
            int arity = header.IsGeneric ? _blobReader.ReadCompressedInteger() : 0;
            int paramCount = _blobReader.ReadCompressedInteger();

            // Return type
            AnalyzeType();

            for (int i = 0; i < paramCount; i++)
            {
                AnalyzeType();
            }

            return _dependenciesOrNull;
        }

        public static DependencyList AnalyzeMemberReferenceSignature(EcmaModule module, BlobReader blobReader, NodeFactory factory, DependencyList dependencies = null)
        {
            return new EcmaSignatureAnalyzer(module, blobReader, factory, dependencies).AnalyzeMemberReferenceSignature();
        }

        private DependencyList AnalyzeMemberReferenceSignature()
        {
            SignatureHeader header = _blobReader.ReadSignatureHeader();
            if (header.Kind == SignatureKind.Method)
            {
                return AnalyzeMethodSignature(header);
            }
            else
            {
                System.Diagnostics.Debug.Assert(header.Kind == SignatureKind.Field);
                // TODO: field signature
                return _dependenciesOrNull;
            }
        }

        public static DependencyList AnalyzePropertySignature(EcmaModule module, BlobReader blobReader, NodeFactory factory, DependencyList dependencies = null)
        {
            return new EcmaSignatureAnalyzer(module, blobReader, factory, dependencies).AnalyzePropertySignature();
        }

        private DependencyList AnalyzePropertySignature()
        {
            SignatureHeader header = _blobReader.ReadSignatureHeader();
            System.Diagnostics.Debug.Assert(header.Kind == SignatureKind.Property);
            return AnalyzeMethodSignature(header);
        }
    }
}
