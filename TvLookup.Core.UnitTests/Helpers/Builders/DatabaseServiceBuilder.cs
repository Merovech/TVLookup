using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TvLookup.Core.Models;
using TvLookup.Core.Models.Api;
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

		public List<ApiTvShow> GenerateTvShows(int count)
		{
			List<ApiTvShow> resultList = new List<ApiTvShow>();
			for (int i = 0; i < count; i++)
			{
				ApiTvShow show = new()
				{
					Id = i + 10,
					Title = $"Title {i}",
					PremiereDate = DateTime.Today.AddDays(-i),
					EndDate = DateTime.Today,
					Genres = new()
					{
						"Genre1",
						"Genre2"
					},
					Language = "en",
					Summary = $"Summary {i}",
					Type = "Some Type"
				};

				resultList.Add(show);
			}

			return resultList;
		}

		public List<ApiTvShowEpisode> GenerateApiEpisodes(int count)
		{
			List<ApiTvShowEpisode> returnList = new();

			for (int i = 0; i < count; i++)
			{
				ApiTvShowEpisode episode = new()
				{
					Id = 100 + i,
					Title = $"Title {i}",
					AirDate = DateTime.Today,
					EpisodeNumber = i + 1,
					SeasonNumber = i + 1,
					Summary = $"Summary for Title {i}",
					Type = "Drama"
				};

				returnList.Add(episode);
			}

			return returnList;
		}
	}
}
