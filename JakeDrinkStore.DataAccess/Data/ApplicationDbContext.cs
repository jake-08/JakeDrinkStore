using JakeDrinkStore.Models;
using Microsoft.EntityFrameworkCore;

namespace JakeDrinkStore.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        // Create Tables in Database using Models
        public DbSet<Category> Categories { get; set; }
        public DbSet<DrinkType> DrinkTypes { get; set; }

    }
}
