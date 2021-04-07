using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace ViewCounter.Services
{
    public class DumpWikimediaFiles
    {
        private string _baseDirectory;
        private int _numberFilesToDownload;

        public DirectoryInfo directoryDestinationInfo { get; set; }

        public DumpWikimediaFiles(string baseDirectory, int numberFilesToDownload)
        {
            _baseDirectory = baseDirectory;
            _numberFilesToDownload = numberFilesToDownload;
        }


        private void DownloadFile(DateTime dateTime)
        {
            Console.WriteLine(string.Format("Downloading file with period {0}", dateTime.ToString("yyyy-MM-dd HH:00")));

            string year = dateTime.Year.ToString();
            string yearMonth = dateTime.ToString("yyyy-MM");
            string fullDateTime = dateTime.ToString("yyyyMMdd-HH0000");

            string downloadedFile = Path.Combine(directoryDestinationInfo.FullName, fullDateTime + ".gz");

            WebClient Client = new WebClient();
            string URL = string.Format("https://dumps.wikimedia.org/other/pageviews/{0}/{1}/pageviews-{2}.gz", year, yearMonth, fullDateTime);
            Client.DownloadFile(URL, downloadedFile);

            Console.WriteLine("File downloaded");
        }

        private void DecompressFile(DateTime dateTime)
        {
            Console.WriteLine("Decompressing File");
            string fullDateTime = dateTime.ToString("yyyyMMdd-HH0000");
            string fileToDecompress = Path.Combine(directoryDestinationInfo.FullName, fullDateTime + ".gz");
            string descompressedFile = Path.Combine(directoryDestinationInfo.FullName, fullDateTime + ".txt");

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
            Console.WriteLine();
        }


        public void DownloadAllFiles()
        {
            DateTime currentTime = DateTime.Now;
            string finalDateTime = currentTime.ToString("yyyyMMdd_HH00");
            string initialDateTime = currentTime.AddHours(_numberFilesToDownload * -1).ToString("yyyyMMdd_HH00");
            string directoryDestination = Path.Combine(_baseDirectory, string.Format("Dumps Wikimedia Files {0} to {1}", initialDateTime, finalDateTime));

            directoryDestinationInfo = new DirectoryInfo(directoryDestination);
            if (!directoryDestinationInfo.Exists || directoryDestinationInfo.EnumerateFiles().ToList().Count != _numberFilesToDownload)
            {
                Console.WriteLine("Initialize download files from last " + _numberFilesToDownload + " hours");
                Console.WriteLine();
                directoryDestinationInfo.Create();
                for (int i = 1; i <= _numberFilesToDownload; i++)
                {
                    DownloadFile(currentTime);
                    DecompressFile(currentTime);
                    currentTime = currentTime.AddHours(-1);
                }
            }
            else
            {
                Console.WriteLine("Files already downloaded and decompressed from the last " + _numberFilesToDownload + " hours");
                Console.WriteLine();
            }
        }

        public void DeleteAllFiles()
        {
            Console.WriteLine("Deleting all files");
            if (directoryDestinationInfo.Exists) directoryDestinationInfo.Delete(true);
            Console.WriteLine("All files was deleted successfully");
            Console.WriteLine();
        }
    }
}
