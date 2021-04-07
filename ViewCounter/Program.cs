using Microsoft.Extensions.Configuration;
using System;
using ViewCounter.Services;

namespace ViewCounter
{
    class Program
    {
        public static int MAX_LENGTH_CHARACTER_BY_LINE = 78;

        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
                var config = builder.Build();
                string basePath = config["FilesToDownload:PathToStore"].ToString();
                int numberFilesToDownload = int.Parse(config["FilesToDownload:NumberLastHours"]);

                string logFolderPath = config["Logs:PathToStore"].ToString();
                CustomLog.folderPath = logFolderPath;

                Console.WriteLine("Application Started");
                Console.WriteLine();
                
                DumpWikimediaFiles dumpWikimediaFiles = new DumpWikimediaFiles(basePath, numberFilesToDownload);
                dumpWikimediaFiles.DownloadAllFiles();

                DisplayHeaderProgram();
                DisplayOptionsProgram(dumpWikimediaFiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine("Verify if the configuration file exist or have the right values");
                Console.ReadKey();
            }

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

        static void DisplayOptionsProgram(DumpWikimediaFiles objDumpWikimediaFiles)
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
            ActionsByOption(option, objDumpWikimediaFiles);
        }

        static void ActionsByOption(string option, DumpWikimediaFiles objDumpWikimediaFiles)
        {
            DumpWikimediaContent dumpWikimediaContent = new DumpWikimediaContent(objDumpWikimediaFiles.directoryDestinationInfo);

            switch (option)
            {
                case "1":
                    Console.WriteLine("Language & Domain count");
                    Console.WriteLine(new string('=', MAX_LENGTH_CHARACTER_BY_LINE));
                    dumpWikimediaContent.ReportViewCountByLanguageAndDomain();
                    Console.WriteLine();
                    Console.WriteLine();
                    DisplayOptionsProgram(objDumpWikimediaFiles);
                    break;
                case "2":
                    Console.WriteLine("Language & Domain count");
                    Console.WriteLine(new string('=', MAX_LENGTH_CHARACTER_BY_LINE));
                    dumpWikimediaContent.ReportViewCountByPageTitle();
                    Console.WriteLine();
                    Console.WriteLine();
                    DisplayOptionsProgram(objDumpWikimediaFiles);
                    break;
                default:
                    Console.Write("Do you like to keep all files downloaded (y/n)?: ");
                    string answer = Console.ReadLine().ToLower();
                    while (answer != "y" && answer != "n")
                    {
                        Console.Write("Invalid option. Do you like to delete all files downloaded (y/n)?: ");
                        answer = Console.ReadLine().ToLower(); ;
                    }
                    if (answer == "n") objDumpWikimediaFiles.DeleteAllFiles();
                    Console.WriteLine();
                    Console.Write("Aplication Ended");
                    break;

            }
        }

    }

}