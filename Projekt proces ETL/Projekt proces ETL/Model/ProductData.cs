using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Projekt_proces_ETL.Model
{
    public class ProductData
    {
        private string url;
        private string pathToFileWebsiteTxt;

        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
        public string Variants { get; set; }
        public string BriefDescription { get; set; }
        public string Rating { get; set; }
        public string NumberOfReviews { get; set; }
        public bool MoreOpinionsExist { get; set; }

        public ProductData(string url, string pathToWebsiteTxt)
        {
            this.url = url;
            this.pathToFileWebsiteTxt = pathToWebsiteTxt;
            GetMainData();
        }

        void GetMainData()
        {
            List<List<string>> opinions = new List<List<string>>();
            Regex regex = new Regex("Pokaż wszystkie [0-9]+ (opinie|opinii)");

            var file = File.ReadAllLines(pathToFileWebsiteTxt);

            for (int i = 0; i < file.Length; i++)
            {
                Match match = regex.Match(file[i]);
                if (match.Success)
                {
                    MoreOpinionsExist = true;
                }

                if (file[i].Contains(" <meta property=\"og:brand\""))
                {
                    Brand = file[i]
                        .Trim()
                        .Substring(35)
                        .Split('"')
                        .FirstOrDefault();
                }

                if (file[i].Contains(" googletag.pubads().setTargeting(\"catMin\""))
                {
                    Type = file[i]
                        .Trim()
                        .Substring(42)
                        .Split('"')
                        .FirstOrDefault();
                }

                if (file[i].Contains("  <strong class=\"js_searchInGoogleTooltip\" data-onselect=\"true\" data-tooltip-autowidth=\"true\">"))
                {
                    ProductName = file[i]
                        .Split(new string[] { "\"true\">" }, StringSplitOptions.None)
                        .Last()
                        .Split(new string[] { "</strong>" }, StringSplitOptions.None)
                        .FirstOrDefault();
                }

                if (file[i].Contains("og:price:amount"))
                {
                    Price = file[i]
                        .Trim()
                        .Substring(42)
                        .Split('"')
                        .FirstOrDefault();
                }

                if (file[i].Contains("<dt>Warianty:</dt>"))
                {
                    int j = i + 11;

                    List<string> variants = new List<string>();
                    while (j < i + 50)
                    {
                        if (!file[j].Contains('<'))
                        {
                            variants.Add(file[j].Trim());
                        }

                        if (file[j].Contains("</dd>"))
                            break;

                        j++;
                    }

                    Variants = string.Join(", ", variants.ToArray());
                }

                if (file[i].Contains("<div class=\"ProductSublineTags\">"))
                {
                    BriefDescription = file[i]
                        .Trim()
                        .Substring(32)
                        .Split('<')
                        .FirstOrDefault();
                }

                if (file[i].Contains("<span class=\"product-score\" itemprop=\"ratingValue\""))
                {
                    Rating = file[i]
                        .Trim()
                        .Substring(66);
                }

                if (file[i].Contains("<span itemprop=\"reviewCount\">"))
                {
                    NumberOfReviews = Regex.Replace(file[i], "[^0-9]+", string.Empty);
                }
            }
        }
    }
}
