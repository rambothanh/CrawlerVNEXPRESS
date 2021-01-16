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

                //Bỏ qua link trực tiếp: //i[contains(@class, ic-live)]
                var linkLive = doc.SelectNodes("//i[contains(@class, ic-live)]");
                if(linkLive != null)
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
            Console.WriteLine("Bam Enter de ket thuc chuong trinh");
            Console.ReadLine();
            // //lấy nội dung từ  database để kiểm tra
            // using (var context = new ClawlerContext())
            // {
            //    //Muốn lấy Content từ News thì phải dùng Include trong using Microsoft.EntityFrameworkCore;
            //    var testNews = context.Newss.Include(n => n.Content);
            //    foreach (var news1 in testNews)
            //    {
            //        //Console.WriteLine(TiengVietKhongDau(news1.Category.Text));
            //        Console.WriteLine(TiengVietKhongDau(news1.Link));
            //        Console.WriteLine(TiengVietKhongDau(news1.Content.FirstOrDefault().Text));
            //    }
            // }
        }

        // Lấy cấu trúc hình ảnh so với text, xử lý content và Text luôn
        // Thực hiện lưu vào news luôn, lưu vị trí của link luôn
        private static void AddImageAndContent(HtmlNode doc, ref News news)
        {
            //Luu y: duong link co video (chua xu ly)
            //var newsContents = htmlDocArticle.DocumentNode
            //   .SelectNodes("//p[@class='Normal subtitle']|//p[@class='description']|//p[@class='Normal']|//div[@id='lead_brandsafe_video']")
            //   .ToList();
            //Đếm tất cả nội dung và hình ảnh.
            var allNodes = doc.SelectNodes("//p[contains(@class,'Normal') or contains(@class,'description')]|//figure/meta[@itemprop='url']");
            var countAllNotes = allNodes.Count();
            ICollection<ImageLink> linkImages = new List<ImageLink>();
            ICollection<Content> contents = new List<Content>();
            for (int i = 0; i < countAllNotes; i++)
            {
                //Lấy Image
                //nếu tồn tại Attribute là content thì nó là image
                //var nodeIsImage = allNodes[i].Attributes["content"];
                var NameNode = allNodes[i].Name;

                if (NameNode == "meta" && !string.IsNullOrEmpty(NameNode))
                {
                    //Lấy link và thêm vào danh sách linkImages

                    linkImages.Add(new ImageLink { Location = i + 1, TextLink = allNodes[i].Attributes["content"].Value });
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
            var catNodes = doc.SelectSingleNode("//ul[@class=\"breadcrumb\"]/li/a" +
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