using System;
using System.IO;
using ViewCounter.Services;

namespace ViewCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning process");
            Console.WriteLine("");
            string basePath = @"D:\Training and Practices Developer\Challenges - Interviews\";
            DateTime currentTime = DateTime.Now;
            string finalDateTime = currentTime.ToString("yyyyMMdd_HH00");
            string initialDateTime = currentTime.AddHours(-5).ToString("yyyyMMdd_HH00");
            string directoryDestination = Path.Combine(basePath, string.Format("Dumps Wikimedia Files {0} to {1}", initialDateTime, finalDateTime));

            DirectoryInfo directoryInfo = new DirectoryInfo(path: directoryDestination);
            if (!directoryInfo.Exists)
            {
                Console.WriteLine("Download files from last " + 5 + " hours");
                directoryInfo.Create();
                for (int i = 1; i <= 5; i++)
                {
                    DumpWikimediaFiles dumpWikimediaFiles = new DumpWikimediaFiles(directoryDestination, currentTime);
                    dumpWikimediaFiles.DownloadFile();
                    dumpWikimediaFiles.DecompressFile();
                    currentTime = currentTime.AddHours(-1);
                }
            } else
                Console.WriteLine("Files were already downloaded");

            Console.WriteLine("Showing result of view count by Language and Domain in the last " + 5 + "hours");
            Console.WriteLine("");

            DumpWikimediaContent dumpWikimediaContent = new DumpWikimediaContent(directoryInfo);
            dumpWikimediaContent.ReportViewCountByLanguageAndDomain();
            Console.WriteLine("");
            dumpWikimediaContent.ReportViewCountByPageTitle();

            Console.ReadKey();

        }
        
    }



}
