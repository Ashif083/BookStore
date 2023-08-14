using BookStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BookTypes> Booktypes { get; set; }

        public DbSet<SpecialTags> SpecialTags { get; set; }

        public DbSet<Books> Books { get; set; }

        public DbSet<Order> Order { get; set; }
        
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<ApplicationUsers> ApplicationUsers { get; set; }


    }
}