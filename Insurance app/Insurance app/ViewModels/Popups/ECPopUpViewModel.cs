using System;
using System.Collections.Generic;
using System.Windows.Input;
using Insurance_app.Models;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    public class EcPopUpViewModel : ObservableObject
    {
        public ObservableRangeCollection<Claim> Claims { get; set; }
        private readonly ExistingClaimsPopup popup;
        public ICommand CloseCommand { get; }

        public EcPopUpViewModel(ExistingClaimsPopup popup, List<Claim> existingClaims)
        {
            this.popup = popup;
            Claims = new ObservableRangeCollection<Claim>(existingClaims);
            CloseCommand = new Command(ClosePopUp);
        }
        private void ClosePopUp()
        {
            Claims = null;
            popup.Dismiss("");
        }
    }
}