using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Linker.Tests.Extensions;
using Mono.Linker.Tests.TestCasesRunner;

namespace Mono.Linker.Tests.TestCases
{
	public static class TestDatabase
	{
		private static TestCase[]? _cachedAllCases;

		public static IEnumerable<object[]> BasicTests ()
		{
			return TestNamesBySuiteName("Basic");
		}

		public static IEnumerable<object[]> MultiAssembly ()
		{
			return TestNamesBySuiteName("MultiAssembly");
		}

        public static IEnumerable<object[]> LinkXml()
        {
            return TestNamesBySuiteName("LinkXml");
        }

        public static IEnumerable<object[]> FeatureSettings()
        {
            return TestNamesBySuiteName("FeatureSettings");
        }

        public static TestCaseCollector CreateCollector ()
		{
			GetDirectoryPaths (out string rootSourceDirectory, out string testCaseAssemblyPath);
			return new TestCaseCollector (rootSourceDirectory, testCaseAssemblyPath);
		}

		public static NPath TestCasesRootDirectory {
			get {
				GetDirectoryPaths (out string rootSourceDirectory, out string _);
				return rootSourceDirectory.ToNPath ();
			}
		}

		static IEnumerable<TestCase> AllCases ()
		{
			if (_cachedAllCases == null)
				_cachedAllCases = CreateCollector ()
					.Collect ()
                    .Where (c => c != null)
					.OrderBy (c => c.DisplayName)
					.ToArray ();

			return _cachedAllCases;
		}

        public static TestCase? GetTestCaseFromName(string name)
        {
            return AllCases().FirstOrDefault (c => c.Name == name);
        }

		static IEnumerable<object[]> TestNamesBySuiteName (string suiteName)
		{
            return AllCases()
                .Where(c => c.TestSuiteDirectory.FileName == suiteName)
                .Select(c => c.DisplayName)
                .OrderBy(c => c)
                .Select(c => new object[] { c });
		}

		static void GetDirectoryPaths (out string rootSourceDirectory, out string testCaseAssemblyPath)
		{
			rootSourceDirectory = Path.GetFullPath (Path.Combine (PathUtilities.GetTestsSourceRootDirectory (), "ILTrim.Tests.Cases"));
			testCaseAssemblyPath = PathUtilities.GetTestAssemblyPath ("ILTrim.Tests.Cases");
		}
	}
}
