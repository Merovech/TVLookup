using System;

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
	}
}
