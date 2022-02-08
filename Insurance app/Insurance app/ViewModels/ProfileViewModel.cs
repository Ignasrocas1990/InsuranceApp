using Insurance_app.Logic;
using Insurance_app.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels
{
    public class ProfileViewModel:ObservableObject
    {
        private UserManager userManager;
        public ProfileViewModel()
        {
            userManager = new UserManager();
        }
    }
}