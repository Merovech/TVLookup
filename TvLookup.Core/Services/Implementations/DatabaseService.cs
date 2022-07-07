using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TvLookup.Core.Models;
using TvLookup.Core.Models.Api;
using TvLookup.Core.Services.Interfaces;
using TvLookup.Core.Utilities;

namespace TvLookup.Core.Services.Implementations
{
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class DatabaseService : ServiceBase<DatabaseService>, IDatabaseService
	{
		// TODO: Consider EFCore
		// TODO: Turn the connection string into options
		private readonly ILogger _logger;
		private readonly DatabaseContext _dbContext;

		public DatabaseService(DatabaseContext context, ILogger<DatabaseService> logger) : base(logger)
		{
			Guard.AgainstNull(context, nameof(context));
			Guard.AgainstNull(logger, nameof(logger));
			_logger = logger;
			_dbContext = context;
		}

		public async Task AddEpisodes(IList<ApiTvShowEpisode> episodes, int showId)
		{
			Guard.AgainstNullOrEmptyList(episodes, nameof(episodes));
			Guard.AgainstValuesLessThan(1, showId, nameof(showId));

			var show = await GetShow(showId);
			if (show == null)
			{
				throw new InvalidOperationException($"Show with ID of {showId} was not found in the database.");
			}

			foreach (var ep in episodes)
			{
				var converted = ConvertEpisode(ep, show.Id);
				_dbContext.Episodes.Add(converted);
			}

			await _dbContext.SaveChangesAsync();
		}

		public async Task AddShow(ApiTvShow show)
		{
			Guard.AgainstNull(show, nameof(show));

			var convertedShow = ConvertApiShow(show);
			_dbContext.Add(convertedShow);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<TvShowEpisode> GetEpisode(int showId, int seasonNumber, int episodeNumber)
		{
			Guard.AgainstValuesLessThan(1, showId, nameof(showId));
			Guard.AgainstValuesLessThan(1, seasonNumber, nameof(seasonNumber));
			Guard.AgainstValuesLessThan(1, episodeNumber, nameof(episodeNumber));

			return await _dbContext
				.Episodes
				.Where(e => e.ShowId == showId && e.SeasonNumber == seasonNumber && e.EpisodeNumber == episodeNumber)
				.SingleOrDefaultAsync();
		}

		public async Task<TvShow> GetShow(int showId)
		{
			Guard.AgainstValuesLessThan(1, showId, nameof(showId));

			return await _dbContext
				.Shows
				.Where(s => s.ApiId == showId)
				.SingleOrDefaultAsync();
		}

		public async Task CreateDatabase()
		{
			await _dbContext.Database.MigrateAsync();

			// Necessary since EF core doesn't provide a nice way of handling PRAGMA statements
			await _dbContext.Database.OpenConnectionAsync();
			await _dbContext.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=DELETE");
			await _dbContext.Database.CloseConnectionAsync();
		}

		private TvShowEpisode ConvertEpisode(ApiTvShowEpisode episode, int showId)
		{
			return new TvShowEpisode
			{
				AirDate = episode.AirDate,
				ApiId = episode.Id,
				EpisodeNumber = episode.EpisodeNumber,
				SeasonNumber = episode.SeasonNumber,
				ShowId = showId,
				Summary = episode.Summary,
				Title = episode.Title,
				Type = episode.Type
			};
		}

		private TvShow ConvertApiShow(ApiTvShow apiShow)
		{
			TvShow show = new()
			{
				ApiId = apiShow.Id,
				PremiereDate = apiShow.PremiereDate,
				EndDate = apiShow.EndDate,
				Language = apiShow.Language,
				Summary = apiShow.Summary,
				Title = apiShow.Title,
				Type = apiShow.Type
			};

			foreach (var g in apiShow.Genres)
			{
				// Look to see if the genre already exists in the DB
				var foundGenre = _dbContext.Genres.FirstOrDefault(dbGenre => dbGenre.Name == g);
				TvGenre genre = new()
				{
					Id = foundGenre == null ? 0 : foundGenre.Id,
					Name = g,
				};

				TvShowGenre tsg = new()
				{
					Show = show
				};

				if (foundGenre == null)
				{
					tsg.Genre = genre;
				}
				else
				{
					tsg.GenreId = foundGenre.Id;
				}

				show.Genres.Add(tsg);
			}

			return show;
		}
	}
}
