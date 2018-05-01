using Microsoft.Practices.Prism.Commands;
using Projekt_proces_ETL.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Projekt_proces_ETL
{
    class ProcessETLViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        public DelegateCommand DownloadData { get; set; }
        public DelegateCommand ECommand { get; set; }
        public DelegateCommand TCommand { get; set; }
        public DelegateCommand LCommand { get; set; }
        public DelegateCommand ETLCommand { get; set; }
        public DelegateCommand AddToDbCommand { get; set; }
        public DelegateCommand SaveOpinionCommand { get; set; }
        public DelegateCommand ClearOpinionsCommand { get; set; }
        public DelegateCommand ExportOpinionsToCsvCommand { get; set; }
        public DelegateCommand LoadFromDBCommand { get; set; }

        //https://www.ceneo.pl/30201447 5 opinii
        //https://www.ceneo.pl/47044601 iphone 195 opinii
        //https://www.ceneo.pl/10631826 25 opinii
        private string productKey = "10631826";
        public string ProductKey
        {
            get { return productKey; }
            set
            {
                IsEnableTButton = false;
                IsEnableLButton = false;
                productKey = value;
                OnPropertyChanged("ProductKey");
            }
        }

        private string productName;
        public string ProductName
        {
            get
            {
                return "Produkt: " + productName;
            }
            set
            {
                productName = value;
                OnPropertyChanged("ProductName");
            }
        }

        private string brand;
        public string Brand
        {
            get
            {
                return "Marka: " + brand;
            }
            set
            {
                brand = value;
                OnPropertyChanged("Brand");
            }
        }

        private string type;
        public string Type
        {
            get
            {
                return "Typ: " + type;
            }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        private string price;
        public string Price
        {
            get
            {
                return "Najniższa cena: " + price;
            }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        private string variants;
        public string Variants
        {
            get
            {
                return "Warianty: " + variants;
            }
            set
            {
                variants = value;
                OnPropertyChanged("Variants");
            }
        }

        private string briefDescription;
        public string BriefDescription
        {
            get
            {
                return "Opis: " + briefDescription;
            }
            set
            {
                briefDescription = value;
                OnPropertyChanged("BriefDescription");
            }
        }

        private string rating;
        public string Rating
        {
            get
            {
                return "Ocena: " + rating;
            }
            set
            {
                rating = value;
                OnPropertyChanged("Rating");
            }
        }

        private string numberOfReviews;
        public string NumberOfReviews
        {
            get
            {
                return "Liczba opinii: " + numberOfReviews;
            }
            set
            {
                numberOfReviews = value;
                OnPropertyChanged("NumberOfReviews");
            }
        }

        public string AuthorName => OpinionListBox == null ? string.Empty : OpinionListBox.AuthorName;

        public List<string> Advantages => OpinionListBox == null ? new List<string>() : OpinionListBox.Advantages;

        public List<string> Disadvantages => OpinionListBox == null ? new List<string>() : OpinionListBox.Disadvantages;

        public string RecommendOrNotRecommend => OpinionListBox == null ? string.Empty : OpinionListBox.RecommendOrNotRecommend;

        public string ReviewSummary => OpinionListBox == null ? string.Empty : OpinionListBox.ReviewSummary;

        public string StarRating => OpinionListBox == null ? string.Empty : OpinionListBox.StarRating;

        public string OpinionDate => OpinionListBox == null ? string.Empty : OpinionListBox.OpinionDate;

        public string OpinionUsefulCounter => OpinionListBox == null ? string.Empty : OpinionListBox.OpinionUsefulCounter;

        public string OpinionUselessCounter => OpinionListBox == null ? string.Empty : OpinionListBox.OpinionUselessCounter;

        public List<string> SubOpinions => OpinionListBox == null ? new List<string>() : OpinionListBox.SubOpinions;
        
        private Opinion opinionListBox;
        public Opinion OpinionListBox
        {
            get { return opinionListBox; }
            set
            {
                opinionListBox = value;

                OnPropertyChanged("OpinionListBox");
                OnPropertyChanged("AuthorName");
                OnPropertyChanged("RecommendOrNotRecommend");
                OnPropertyChanged("ReviewSummary");
                OnPropertyChanged("StarRating");
                OnPropertyChanged("OpinionDate");
                OnPropertyChanged("OpinionUsefulCounter");
                OnPropertyChanged("OpinionUselessCounter");
                OnPropertyChanged("Advantages");
                OnPropertyChanged("Disadvantages");
                OnPropertyChanged("SubOpinions");
            }
        }

        private ObservableCollection<Opinion> opinionsListBox;
        public ObservableCollection<Opinion> OpinionsListBox
        {
            get { return opinionsListBox; }
            set
            {
                opinionsListBox = value;
                OnPropertyChanged("OpinionsListBox");
            }
        }

        private string headerNumberOfReviews;
        public string HeaderNumberOfReviews
        {
            get
            {
                return headerNumberOfReviews ?? "Opinie (0)";
            }
            set
            {
                headerNumberOfReviews = value;
                OnPropertyChanged("HeaderNumberOfReviews");
            }
        }

        private bool isEnableTButton = false;
        public bool IsEnableTButton
        {
            get
            {
                return isEnableTButton;
            }
            set
            {
                isEnableTButton = value;
                OnPropertyChanged("IsEnableTButton");
            }
        }

        private bool isEnableLButton = false;
        public bool IsEnableLButton
        {
            get
            {
                return isEnableLButton;
            }
            set
            {
                isEnableLButton = value;
                OnPropertyChanged("IsEnableLButton");
            }
        }

        private Product logic;
        #endregion

        public ProcessETLViewModel()
        {
            ECommand = new DelegateCommand(ExtractCommand);
            TCommand = new DelegateCommand(TransformCommand);
            LCommand = new DelegateCommand(LoadCommand);
            ETLCommand = new DelegateCommand(WholeProcess);
            ClearOpinionsCommand = new DelegateCommand(ClearOpinions);
            ExportOpinionsToCsvCommand = new DelegateCommand(ExportOpinionsToCsv);
            SaveOpinionCommand = new DelegateCommand(SaveOpinionAsTxt);
            LoadFromDBCommand = new DelegateCommand(LoadProducts);
        }

        #region Private Methods

        void ClearOpinions()
        {
            
        }

        void ExportOpinionsToCsv()
        {
            
        }

        void LoadProducts()
        {
            
        }

        void SaveOpinionAsTxt()
        {
            
        }

        void ExtractCommand()
        {
            bool boolValidationPageExist;
            this.logic = null;

            this.logic = new Product(ProductKey);
            this.logic.FirstStepCreateTempAndExtractWebsite(out boolValidationPageExist);

            if (!boolValidationPageExist)
            {
                ProductKey = "Nie ma takiego produktu!";
                return;
            }

            IsEnableTButton = true;
        }

        void TransformCommand()
        {
            this.logic.SecondStepExtractOpinions();
            AssignData();
            IsEnableLButton = true;
        }

        void LoadCommand()
        {            
            
        }

        void WholeProcess()
        {
            bool boolValidationPageExist;
            this.logic = null;

            this.logic = new Product(ProductKey);
            this.logic.FirstStepCreateTempAndExtractWebsite(out boolValidationPageExist);

            if (!boolValidationPageExist)
            {
                ProductKey = "Nie ma takiego produktu!";
                return;
            }

            this.logic.SecondStepExtractOpinions();
            AssignData();            
            LoadCommand();
        }

        void AssignData()
        {
            this.ProductName = this.logic.ProductName;
            this.Brand = this.logic.Brand;
            this.Type = this.logic.Type;
            this.Price = this.logic.Price;
            this.Variants = this.logic.Variants;
            this.Rating = this.logic.Rating;
            this.NumberOfReviews = this.logic.NumberOfReviews;
            this.BriefDescription = this.logic.BriefDescription;
            this.HeaderNumberOfReviews = string.Concat("Opinie (", logic.Opinions.OpinionsWithData.Count, ")");

            OpinionsListBox = new ObservableCollection<Opinion>();
            foreach (var item in logic.Opinions.OpinionsWithData)
            {
                OpinionsListBox.Add(item);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
