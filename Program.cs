using System;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Fizzler;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using CrawlerVNEXPRESS.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CrawlerVNEXPRESS
{
    class Program
    {
        static void Main(string[] args)
        {


            // Khu vực Crawler
            #region Crawler

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
                if (CheckLinkInDataBase(link))
                {
                    continue; // Tiêp tục vòng lập, bỏ qua link này
                }
                //Thêm link vào news
                news.Link = link;
                //Load link để được đối tượng HtmlDocument
                HtmlDocument htmlDocArticle = htmlWeb.Load(link);
                //Lấy đối tượng htmlNode
                var doc = htmlDocArticle.DocumentNode;
                // Thêm ngày đăng tin vào news
                GetDatePost(doc, news);
                // Thêm tiêu đề tin vào news
                GetTitle(doc, news);
                // Thêm tiêu đề vào news
                GetCategory(doc, news);

                TakeImageAndStructure(doc, news);
                //Lay noi dung
                //Chuyển qua dùng XPath
                //Giá trị class lúc thì có khoản trắng, lúc thì không
                // dùng contain starts-with(@id,'message')
                //var newsContents = htmlDocArticle.DocumentNode
                //    .SelectNodes("//p[@class='description']|//article[starts-with(@class,'fck_detail')]/p[@class=\"Normal\"]|//div[@class=\"fck_detail\"]/p[@class=\"Normal\"]").ToList();

                //Thử cách đơn giản hơn, lấy toàn bộ p có class description và p có class Normal
                // Thêm cả p có class là Normal subtitle nữa
                //Link video thì lấy div có id lead_brandsafe_video

                var newsContents = htmlDocArticle.DocumentNode
                   .SelectNodes("//p[@class='Normal subtitle']|//p[@class='description']|//p[@class='Normal']|//div[@id='lead_brandsafe_video']")
                   .ToList();

                ICollection<Content> contents = new List<Content>();
                foreach (var newsContent in newsContents)
                {
                    Content content = new Content { };
                    content.Text = newsContent.InnerText;
                    if (content != null)
                        contents.Add(content);
                }

                // Gán contents vào news
                if (contents != null)
                    news.Content = contents;

                // Lấy link image trong bài viết:
                // /html/body/section[4]/div/div[2]/article/figure/meta[1]
                // Attributes["content"].Value
                // 
                // Ở phiên bản di động, đường link Image sẽ nằm ở:
                // "//figure/div/img"  Attributes["src"].Value
                // Sẽ xử lý phần này sau

                var linkImageNodes = htmlDocArticle.DocumentNode
                   .SelectNodes("//figure/meta[@itemprop='url']");
                // Check if exist
                if (linkImageNodes != null)
                {
                    //Lấy Attributes content (chứa link hình ảnh)
                    ICollection<ImageLink> linkImages = new List<ImageLink>();
                    foreach (var linkImageNode in linkImageNodes)
                    {
                        ImageLink linkImage = new ImageLink();
                        linkImage.TextLink = linkImageNode.Attributes["content"].Value;
                        linkImages.Add(linkImage);
                    }
                    // Gán link ảnh vào new
                    if (linkImages != null)
                        news.ImageLink = linkImages;
                }

                //Thêm một tin vào database
                CheckCatAndAddNews(news);

            }

            //lấy nội dung từ  database để kiểm tra
            // using (var context = new ClawlerContext())
            // {
            //     //Muốn lấy Content từ News thì phải dùng Include trong using Microsoft.EntityFrameworkCore;
            //     var testNews = context.Newss.Include(n => n.Category);
            //     foreach (var news1 in testNews)
            //     {
            //         Console.WriteLine(TiengVietKhongDau(news1.Category.Text));
            //         Console.WriteLine(TiengVietKhongDau(news1.Link));
            //         //Console.WriteLine(TiengVietKhongDau(news1.Content.FirstOrDefault().Text));
            //     }

            // }

            Console.WriteLine("Bam Enter de ket thuc chuong trinh");
            Console.ReadLine();
            #endregion
        }

        // Lấy cấu trúc hình ảnh so với text, xử lý content và Text luôn
        // Thực hiện lưu vào news luôn, lưu vị trí của link luôn
        private static void TakeImageAndStructure(HtmlNode doc, News news)
        {
            // HtmlWeb htmlWeb = new HtmlWeb();
            // HtmlDocument document = htmlWeb.Load("https://vnexpress.net/10-xu-huong-dinh-hinh-nganh-cong-nghe-nam-2021-4219399.html");
            //var doc = document.DocumentNode;
            //Đếm tất cả nội dung và hình ảnh.
            var allNodes = doc.SelectNodes("//p[@class='Normal subtitle']|//p[@class='description']|//p[@class='Normal']|//figure/meta[@itemprop='url']");
            var countAllNotes = allNodes.Count();
            string structLink = "";
            for (int i = 0; i < countAllNotes; i++)
            {
                var nodeIsImage = allNodes[i].Attributes["content"];
                if (nodeIsImage != null)
                {
                    structLink = structLink + $"{i + 1} ";
                }
            }

            Console.WriteLine(structLink);
        }

        // Lấy Cat
        private static void GetCategory(HtmlNode doc, News news)
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
        private static void GetTitle(HtmlNode doc, News news)
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
        private static void GetDatePost(HtmlNode doc, News news)
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
