using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopM4_Models;
using System;

namespace ShopM4_DataMigrations.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<ShopM4_Models.MyModel> MyModel { get; set; }

        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<QueryHeader> QueryHeader { get; set; }
        public DbSet<QueryDetail> QueryDetail { get; set; }
    }
}
