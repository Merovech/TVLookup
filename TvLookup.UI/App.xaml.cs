using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using TvLookup.Core.Services.Implementations;
using TvLookup.Core.Services.Interfaces;

namespace TvLookup.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IServiceProvider ServiceProvider
		{
			get; private set;
		}

		public App()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddLogging(builder =>
			{
				builder.ClearProviders();
				builder.SetMinimumLevel(LogLevel.Trace);
				builder.AddNLog("nlog.config");
			});

			RegisterInjectables(serviceCollection);
			ServiceProvider = serviceCollection.BuildServiceProvider();
		}

		private void OnStartup(object sender, StartupEventArgs e)
		{
			var mainWindow = ServiceProvider.GetService<MainWindow>();
			mainWindow.Show();
		}

		private static void RegisterInjectables(IServiceCollection serviceCollection)
		{
			// TODO: Expand this to use reflection, since we may have a bunch of injectables.
			serviceCollection.AddTransient<IApiService, ApiService>();
			serviceCollection.AddSingleton<MainWindow>();
		}
	}
}
