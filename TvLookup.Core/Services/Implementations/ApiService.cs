using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TvLookup.Core.Models.Api;
using TvLookup.Core.Services.Interfaces;

namespace TvLookup.Core.Services.Implementations
{
	/// <summary>
	/// Implementation of portions of the TvMaze API
	/// </summary>
	[DependencyInjectionType(DependencyInjectionType.Service)]
	public class ApiService : ServiceBase<ApiService>, IApiService
	{
		private static readonly HttpClient Client = new();
		private const string ROOT_URL = "https://api.tvmaze.com";

		public ApiService(ILogger<ApiService> logger) : base(logger)
		{
		}

		public async Task<List<ApiTvShow>> FindShow(string searchString)
		{
			var apiPath = $"/search/shows?q={WebUtility.HtmlEncode(searchString)}";
			var results = await ExecuteApi<List<ApiTvShowSearchResult>>(apiPath);

			// Result order is already by relevance, so no need to re-sort.  However, we don't need the score.
			// Unsure if this is strictly necessary, but we're not dealing with a lot of data here so it can't
			// really hurt.
			var shows = results.Select(res => res.Show).ToList();

			Logger.LogInformation("Found {count} shows.", shows.Count);
			return shows;
		}

		public async Task<List<ApiTvShowEpisode>> GetEpisodes(int showId)
		{
			var apiPath = $"/shows/{showId}/episodes";
			var results = await ExecuteApi<List<ApiTvShowEpisode>>(apiPath);

			Logger.LogInformation("Found {count} episodes.", results.Count);
			return results;
		}

		private async Task<T> ExecuteApi<T>(string apiPath)
		{
			var response = (await Client.GetAsync($"{ROOT_URL}{apiPath}")).EnsureSuccessStatusCode();
			var json = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(json);
		}
	}
}
