using HtmlAgilityPack;
using Projekt_proces_ETL.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Projekt_proces_ETL.ViewModel
{
    internal class Product
    {
        private string url = "https://www.ceneo.pl/";
        private string pathToFileWebsiteTxt = string.Empty;
        private string pathToFileOneOpinion = string.Empty;

        public Opinions Opinions { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
        public string Variants { get; set; }
        public string BriefDescription { get; set; }
        public string Rating { get; set; }
        public string NumberOfReviews { get; set; }
        public string ProductKey { get; set; }
        public string Description { get; set; }

        public Product(string productKey)
        {
            this.url = string.Concat(url, productKey);
        }

        public Product()
        { }

        public void FirstStepCreateTempAndExtractWebsite(out bool result)
        {
            CreateDictionaryAndFilesInTemp();
            result = ConvertHTMLToTxt(url);
        }

        public void SecondStepExtractOpinions()
        {
            bool moreOpinionsExist = false;
            int opinionNumberPage = 1;
            string opinionUrl = "opinie-";
            string opinionFirstPage = "#tab=reviews";
            OpinionDataHTML opinionData = new OpinionDataHTML();
            Dictionary<int, List<string>> dictionaryWithHTMLOpinions = new Dictionary<int, List<string>>();

            ProductData mainData = new ProductData(url, pathToFileWebsiteTxt);
            this.Brand = mainData.Brand;
            this.BriefDescription = mainData.BriefDescription;
            this.NumberOfReviews = mainData.NumberOfReviews;
            this.Price = mainData.Price;
            this.ProductName = mainData.ProductName;
            this.Rating = mainData.Rating;
            this.Type = mainData.Type;
            this.Variants = mainData.Variants;
            moreOpinionsExist = mainData.MoreOpinionsExist;

            if (moreOpinionsExist)
            {
                bool isMorePage = ExistMoreThanOnePage(pathToFileWebsiteTxt);

                if (opinionNumberPage == 1)
                {
                    string subUrl = string.Format(
                        "{0}/{1}",
                        url,
                        opinionFirstPage
                        );

                    ConvertHTMLToTxt(subUrl);
                    GetHTMLOpinions(pathToFileWebsiteTxt, opinionData);
                }

                while (isMorePage)
                {
                    opinionNumberPage++;

                    string subUrl = string.Format(
                        "{0}/{1}{2}",
                        url,
                        opinionUrl,
                        opinionNumberPage.ToString()
                        );

                    ConvertHTMLToTxt(subUrl);
                    GetHTMLOpinions(pathToFileWebsiteTxt, opinionData);

                    isMorePage = ExistMoreThanOnePage(pathToFileWebsiteTxt);
                }
            }
            else
            {
                GetHTMLOpinions(pathToFileWebsiteTxt, opinionData);
            }

            dictionaryWithHTMLOpinions = opinionData.DictionaryWithHTMLOpinions;

            Opinions = new Opinions(dictionaryWithHTMLOpinions, pathToFileOneOpinion);

            RemoveFilesFromTemp();
        }

        private void GetHTMLOpinions(string filePath, OpinionDataHTML opinionData)
        {
            var file = File.ReadAllLines(filePath);
            opinionData.GetHTMLOpinions(file);
        }

        private static bool ExistMoreThanOnePage(string filePath)
        {
            bool result = false;
            var file = File.ReadAllLines(filePath);

            Regex regexNextPageWithNumber = new Regex("[0-9]+</a></li>");
            Regex regexNastepna = new Regex("Następna");

            for (int i = 2; i < file.Length; i++)
            {
                int j = i - 1;
                int x = i - 2;

                Match matchNextPageWithoutDots = regexNextPageWithNumber.Match(file[j]); //jest list bez ... przy Nastepna
                Match matchNextPageWithDots = regexNextPageWithNumber.Match(file[x]); // jesli przy Następna są ...
                Match matchNext = regexNastepna.Match(file[i]);

                if (true
                    && matchNext.Success
                    && (matchNextPageWithDots.Success || matchNextPageWithoutDots.Success))
                {
                    result = true;
                }
            }

            return result;
        }

        private bool ConvertHTMLToTxt(string url)
        {
            File.WriteAllText(pathToFileWebsiteTxt, string.Empty);

            try
            {
                var web = new HtmlWeb();
                var html = web.Load(url).ParsedText;
                File.WriteAllText(pathToFileWebsiteTxt, html);

                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        private void CreateDictionaryAndFilesInTemp()
        {
            string path = Path.Combine(Path.GetTempPath(), "Process ETL");
            pathToFileWebsiteTxt = Path.Combine(path, "Website.txt");
            pathToFileOneOpinion = Path.Combine(path, "Opinion.txt");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(pathToFileWebsiteTxt))
            {
                File.Create(pathToFileWebsiteTxt)
                    .Close();
            }

            if (!File.Exists(pathToFileOneOpinion))
            {
                File.Create(pathToFileOneOpinion)
                    .Close();
            }
        }

        private void RemoveFilesFromTemp()
        {
            if (File.Exists(pathToFileWebsiteTxt))
            {
                File.Delete(pathToFileWebsiteTxt);
            }

            if (File.Exists(pathToFileOneOpinion))
            {
                File.Delete(pathToFileOneOpinion);
            }
        }
    }
}