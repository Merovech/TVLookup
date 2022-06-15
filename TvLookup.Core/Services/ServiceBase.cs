using Microsoft.Extensions.Logging;

namespace TvLookup.Core.Services
{
	public abstract class ServiceBase<T>
	{
		protected ILogger<T> Logger
		{
			get; private set;
		}

		protected ServiceBase(ILogger<T> logger)
		{
			Logger = logger;
		}
	}
}
