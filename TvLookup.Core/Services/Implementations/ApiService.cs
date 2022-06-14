using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TvLookup.Core.Models;
using TvLookup.Core.Services.Interfaces;

namespace TvLookup.Core.Services.Implementations
{
	/// <summary>
	/// Implementation of portions of the TvMaze API
	/// </summary>
	// TODO: Logging
	public class ApiService : IApiService
	{
		private static HttpClient Client = new();
		private const string ROOT_URL = "https://api.tvmaze.com";

		public async Task<List<TvShow>> FindShow(string searchString)
		{
			var apiPath = $"/search/shows?q={WebUtility.HtmlEncode(searchString)}";
			var results = await ExecuteApi<List<TvShowSearchResult>>(apiPath);

			// Result order is already by relevance, so no need to re-sort.  However, we don't need the score.
			// Unsure if this is strictly necessary, but we're not dealing with a lot of data here so it can't
			// really hurt.
			var shows = results.Select(res => res.Show).ToList();

			// TODO: Again, logging
			foreach (var s in shows)
			{
				Debug.WriteLine(s.ToString());
			}

			return shows;
		}

		public async Task<List<TvShowEpisode>> GetEpisodes(int showId)
		{
			var apiPath = $"/shows/{showId}/episodes";
			var results = await ExecuteApi<List<TvShowEpisode>>(apiPath);

			// TODO: Again, logging
			foreach (var r in results)
			{
				Debug.WriteLine(r.ToString());
			}

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
