using System;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Basic
{
    [Kept]
	class FieldRVA
	{
        [Kept]
        static ReadOnlySpan<byte> Bytes => new byte[] { 1, 2, 3 };

        [Kept]
        static int Main() => Bytes[0];
	}
}
