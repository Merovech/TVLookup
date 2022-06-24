using System;

namespace TvLookup.Core.Models
{
	/// <summary>
	/// Minimal set of properties from the TVMaze API that the app uses.  In general there's a lot more here,
	/// but as the app currently doesn't need it they'll just clutter the data with useless information.  When
	/// a new feature demands a property that isn't here, we can add it and it will automaticall be deserialized.
	/// </summary>
	public class TvShowEpisode
	{
		public int Id
		{
			get; set;
		}

		public int ApiId
		{
			get; set;
		}

		public string Title
		{
			get; set;
		}

		public int SeasonNumber
		{
			get; set;
		}

		public int EpisodeNumber
		{
			get; set;
		}

		public string Type
		{
			get; set;
		}

		public DateTime AirDate
		{
			get; set;
		}

		public string Summary
		{
			get; set;
		}

		// Temporary, for debugging and early-deveopment purposes
		// TODO: Remove this, maybe.
		public override string ToString()
		{
			return $"S{SeasonNumber:00}E{EpisodeNumber:00}: {Title} ({AirDate:MM/dd/yyyy})";
		}
	}
}
