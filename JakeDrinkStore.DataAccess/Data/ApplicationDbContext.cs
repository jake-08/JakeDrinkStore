﻿using JakeDrinkStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JakeDrinkStore.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        // Create Tables in Database using Models
        public DbSet<Tag> Tags { get; set; }
        public DbSet<DrinkType> DrinkTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set Scehema Name e.g. -> drinkstore.Tags
            modelBuilder.HasDefaultSchema("drinkstore");

            // this method is for IdentityDbContext, otherwise will get error
            base.OnModelCreating(modelBuilder);
        }
    }
}
