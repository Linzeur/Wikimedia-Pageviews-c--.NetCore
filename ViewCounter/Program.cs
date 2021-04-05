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
            WebClient Client = new WebClient();
            DateTime currentDateTime = DateTime.Now;
            string destinyFolder = @"D:\Training and Practices Developer\Challenges - Interviews\";
            /*for (int i = 1; i <= 5; i++)
            {*/
            string currentYear = currentDateTime.Year.ToString();
            string currentYearMonth = currentDateTime.ToString("yyyy-MM");
            string currentTime = currentDateTime.ToString("yyyyMMdd-HH0000");
            string URL = string.Format("https://dumps.wikimedia.org/other/pageviews/{0}/{1}/pageviews-{2}.gz", currentYear, currentYearMonth, currentTime);
            Console.WriteLine("Descagando Archivo");
            string downloadedFile = destinyFolder + currentTime + ".gz";
            Client.DownloadFile(URL, downloadedFile);

            Console.WriteLine("Descromprimiendo Archivo");
            string descompressedFile = destinyFolder + currentTime + ".txt";
            using (FileStream fInStream = new FileStream(downloadedFile, FileMode.Open, FileAccess.Read))
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

            /*currentDateTime.AddHours(-1);
        }*/


        }
    }
}
