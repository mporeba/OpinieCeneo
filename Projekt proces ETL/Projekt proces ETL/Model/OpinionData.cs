using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Projekt_proces_ETL.Model
{
    public class OpinionData
    {
        public string AuthorName { get; set; }
        public string RecommendOrNotRecommend { get; set; }
        public string ReviewSummary { get; set; }
        public string StarRating { get; set; }
        public string OpinionDate { get; set; }
        public string OpinionUsefulCounter { get; set; }
        public string OpinionUselessCounter { get; set; }
        public List<string> Advantages { get; set; }
        public List<string> Disadvantages { get; set; }
        public List<string> SubOpinions { get; set; }

        private List<string> subReviewSummary = new List<string>();
        private List<string> subAuthorName = new List<string>();

        public OpinionData(List<string> content)
        {
            GetOpinionData(content);
        }

        void GetOpinionData(List<string> content)
        {
            int reviewSummaryCounter = 0;
            Regex starRatingRegex = new Regex("(([0-9],[0-9])|[0-9])/5</span>");

            AuthorName = content[7]
                .Split('<')
                .FirstOrDefault()
                .ToString()
                .Trim();

            if (content[13].Contains("Polecam"))
            {
                RecommendOrNotRecommend = "Poleca";
            }
            else
            {
                RecommendOrNotRecommend = "Nie Poleca";
            }

            for (int i = 0; i < content.Count; i++)
            {
                Match matchStarRating = starRatingRegex.Match(content[i]);
                if (matchStarRating.Success)
                {
                    StarRating = matchStarRating
                        .Value
                        .Split('/')
                        .FirstOrDefault();
                }

                if (content[i].Contains("time datetime="))
                {
                    OpinionDate = new Regex("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}")
                        .Match(content[i])
                        .Value;
                }

                if (content[i].Contains("<p class=\"product-review-body\">"))
                {
                    string result = content[i]
                        .Trim()
                        .Substring(31)
                        .Split('<')
                        .FirstOrDefault();

                    reviewSummaryCounter++;

                    if (reviewSummaryCounter == 1)
                    {
                        ReviewSummary = result;
                    }
                    else
                    {
                        subReviewSummary.Add(result);
                    }
                }

                if (content[i].Contains("<p class=\"product-review-byline\">"))
                {
                    int j = i + 1;
                    string result = content[j]
                        .Trim()
                        .Substring(8)
                        .Split('<')
                        .FirstOrDefault();

                    subAuthorName.Add(result);
                }

                if (content[i].Contains("<span class=\"pros\">Zalety</span>"))
                {
                    int stardIndex = i;
                    Advantages = GetAdvantagesOrDisadvantages(content, stardIndex);
                }

                if (content[i].Contains("<span class=\"cons\">Wady</span>"))
                {
                    int stardIndex = i;
                    Disadvantages = GetAdvantagesOrDisadvantages(content, stardIndex);
                }

                Regex regexOpinionUsefulOrUseless = new Regex("[0-9]+</span></button>$");
                Match match = regexOpinionUsefulOrUseless.Match(content[i]);

                if (content[i].Contains("vote-yes js_product-review-vote js_vote-yes"))
                {
                    OpinionUsefulCounter = match
                        .Value
                        .Split('<')
                        .FirstOrDefault();
                }

                if (content[i].Contains("vote-no js_product-review-vote js_vote-no"))
                {
                    OpinionUselessCounter = match
                        .Value
                        .Split('<')
                        .FirstOrDefault();
                }
            }

            SubOpinions = CombineSubAuthorAndSubSummary(subAuthorName, subReviewSummary);
        }

        List<string> GetAdvantagesOrDisadvantages(List<string> content, int startIndex)
        {
            List<string> advantageContentList = new List<string>();
            List<string> result = new List<string>();
            int liOpenCounter = 0;
            int liCloseCounter = 0;
            int startIndexAdvantage = 0;
            int endIndexAdvantage = 0;

            for (int i = startIndex; i < content.Count; i++)
            {
                if (content[i].Contains("<li"))
                {
                    liOpenCounter++;
                    startIndexAdvantage = i;
                }

                if (content[i].Contains("</li>"))
                {
                    endIndexAdvantage = i;
                    liCloseCounter++;
                }

                if ((liOpenCounter == liCloseCounter) && (liOpenCounter != 0))
                {
                    endIndexAdvantage++;
                    for (int j = startIndexAdvantage; j < endIndexAdvantage; j++)
                    {
                        advantageContentList.Add(content[j]);
                    }
                    liOpenCounter = 0;
                    liCloseCounter = 0;
                }

                if (content[i].Contains("</ul>"))
                {
                    if (advantageContentList != null)
                    {
                        result = GetAdvantageHelper(advantageContentList);
                    }
                    break;
                }
            }

            return result;
        }

        List<string> GetAdvantageHelper(List<string> content)
        {
            string regexPattern = "(<li>)|(</li>)";
            Regex regexLi = new Regex(regexPattern);

            List<string> helperList = new List<string>();
            List<string> result = new List<string>();

            foreach (var item in content)
            {
                Match matchLi = regexLi.Match(item);

                if (matchLi.Success)
                {
                    helperList.Add(
                        Regex.Replace(item, regexPattern, "")
                        );
                }
                else
                {
                    helperList.Add(item);
                }
            }

            if (helperList.Any())
            {
                foreach (var item in helperList)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        result.Add(
                            string.Concat("- ", item.Trim())
                            );
                    }
                }
            }

            return result;
        }

        List<string> CombineSubAuthorAndSubSummary(List<string> author, List<string> summary)
        {
            List<string> combine = new List<string>();

            for (int i = 0; i < author.Count; i++)
            {
                combine.Add(
                    string.Concat(
                        "- ",
                        author[i],
                        "\n",
                        summary[i]
                        ));
            }

            return combine;
        }
    }

    public class OpinionDataHTML
    {
        private int opinionCounter = 0;
        public Dictionary<int, List<string>> DictionaryWithHTMLOpinions { get; private set; }

        public void GetHTMLOpinions(string[] page)
        {
            int startIndex = 0;
            string pattern = "li class=\"review-box js_product-review\"";

            if (DictionaryWithHTMLOpinions == null)
                DictionaryWithHTMLOpinions = new Dictionary<int, List<string>>();

            for (int i = 0; i < page.Length; i++)
            {
                if (page[i].Contains(pattern))
                {
                    startIndex = i;
                    DictionaryWithHTMLOpinions.Add(opinionCounter, GetHTMLOpinion(startIndex, page));
                    opinionCounter++;
                }
            }
        }

        /// <summary>
        /// Pobieranie opinii z kodu HTML, od znacznika <li> do znacznika </li>
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        List<string> GetHTMLOpinion(int startIndex, string[] file)
        {
            int endIndex = 0;
            List<string> opinion = new List<string>();
            int liOpenCounter = 0;
            int liCloseCounter = 0;

            for (int i = startIndex; i < file.Length; i++)
            {
                if (file[i].Contains("<li"))
                    liOpenCounter++;

                if (file[i].Contains("</li>"))
                    liCloseCounter++;

                if ((liOpenCounter == liCloseCounter) && (liOpenCounter != 0))
                {
                    endIndex = ++i;

                    opinion = OneOpinionHTML(startIndex, endIndex, file);
                    break;
                }
            }
            return opinion;
        }

        List<string> OneOpinionHTML(int startIndex, int endIndex, string[] file)
        {
            List<string> opinion = new List<string>();
            for (int i = startIndex; i < endIndex; i++)
            {
                opinion.Add(file[i]);
            }
            return opinion;
        }
    }
}
