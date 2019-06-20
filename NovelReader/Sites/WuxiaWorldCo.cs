using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NovelReader;
using System.Globalization;
using System.Drawing;

namespace NovelCrawler
{
    public static class WuxiaWorldCo
    {
        public static Book GetBook(string url)
        {
            string html = Utility.GetHtml(url);
            string title = url.Split('/').Skip(3).First();
            return new Book() {
                Annotation=GetDescription(html).Replace("\"","").TrimStart().TrimEnd(),
                Author=GetAuthor(html),
                Chapters = GetChapters(url,html),
                Title= title.Replace('-',' ')
            };
        }

        private static string GetDescription(string html) {
            return html.SubString("<div id=\"intro\">", "</div>");
        }

        private static string GetAuthor(string html) {
            return html.SubString("<p>Author：", "</p>");
        }

        private static IEnumerable<Chapter> GetChapters(string url,string html)
        {
            int count = 1;
            int ind = 0;
            html = html.SubString("<div id=\"list\">", "<script>");
            var link = "href=\"";
            while (ind >= 0) {
                ind = html.IndexOf(link, ind+1);
                if (ind == -1)
                    break;
                var indE = html.IndexOf('"', ind + link.Length + 1);
                var chapter = html.Substring(ind+link.Length, indE - ind-6);
                yield return GetChapter(url,chapter);
                Report.Progress(count, count - 1);
                count++;
            }
        }

        private static Chapter GetChapter(string url,string chapter)
        {
            string html = Utility.GetHtml(url + chapter);
            string title = Utility.GetTitle(html).Split('-')[1];
            html=html.SubString("<div id=\"content\">", "</div>");
            return new Chapter() {
                Title = title.Replace("&","and"),
                Body = "<p>" + Utility.StripAmper(Utility.StripScript(html.Replace("<br>","<br />"))) + "</p>"
            };
        }
    }
}
