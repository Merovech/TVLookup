using Microsoft.EntityFrameworkCore;
using TvLookup.Core.Models;

namespace TvLookup.Core
{
	[DependencyInjectionType(DependencyInjectionType.Other)]
	public class DatabaseContext : DbContext
	{
		public DatabaseContext()
		{
		}

		public DatabaseContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<TvShow> Shows
		{
			get; set;
		}

		public DbSet<TvShowEpisode> Episodes
		{
			get; set;
		}

		public DbSet<TvGenre> Genres
		{
			get; set; 
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// TODO: Constant, or other setting
			optionsBuilder.UseSqlite("Data Source = tvlookup.db;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TvShow>().ToTable("Shows");
			modelBuilder.Entity<TvShowEpisode>().ToTable("Episodes");
			modelBuilder.Entity<TvGenre>().ToTable("Genres");

			modelBuilder.Entity<TvShowGenre>().HasKey(sg => new { sg.ShowId, sg.GenreId });
			modelBuilder.Entity<TvShowGenre>()
				.HasOne(sg => sg.Show)
				.WithMany(s => s.Genres)
				.HasForeignKey(sg => sg.ShowId);
			modelBuilder.Entity<TvShowGenre>()
				.HasOne(sg => sg.Genre)
				.WithMany(g => g.Shows)
				.HasForeignKey(sg => sg.GenreId);
		}
	}
}
