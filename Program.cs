using CrawlerVNEXPRESS.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerVNEXPRESS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Lấy Các đường link tin tức trong trang chủ vnexpress:
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load("https://vnexpress.net/");
            var tagLinkArticles = document
                            .DocumentNode
                            .QuerySelectorAll("article.item-news>h3.title-news>a").ToList();
            //Duyệt qua các đường link vừa lấy được, lấy nội dung, kiểm tra
            //và lưu vào database
            foreach (var tagLink in tagLinkArticles)
            {
                News news = new News { };
                var link = tagLink.Attributes["href"].Value;

                //Link podcast để test
                //var link = "https://vnexpress.net/le-nham-chuc-khac-thuong-cua-tong-thong-my-4222161.html";

                //Link slide để test
                //var link = "https://vnexpress.net/tuan-tra-trong-dem-am-2-do-c-4220047.html";

                //Bỏ qua link đã lấy    
                if (CheckLinkInDataBase(link))
                {
                    continue; // Tiêp tục vòng lập, bỏ qua link này
                }
                //Bỏ qua link video
                if (link.Contains("video"))
                {
                    continue; // Tiêp tục vòng lập, bỏ qua link này
                }

                //Load link để được đối tượng HtmlDocument
                HtmlDocument htmlDocArticle = htmlWeb.Load(link);
                //Lấy đối tượng htmlNode
                var doc = htmlDocArticle.DocumentNode;

                //Bỏ qua link trực tiếp, và link podcast
                //Check type of news return: live, slide_show, podcast, normal
                var type = CheckType(doc);
                if (type == "live" || type == "podcast")
                {
                    continue; // Tiêp tục vòng lập, bỏ qua link này
                }

                //Thêm link vào news
                news.Link = link;
                // Thêm ngày đăng tin vào news
                GetDatePost(doc, ref news);
                // Thêm tiêu đề tin vào news
                GetTitle(doc, ref news);
                // Thêm tiêu đề vào news
                GetCategory(doc, ref news);
                // Thêm LinkImage và Content
                AddImageAndContent(doc, ref news);
                // Kiểm tra Cat và add news vào database
                CheckCatAndAddNews(news);



            }

            // //lấy nội dung từ  database để kiểm tra
            // using (var context = new ClawlerContext())
            // {
            //     //Muốn lấy Content từ News thì phải dùng Include trong using Microsoft.EntityFrameworkCore;
            //     var testNews = context.Newss.Include(n => n.ImageLink);
            //     foreach (var news1 in testNews)
            //     {
            //         //Console.WriteLine(TiengVietKhongDau(news1.Category.Text));
            //         Console.WriteLine(TiengVietKhongDau(news1.Link));
            //         foreach(var imageLink in news1.ImageLink){
            //            Console.WriteLine(TiengVietKhongDau(imageLink?.Captain ??""));
            //            Console.WriteLine(imageLink?.TextLink);
            //         }
                    
            //         // Console.WriteLine(TiengVietKhongDau(news1.ImageLink?.FirstOrDefault()?.TextLink));
            //     }
            // }
        }


        //Check type of news return live slide_show podcast normal
        private static string CheckType(HtmlNode doc)
        {
            //Nếu tồn tại //i[contains(@class, ic-live)] là bài viết trực tiếp
            var linkLive = doc.SelectNodes("//i[contains(@class, ic-live)]");
            if (linkLive != null)
                return "live";

            //Nếu tồn tại div[contains(@class,"item_slide_show")], là bài viết slide_show
            var linkSlideShow = doc.SelectNodes("//div[contains(@class,'item_slide_show')]");
            if (linkSlideShow != null)
                return "slide_show";

            var catNodes = doc.SelectSingleNode(
                "//ul[@class=\"breadcrumb\"]/li/a" +
                "|//h2[@class=\"title_header\"]/a" +
                "|//div[@class=\"breadcrumb\"]/a[2]");
            if (catNodes?.InnerText == "Podcast")
                return "podcast";

            return "normal";

        }

        private static void AddImageAndContent(HtmlNode doc, ref News news)
        {
            string type = CheckType(doc);
            if (type == "normal")
                AddImageAndContentNormal(doc, ref news);
            if (type == "slide_show")
                AddImageAndContentSlideShow(doc, ref news);
        }

        // Lấy cấu trúc hình ảnh so với text, xử lý content và Text luôn
        // Thực hiện lưu vào news luôn, lưu vị trí của link luôn
        // Normal news:
        private static void AddImageAndContentNormal(HtmlNode doc, ref News news)
        {

            ///meta[@itemprop='url']"
            var allNodes = doc.SelectNodes("//p[contains(@class,'Normal') or contains(@class,'description')]|//figure");
            var node = allNodes[0];
            //Đếm tất cả nội dung và hình ảnh.
            var countAllNotes = allNodes.Count();
            ICollection<ImageLink> linkImages = new List<ImageLink>();
            ICollection<Content> contents = new List<Content>();
            for (int i = 0; i < countAllNotes; i++)
            {
                //Lấy Image
                //nếu là Image thì thẻ có tên là figure
                var NameNode = allNodes[i].Name;
                if (NameNode == "figure" && !string.IsNullOrEmpty(NameNode))
                {
                    //Lấy caption của image
                    //figure/figcaption/p
                    //Nhở bỏ hai dấu // (để tìm kiếm con trực tiếp)
                    var captain = allNodes[i].SelectNodes("figcaption/p")?.FirstOrDefault().InnerText;
                    //Lấy link và thêm vào danh sách linkImages
                    linkImages.Add(new ImageLink
                    {
                        Captain = captain,
                        Location = i + 1,
                        //Nhở bỏ hai dấu // (để tìm kiếm con trực tiếp)
                        TextLink = allNodes[i].SelectNodes("meta[@itemprop='url']")
                            ?.FirstOrDefault()
                            .Attributes["content"].Value
                    });
                }
                //Lấy nội dung (bao gồm description và subtitle)
                //Có thời gian sẽ tách ra
                if (NameNode == "p" && !string.IsNullOrEmpty(NameNode))
                {
                    contents.Add(new Content { Location = i + 1, Text = allNodes[i].InnerText });
                }
            }

            news.ImageLink = linkImages;
            news.Content = contents;
        }

        // Get slide_show news:
        private static void AddImageAndContentSlideShow(HtmlNode doc, ref News news)
        {
            //Tìm đầy đủ content (không trùng lắp): //div[contains(@class,'item_slide_show')]/div[not(contains(@style,'display: none'))]/p[contains(@class,'Normal')]
            //Ngắn gọn hơn: //div[not(contains(@style,'display: none'))]/p[contains(@class,'Normal')]
            //điều chỉnh để tìm được description chuẩn hơn
            //Image: nằm trong thẻ //picture/img attribute data-src 
            var allNodes = doc.SelectNodes("//div[contains(@class,'width-detail-photo')]/p[contains(@class,'description')]|" +
                "//div[not(contains(@style,'display: none'))]/p[contains(@class,'Normal')]|" +
                "//picture/img");
            //Đếm tất cả nội dung và hình ảnh.
            var countAllNotes = allNodes.Count();
            ICollection<ImageLink> linkImages = new List<ImageLink>();
            ICollection<Content> contents = new List<Content>();
            for (int i = 0; i < countAllNotes; i++)
            {
                //Lấy Image
                //nếu tồn tại Attribute là data-src thì nó là image
                //var nodeIsImage = allNodes[i].Attributes["data-src"];
                var NameNode = allNodes[i].Name;

                if (NameNode == "img" && !string.IsNullOrEmpty(NameNode))
                {
                    //Lấy link và thêm vào danh sách linkImages
                    linkImages.Add(new ImageLink { Location = i + 1, TextLink = allNodes[i].Attributes["data-src"].Value });
                }
                //Lấy nội dung (bao gồm description và subtitle)
                //Có thời gian sẽ tách ra
                if (NameNode == "p" && !string.IsNullOrEmpty(NameNode))
                {
                    contents.Add(new Content { Location = i + 1, Text = allNodes[i].InnerText });
                }
            }

            news.ImageLink = linkImages;
            news.Content = contents;
        }

        // Lấy Cat
        private static void GetCategory(HtmlNode doc, ref News news)
        {
            // Lấy chủ đề của bài viết thường, bài viết góc nhìn, bài viết video
            // Lấy catNodes
            var catNodes = doc.SelectSingleNode(
                "//ul[@class=\"breadcrumb\"]/li/a" +
                "|//h2[@class=\"title_header\"]/a" +
                "|//div[@class=\"breadcrumb\"]/a[2]");
            if (catNodes != null)
            {
                news.Category = new Category { Text = catNodes.InnerText };
            }
        }

        // lấy tiêu đề:
        private static void GetTitle(HtmlNode doc, ref News news)
        {
            //Lấy tiêu đề nằm trong <h1 class="title-detail"> hoặc <h1 class="title_gn_detail">
            //"h1[class*=\"title\"]" chọn element h1 có class chứa chữ "title"
            var titleNodes = doc.QuerySelector("h1[class*=\"title\"]");
            if (titleNodes != null)
            {
                news.Title = titleNodes.InnerText;
            }
        }

        // Lấy date tin tức
        private static void GetDatePost(HtmlNode doc, ref News news)
        {
            //Lẩy thời gian đăng tin, nằm trong <span class="time"> hoặc <span class="time-now">
            //span[class*="time"] chọn element span có class chứa chữ time (time hoặc time-now đều thoả mãn
            // Dùng lại Xpath: //span[contains(@class, "date") or contains(@class, "time")]
            //Lấy node trước để tránh lỗi:
            var datePostNote = doc.SelectNodes("//span[contains(@class, 'date') or contains(@class, 'time')]");
            //Check null
            if (datePostNote != null)
            {
                news.DatePost = datePostNote.FirstOrDefault().InnerText;
            }
        }

        // Kiểm tra Category của News và add để đảm bảo duy nhất.
        private static void CheckCatAndAddNews(News news)
        {
            using (var context = new ClawlerContext())
            {
                // Lấy Cat từ Paran
                var catParam = news.Category.Text;
                // Kiểm cat giống như thế từ Database
                var catDatabase = context.Categories.FirstOrDefault(c => c.Text == catParam);
                // Nếu trong Database đã có
                if (catDatabase != null)
                {
                    news.Category = catDatabase;
                }

                context.Newss.Add(news);
                context.SaveChanges();
            }
        }

        // Kiểm tra link VNexpress đã được lưu ở database chưa.
        private static bool CheckLinkInDataBase(string linkPara)
        {
            // lấy nội dung từ  database:
            using (var context = new ClawlerContext())
            {
                return context.Newss.Any(n => n.Link == linkPara);
            }
        }

        // Dùng để test trên console đỡ rối mắt
        public static string TiengVietKhongDau(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}