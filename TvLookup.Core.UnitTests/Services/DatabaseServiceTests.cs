using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvLookup.Core.Services.Implementations;
using TvLookup.Core.Services.Interfaces;
using TvLookup.Core.UnitTests.Helpers.Builders;

namespace TvLookup.Core.UnitTests.Services
{
	[TestClass]
	public  class DatabaseServiceTests : TestBase<IDatabaseService, DatabaseServiceBuilder>
	{
		private const string DB_NAME = "test-database.db";

		[TestInitialize]
		public virtual void InitializeTest()
		{
			var options = new DbContextOptionsBuilder<DatabaseContext>()
				.UseSqlite($"Data Source={DB_NAME};")
				.Options;

			DatabaseContext ctx = new DatabaseContext(options);

			// Recreate a fresh DB each time
			Task.Run(() => ctx.Database.MigrateAsync()).Wait();

			Builder = new DatabaseServiceBuilder();
		}

		[TestCleanup]
		public void CleanupTest()
		{

			if (File.Exists(DB_NAME))
			{
				File.Delete(DB_NAME);
			}
		}

		[TestClass]
		public class ConstructorTests : DatabaseServiceTests
		{
			[TestMethod]
			public virtual void Instantiates_Correctly_With_Valid_Values()
			{
				Builder.Logger = new Mock<ILogger<DatabaseService>>().Object;
				Builder.Context = new Mock<DatabaseContext>().Object;

				Builder.Build();

				// If we got here, no issues
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public virtual void Should_Throw_With_Null_Context()
			{
				Builder.Logger = new Mock<ILogger<DatabaseService>>().Object;
				Builder.Build();
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public void Should_Throw_With_Null_Logger()
			{
				Builder.Context = new DatabaseContext();
				Builder.Build();
			}
		}
	}
}
