using ExpenseTrackerApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<CategoryColor> categoriesColors { get; set; }
        public DbSet<CategoryType> categoriesTypes { get; set; }
        public DbSet<CategoryIcon> categoriesIcons { get; set; }
        public DbSet<SocialLink> socialLinks { get; set; }
        public DbSet<Footer> footers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal properties
            modelBuilder.Entity<Category>()
                .Property(c => c.monthlyBudget)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ApplicationUser>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SocialLink>()
                .HasOne(s => s.Footer)
                .WithMany()
                .HasForeignKey(s => s.FooterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure foreign keys with no cascade delete
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ApplicationUser)
                .WithMany()
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Similar configurations for other models
            modelBuilder.Entity<Category>()
                .HasOne(c => c.CategoryType)
                .WithMany()
                .HasForeignKey(c => c.CategoryTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.CategoryIcon)
                .WithMany()
                .HasForeignKey(c => c.CategoryIconId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.CategoryColor)
                .WithMany()
                .HasForeignKey(c => c.CategoryColorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
