using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;


namespace CrawlerVNEXPRESS.Models
{
    public class ClawlerContext : DbContext
    {
        // public ClawlerContext(DbContextOptions<ClawlerContext> options)
        //     : base(options)
        // {
        // }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        //UseInMemoryDatabase
        optionsBuilder.UseInMemoryDatabase("Newss");
        //optionsBuilder.UseSqlite(@"Data Source=DataVnexpress.db");
        //optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=TenDataBase;Trusted_Connection=True;");
        
        //Config Unique
        //  optionsBuilder.Entity<News>()
        //     .HasIndex(p => new { p.FirstName, p.LastName })
        //     .IsUnique(true);
        //optionsBuilder.Entity()
    }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasAlternateKey(c => c.Text)
            .HasName("AlternateKey_Category");
    }
        public DbSet<News> Newss { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<ImageLink> ImageLinks { get; set; }
            
    }
}