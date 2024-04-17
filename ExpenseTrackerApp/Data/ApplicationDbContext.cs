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
    }
}
