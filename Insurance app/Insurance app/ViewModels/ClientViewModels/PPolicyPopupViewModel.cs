using System;
using System.Collections.Generic;
using System.Windows.Input;
using Insurance_app.Models;
using Insurance_app.Pages.ClientPages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.ClientViewModels
{
    public class PPolicyPopupViewModel:ObservableObject
    {
        private PreviousPolicyPopup popup;
        public ICommand CloseCommand { get; }
        public ObservableRangeCollection<Policy> PreviousPolicies { get; set; }

        public PPolicyPopupViewModel(PreviousPolicyPopup popup, List<Policy> previousPolicies)
        {
            this.popup = popup;
            PreviousPolicies = new ObservableRangeCollection<Policy>(previousPolicies);
            CloseCommand = new Command(ClosePopUp);

        }

        private void ClosePopUp()
        {
            popup.Dismiss("");
        }
    }
}