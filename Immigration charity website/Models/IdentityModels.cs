using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Immigration_charity_website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }
        public virtual ICollection<Rates> Ratings { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
           : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Rates> Rates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rates>()
                .HasKey(r => r.Id)
                .ToTable("Rates");

            modelBuilder.Entity<Rates>()
                .Property(r => r.CreatedAt)
                .HasColumnType("datetime2")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed); // Consider removing if setting DateTime manually

            modelBuilder.Entity<Rates>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .WillCascadeOnDelete(false);

            // Ensure the IndexAttribute is used correctly
            modelBuilder.Entity<Rates>()
                .Property(r => r.UserId)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UserId")));
        }
    }

}
