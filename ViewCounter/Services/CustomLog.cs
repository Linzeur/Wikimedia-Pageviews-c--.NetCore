using System;
using System.IO;

namespace ViewCounter.Services
{
    public class CustomLog
    {
        public static string folderPath;

        public static void WriteLog(string message, string stacktrace)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                if (!directoryInfo.Exists) directoryInfo.Create();

                Console.WriteLine();

                DateTime current = DateTime.Now;
                string fileName = string.Format("Log_{0}.txt", current.ToString("yyyyMMdd"));
                string fullPath = Path.Combine(folderPath, fileName);
                using (StreamWriter sw = new StreamWriter(fullPath, true))
                {
                    sw.WriteLine("Time = {0}", current.ToString("HH:mm:ss"));
                    sw.WriteLine("Message = {0}", message);
                    sw.WriteLine("Stacktrace = {0}", stacktrace);
                    sw.WriteLine();
                }

                Console.WriteLine("There was an issue in the application. Check the log file");
                Console.WriteLine("Press enter key to end");
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ReadKey();
            }
        }
    }
}
