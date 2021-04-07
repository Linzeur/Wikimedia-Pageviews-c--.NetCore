using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using ViewCounter.Services;

namespace ViewCounter
{
    class Program
    {
        public static int MAX_LENGTH_CHARACTER_BY_LINE = 78;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
            var config = builder.Build();
            string basePath = config["FilesToDownload:PathToStore"].ToString();
            int numberFilesToDownload = int.Parse(config["FilesToDownload:NumberLastHours"]);

            Console.WriteLine("Application Started");
            Console.WriteLine();
            DateTime currentTime = DateTime.Now;
            string finalDateTime = currentTime.ToString("yyyyMMdd_HH00");
            string initialDateTime = currentTime.AddHours(numberFilesToDownload * -1).ToString("yyyyMMdd_HH00");
            string directoryDestination = Path.Combine(basePath, string.Format("Dumps Wikimedia Files {0} to {1}", initialDateTime, finalDateTime));

            DirectoryInfo directoryInfo = new DirectoryInfo(path: directoryDestination);
            if (!directoryInfo.Exists)
            {
                Console.WriteLine("Initialize download files from last " + numberFilesToDownload + " hours");
                Console.WriteLine();
                directoryInfo.Create();
                for (int i = 1; i <= numberFilesToDownload; i++)
                {
                    DumpWikimediaFiles dumpWikimediaFiles = new DumpWikimediaFiles(directoryDestination, currentTime);
                    dumpWikimediaFiles.DownloadFile();
                    dumpWikimediaFiles.DecompressFile();
                    currentTime = currentTime.AddHours(-1);
                }
            }
            else
            {
                Console.WriteLine("Files already downloaded and decompressed from the last " + numberFilesToDownload + " hours");
                Console.WriteLine();
            }

            DisplayHeaderProgram();
            DisplayOptionsProgram(directoryInfo);

        }
        static void DisplayHeaderProgram()
        {
            Console.WriteLine(new string('*', MAX_LENGTH_CHARACTER_BY_LINE));
            Console.Write("*");
            Console.Write(new string(' ', MAX_LENGTH_CHARACTER_BY_LINE - 2));
            Console.WriteLine("*");
            string titleProgram = "Welcome to Analysis of Data for Pageview from Wikimedia Foundation";
            Console.WriteLine(string.Format("*{0}{1}{0}*", new string(' ', 5), titleProgram));
            Console.Write("*");
            Console.Write(new string(' ', MAX_LENGTH_CHARACTER_BY_LINE - 2));
            Console.WriteLine("*");
            Console.WriteLine(new string('*', MAX_LENGTH_CHARACTER_BY_LINE));
            Console.WriteLine();
        }

        static void DisplayOptionsProgram(DirectoryInfo directory)
        {
            Console.WriteLine("List options to choose:");
            Console.WriteLine("\t1: Display the max viewed count for language & domain by file");
            Console.WriteLine("\t2: Display the max viewed count for page title by file");
            Console.WriteLine("\t3: End application");
            Console.WriteLine();
            Console.Write("What option would you like to choose?: ");
            string option = Console.ReadLine();
            while (option != "1" && option != "2" && option != "3")
            {
                Console.Write("Invalid option. Please choose a valid option: ");
                option = Console.ReadLine();
            }
            Console.WriteLine();
            ActionsByOption(option, directory);
        }

        static void ActionsByOption(string option, DirectoryInfo directory)
        {
            DumpWikimediaContent dumpWikimediaContent = new DumpWikimediaContent(directory);

            switch (option)
            {
                case "1":
                    Console.WriteLine("Language & Domain count");
                    Console.WriteLine(new string('=', MAX_LENGTH_CHARACTER_BY_LINE));
                    dumpWikimediaContent.ReportViewCountByLanguageAndDomain();
                    Console.WriteLine();
                    Console.WriteLine();
                    DisplayOptionsProgram(directory);
                    break;
                case "2":
                    Console.WriteLine("Language & Domain count");
                    Console.WriteLine(new string('=', MAX_LENGTH_CHARACTER_BY_LINE));
                    dumpWikimediaContent.ReportViewCountByPageTitle();
                    Console.WriteLine();
                    Console.WriteLine();
                    DisplayOptionsProgram(directory);
                    break;
                default:
                    Console.Write("Do you like to keep all files downloaded (y/n)?: ");
                    string answer = Console.ReadLine().ToLower();
                    while (answer != "y" && answer != "n")
                    {
                        Console.Write("Invalid option. Do you like to delete all files downloaded (y/n)?: ");
                        answer = Console.ReadLine().ToLower(); ;
                    }
                    if (answer == "n") Console.WriteLine("Deleting files");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Please, press enter key to end program...");
                    Console.ReadKey();
                    break;

            }
        }

    }



}