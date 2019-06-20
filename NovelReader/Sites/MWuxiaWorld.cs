using NovelCrawler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NovelReader.Sites
{
    class MWuxiaWorld
    {
        public static Book GetBook(string url)
        {
            string html = Utility.GetHtml(url);
            string title = url.Split('/').Skip(3).First();
            return new Book()
            {
                Annotation = GetDescription(html).Replace("\"", "").TrimStart().TrimEnd(),
                Author = GetAuthor(html),
                Chapters = GetChapters(url, html),
                Title = title.Replace('-', ' ')
            };
        }

        private static string GetDescription(string html)
        {
            return html.SubString("<p class=\"review\" style=\"height:69px;overflow:hidden;\">", "</p>");
        }

        private static string GetAuthor(string html)
        {
            return html.SubString("<p class=\"author\">Author：", "</p>");
        }

        private static IEnumerable<Chapter> GetChapters(string url, string html)
        {
            int count = 1;
            int ind = 0;
            html = Utility.GetHtml(Path.Combine(url, "all.html"));
            var link = "href=\"";
            html=html.SubString("<div  id=\"chapterlist\" class=\"directoryArea\">", "<p class=\"Readpage\" id=\"bottom\"");
            while (ind >= 0) {
                ind = html.IndexOf(link, ind + 1);
                if (ind == -1)
                    break;
                var indE = html.IndexOf('"', ind + link.Length + 1);
                var chapter = html.Substring(ind + link.Length, indE - ind - 6);
                if (chapter.StartsWith("#"))
                    continue;
                yield return GetChapter(url, chapter);
                Report.Progress(count, count - 1);
                count++;
            }
        }

        private static Chapter GetChapter(string url, string chapter)
        {
            string html = Utility.GetHtml(url + chapter);
            string title = Utility.GetTitle(html).Split('-')[0].Split('_')[1];
            var chp = html.IndexOf("<div id=\"chaptercontent\" class=\"Readarea ReadAjax_content\">");
            var adEnd = html.IndexOf("</div>",chp)+6;
            html = html.Substring(adEnd, html.IndexOf("<amp-auto-ads",chp) - adEnd);
            return new Chapter() {
                Title = title.Replace("&", "and"),
                Body = "<p>" + Utility.StripAmper(html
                .Replace("<br>", "<br/>")
                .Replace("</br>","<br/>")) + "</p>"
            };
        }
    }
}
