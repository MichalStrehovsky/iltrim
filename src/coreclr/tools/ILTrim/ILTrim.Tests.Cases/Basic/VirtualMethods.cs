using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Basic
{
#pragma warning disable 169

    [Kept]
    public class VirtualMethods
    {
        [Kept]
        static void Main()
        {
            BaseType b = new DerivedType();
            b.Method1();

            ((object)default(MyValueType)).ToString();
        }
    }

    [Kept]
    class BaseType
    {
        [Kept]
        public virtual void Method1() { }
        public virtual void Method2() { }
    }

    class DerivedType : BaseType
    {
        [Kept]
        public override void Method1() { }
        public override void Method2() { }
    }

    [Kept]
    struct MyValueType
    {
        [Kept]
        public override string ToString() => "";
    }
}
