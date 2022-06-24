using System.Text.Json.Serialization;

namespace TvLookup.Core.Models.Api
{
	/// <summary>
	/// Search result returned by TVMaze when looking for a show by name.
	/// </summary>
	public class ApiTvShowSearchResult
	{
		[JsonPropertyName("score")]
		public double Resultscore
		{
			get; set;
		}

		[JsonPropertyName("show")]
		public ApiTvShow Show
		{
			get; set;
		}
	}
}
