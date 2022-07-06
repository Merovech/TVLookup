using System.Collections.Generic;

namespace TvLookup.Core.Models
{
	public class TvGenre
	{
		public TvGenre()
		{
			Shows = new();
		}

		public int Id
		{
			get; set;
		}

		public string Name
		{
			get; set;
		}

		public List<TvShowGenre> Shows
		{
			get; set;
		}
	}
}
