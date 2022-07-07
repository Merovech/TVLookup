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

		protected async Task<List<ApiTvShow>> InsertShows(int count, bool includeDuplicate)
		{
			var resultList = Builder.GenerateTvShows(count);
			foreach (var show in resultList)
			{
				await Target.AddShow(show);
			}

			if (includeDuplicate)
			{
				await Target.AddShow(resultList.First());
			}

			return resultList;
		}

		protected async Task<List<ApiTvShowEpisode>> InsertEpisodes(int count, int showId, bool includeDuplicates)
		{
			var resultList = Builder.GenerateApiEpisodes(count);
			if (includeDuplicates)
			{
				resultList.Add(resultList.First());
			}

			await Target.AddEpisodes(resultList, showId);
			return resultList;
		}

		[TestClass]
		public class AddShowTests : DatabaseServiceTests
		{
			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public async virtual Task Should_Throw_On_Null_Show()
			{
				await Target.AddShow(null);
			}

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
			[ExpectedException(typeof(ArgumentException))]
			public async Task Should_Throw_For_Zero_Id()
			{
				await Target.GetShow(0);
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentException))]
			public async Task Should_Throw_For_Negative_Id()
			{
				await Target.GetShow(-1);
			}

			[TestMethod]
			public async Task Should_Return_Null_For_Not_Found()
			{
				var result = await Target.GetShow(100);
				Assert.IsNull(result, nameof(result));
			}

			[TestMethod]
			public async Task Should_Return_Show_For_Valid_Inputs()
			{
				var expected = await InsertShows(2, false);
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
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Throw_When_Duplicate_Show_Is_Found()
			{
				await InsertShows(1, true);
				_ = await Target.GetShow(10);
			}
		}

		[TestClass]
		public class AddEpisodesTests : DatabaseServiceTests
		{
			[TestInitialize]
			public override void InitializeTest()
			{
				base.InitializeTest();
				Task.Run(async () => await InsertShows(3, false)).Wait();
			}

			[TestMethod]
			[ExpectedException(typeof(ArgumentNullException))]
			public async Task Should_Throw_On_List()
			{
				await Target.AddEpisodes(null, 10);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Throw_On_Empty_List()
			{
				await Target.AddEpisodes(new List<ApiTvShowEpisode>(), 10);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Throw_On_Nonexistent_Show_Id()
			{
				var episode = Builder.GenerateApiEpisodes(1);
				await Target.AddEpisodes(episode, 100000);
			}

			[TestMethod]
			public async Task Should_Add_Episodes_Successfully()
			{
				var episodes = Builder.GenerateApiEpisodes(10).OrderBy(e => e.Id).ToList();
				var show = Builder.Context.Shows.First();
				await Target.AddEpisodes(episodes, show.ApiId);

				var foundEpisodes = Builder.Context.Episodes.OrderBy(e => e.ApiId).ToList();
				for (int i = 0; i < foundEpisodes.Count; i++)
				{
					var actualEpisode = foundEpisodes[i];
					var expectedEpisode = episodes[i];
					Assert.AreEqual(show.Id, actualEpisode.ShowId);
					Assert.AreEqual(expectedEpisode.Id, actualEpisode.ApiId);
					Assert.AreEqual(expectedEpisode.Title, actualEpisode.Title);
					Assert.AreEqual(expectedEpisode.Summary, actualEpisode.Summary);
					Assert.AreEqual(expectedEpisode.SeasonNumber, actualEpisode.SeasonNumber);
					Assert.AreEqual(expectedEpisode.EpisodeNumber, actualEpisode.EpisodeNumber);
					Assert.AreEqual(expectedEpisode.AirDate, actualEpisode.AirDate);
					Assert.AreEqual(expectedEpisode.Type, actualEpisode.Type);
					Assert.IsTrue(actualEpisode.Id > 0);
				}
			}
		}

		[TestClass]
		public class GetEpisodesTests : DatabaseServiceTests
		{
			[TestMethod]
			[DynamicData(nameof(GetInvalidParameterValidationData), DynamicDataSourceType.Method)]
			[ExpectedException(typeof(ArgumentException))]
			public async Task Show_Throw_When_Any_Parameter_Is_Less_Than_1(int showId, int seasonNumber, int episodeNumber)
			{
				await Target.GetEpisode(showId, seasonNumber, episodeNumber);
			}


			[TestMethod]
			public async Task Should_Return_Valid_Result_When_Episode_Exists()
			{
				await InsertShows(1, false);
				var show = Builder.Context.Shows.First();
				var episodes = await InsertEpisodes(5, show.ApiId, false);
				var expectedEpisode = episodes.First();

				var result = await Target.GetEpisode(show.Id, expectedEpisode.SeasonNumber, expectedEpisode.EpisodeNumber);
				Assert.IsNotNull(result);
				Assert.AreEqual(show.Id, result.ShowId);
				Assert.AreEqual(expectedEpisode.SeasonNumber, result.SeasonNumber);
				Assert.AreEqual(expectedEpisode.EpisodeNumber, result.EpisodeNumber);
				Assert.AreEqual(expectedEpisode.Title, result.Title);
				Assert.AreEqual(expectedEpisode.Summary, result.Summary);
				Assert.AreEqual(expectedEpisode.Type, result.Type);
				Assert.AreEqual(expectedEpisode.AirDate, result.AirDate);
			}

			[TestMethod]
			[ExpectedException(typeof(InvalidOperationException))]
			public async Task Should_Throw_When_Duplicate_Episode_Is_Found()
			{
				await InsertShows(1, false);
				var show = Builder.Context.Shows.First();
				var episodes = await InsertEpisodes(5, show.Id, true);
				var expectedEpisode = episodes.First();

				_ = await Target.GetEpisode(show.ApiId, expectedEpisode.SeasonNumber, expectedEpisode.EpisodeNumber);
			}

			[TestMethod]
			[DynamicData(nameof(GetValidParameterData), DynamicDataSourceType.Method)]
			public async Task Should_Return_Null_When_Episode_Is_Not_Found(int showId, int seasonNumber, int episodeNumber)
			{
				// We should get null whenever we passed in valid values for an item that's not in the database.
				// This could be when a show doesn't exist, the season doesn't exist for the show, or the episode
				// doesn't exist for the season.
				await InsertShows(1, false);
				var show = Builder.Context.Shows.First();
				await InsertEpisodes(5, show.ApiId, false);

				var result = await Target.GetEpisode(showId, seasonNumber, episodeNumber);
				Assert.IsNull(result);

			}

			// With three parameters, sometimes testing multiple values, DynamicData prevents us from having to write
			// a billion tiny tests.
			private static IEnumerable<object[]> GetInvalidParameterValidationData()
			{
				return new List<object[]>()
				{
					// Format: { showId, seasonNumber, episodeNumber }
					new object[] { 0, 1, 1 },
					new object[] { -1, 1, 1 },
					new object[] { 1, 0, 1 },
					new object[] { 1, -1, 1 },
					new object[] { 1, 1, 0 },
					new object[] { 1, 1, -1 }
				};
			}

			private static IEnumerable<object[]> GetValidParameterData()
			{
				return new List<object[]>()
				{
					// Format { showId, seasonNumber, episodeNumber }
					new object[] { 100, 1, 1},
					new object[] { 10, 100, 1},
					new object[] { 10, 1, 100}
				};
			}
		}
	}
}
