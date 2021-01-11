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
        //optionsBuilder.UseSqlite("Data Source=LocalDataSQLite.db");
        //optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=TenDataBase;Trusted_Connection=True;");
    }
        public DbSet<News> Newss { get; set; }
            
    }
}