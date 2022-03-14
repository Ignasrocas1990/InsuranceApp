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
        public EditorViewModel(EditorPopup popup)
        {
            this.popup = popup;
            CloseCommand = new Command(Close);
            SubmitCommand = new Command(Submit);
        }

        private void Submit()
        {
            popup.Dismiss(extraInfo);

        }
        private void Close()
        {
            popup.Dismiss("");
        }

        private string extraInfo;
        public string ExtraInfoDisplay
        {
            get => extraInfo;
            set => SetProperty(ref extraInfo, value);
        }
        
        
        
        
    }
}