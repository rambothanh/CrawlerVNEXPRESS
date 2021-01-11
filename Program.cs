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
            // Add Service
            //private static SiteService _siteService;
            // using (var context = new ClawlerContext()) {
            //     //Test context
            //     var news = new News()
            //     {
            //          Title = "Bill"
            //     };
            //     context.Newss.Add(news);
            //     context.SaveChanges();
            //     Console.WriteLine(context.Newss.FirstOrDefault(n => n!=null).Title);
            // }

            //Tạo danh sách các News
            //List<News> newss = null;

            // Khu vực Crawler
            #region Crawler

            //Lấy Các đường link tin tức trong trang chủ vnexpress:
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = htmlWeb.Load("https://vnexpress.net/");
            var tagLinkArticles = document
                            .DocumentNode
                            .QuerySelectorAll("article.item-news>h3.title-news>a").ToList();

            //Duyệt qua các đường link vừa lấy được:
            foreach (var tagLink in tagLinkArticles)
            {
                News news = new News { };

                var link = tagLink.Attributes["href"].Value;
                HtmlDocument htmlDocArticle = htmlWeb.Load(link);

                //Lẩy thời gian đăng tin, nằm trong <span class="time"> hoặc <span class="time-now">
                //span[class*="time"] chọn element span có class chứa chữ time (time hoặc time-now đều thoả mãn
                var datePost = htmlDocArticle.DocumentNode.QuerySelector("span[class*=\"time\"]").InnerText;

                //Lấy tiêu đề nằm trong <h1 class="title-detail"> hoặc <h1 class="title_gn_detail">
                //"h1[class*=\"title\"]" chọn element h1 có class chứa chữ "title"
                var title = htmlDocArticle.DocumentNode.QuerySelector("h1[class*=\"title\"]").InnerText;

                //Lấy chủ đề 3 kiểu bài viết khác nhau có các chủ đề nằm ở chỗ khác nhau
                //<a data-medium="Menu-TheGioi" href="/the-gioi" title="Thế giới" data-itm-source="#vn_source=Detail&amp;vn_campaign=Header&amp;vn_medium=Menu-TheGioi&amp;vn_term=Desktop" data-itm-added="1">Thế giới</a>
                //<a data-medium="Menu-GocNhin" href="/goc-nhin" data-itm-source="#vn_source=Detail&amp;vn_campaign=Header&amp;vn_medium=Menu-GocNhin&amp;vn_term=Desktop" data-itm-added="1">Góc nhìn</a>

                //var tagCatNomal = htmlDocArticle.DocumentNode.QuerySelector("ul.breadcrumb>li>a");
                //var tagCatGocNhin = htmlDocArticle.DocumentNode.QuerySelector("h2.title_header>a");
                ////Lấy tag a thứ 2 thuộc div có class là breadcrumb
                //var tagCatVideo = htmlDocArticle.DocumentNode.QuerySelector("div.breadcrumb>a:nth-child(2)");
                //string cat;
                //if (tagCatNomal !=null)
                //{
                //    cat = tagCatNomal.InnerText;
                //}
                //else if(tagCatGocNhin != null)
                //{
                //    cat = tagCatGocNhin.InnerText;
                //}
                //else
                //{
                //    cat = tagCatVideo.InnerText;
                //}

                // Thử dùng XPath
                // Lấy chủ đề của bài viết thường, bài viết góc nhìn, bài viết video
                var cat = htmlDocArticle
                    .DocumentNode
                    .SelectSingleNode("//ul[@class=\"breadcrumb\"]/li/a" +
                    "|//h2[@class=\"title_header\"]/a" +
                    "|//div[@class=\"breadcrumb\"]/a[2]")
                    .InnerText;

                //Add các nội dung lấy được vào news
                news.Link = link;
                news.Title = title;
                news.DatePost = datePost;
                news.Category = cat;

                // Console.WriteLine("Chu de: " + TiengVietKhongDau(cat));
                // Console.WriteLine("Tieu de: " + TiengVietKhongDau(title));
                // Console.WriteLine("link: " + link);
                // Console.WriteLine("Ngay dang: " + TiengVietKhongDau(datePost));


                //Lay noi dung
                //Chuyển qua dùng XPath
                //Giá trị class lúc thì có khoản trắng, lúc thì không
                // dùng contain starts-with(@id,'message')
                //var newsContents = htmlDocArticle.DocumentNode
                //    .SelectNodes("//p[@class='description']|//article[starts-with(@class,'fck_detail')]/p[@class=\"Normal\"]|//div[@class=\"fck_detail\"]/p[@class=\"Normal\"]").ToList();

                //Thử cách đơn giản hơn, lấy toàn bộ p có class description và p có class Normal
                //Link video thì lấy div có id lead_brandsafe_video
                var newsContents = htmlDocArticle.DocumentNode
                   .SelectNodes("//p[@class='description']|//p[@class='Normal']|//div[@id='lead_brandsafe_video']")
                   .ToList();

                ICollection<Content> contents = new List<Content>();
                foreach (var newsContent in newsContents)
                {
                    Content content = new Content { };
                    content.Text = newsContent.InnerText;
                    // Console.WriteLine(newsContent.InnerText);
                    // Console.WriteLine(content.Text);
                    if(content!=null)
                        contents.Add(content);
                }
                if(contents!=null)
                    news.Content = contents;

                //Thêm một tin vào database
                using (var context = new ClawlerContext())
                {
                    context.Newss.Add(news);
                    context.SaveChanges();

                }
            }

            //Test database:
            using (var context = new ClawlerContext())
            {
                //Muốn lấy Content từ News thì phải dùng Include trong using Microsoft.EntityFrameworkCore;
                var testNews = context.Newss.Include(n =>n.Content);
                foreach(var news1 in testNews){
                    Console.WriteLine(TiengVietKhongDau(news1.Title));
                    Console.WriteLine(TiengVietKhongDau(news1.Link));
                    Console.WriteLine(TiengVietKhongDau(news1.Content.FirstOrDefault().Text));
                }

            }


            Console.WriteLine("Bam Enter de ket thuc chuong trinh");
            Console.ReadLine();
            #endregion
        }

        public static string TiengVietKhongDau(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

    }
}
