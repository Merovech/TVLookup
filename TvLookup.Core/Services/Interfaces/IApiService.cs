using System.Collections.Generic;
using System.Threading.Tasks;
using TvLookup.Core.Models.Api;

namespace TvLookup.Core.Services.Interfaces
{
	/// <summary>
	/// TVMaze API.  Used to get show information if it's not already in the database. 
	/// </summary>
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IApiService
	{
		Task<List<ApiTvShow>> FindShow(string searchString);

		Task<List<ApiTvShowEpisode>> GetEpisodes(int showId);
	}
}
