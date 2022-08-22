using GymBokning.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymBokning.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUserGymClass>()
                .HasKey(t => new { t.ApplicationUserId,t.GymClassId });
            // queryfilter here for showing gymclasses which starts in future only
          //  builder.Entity<GymClass>().HasQueryFilter(g => g.StartTime > DateTime.Now);
        }
        public DbSet<GymClass> GymClasses { get; set; }

    }
}