using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> products { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}

