using Microsoft.Extensions.Logging;
using TvLookup.Core.Services.Implementations;
using TvLookup.Core.Services.Interfaces;

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
