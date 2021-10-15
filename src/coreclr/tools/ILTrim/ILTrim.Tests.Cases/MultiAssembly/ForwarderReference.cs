
using Mono.Linker.Tests.Cases.Expectations.Assertions;
using Mono.Linker.Tests.Cases.Expectations.Metadata;

namespace Mono.Linker.Tests.Cases.MultiAssembly
{
    [SetupLinkerAction ("link", "Forwarder")]
    [SetupCompileBefore("Forwarder.dll", new[] { "Dependencies/ForwardedType.cs" })]

    [SetupCompileAfter ("ForwardedType.dll", new[] { "Dependencies/ForwardedType.cs" })]
    [SetupCompileAfter("Forwarder.dll", new[] { "Dependencies/Forwarder.cs" }, references: new[] { "ForwardedType.dll" })]

    [KeptMemberInAssembly("ForwardedType.dll", typeof(ForwardedType), "Kept()")]
    [KeptMemberInAssembly("ForwardedType.dll", typeof(ForwardedType), nameof(ForwardedType.KeptField))]
    public class ForwarderReference
    {
        public static void Main()
        {
            ForwardedType.Kept();
            ForwardedType.KeptField = 0;
        }
    }
}
