// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ILTrim.Tests
{
    public class TestCase
    {
        public TestCase(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string ToString() => Name;
    }

    public class TestGen
    {
        public static IEnumerable<object[]> GetTests()
        {
            yield return new object[] { "Some1" };
            yield return new object[] { "Some2" };
        }
    }

    public class TestCaseGeneration
    {
        [Theory]
        [MemberData(nameof(TestGen.GetTests), MemberType = typeof(TestGen))]
        public void TestWithData(string t)
        {

        }
    }
}
