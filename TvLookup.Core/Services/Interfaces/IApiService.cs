using System.Collections.Generic;
using System.Threading.Tasks;
using TvLookup.Core.Models;

namespace TvLookup.Core.Services.Interfaces
{
	/// <summary>
	/// TVMaze API.  Used to get show information if it's not already in the database. 
	/// </summary>
	public interface IApiService
	{
		Task<List<TvShow>> FindShow(string searchString);
		
		Task<List<TvShowEpisode>> GetEpisodes(int showId);
	}
}
