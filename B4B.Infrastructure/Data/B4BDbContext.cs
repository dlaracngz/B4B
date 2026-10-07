using B4B.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Data
{
    public class B4BDbContext : DbContext
    {
        public B4BDbContext(DbContextOptions<B4BDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Admin> Admins { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(B4BDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}