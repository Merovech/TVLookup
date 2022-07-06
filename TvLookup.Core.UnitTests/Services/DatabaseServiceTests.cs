using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvLookup.Core.Models.Api;
using TvLookup.Core.Services.Implementations;
using TvLookup.Core.Services.Interfaces;
using TvLookup.Core.UnitTests.Helpers.Builders;

namespace TvLookup.Core.UnitTests.Services
{
	[TestClass]
	public class DatabaseServiceTests : TestBase<IDatabaseService, DatabaseServiceBuilder>
	{
		private const string DB_NAME = "test-database.db";

		[TestInitialize]
		public virtual void InitializeTest()
		{
			DatabaseContext ctx = new DatabaseContext(DB_NAME);

			// Recreate a fresh DB each time
			ctx.Database.EnsureDeleted();
			ctx.Database.Migrate();
			ctx.Database.OpenConnection();
			ctx.Database.ExecuteSqlRaw("PRAGMA journal_mode=DELETE");
			ctx.Database.CloseConnection();

			Builder = new DatabaseServiceBuilder
			{
				Logger = new Mock<ILogger<DatabaseService>>().Object,
				Context = ctx
			};

			Target = Builder.Build();
		}

		[TestClass]
		public class ConstructorTests : DatabaseServiceTests
		{
			[TestInitialize]
			public override void InitializeTest()
			{
				Builder = new();
			}

