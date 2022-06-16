using System.Linq;
using System.Windows;
using Microsoft.Extensions.Logging;
using TvLookup.Core;
using TvLookup.Core.Services.Interfaces;

namespace TvLookup.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[DependencyInjectionType(DependencyInjectionType.Singleton)]
	public partial class MainWindow : Window
	{
		private readonly IApiService _apiService;

		public MainWindow(IApiService apiService)
		{
			_apiService = apiService;
			InitializeComponent();
		}

		// Temporary, just to test out the API
		private async void OnClick(object sender, RoutedEventArgs e)
		{
			// All of this is dumped into the debug window for the time being.  Until the Core functionality
			// is fleshed out, the UI will serve only to allow me to do live tests.  Eventually those tests
			// wil go into unit tests and this will turn into an MVVM WPF UI.
			var shows = await _apiService.FindShow(this.tmpInput.Text);
			if (shows.Any())
			{
				var episodes = await _apiService.GetEpisodes(shows.First().Id);
			}
		}
	}
}
