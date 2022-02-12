using System;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Java.Lang;
using Xamarin.Essentials;
using Xamarin.Forms;
using Exception = System.Exception;

namespace Insurance_app
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var svm = ShellViewModel.GetInstance();
                var t = ((HomeViewModel) svm.GetViewModel(Converter.HomeViewModel));
                t.Dispose();
                t = null;
                var t2 = ((ClaimViewModel) svm.GetViewModel(Converter.ClaimViewModel));
                    t2.Dispose();
                    t2 = null;

                var t3 = ((ProfileViewModel) svm.GetViewModel(Converter.ProfileViewModel));
                t3.Dispose();
                t3 = null;
                var t4 = ((ReportViewModel) svm.GetViewModel(Converter.ReportViewModel));
                t4.Dispose();
                svm.Dispose();
                svm = null;
                if (App.RealmApp.CurrentUser != null)
                {
                    await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                }
                await Current.GoToAsync($"//{nameof(LogInPage)}",true);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}