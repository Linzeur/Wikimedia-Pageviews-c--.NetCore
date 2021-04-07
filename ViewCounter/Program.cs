using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using ViewCounter.Entities;

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
                    DownloadFile(directoryDestination, currentTime);
                    DecompressFile(directoryDestination, currentTime);
                    currentTime = currentTime.AddHours(-1);
                }
            }

            Console.WriteLine("Showing result of view count by Language and Domain in the last " + 5 + "hours");
            Console.WriteLine("");

            ReportViewCountByLanguageAndDomain(directoryInfo);
            ReportViewCountByPageTitle(directoryInfo);

            Console.ReadKey();


            /*
            


            Console.ReadKey();

            /*currentDateTime.AddHours(-1);
        }*/

        }





        static void DownloadFile(string directory, DateTime dateTime)
        {
            Console.WriteLine(String.Format("Downloading file with period {0}", dateTime.ToString("yyyy-MM-dd HH:00")));

            string year = dateTime.Year.ToString();
            string yearMonth = dateTime.ToString("yyyy-MM");
            string fullDateTime = dateTime.ToString("yyyyMMdd-HH0000");

            string downloadedFile = Path.Combine(directory, fullDateTime + ".gz");

            WebClient Client = new WebClient();
            string URL = string.Format("https://dumps.wikimedia.org/other/pageviews/{0}/{1}/pageviews-{2}.gz", year, yearMonth, fullDateTime);
            Client.DownloadFile(URL, downloadedFile);

            Console.WriteLine("File downloaded");
        }

        static void DecompressFile(string directory, DateTime dateTime)
        {
            Console.WriteLine("Decompressing File");
            string fullDateTime = dateTime.ToString("yyyyMMdd-HH0000");
            string fileToDecompress = Path.Combine(directory, fullDateTime + ".gz");
            string descompressedFile = Path.Combine(directory, fullDateTime + ".txt");

            using (FileStream fInStream = new FileStream(fileToDecompress, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                {
                    using (FileStream fOutStream = new FileStream(descompressedFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] tempBytes = new byte[4096];
                        int descompressedBytes = 0;
                        while ((descompressedBytes = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                        {
                            fOutStream.Write(tempBytes, 0, descompressedBytes);
                        }
                    }
                }
            }

            FileInfo fileInfo = new FileInfo(fileToDecompress);
            if (fileInfo.Exists) fileInfo.Delete();

            Console.WriteLine("File decompressed");
            Console.WriteLine("==========");
            Console.WriteLine("");
        }

        static void DownloadAndDecompressFilesByRangeTime()
        {
            string basePath = @"D:\Training and Practices Developer\Challenges - Interviews\";
            DateTime currentTime = DateTime.Now;
            string finalDateTime = currentTime.ToString("yyyyMMdd_HH00");
            string initialDateTime = currentTime.AddHours(-5).ToString("yyyyMMdd_HH00");
            string directoryDestination = Path.Combine(basePath, string.Format("Dumps Wikimedia Files {0} to {1}", initialDateTime, finalDateTime));

            DirectoryInfo directoryInfo = new DirectoryInfo(path: directoryDestination);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                for (int i = 1; i <= 5; i++)
                {
                    DownloadFile(directoryDestination, currentTime);
                    DecompressFile(directoryDestination, currentTime);
                    currentTime = currentTime.AddHours(-1);
                }
            }


        }

        static void ReportViewCountByLanguageAndDomain(DirectoryInfo directoryInfo)
        {
            int[] sizeColumns = new int[4] { 10, 20, 25, 20 };
            string[] columns = new string[4];
            columns[0] = "Period".PadRight(sizeColumns[0]);
            columns[1] = "Language".PadRight(sizeColumns[1]);
            columns[2] = "Domain".PadRight(sizeColumns[2]);
            columns[3] = "View Count".PadRight(sizeColumns[3]);
            string header = string.Join("|", columns);
            Console.WriteLine(header);
            Console.WriteLine(new String('=', header.Length));


            List<FileInfo> listFiles = directoryInfo.EnumerateFiles().ToList();
            for (int i = 0; i < listFiles.Count; i++)
            {
                GetMaxViewCountByDomainAndLanguage(sizeColumns, listFiles[i].FullName, listFiles[i].Name);
            }
        }

        static void GetMaxViewCountByDomainAndLanguage(int[] sizeColumns, string fullPath, string fileName)
        {
            List<Domain> listDomains = new List<Domain>();
            using (StreamReader sw = new StreamReader(fullPath))
            {
                string line = sw.ReadLine();
                string[] fields = line.Split(" ");

                string domainCode = fields[0];
                int viewCount = int.Parse(fields[2]);
                Domain domain = new Domain(domainCode, viewCount);
                listDomains.Add(domain);

                while (!sw.EndOfStream)
                {
                    line = sw.ReadLine();
                    fields = line.Split(" ");

                    domainCode = fields[0];
                    viewCount = int.Parse(fields[2]);

                    int lastIndex = listDomains.Count - 1;
                    if (domainCode == listDomains[lastIndex].domainCode)
                        listDomains[lastIndex].viewCount += viewCount;
                    else
                    {
                        domain = new Domain(domainCode, viewCount);
                        listDomains.Add(domain);
                    }
                }
            }
            Domain domainMaxViewCount = listDomains.OrderByDescending(val => val.viewCount).ElementAt(0);
            listDomains.Clear();
            string[] columns = new string[4];
            columns[0] = (fileName.Substring(9, 2) + "hrs").PadRight(sizeColumns[0]);

            string[] arrDomainCode = domainMaxViewCount.domainCode.Split(".");
            columns[1] = arrDomainCode[0].PadRight(sizeColumns[1]);
            columns[2] = arrDomainCode[1].PadRight(sizeColumns[2]);

            columns[3] = domainMaxViewCount.viewCount.ToString().PadRight(sizeColumns[3]);

            Console.WriteLine(string.Join(" ", columns));
        }

        static void ReportViewCountByPageTitle(DirectoryInfo directoryInfo)
        {
            int[] sizeColumns = new int[3] { 10, 30, 20 };
            string[] columns = new string[3];
            columns[0] = "Period".PadRight(sizeColumns[0]);
            columns[1] = "Page".PadRight(sizeColumns[1]);
            columns[2] = "View Count".PadRight(sizeColumns[2]);
            string header = string.Join("|", columns);
            Console.WriteLine(header);
            Console.WriteLine(new String('=', header.Length));


            List<FileInfo> listFiles = directoryInfo.EnumerateFiles().ToList();
            for (int i = 0; i < listFiles.Count; i++)
            {
                GetMaxViewCountByPageTitle(sizeColumns, listFiles[i].FullName, listFiles[i].Name);
            }
        }

        static void GetMaxViewCountByPageTitle(int[] sizeColumns, string fullPath, string fileName)
        {
            Dictionary<string, Page> dictionaryPages = new Dictionary<string, Page>();
            using (StreamReader sw = new StreamReader(fullPath))
            {
                while (!sw.EndOfStream)
                {
                    string line = sw.ReadLine();
                    string[] fields = line.Split(" ");

                    string pageTitle = fields[1];
                    int viewCount = int.Parse(fields[2]);
                    
                    if (dictionaryPages.ContainsKey(pageTitle))
                        dictionaryPages[pageTitle].viewCount += viewCount;
                    else
                        dictionaryPages.Add(pageTitle, new Page(pageTitle, viewCount));
                }
            }

            Page pageMaxViewCount = dictionaryPages.OrderByDescending(page => page.Value.viewCount).First().Value;
            dictionaryPages.Clear();

            string[] columns = new string[3];
            columns[0] = (fileName.Substring(9, 2) + "hrs").PadRight(sizeColumns[0]);
            columns[1] = pageMaxViewCount.title.PadRight(sizeColumns[1]);
            columns[2] = pageMaxViewCount.viewCount.ToString().PadRight(sizeColumns[2]);

            Console.WriteLine(string.Join(" ", columns));
        }

    }



}
