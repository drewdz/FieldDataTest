namespace FieldDataTest.ViewModels
{
    public class ConesViewModel : BaseViewModel, IViewModel
    {
        #region Fields

        #endregion Fields

        #region Constructors

        #endregion Constructors

        #region Properties

        private string _Cone1 = string.Empty;
        public string Cone1
        {
            get => _Cone1; 
            set => SetProperty(ref _Cone1, value);
        }

        private string _Cone2 = string.Empty;
        public string Cone2
        {
            get => _Cone2;
            set => SetProperty(ref _Cone2, value);
        }

        private string _Cone3 = string.Empty;
        public string Cone3
        {
            get => _Cone3;
            set => SetProperty(ref _Cone3, value);
        }

        private string _Cone4 = string.Empty;
        public string Cone4
        {
            get => _Cone4;
            set => SetProperty(ref _Cone4, value);
        }

        #endregion Properties

        #region Operations

        public override void ViewLoaded()
        {
        }

        #endregion Operations
    }
}
