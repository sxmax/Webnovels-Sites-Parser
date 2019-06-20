using NovelCrawler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace NovelReader
{
    class Epub
    {
        private static readonly string root = "C:/Users/User/Downloads";

        private static void CreateSheets(ZipArchive zip)
        {
            var page = zip.CreateEntry("page_styles.css");
            using (var sr = new StreamWriter(page.Open()))
                sr.Write("@page {margin-bottom: 5pt;margin-top: 5pt} table {width: 100%; max-width: 100%; margin-bottom: 1rem;");
            var style = zip.CreateEntry("stylesheet.css");
            using (var sr = new StreamWriter(style.Open()))
                sr.Write(".calibre {display: block;font-size: 1em;margin: 0 5pt;padding: 0}");
        }

        private static List<string> CreateChapters(ZipArchive zip,IEnumerable<Chapter> chapters)
        {
            List<string> titles = new List<string>();
            foreach (var item in chapters)
            {
                var ch = zip.CreateEntry($"index{titles.Count+1}.xhtml");
                using (StreamWriter sr = new StreamWriter(ch.Open(), Encoding.UTF8))
                {                  
                    sr.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?><html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    sr.Write($"<head><title>{item.Title}</title><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
                    sr.Write("<link href=\"stylesheet.css\" rel=\"stylesheet\" type=\"text/css\"/><link href=\"page_styles.css\" rel=\"stylesheet\" type=\"text/css\"/></head>");
                    sr.Write($"<body><h4>{item.Title}</h4>{item.Body}</body></html>");
                }
                titles.Add(item.Title);
            }
            return titles;
        }

        private static void WriteChapters(Book book,ZipArchive zip)
        {
            var list = CreateChapters(zip, book.Chapters);
            var cont = zip.CreateEntry("content.opf");
            var stream = cont.Open();
            string author = book.Author ?? "Author";
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)) {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?><package xmlns=\"http://www.idpf.org/2007/opf\" unique-identifier=\"uuid_id\" version=\"2.0\"><metadata xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:opf=\"http://www.idpf.org/2007/opf\" xmlns:calibre=\"http://calibre.kovidgoyal.net/2009/metadata\" xmlns:dcterms=\"http://purl.org/dc/terms/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
                writer.Write($"<dc:title>{book.Title}</dc:title>");
                writer.Write($"<dc:creator opf:role=\"aut\" opf:file-as=\"dotNet\">{author}</dc:creator>");
                writer.Write($"<dc:contributor opf:role=\"bkp\">{author}</dc:contributor>");
                writer.Write("<dc:identifier id=\"uuid_id\" opf:scheme=\"uuid\">db8ddb4a-5c27-4d80-8f97-87cf5dc3dc86</dc:identifier><dc:language>de</dc:language><dc:identifier opf:scheme=\"calibre\">db8ddb4a-5c27-4d80-8f97-87cf5dc3dc86</dc:identifier></metadata>");
                writer.Write("<manifest><item href=\"intro.xhtml\" id=\"begin\" media-type=\"application/xhtml+xml\"/>");
                for (int i = 1; i <= list.Count; i++)                
                    writer.Write($"<item href=\"index{i}.xhtml\" id=\"id{i}\" media-type=\"application/xhtml+xml\"/>");
                writer.Write("<item href=\"toc.ncx\" id=\"ncx\" media-type=\"application/x-dtbncx+xml\"/><item href=\"page_styles.css\" id=\"css\" media-type=\"text/css\"/><item href=\"stylesheet.css\" id=\"css\" media-type=\"text/css\"/>");
                writer.Write("</manifest><spine toc=\"ncx\"><itemref idref=\"begin\"/>");
                for (int i = 1; i <= list.Count; i++)
                    writer.Write($"<itemref idref=\"id{i}\"/>");
                writer.Write("</spine><guide><reference href=\"intro.xhtml\" title=\"Cover\" type=\"cover\"/></guide></package>");
            }
            CreateToc(zip,list);
        }        

        private static void CreateToc(ZipArchive zip, List<string> titles)
        {
            var toc = zip.CreateEntry("toc.ncx");
            using (var xml = new StreamWriter(toc.Open()))
            {
                xml.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                xml.Write("<ncx xmlns=\"http://www.daisy.org/z3986/2005/ncx/\" version=\"2005-1\" xml:lang=\"deu\">");
                xml.Write("<head><meta content=\"db8ddb4a-5c27-4d80-8f97-87cf5dc3dc86\" name=\"dtb:uid\"/><meta content=\"2\" name=\"dtb:depth\"/><meta content=\"calibre(2.83.0)\" name=\"dtb:generator\"/><meta content=\"0\" name=\"dtb:totalPageCount\"/><meta content=\"0\" name=\"dtb:maxPageNumber\"/></head>");
                xml.Write("<docTitle><text>Table of Contents</text></docTitle>");
                xml.Write("<navMap>");
                xml.Write("<navPoint id=\"intro\" playOrder=\"1\"><navLabel><text>Annotation</text></navLabel><content src=\"intro.xhtml\"/></navPoint>");
                for (int i = 1; i<=titles.Count; i++)
                    xml.Write($"<navPoint id=\"pid{i}\" playOrder=\"{i+1}\"><navLabel><text>{titles[i-1]}</text></navLabel><content src=\"index{i}.xhtml\"/></navPoint>");
                xml.Write("</navMap>");
                xml.Write("</ncx>");
            }
        }

        private static void WriteContainer(ZipArchive zip)
        {
            var container = zip.CreateEntry("META-INF/container.xml",CompressionLevel.Fastest);
            using(StreamWriter xml=new StreamWriter(container.Open(), Encoding.UTF8))
                xml.Write("<?xml version=\"1.0\"?><container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\"><rootfiles><rootfile full-path=\"content.opf\" media-type=\"application/oebps-package+xml\"/></rootfiles></container>");
        }

        private static void CreateMime(ZipArchive zip)
        {
            var mime = zip.CreateEntry("mimetype",CompressionLevel.NoCompression);
            using (var sr = new StreamWriter(mime.Open()))
                sr.Write("application/epub+zip");
        }

        private static void CreateDescription(ZipArchive zip,string title,string description) {
            var desc = zip.CreateEntry("intro.xhtml", CompressionLevel.Fastest);
            string body = description ?? "No Description";
            using(var sr=new StreamWriter(desc.Open())) {
                sr.Write($"<?xml version=\"1.0\" encoding=\"UTF-8\"?><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>{title}</title></head><body><h4>Title: {title}</h4><p>{Utility.StripHtml(body)}</p></body></html>");
            }
        }

        public static void CreateBook(Book book)
        {
            using (var fs = new FileStream(Path.Combine(root, book.Title + ".epub"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Create, false)) {
                CreateMime(zip);
                CreateSheets(zip);
                WriteContainer(zip);
                CreateDescription(zip, book.Title, book.Annotation);
                WriteChapters(book, zip);
            }
        }
    }
}
