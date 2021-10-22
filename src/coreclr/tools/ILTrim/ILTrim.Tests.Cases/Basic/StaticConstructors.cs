// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.Basic
{
    public class StaticConstructors
    {
        public static void Main()
        {
            StaticFieldInitializer.Foo();
            StaticFieldInitializerAndCtor.Foo();
            UnusedStaticFieldInitializer.Foo();
        }

        [KeptMember(".cctor()")]
        static class StaticFieldInitializer
        {
            [Kept]
            public static int someStaticField = 123;

            [Kept]
            public static int Foo()
            {
                return someStaticField;
            }
        }

        static class StaticFieldInitializerAndCtor
        {
            public static object o = new object();

            static StaticFieldInitializerAndCtor()
            {
            }

            [Kept]
            public static void Foo()
            {
            }
        }

        static class UnusedStaticFieldInitializer
        {
            public static object o = new object();

            [Kept]
            public static void Foo()
            {
            }
        }

        class UnusedStaticCtor
        {
            static UnusedStaticCtor()
            {
            }
        }
    }
}
