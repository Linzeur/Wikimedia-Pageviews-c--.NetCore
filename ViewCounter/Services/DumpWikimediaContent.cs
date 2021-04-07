using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViewCounter.Entities;

namespace ViewCounter.Services
{
    public class DumpWikimediaContent
    {
        public DirectoryInfo _directory;

        private Dictionary<string, string> domainTrailingPart = new Dictionary<string, string>()
        {
            {"-", "wikipedia" },
            {"b", "wikibooks" },
            {"d", "wiktionary" },
            {"f", "wikimediafoundation" },
            {"m", "wikimedia" },
            {"mw", "global domain" },
            {"n", "wikinews" },
            {"q", "wikiquote" },
            {"s", "wikisource" },
            {"v", "wikiversity" },
            {"voy", "wikivoyage" },
            {"w", "mediawiki" },
            {"wd", "wikidata" }
        };

        public DumpWikimediaContent(DirectoryInfo directory)
        {
            _directory = directory;
        }


        private void GetMaxViewCountByDomainAndLanguage(int[] sizeColumns, string fullPath, string fileName)
        {
            Domain domainMaxViewCount = null;
            Domain tempDomain = null;

            using (StreamReader sw = new StreamReader(fullPath))
            {

                while (!sw.EndOfStream)
                {
                    string line = sw.ReadLine();
                    string[] fields = line.Split(" ");

                    string domainCode = fields[0];
                    int viewCount = int.Parse(fields[2]);

                    if (tempDomain == null)
                        tempDomain = new Domain(domainCode, viewCount);
                    else
                    {
                        if (domainCode == tempDomain.domainCode)
                            tempDomain.viewCount += viewCount;
                        else
                        {
                            if (domainMaxViewCount == null || tempDomain.viewCount > domainMaxViewCount.viewCount)
                                domainMaxViewCount = new Domain(tempDomain.domainCode, tempDomain.viewCount);
                            else
                                tempDomain = new Domain(domainCode, viewCount);
                        }
                    }
                }
            }
            string[] columns = new string[4];
            columns[0] = (fileName.Substring(9, 2) + "hrs").PadRight(sizeColumns[0]);

            string[] arrDomainCode = domainMaxViewCount.domainCode.Split(".");
            columns[1] = arrDomainCode[0].PadRight(sizeColumns[1]);
            columns[2] =  domainTrailingPart[arrDomainCode[1]].PadRight(sizeColumns[2]);
            columns[3] = domainMaxViewCount.viewCount.ToString().PadRight(sizeColumns[3]);

            Console.WriteLine(string.Join(" ", columns));
        }

        private void GetMaxViewCountByPageTitle(int[] sizeColumns, string fullPath, string fileName)
        {
            Dictionary<string, Page> dictionaryPages = new Dictionary<string, Page>();
            using (StreamReader sw = new StreamReader(fullPath))
            {
                while (!sw.EndOfStream)
                {
                    string line = sw.ReadLine();
                    string[] fields = line.Split(" ");

                    string pageTitle = fields[1].ToLower();
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


        public void ReportViewCountByLanguageAndDomain()
        {
            int[] sizeColumns = new int[4] { 10, 20, 25, 20 };
            string[] columns = new string[4];
            columns[0] = "Period".PadRight(sizeColumns[0]);
            columns[1] = "Language".PadRight(sizeColumns[1]);
            columns[2] = "Domain".PadRight(sizeColumns[2]);
            columns[3] = "View Count".PadRight(sizeColumns[3]);
            string header = string.Join("|", columns);
            Console.WriteLine(header);
            Console.WriteLine(new string('=', header.Length));


            List<FileInfo> listFiles = _directory.EnumerateFiles().ToList();
            for (int i = 0; i < listFiles.Count; i++)
            {
                GetMaxViewCountByDomainAndLanguage(sizeColumns, listFiles[i].FullName, listFiles[i].Name);
            }
        }

        public void ReportViewCountByPageTitle()
        {
            int[] sizeColumns = new int[3] { 10, 30, 20 };
            string[] columns = new string[3];
            columns[0] = "Period".PadRight(sizeColumns[0]);
            columns[1] = "Page".PadRight(sizeColumns[1]);
            columns[2] = "View Count".PadRight(sizeColumns[2]);
            string header = string.Join("|", columns);
            Console.WriteLine(header);
            Console.WriteLine(new string('=', header.Length));


            List<FileInfo> listFiles = _directory.EnumerateFiles().ToList();
            for (int i = 0; i < listFiles.Count; i++)
            {
                GetMaxViewCountByPageTitle(sizeColumns, listFiles[i].FullName, listFiles[i].Name);
            }
        }

        
    }
}
