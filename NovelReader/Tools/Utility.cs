using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NovelCrawler
{
    public static class Utility
    {
        private static Regex regex = new Regex(@"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", RegexOptions.Compiled);
        private static Regex script = new Regex(@"<script[^>]*>[\s\S]*?</script>", RegexOptions.Compiled);
        private static Regex div = new Regex(@"(<div[^>]*>|</div>)",RegexOptions.Compiled);
        private static Regex amper = new Regex(@"&.{1,9};", RegexOptions.Compiled);

        public static string GetHtml(string url) {            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.ConnectionLimit = 500;
            request.Timeout = 5000;
            request.Method = "GET";
            string data = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                }
            }
            return data;
        }

        public static string StripScript(string html) => script.Replace(html, "");
        public static string StripHtml(string html) =>amper.Replace(regex.Replace(html,""),"");
        public static string StripAmper(string html) => amper.Replace(html, "");
        public static string StripDiv(string html) => div.Replace(html, "");

        public static string GetTitle(string html)
        {
            return html.SubString("<title>", "</title>");
        }

        public static string SubString(this string str,string begin,string end)
        {
            int start = str.IndexOf(begin)+begin.Length;
            int length = str.IndexOf(end, start) - start;
            return str.Substring(start, length);
        }

        public static string Tabs(this char chr,int count)
        {
            return new string(chr, count);
        }
    }
}
