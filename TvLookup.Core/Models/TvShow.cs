using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TvLookup.Core.Models
{
	/// <summary>
	/// Minimal set of properties from the TVMaze API that the app uses.  In general there's a lot more here,
	/// but as the app currently doesn't need it they'll just clutter the data with useless information.  When
	/// a new feature demands a property that isn't here, we can add it and it will automaticall be deserialized.
	/// </summary>
	public class TvShow
	{
		[JsonPropertyName("id")]
		public int Id
		{
			get; set;
		}

		[JsonPropertyName("name")]
		public string Name
		{
			get; set;
		}

		[JsonPropertyName("type")]
		public string Type
		{
			get; set;
		}

		[JsonPropertyName("language")]
		public string Language
		{
			get; set;
		}

		[JsonPropertyName("genres")]
		public List<string> Genres
		{
			get; set;
		}

		[JsonPropertyName("premiered")]
		public DateTime? Premiered
		{
			get; set;
		}

		[JsonPropertyName("ended")]
		public DateTime? Ended
		{
			get; set;
		}

		[JsonPropertyName("summary")]
		public string Summary
		{
			get; set;
		}

		// Temporary, for debugging and early-deveopment purposes
		// TODO: Remove this, maybe.
		public override string ToString()
		{
			return $"{Name} ({Premiered?.Year.ToString() ?? "unknown"}-{Ended?.Year})";
		}
	}
}
