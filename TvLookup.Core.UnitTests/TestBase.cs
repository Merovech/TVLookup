using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvLookup.Core.UnitTests
{
	public class TestBase<TTarget, TBuilder> where TBuilder : IBuilder<TTarget>
	{
		protected TTarget Target
		{
			get; set;
		}

		protected TBuilder Builder
		{
			get; set;
		}
	}
}
