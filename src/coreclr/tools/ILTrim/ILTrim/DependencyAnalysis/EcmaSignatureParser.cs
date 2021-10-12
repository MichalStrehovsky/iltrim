// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection.Metadata;

namespace ILTrim.Core.DependencyAnalysis
{
    public struct EcmaSignatureParser
    {
#pragma warning disable 0169
        private BlobReader _reader;
        private BlobWriter _writer;
#pragma warning restore 0169


        public EcmaSignatureParser(BlobReader reader, BlobWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }
    }
}
