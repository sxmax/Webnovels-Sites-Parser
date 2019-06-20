using NovelCrawler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelReader
{
    class RoyalRoadl
    {
        private static string url = "https://www.royalroad.com/";

        public static Book GetBook(string url)
        {
            return new Book()
            {
                Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(url.Split('/').Last().Replace("-"," ")),
                Chapters = GetChapters(url)
            };
        }

        public static IEnumerable<Chapter> GetChapters(string url)
        {
            int count = 1;
            string html = Utility.GetHtml(url);
            int ind = 0;
            int indE = 0;
            var begin = html.IndexOf("<table class=\"table no-border\" id=\"chapters\">");
            var end = html.IndexOf("</table>", begin);
            html = html.Substring(begin + 1, end - begin);
            var link = "href=\"";
            while (ind >= 0)
            {
                ind = html.IndexOf(link, indE);
                indE = html.IndexOf('"', ind + link.Length + 1);
                var chapter = html.Substring(ind + link.Length, indE - ind - 6);
                if (chapter.StartsWith("/"))
                {
                    yield return GetChapter(chapter);
                    Report.Progress(count, count - 1);
                    count++;
                }
            }
        }

        public static Chapter GetChapter(string url)
        {
            string html = Utility.GetHtml(RoyalRoadl.url + url);
            var begin = "<div class=\"chapter-inner chapter-content\">";
            var end = "<h6 class";
            var indB = html.IndexOf(begin) + begin.Length;
            var indE = html.IndexOf(end, indB);
            string body =Utility.StripDiv(Utility.StripAmper(html.Substring(indB, indE - indB)));

            return new Chapter()
            {
                Title = Utility.GetTitle(html).Split('-').First(),
                Body = body
            };
        }
    }
}
