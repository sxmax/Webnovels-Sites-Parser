using NovelReader;
using System;
using System.Linq;
using NovelReader.Sites;

namespace NovelCrawler
{
    class Program
    {
        private static void Greet()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("{0}{1}NovelCrawler{1}{0} ",' '.Tabs(10),'-'.Tabs(43));
            Console.WriteLine("{0}{1}{2}{1}{0}", ' '.Tabs(10), ' '.Tabs(43),"Create Novels");
            Console.ResetColor();
        }
        public static bool exit = true;
        public static bool greeted = false;
        static void Main(string[] args)
        {
            if (!greeted) {
                greeted = true;
                Greet();
            }
            try {
                while (exit) {
                    Console.Write("lnquery> ");
                    var str = Console.ReadLine();
                    var query = str
                        .Replace("https://","")
                        .Replace("http://","");
                    if (query.StartsWith("wuxiaworld.com"))
                        WuxiaWorldBook(str);
                    else if (query.StartsWith("wuxiaworld.co"))
                        WuxiaCoBook(str);
                    else if (query.StartsWith("royalroad.com"))
                        RoyalRoadBook(str);
                    else if (query.StartsWith("novelfull.com"))
                        NovelFullBook(str);
                    else if (query.StartsWith("m.wuxiaworld.co"))
                        MWuxiaWorldBook(str);
                    else
                        Commands.Execute(str, ref exit);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n"+ex.Message);
                Main(args);
            }
        }

        private static void BaseBook(string query,Func<string,Book> func)
        {
            Console.Write("In Progress ");
            var flags = query.Split(' ');
            var book = func(flags[0]);
            var chapters = book.Chapters;
            if (flags.Length > 1) {
                int skip = flags[1] == "" ? 0 : int.Parse(flags[1]);
                int take = flags[2] == "" ? 0 : int.Parse(flags[2]);
                if (skip > 0)
                    book.Chapters = chapters.Skip(skip);
                if (take > 0)
                    book.Chapters = chapters.Take(take);
            }
            Epub.CreateBook(book);
            Console.WriteLine(" Done");
        }

        private static void NovelFullBook(string query) => BaseBook(query, NovelFull.GetBook);

        private static void RoyalRoadBook(string query) => BaseBook(query, RoyalRoadl.GetBook);

        private static void WuxiaCoBook(string query) => BaseBook(query, WuxiaWorldCo.GetBook);

        private static void WuxiaWorldBook(string query) => BaseBook(query, WuxiaWorld.GetBook);

        private static void MWuxiaWorldBook(string query) => BaseBook(query, MWuxiaWorld.GetBook);
    }
}
