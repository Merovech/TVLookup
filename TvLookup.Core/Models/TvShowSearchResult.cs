using System.Text.Json.Serialization;

namespace TvLookup.Core.Models
{
	/// <summary>
	/// Search result returned by TVMaze when looking for a show by name.
	/// </summary>
	public class TvShowSearchResult
	{
		[JsonPropertyName("score")]
		public double Resultscore
		{
			get; set;
		}

		[JsonPropertyName("show")]
		public TvShow Show
		{
			get; set;
		}
	}
}
