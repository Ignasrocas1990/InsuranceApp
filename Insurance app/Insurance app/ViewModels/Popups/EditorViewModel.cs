using System.Windows.Input;
using Insurance_app.Pages.Popups;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    
    public class EditorViewModel : ObservableObject
    {
        private readonly EditorPopup popup;
        public ICommand CloseCommand { get; }
        public ICommand SubmitCommand { get; }
        public EditorViewModel(EditorPopup popup, string heading, bool readOnly, string popupDisplayText)
        {
            this.popup = popup;
            CloseCommand = new Command(Close);
            SubmitCommand = new Command(Submit);
            HeadingDisplay = heading;
            ReadOnlyDisplay = readOnly;
            ExtraInfoDisplay = popupDisplayText;
        }

        private void Submit()
        {
            popup.Dismiss(extraInfo);

        }
        private void Close()
        {
            popup.Dismiss("");
        }

        private string heading;
        public string HeadingDisplay
        {
            get => heading;
            set => SetProperty(ref heading, value);
        }

        private string extraInfo;
        public string ExtraInfoDisplay
        {
            get => extraInfo;
            set => SetProperty(ref extraInfo, value);
        }

        private bool readOnly;
        public bool ReadOnlyDisplay
        {
            get => readOnly;
            set => SetProperty(ref readOnly, value);
        }




    }
}