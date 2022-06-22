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

				command.CommandText = @"CREATE TABLE Shows (
											Id INTEGER NOT NULL CONSTRAINT PK_Shows PRIMARY KEY AUTOINCREMENT,
											Title TEXT NOT NULL,
											PremiereDate TEXT,
											EndDate TEXT,
											Summary TEXT,
											Type TEXT,
											Language TEXT,
											Genres TEXT
										)";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE Episodes (
											Id INTEGER NOT NULL CONSTRAINT PK_Shows PRIMARY KEY AUTOINCREMENT,
											ShowId INTEGER NOT NULL,
											Title TEXT NOT NULL,
											SeasonNumber INTEGER NOT NULL,
											EpisodeNumber INTEGER NOT NULL,
											BroadcastDate TEXT NOT NULL,
											Summary TEXT,
											Type TEXT,
											FOREIGN KEY(ShowId) REFERENCES Show(Id)
										)";
				await command.ExecuteNonQueryAsync();
			}

			_logger.LogInformation("New database created.");
		}

		public async Task<TvShowEpisode> GetEpisode(int showId, int seasonNumber, int episodeNumber)
		{
			using (var conn = new SqliteConnection(CONN_STR))
			{
				var commandText = @"SELECT * FROM Episodes
									WHERE ShowId = @ShowId AND SeasonNumber = @SeasonNumber AND EpisodeNumber = @EpisodeNumber";

				var countCommandText = @"SELECT COUNT(Id) FROM Episodes
										WHERE ShowId = @ShowId AND SeasonNumber = @SeasonNumber AND EpisodeNumber = @EpisodeNumber";

				var command = new SqliteCommand(countCommandText);
				command.Parameters.AddWithValue("@ShowId", showId);
				command.Parameters.AddWithValue("@SeasonNumber", seasonNumber);
				command.Parameters.AddWithValue("@EpisodeNumber", episodeNumber);

				await conn.OpenAsync();
				var count = (int)await command.ExecuteScalarAsync();
				if (count > 1)
				{
					// TODO: Better exception handling
					throw new Exception("More than one record found.");
				}

				command.CommandText = commandText;
				var reader = await command.ExecuteReaderAsync();

				if (!reader.HasRows)
				{
					return null;
				}

				var episode = new TvShowEpisode()
				{
					Id = reader.GetInt32(0),
					Title = reader.GetString(1),
					SeasonNumber = reader.GetInt32(2),
					EpisodeNumber = reader.GetInt32(3),
					AirDate = reader.GetDateTime(4),
					Summary = reader.GetString(5),
					Type = reader.GetString(6)
				};

				reader.Close();

				return episode;
			}
		}

		public async Task<TvShow> GetShow(int showId)
		{
			using (var conn = new SqliteConnection(CONN_STR))
			{
				var commandText = "SELECT * FROM Shows  WHERE Id = @ShowId";
				var countCommandText = "SELECT COUNT(Id) FROM Shows  WHERE Id = @ShowId";

				var command = new SqliteCommand(countCommandText);
				command.Parameters.AddWithValue("@Id", showId);

				await conn.OpenAsync();
				var count = (int)await command.ExecuteScalarAsync();
				if (count > 1)
				{
					// TODO: Better exception handling
					throw new Exception("More than one record found.");
				}

				command.CommandText = commandText;
				var reader = await command.ExecuteReaderAsync();

				if (!reader.HasRows)
				{
					return null;
				}

				var result = new TvShow
				{
					Id = reader.GetInt32(0),
					Title = reader.GetString(1),
					PremiereDate = reader.GetDateTime(2),
					EndDate = reader.GetDateTime(3),
					Summary = reader.GetString(4),
					Type = reader.GetString(5),
					Language = reader.GetString(6),
					Genres = reader.GetString(7).Split(',').ToList()	// TODO: Table for genres?
				};

				return result;
			}
		}
	}
}
