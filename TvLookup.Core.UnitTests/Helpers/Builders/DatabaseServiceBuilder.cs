using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TvLookup.Core.Services.Interfaces;
using TvLookup.Core.Services.Implementations;

namespace TvLookup.Core.UnitTests.Helpers.Builders
{
	public class DatabaseServiceBuilder : IBuilder<IDatabaseService>
	{
		public ILogger<DatabaseService> Logger
		{
			get; set;
		}

		public DatabaseContext Context
		{
			get; set;
		}

		public IDatabaseService Build()
		{
			return new DatabaseService(Context, Logger);
		}
	}
}
