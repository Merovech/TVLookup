using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using TvLookup.Core.Models;
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

		public Task AddEpisodes(IList<TvShowEpisode> episodes)
		{
			throw new NotImplementedException();
		}

		public Task AddShow(TvShow show)
		{
			throw new NotImplementedException();
		}

		public async Task CreateDatabase()
		{
			using (var conn = new SqliteConnection(CONN_STR))
			{
				var command = new SqliteCommand { Connection = conn };
				await conn.OpenAsync();

				command.CommandText = "PRAGMA journal_mode=DELETE";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE ""Shows"" (
											""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Shows"" PRIMARY KEY AUTOINCREMENT,
											""Title"" TEXT NOT NULL,
											""PremiereDate"" TEXT,
											""EndDate"" TEXT
										)";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE ""Episodes"" (
											""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Shows"" PRIMARY KEY AUTOINCREMENT,
											""Title"" TEXT NOT NULL,
											""SeasonNumber"" INTEGER NOT NULL,
											""EpisodeNumber"" INTEGER NOT NULL,
											""BroadcastDate"" TEXT NOT NULL
										)";
				await command.ExecuteNonQueryAsync();
			}

			_logger.LogInformation("New database created.");
		}

		public Task<TvShowEpisode> GetEpisode()
		{
			throw new NotImplementedException();
		}

		public Task<TvShow> GetShow()
		{
			throw new NotImplementedException();
		}
	}
}
