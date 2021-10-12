﻿using System;

namespace Mono.Linker.Tests.Cases.Expectations.Assertions
{
	/// <summary>
	/// Verifies that a module reference exists in the test case assembly
	/// </summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class KeptModuleReferenceAttribute : KeptAttribute
	{
		public KeptModuleReferenceAttribute (string name)
		{
			if (string.IsNullOrEmpty (name))
				throw new ArgumentException ("Value cannot be null or empty.", nameof (name));
		}
	}
}
