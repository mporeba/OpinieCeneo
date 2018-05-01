using Projekt_proces_ETL.Model;
using System.Collections.Generic;

namespace Projekt_proces_ETL.ViewModel
{
    class Opinion
    {
        private List<string> subReviewSummary = new List<string>();
        private List<string> subAuthorName = new List<string>();

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
        
        public Opinion(List<string> content)
        {
            GetOpinion(content);
        }

        void GetOpinion(List<string> content)
        {
            OpinionData opinionData = new OpinionData(content);
            this.Advantages = opinionData.Advantages;
            this.AuthorName = opinionData.AuthorName;
            this.Disadvantages = opinionData.Disadvantages;
            this.OpinionDate = opinionData.OpinionDate;
            this.OpinionUsefulCounter = opinionData.OpinionUsefulCounter;
            this.OpinionUselessCounter = opinionData.OpinionUselessCounter;
            this.RecommendOrNotRecommend = opinionData.RecommendOrNotRecommend;
            this.ReviewSummary = opinionData.ReviewSummary;
            this.StarRating = opinionData.StarRating;
            this.SubOpinions = opinionData.SubOpinions;
        }
    }
}
