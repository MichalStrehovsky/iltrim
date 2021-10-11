using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Xunit;

namespace ILTrim.Tests
{
    public class TestCases
    {
        [Fact]
        public unsafe void CheckKept()
        {
            var testAssembly = GetTestAssembly();

            var trimmedStream = new MemoryStream();
            fixed (byte* asmPtr = testAssembly)
            {
                using var peReader = new PEReader(asmPtr, testAssembly.Length);
                ILTrim.Trimmer.TrimAssembly(peReader, trimmedStream);
            }

            var untrimmed = LoadTestAssembly(testAssembly);
            var trimmed = LoadTestAssembly(trimmedStream.GetBuffer());

            foreach (var untrimmedType in untrimmed.GetTypes())
            {
                Type trimmedType;
                string typeName = untrimmedType.FullName!;
                {
                    Type? trimmedTypeCandidate = trimmed.GetType(typeName);
                    if (HasKeptAttribute(untrimmedType))
                    {
                        if (trimmedTypeCandidate is null)
                        {
                            Assert.True(false, $"Type '{typeName}' was not kept.");
                            continue;
                        }

                        trimmedType = trimmedTypeCandidate!;
                    }
                    else
                    {
                        // This causes trouble since we're also trimming the CoreLib stubs
                        // Once we switch tests over the real corlib and we won't validate corlib,
                        // then we should reenable this.
                        //Assert.True(trimmedTypeCandidate is null, $"Type '{typeName}' was not removed.");
                        continue;
                    }
                }

                foreach (var untrimmedMember in untrimmedType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    // TODO: Handle overloads
                    string memberName = untrimmedMember.Name;
                    MemberInfo? trimmedMember = trimmedType.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).FirstOrDefault();
                    if (HasKeptAttribute(untrimmedMember))
                    {
                        Assert.True(trimmedMember is not null, $"Member '{memberName}' was not kept.");
                    }
                    else
                    {
                        // This causes trouble since we're also trimming the CoreLib stubs
                        // Once we switch tests over the real corlib and we won't validate corlib,
                        // then we should reenable this.
                        //Assert.True(trimmedMember is null, $"Member '{memberName}' was not removed.");
                    }
                }
            }

            static Assembly LoadTestAssembly(byte[] assembly)
            {
                var resolver = new CustomResolver(assembly);
                var loadContext = new MetadataLoadContext(resolver, "ILTrim.Tests.Cases");

                return Assert.Single(loadContext.GetAssemblies());
            }
        }

        private sealed class CustomResolver : MetadataAssemblyResolver
        {
            private byte[] _assembly;
            public CustomResolver(byte[] assembly)
            {
                _assembly = assembly;
            }

            public override Assembly? Resolve(MetadataLoadContext context, AssemblyName assemblyName)
            {
                return context.LoadFromByteArray(_assembly);
            }
        }

        private static bool HasKeptAttribute(MemberInfo memberInfo) => HasKeptAttribute(memberInfo.GetCustomAttributesData());

        private static bool HasKeptAttribute(IEnumerable<CustomAttributeData> data)
        {
            foreach (var d in data)
            {
                if (d.AttributeType.Name == "KeptAttribute")
                {
                    return true;
                }
            }
            return false;
        }

        public static byte[] GetTestAssembly()
        {
            var srcFiles = Directory.GetFiles(
                Path.Combine(GetContainingDirectory(), "..", "ILTrim.Tests.Cases"),
                "*.cs",
                SearchOption.AllDirectories);

            var trees = srcFiles.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f)));

            var comp = CSharpCompilation.Create(assemblyName: "ILTrim.Tests.Cases", trees);
            var peStream = new MemoryStream();
            var emitResult = comp.Emit(peStream, options: new EmitOptions().WithRuntimeMetadataVersion("5.0"));
            Assert.True(emitResult.Success);
            return peStream.GetBuffer();
        }

        private static string GetContainingDirectory([CallerFilePath]string path = "")
            => Path.GetDirectoryName(path)!;
    }
}
