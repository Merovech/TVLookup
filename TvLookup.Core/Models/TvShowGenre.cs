namespace TvLookup.Core.Models
{
	public class TvShowGenre
	{
		public int ShowId
		{
			get; set;
		}

		public TvShow Show
		{
			get; set;
		}

		public int GenreId
		{
			get; set;
		}

		public TvGenre Genre
		{
			get; set;
		}
	}
}
