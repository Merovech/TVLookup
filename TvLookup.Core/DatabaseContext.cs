using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TvLookup.Core.Models;
using TvLookup.Core.Models.Api;

namespace TvLookup.Core
{
	public class DatabaseContext : DbContext
	{
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
