using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Basic
{
	class UnusedBodyGetsRemoved
	{
		public static void Main ()
		{
			UnusedBodyType unusedBody = null;
			if (unusedBody != null)
				unusedBody.UnusedBody();
		}

		class UnusedBodyType
		{
			[Kept]
			[ExpectedInstructionSequence(new[] {
				"ldnull",
				"throw"
			})]
			public void UnusedBody () => DoSomethingExpensive ();

			static void DoSomethingExpensive () { }
		}
	}
}
