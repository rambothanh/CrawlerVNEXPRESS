using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrawlerVNEXPRESS.Models
{
    public class Category{
       
        public long Id { get; set; }
        
        public string Text {get;set;}
        ICollection<News> News {get;set;}
        
    }
}