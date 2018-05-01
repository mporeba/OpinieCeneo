using Projekt_proces_ETL.ViewModel;
using System.Collections.Generic;
using System.IO;

namespace Projekt_proces_ETL.Model
{
    class Opinions
    {
        public List<Opinion> OpinionsWithData { get; set; }

        public Opinions(Dictionary<int, List<string>> opinionsList, string pathToOpinionTxt)
        {
            GetOpinions(pathToOpinionTxt, opinionsList);
        }

        void GetOpinions(string pathToOpinionTxt, Dictionary<int, List<string>> opinions)
        {
            if (OpinionsWithData == null)
                OpinionsWithData = new List<Opinion>();

            foreach (var opinionHTML in opinions.Values)
            {
                File.WriteAllLines(pathToOpinionTxt, opinionHTML);

                OpinionsWithData.Add(
                    new Opinion(opinionHTML)
                    );
            }
        }
    }
}
