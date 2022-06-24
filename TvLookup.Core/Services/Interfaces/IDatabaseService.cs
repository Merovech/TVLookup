using System.Collections.Generic;
using System.Threading.Tasks;
using TvLookup.Core.Models.Api;

namespace TvLookup.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IDatabaseService
	{
		Task<ApiTvShow> GetShow(int id);

		Task<ApiTvShowEpisode> GetEpisode(int showId, int seasonNumber, int episodeNumber);

		Task AddShow(ApiTvShow show);

		Task AddEpisodes(IList<ApiTvShowEpisode> episodes);

		Task CreateDatabase();
	}
}
