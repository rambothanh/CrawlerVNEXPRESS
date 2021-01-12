using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrawlerVNEXPRESS.Models
{
    public class News
    {

        public long Id { get; set; }
        // Mỗi News chỉ có 1 Category, tạo ForeignKey
        // tại News
        [ForeignKey("Category")]
        public long CategoryRefId { get; set; }
        public Category Category { get; set; }
        public string Title { get; set; }
        public string DatePost { get; set; }
        public string Link { get; set; }
        // Mỗi News có thể có nhiều Content, đã để ForeignKey
        // ở class Content rồi, ở đây tạo 1 List hoặc 
        // ICollection
        public ICollection<Content> Content { get; set; }
        //Tương tự như Content
        public ICollection<ImageLink> ImageLink { get; set; }
    }
}