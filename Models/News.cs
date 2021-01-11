using System.Collections.Generic;

namespace CrawlerVNEXPRESS.Models
{
    public class News{
       
        public long Id { get; set; }
         public string Category {get;set;}
        public string Title {get;set;}
        public string DatePost {get;set;}
        public string Link {get;set;}
        
        public ICollection<Content> Content {get;set;}
        
        public ICollection<ImageLink>  ImageLink { get; set; }
    }
}