﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Reflection.PortableExecutable;
using ILTrim;

namespace Mono.Linker.Tests.TestCasesRunner
{
    public class TrimmerDriver
    {
        public void Trim (TrimmerOptions options)
        {
            using var fs = File.OpenRead(options.InputPath!);
            using var pe = new PEReader(fs);
            using var output = File.Create(Path.Combine(options.OutputDirectory!, Path.GetFileName(options.InputPath!)));
            Trimmer.TrimAssembly(pe, output);
        }
    }
}
