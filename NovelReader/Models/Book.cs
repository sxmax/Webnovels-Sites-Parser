using System.Collections.Generic;

namespace NovelCrawler
{
    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Annotation { get; set; }
        public IEnumerable<Chapter> Chapters { get; set; }
    }
}
