using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NovelReader;
namespace NovelCrawler
{
    public static class WuxiaWorld
    {
        private static string url = "http://wuxiaworld.com";

        public static Book GetBook(string url)
        {
            string html = Utility.GetHtml(url);
            return new Book()
            {
                Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string.Join(" ",url.Split('/').Last().Split('-'))),
                Chapters = GetChapters(html)
            };
        }

        public static IEnumerable<Chapter> GetChapters(string html)
        {
            int count = 1;
            int ind = 0;
            int indE = 0;
            var begin = html.IndexOf("<div class=\"panel-group\"");
            var end = html.IndexOf("<script id=",begin);
            html = html.Substring(begin + 1, end - begin);
            var link = "href=\"";
            while (ind >= 0) {
                ind = html.IndexOf(link,indE);
                indE = html.IndexOf('"', ind + link.Length + 1);
                var chapter= html.Substring(ind+link.Length, indE - ind-6);
                if (chapter.StartsWith("/")) {
                    yield return GetChapter(chapter);
                    Report.Progress(count, count - 1);
                    count++;
                }
            }
        }

        public static Chapter GetChapter(string url)
        {
            string html = Utility.GetHtml(WuxiaWorld.url +url);
            string body = "<p>" +Utility.StripAmper(Utility.StripDiv(html.SubString("<div class=\"fr-view\">", "<div>"))).Replace("<br>","<br/>") + "</p>";
            body = body.Replace("Next Chapter", "").Replace("Previous Chapter", "");

            return new Chapter() {
                Title = Utility.GetTitle(html).Split('-')[1],
                Body = body
            };
        }
    }
}
