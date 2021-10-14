using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Basic
{
    [Kept]
	class FieldRVA
	{
        [Kept]
        static byte[] Bytes => new byte[] { 1, 2, 3, 4, 5 };

        [Kept]
        static int Main() => Bytes.Length;
	}
}
