using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ViewCounter.Services
{
    public class DumpWikimediaFiles
    {
        public string _directory;
        public DateTime _dateTime;

        public DumpWikimediaFiles(string directory, DateTime dateTime)
        {
            this._directory = directory;
            this._dateTime = dateTime;
        }

        public void DownloadFile()
        {
            Console.WriteLine(string.Format("Downloading file with period {0}", _dateTime.ToString("yyyy-MM-dd HH:00")));

            string year = _dateTime.Year.ToString();
            string yearMonth = _dateTime.ToString("yyyy-MM");
            string fullDateTime = _dateTime.ToString("yyyyMMdd-HH0000");

            string downloadedFile = Path.Combine(_directory, fullDateTime + ".gz");

            WebClient Client = new WebClient();
            string URL = string.Format("https://dumps.wikimedia.org/other/pageviews/{0}/{1}/pageviews-{2}.gz", year, yearMonth, fullDateTime);
            Client.DownloadFile(URL, downloadedFile);

            Console.WriteLine("File downloaded");
        }

        public void DecompressFile()
        {
            Console.WriteLine("Decompressing File");
            string fullDateTime = _dateTime.ToString("yyyyMMdd-HH0000");
            string fileToDecompress = Path.Combine(_directory, fullDateTime + ".gz");
            string descompressedFile = Path.Combine(_directory, fullDateTime + ".txt");

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
    }
}
