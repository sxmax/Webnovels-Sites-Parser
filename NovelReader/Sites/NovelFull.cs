using NovelCrawler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovelReader
{
    class NovelFull
    {
        private static string url = "http://novelfull.com";

        public static IEnumerable<Chapter> GetChapters(string url)
        {
            int count = 1;
            string mask(int n) => $"?page={n}&per-page=50";
            var link = "href=\"";

            string html = Utility.GetHtml(url);
            int lastIndex = html.IndexOf("<li class=\"last\"");
            int strPage =html.IndexOf("data-page=\"",lastIndex) + 11;
            int amp = html.IndexOf('"', strPage);
            int pages = Convert.ToInt32(html.Substring(strPage,amp-strPage))+1;
                        
            for (int i = 1; i <=pages; i++)
            {
                var tmp = url + mask(i);
                string index = Utility.GetHtml(tmp);

                int cind = 0; 
                var b = "<div class=\"col-xs-12\" id=\"list-chapter\">";
                var begin = index.IndexOf(b) + b.Length;
                var endIndex = index.IndexOf("<ul class=\"pagination", begin);
                var toc = index.Substring(begin, endIndex - begin);

                while(cind>=0)
                {
                    cind = toc.IndexOf(link, cind + 1);
                    if (cind == -1)
                        break;
                    var qu = toc.IndexOf('"', cind + link.Length + 1);
                    var curl = toc.Substring(cind + link.Length, qu - cind - 6);
                    yield return GetChapter(NovelFull.url,curl);

                    Report.Progress(count, count - 1);
                    count++;
                }
            }
        }

        public static Book GetBook(string query)
        {
            return new Book()
            {
                Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(query.Split('/').Last().Replace('-', ' ').Replace(".html","")),
                Chapters = GetChapters(query)
            };
        }

        private static Chapter GetChapter(string url,string chapter)
        {
            string html = Utility.GetHtml(url + chapter);
            var begin = "<div id=\"chapter-content\"";
            var end = "<div class=\"adsmobile";
            var indB = html.IndexOf(begin)+127;
            var indE = html.IndexOf(end, indB+1);
            string body =Utility.StripDiv(Utility.StripAmper(html.Substring(indB, indE - indB)));
            return new Chapter()
            {
                Title = Utility.GetTitle(html).Split('-')[1].Replace("online free",""),
                Body = body
            };
        }
    }
}
