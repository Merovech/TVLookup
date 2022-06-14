using System;
using System.Text.Json.Serialization;

namespace TvLookup.Core.Models
{
	/// <summary>
	/// Minimal set of properties from the TVMaze API that the app uses.  In general there's a lot more here,
	/// but as the app currently doesn't need it they'll just clutter the data with useless information.  When
	/// a new feature demands a property that isn't here, we can add it and it will automaticall be deserialized.
	/// </summary>
	public class TvShowEpisode
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

		[JsonPropertyName("season")]
		public int Season
		{
			get; set;
		}

		[JsonPropertyName("number")]
		public int Number
		{
			get; set;
		}

		[JsonPropertyName("type")]
		public string Type
		{
			get; set;
		}

		[JsonPropertyName("airdate")]
		public DateTime AirDate
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
			return $"S{Season:00}E{Number:00}: {Name} ({AirDate:MM/dd/yyyy})";
		}
	}
}
