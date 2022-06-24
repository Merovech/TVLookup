using System.Collections.Generic;

namespace TvLookup.Core.Models
{
	public class TvGenre
	{
		public int Id
		{
			get; set;
		}

		public string Name
		{
			get; set;
		}

		public ICollection<TvShowGenre> Shows
		{
			get; set;
		}
	}
}