			[TestMethod]
			public virtual void Instantiates_Correctly_With_Valid_Values()
			{
				Builder.Logger = new Mock<ILogger<DatabaseService>>().Object;
				Builder.Context = new Mock<DatabaseContext>("some_name").Object;

				Builder.Build();
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

		[TestClass]
		public class AddShowTests : DatabaseServiceTests
		{
			[TestMethod]
			public async virtual Task Should_Add_Successfully()
			{
				ApiTvShow showToAdd = new()
				{
					Title = "Test show",
					EndDate = DateTime.Now,
					PremiereDate = DateTime.Now.AddDays(-1),
					Genres = new() { "Genre1", "Genre2" },
					Id = 10,
					Language = "en",
					Summary = "Show summary",
					Type = "Drama"
				};

				await Target.AddShow(showToAdd);
				Assert.AreEqual(1, Builder.Context.Shows.Count());
				Assert.AreEqual(2, Builder.Context.Genres.Count());

				var foundShow = await Builder.Context.Shows.FirstOrDefaultAsync();
				Assert.IsNotNull(foundShow);
				Assert.AreEqual(showToAdd.Title, foundShow.Title);
				Assert.AreEqual(showToAdd.Summary, foundShow.Summary);
				Assert.AreEqual(showToAdd.Type, foundShow.Type);
				Assert.AreEqual(1, foundShow.Id);
				Assert.AreEqual(showToAdd.Language, foundShow.Language);
				Assert.AreEqual(showToAdd.Genres.Count, foundShow.Genres.Count);
				Assert.AreEqual(1, foundShow.Id);
				Assert.AreEqual(showToAdd.Id, foundShow.ApiId);

				var insertedGenres = foundShow.Genres.Select(g => g.Genre.Name).ToList();
				var expectedGenres = showToAdd.Genres;
				insertedGenres.Sort();
				expectedGenres.Sort();
				Assert.IsTrue(insertedGenres.SequenceEqual(expectedGenres));
			}

			[TestMethod]
			public async virtual Task Should_Not_Duplicate_Reused_Genres()
			{
				ApiTvShow showToAdd1 = new()
				{
					Title = "Test show",
					EndDate = DateTime.Now,
					PremiereDate = DateTime.Now.AddDays(-1),
					Genres = new() { "Genre1", "Genre2" },
					Id = 10,
					Language = "en",
					Summary = "Show summary",
					Type = "Drama"
				};

				ApiTvShow showToAdd2 = new()
				{
					Title = "Test show 2",
					EndDate = DateTime.Now,
					PremiereDate = DateTime.Now.AddDays(-1),
					Genres = new() { "Genre1", "Genre2" },
					Id = 10,
					Language = "en",
					Summary = "Show summary",
					Type = "Drama"
				};

				await Target.AddShow(showToAdd1);
				await Target.AddShow(showToAdd2);
				Assert.AreEqual(2, Builder.Context.Shows.Count());

				var genres = Builder.Context.Genres.ToList();
				Assert.AreEqual(2, genres.Count);

				var foundShow1 = await Builder.Context.Shows.FirstOrDefaultAsync(s => s.Title == showToAdd1.Title);
				var foundShow2 = await Builder.Context.Shows.FirstOrDefaultAsync(s => s.Title == showToAdd2.Title);
				Assert.IsNotNull(foundShow1);
				Assert.IsNotNull(foundShow2);
				var insertedGenres1 = foundShow1.Genres.Select(g => g.Genre).OrderBy(g => g.Id).ToList();
				var insertedGenres2 = foundShow2.Genres.Select(g => g.Genre).OrderBy(g => g.Id).ToList();
				Assert.IsTrue(genres.SequenceEqual(insertedGenres1));
				Assert.IsTrue(genres.SequenceEqual(insertedGenres2));
			}
		}

		[TestClass]
		public class GetShowTests : DatabaseServiceTests
		{
			[TestMethod]
			public async Task Should_Return_Show_For_Valid_Inputs()
			{
				var expected = await InsertValidData(2);
				for (int i = 0; i < expected.Count; i++)
				{
					var targetId = i + 10;
					var expectedShow = expected[i];
					var actualShow = await Target.GetShow(targetId);

					Assert.IsNotNull(actualShow, nameof(actualShow));
					Assert.AreEqual(i + 1, actualShow.Id);
					Assert.AreEqual(expectedShow.Id, actualShow.ApiId);
					Assert.AreEqual(expectedShow.Title, actualShow.Title);
					Assert.AreEqual(expectedShow.Language, actualShow.Language);
					Assert.AreEqual(expectedShow.Summary, actualShow.Summary);
					Assert.AreEqual(expectedShow.PremiereDate, actualShow.PremiereDate);
					Assert.AreEqual(expectedShow.Type, actualShow.Type);
					Assert.AreEqual(expectedShow.EndDate, actualShow.EndDate);
					Assert.IsNotNull(actualShow.Genres);
					Assert.AreEqual(expectedShow.Genres.Count, actualShow.Genres.Count);
					for (int j = 0; j < actualShow.Genres.Count; j++)
					{
						Assert.AreEqual(expectedShow.Genres[j], actualShow.Genres[j].Genre.Name);
					}
				}
			}

			[TestMethod]
			public async Task Should_Return_Null_For_Not_Found()
			{
				var result = await Target.GetShow(100);
				Assert.IsNull(result, nameof(result));
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Throw_When_Duplicate_Show_Is_Found()
			{
				await InsertDuplicateData();
				_ = await Target.GetShow(10);
			}

			private async Task<List<ApiTvShow>> InsertValidData(int count)
			{
				List<ApiTvShow> result = new List<ApiTvShow>();
				for (int i = 0; i < count; i++)
				{
					ApiTvShow show = new()
					{
						Id = i + 10,
						Title = $"Title {i}",
						PremiereDate = DateTime.Today.AddDays(-i),
						EndDate = DateTime.Today,
						Genres = new()
					{
						"Genre1",
						"Genre2"
					},
						Language = "en",
						Summary = $"Summary {i}",
						Type = "Some Type"
					};

					await Target.AddShow(show);
					result.Add(show);
				}

				return result;
			}

			private async Task InsertDuplicateData()
			{
				List<ApiTvShow> result = new List<ApiTvShow>();
				ApiTvShow show = new()
				{
					Id = 10,
					Title = "Title",
					PremiereDate = DateTime.Today.AddDays(-1),
					EndDate = DateTime.Today,
					Genres = new()
					{
						"Genre1",
						"Genre2"
					},
					Language = "en",
					Summary = "Summary",
					Type = "Some Type"
				};

				await Target.AddShow(show);
				await Target.AddShow(show);
			}
		}
	}
}
