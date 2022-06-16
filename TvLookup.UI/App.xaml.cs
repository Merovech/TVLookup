using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using TvLookup.Core;
using TvLookup.Core.Services.Implementations;
using TvLookup.Core.Services.Interfaces;

namespace TvLookup.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly string[] loadableAssemblyNames = new[]
		{
			"TvLookup.Core",
			"TvLookup.UI"
		};

		public static IServiceProvider ServiceProvider
		{
			get; 
			private set;
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

		private void RegisterInjectables(IServiceCollection serviceCollection)
		{
			var logger = NLog.LogManager.GetCurrentClassLogger();

			var injectables = GetAllReferencedAssemblyTypes();
			var interfaceList = new List<Type>();
			var implementationList = new List<Type>();
			var singletonList = new List<Type>();
			var otherList = new List<Type>();

			foreach (var inj in injectables)
			{
				AddTypeForInjection(inj, interfaceList, implementationList, singletonList, otherList);
			}

			// We can do this in one loop, using the larger of the interface and other lists as our counter.
			// We're taking advantage of the fact that interfaces and implementations should have the same name,
			// on with an I prefix on the interface.  So they can be done in parallel.
			int i = 0;
			while (i < interfaceList.Count || i < singletonList.Count || i < otherList.Count)
			{
				if (i < interfaceList.Count)
				{
					logger.Log(NLog.LogLevel.Info, "DI Registration: {interfacename}, {implementationname} (interface)", interfaceList[i].Name, implementationList[i].Name);
					serviceCollection.AddTransient(interfaceList[i], implementationList[i]);
				}

				if (i < singletonList.Count)
				{
					logger.Log(NLog.LogLevel.Info, "DI Registration: {injectable} (singleton)", singletonList[i].Name);
					serviceCollection.AddSingleton(singletonList[i]);
				}

				if (i < otherList.Count)
				{
					logger.Log(NLog.LogLevel.Info, "DI Registration: {injectable} (other)", otherList[i].Name);
					serviceCollection.AddTransient(otherList[i]);
				}

				i++;
			}
		}

		private static void AddTypeForInjection(Type t, List<Type> interfaceList, List<Type> implementationList, List<Type> singletonList, List<Type> otherList)
		{
			// Check to see if there's a DependencyInjectionType attribute
			var attribute = t.GetCustomAttribute<DependencyInjectionTypeAttribute>();
			if (attribute == null)
			{
				return;
			}

			switch (attribute.Type)
			{
				case DependencyInjectionType.Service:
					implementationList.Add(t);
					break;

				case DependencyInjectionType.Interface:
					interfaceList.Add(t);
					break;

				case DependencyInjectionType.Singleton:
					singletonList.Add(t);
					break;

				case DependencyInjectionType.Other:
					otherList.Add(t);
					break;

				case DependencyInjectionType.None:
				default:
					break;
			}
		}

		private List<Type> GetAllReferencedAssemblyTypes()
		{
			var referencedAssemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			var validAssemblies = referencedAssemblies.Where(a => loadableAssemblyNames.Contains(a.Name)).ToList();

			var returnList = new List<Type>();
			foreach (var assm in validAssemblies)
			{
				var loadedAssembly = Assembly.Load(assm);
				returnList.AddRange(loadedAssembly.GetTypes());
			}

			// Don't forget the executing assembly itself!
			returnList.AddRange(Assembly.GetExecutingAssembly().GetTypes());
			return returnList;
		}
	}
}
