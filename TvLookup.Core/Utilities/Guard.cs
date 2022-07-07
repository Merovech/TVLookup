using System;
using System.Collections.Generic;
using System.Linq;

namespace TvLookup.Core.Utilities
{
	public static class Guard
	{
		public static void AgainstNull(object obj, string argumentName)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		public static void AgainstNullOrEmptyList<T>(IEnumerable<T> list, string argumentName)
		{
			if (list == null)
			{
				throw new ArgumentNullException(argumentName);
			}

			if (!list.Any())
			{
				throw new InvalidOperationException($"'{argumentName}' cannot be empty");
			}
		}

		public static void AgainstValuesLessThan(int target, int val, string argumentName)
		{
			if (val < target)
			{
				throw new ArgumentException($"{argumentName} cannot be less than {target}");
			}
		}
	}
}
