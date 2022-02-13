using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    public class InfoPopupViewModel : ObservableObject
    {
        private InfoPopup infoPopup;
        public ICommand CloseCommand { get; }


        public InfoPopupViewModel(InfoPopup infoPopup, string type)
        {
            this.infoPopup = infoPopup;
            CloseCommand = new Command(Close);
            InfoPicker(type);
        }

        private void Close()
        {
            infoPopup.Dismiss("");
        }

        private void InfoPicker(string type)
        {
            string longInfoString = StaticOptions.InfoTest(type);
            var splitInfo = longInfoString.Split('~');
            InfoDisplayH1 = splitInfo[0];
            InfoDisplayH2 = splitInfo[1];
            InfoDisplayH3 = splitInfo[2];
            InfoDisplayC1 = splitInfo[3];
            InfoDisplayC2 = splitInfo[4];
            InfoDisplayC3 = splitInfo[5];
            
        }
        private string column1Head;
        public string InfoDisplayH1
        {
            get => column1Head;
            set => SetProperty(ref column1Head, value);
        }
        private string column2Head;
        public string InfoDisplayH2
        {
            get => column2Head;
            set => SetProperty(ref column2Head, value);
        }
        private string column3Head;
        public string InfoDisplayH3
        {
            get => column3Head;
            set => SetProperty(ref column3Head, value);
        }
        
        private string column1;
        public string InfoDisplayC1
        {
            get => column1;
            set => SetProperty(ref column1, value);
        }
        private string column2;
        public string InfoDisplayC2
        {
            get => column2;
            set => SetProperty(ref column2, value);
        }

        private string column3;

        public string InfoDisplayC3
        {
            get => column3;
            set => SetProperty(ref column3, value);
        }
    }
}