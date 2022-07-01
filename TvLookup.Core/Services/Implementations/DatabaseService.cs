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
			foreach (var ep in episodes)
			{
				var converted = ConvertEpisode(ep, showId);
				_dbContext.Episodes.Add(converted);
			}

			await _dbContext.SaveChangesAsync();
		}

		public async Task AddShow(ApiTvShow show)
		{
			// TODO: There's got to be a better way of dong this

			// First, convert the show and add it to the DB
			var convertedShow = ConvertApiShow(show);
			_dbContext.Add(convertedShow);
			await _dbContext.SaveChangesAsync();

			// Next, convert the genres and add them to the DB
			List<TvGenre> genres = new();
			foreach (var g in show.Genres)
			{
				genres.Add(await ConvertOrAddGenre(g));
			}

			// Finally, add the associated TvShowGenres to the DB
			List<TvShowGenre> showGenres = genres.Select(g => new TvShowGenre { GenreId = g.Id, Genre = g, ShowId = convertedShow.Id, Show = convertedShow }).ToList();
			await _dbContext.AddRangeAsync(showGenres);
			_dbContext.SaveChanges();
		}

		public async Task CreateDatabase()
		{
			await _dbContext.Database.MigrateAsync();
		}

		public async Task<TvShowEpisode> GetEpisode(int showId, int seasonNumber, int episodeNumber)
		{
			return await _dbContext
				.Episodes
				.Where(e => e.ShowId == showId && e.SeasonNumber == seasonNumber && e.EpisodeNumber == episodeNumber)
				.SingleOrDefaultAsync();
		}

		public async Task<TvShow> GetShow(int showId)
		{
			return await _dbContext
				.Shows
				.Where(s => s.ApiId == showId)
				.SingleOrDefaultAsync();
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
			return new TvShow
			{
				Id = -1,
				ApiId = apiShow.Id,
				PremiereDate = apiShow.PremiereDate,
				EndDate = apiShow.EndDate,
				Language = apiShow.Language,
				Summary = apiShow.Summary,
				Title = apiShow.Title,
				Type = apiShow.Type
			};
		}

		private async Task<TvGenre> ConvertOrAddGenre(string genre)
		{
			var foundItem = _dbContext.Genres.FirstOrDefault(g => g.Name == genre);
			if (foundItem != null)
			{
				// Add it to the DB
				var addedGenre = _dbContext.Genres.Add(new TvGenre { Name = genre });
				await _dbContext.SaveChangesAsync();
				return addedGenre.Entity;
			}

			return foundItem;
		}
	}
}
