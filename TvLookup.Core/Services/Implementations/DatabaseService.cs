using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using TvLookup.Core.Models.Api;
using TvLookup.Core.Services.Interfaces;

namespace TvLookup.Core.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	internal class DatabaseService : ServiceBase<DatabaseService>, IDatabaseService
	{
		// TODO: Consider EFCore
		// TODO: Turn the connection string into options
		private const string CONN_STR = "Data Source=tvlookup.db;";
		private readonly ILogger _logger;

		public DatabaseService(ILogger<DatabaseService> logger) : base(logger)
		{
			_logger = logger;
		}

		public Task AddEpisodes(IList<ApiTvShowEpisode> episodes)
		{
			throw new NotImplementedException();
		}

		public Task AddShow(ApiTvShow show)
		{
			throw new NotImplementedException();
		}

		public async Task CreateDatabase()
		{
			// TODO: EF should do this
		}

		public async Task<ApiTvShowEpisode> GetEpisode(int showId, int seasonNumber, int episodeNumber)
		{
			// TODO: EFCore will do this
			return null;
		}

		public async Task<ApiTvShow> GetShow(int showId)
		{
			// TODO: EFCore will do this
			return null;
		}
	}
}
