using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext (DbContextOptions<RestaurantContext> options)
            : base(options)
        {
        }

        public DbSet<Backend.Models.Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<Backend.Models.ProductOrder> ProductOrders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasOne(u => u.ShoppingCart)
                .WithOne(c => c.User)
                .HasForeignKey<ShoppingCart>(b => b.UserId);
 

        }
    }
}
