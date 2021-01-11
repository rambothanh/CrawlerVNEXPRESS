using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerVNEXPRESS.Models
{
    public class Content{
       
        public long Id { get; set; }
        public string Text {get;set;}
        [ForeignKey("News")]
        public long NewsRefId  {get;set;}
        public News News  {get;set;}
        
    }
}