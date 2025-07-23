using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoftIToFlix.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SoftIToFlix.Data
{
	public class ApplicationContext : IdentityDbContext<SoftIToFlixUser,SoftIToRole,long>
	{

		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base (options)
		{
		}
		public DbSet<Category>? Categories { get; set; }
        public DbSet<Media>? Medias { get; set; }
        public DbSet<MediaCategory>? MediaCategories { get; set; }
       
        public DbSet<Star>? Stars { get; set; }
        public DbSet<Director>? Directors { get; set; }
        public DbSet<MediaStar>? MediaStars { get; set; }
        public DbSet<MediaDirector>? MediaDirectors { get; set; }
        public DbSet<Episode>? Episodes { get; set; }
        public DbSet<Restriction>? Restrictions { get; set; }
        public DbSet<MediaRestriction>? MediaRestrictions { get; set; }
        public DbSet<UserWatched>? UserWatcheds { get; set; }
        public DbSet<UserFavorite>? UserFavorites { get; set; }
        public DbSet<UserPlan>? UserPlans { get; set; }
        public DbSet<Plan>? Plans { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            
            modelBuilder.Entity<MediaCategory>().HasKey(r => new { r.MediaId, r.CategoryId });
            modelBuilder.Entity<UserPlan>().HasKey(r => new { r.UserId, r.PlanId });
            modelBuilder.Entity<MediaStar>().HasKey(s => new { s.MediaId, s.StarId });
            modelBuilder.Entity<MediaDirector>().HasKey(r => new { r.MediaId, r.DirectorId });
            modelBuilder.Entity<MediaRestriction>().HasKey(s => new { s.MediaId, s.RestrictionId });
            modelBuilder.Entity<UserFavorite>().HasKey(s => new { s.UserId, s.MediaId });
            modelBuilder.Entity<UserWatched>().HasKey(s => new {  s.UserId , s.EpisodeId, });
            modelBuilder.Entity<Episode>().HasIndex(s => new { s.SeasonNumber, s.MediaId, s.EpisodeNumber}).IsUnique(true);

            base.OnModelCreating(modelBuilder);


        }
    }
}

