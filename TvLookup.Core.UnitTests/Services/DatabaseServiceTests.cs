using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvLookup.Core.Services.Interfaces;
using TvLookup.Core.UnitTests.Helpers.Builders;

namespace TvLookup.Core.UnitTests.Services
{
	// Note that we're not going to test that CreateDatabase() works, since it's used in InitializeTest() for everything
	// except for the constructor tests.
	[TestClass]
	public  class DatabaseServiceTests : TestBase<IDatabaseService, DatabaseServiceBuilder>
	{
		private const string DB_NAME = "test-database.db";

		[TestInitialize]
		public virtual void InitializeTest()
		{
			Builder = new DatabaseServiceBuilder();
			Target = Builder.Build();

			// Recreate a fresh DB each time
			Task.Run(() => Target.CreateDatabase()).Wait();
		}

		[TestCleanup]
		public void CleanupTest()
		{

			if (File.Exists(DB_NAME))
			{
				File.Delete(DB_NAME);
			}
		}
	}
}
