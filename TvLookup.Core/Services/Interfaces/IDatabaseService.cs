using System.Collections.Generic;
using System.Threading.Tasks;
using TvLookup.Core.Models;

namespace TvLookup.Core.Services.Interfaces
{
	[DependencyInjectionType(DependencyInjectionType.Interface)]
	public interface IDatabaseService
	{
		Task<TvShow> GetShow();

		Task<TvShowEpisode> GetEpisode();

		Task AddShow(TvShow show);

		Task AddEpisodes(IList<TvShowEpisode> episodes);

		Task CreateDatabase();
	}
}
