using System.Collections.Generic;
using System.Threading.Tasks;
using TvLookup.Core.Models;
using TvLookup.Core.Models.Api;

namespace TvLookup.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IDatabaseService
	{
		Task<TvShow> GetShow(int id);

		Task<TvShowEpisode> GetEpisode(int showId, int seasonNumber, int episodeNumber);

		Task AddShow(ApiTvShow show);

		Task AddEpisodes(IList<ApiTvShowEpisode> episodes, int showId);

		Task CreateDatabase();
	}
}
