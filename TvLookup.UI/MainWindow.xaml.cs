using System.Linq;
using System.Windows;
using TvLookup.Core.Services.Implementations;

namespace TvLookup.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		// Temporary, just to test out the API
		private async void OnClick(object sender, RoutedEventArgs e)
		{
			ApiService svc = new ApiService();

			// All of this is dumped into the debug window for the time being.  Until the Core functionality
			// is fleshed out, the UI will serve only to allow me to do live tests.  Eventually those tests
			// wil go into unit tests and this will turn into an MVVM WPF UI.
			var shows = await svc.FindShow(this.tmpInput.Text);
			if (shows.Any())
			{
				var episodes = await svc.GetEpisodes(shows.First().Id);
			}
		}
	}
}
