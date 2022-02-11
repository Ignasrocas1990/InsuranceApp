using System;
using Insurance_app.Logic;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;

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
                ((HomeViewModel) svm.GetViewModel(Converter.HomeViewModel)).Dispose();
                ((ClaimViewModel) svm.GetViewModel(Converter.ClaimViewModel)).Dispose();
                ((ProfileViewModel) svm.GetViewModel(Converter.ProfileViewModel)).Dispose();
                ((ReportViewModel) svm.GetViewModel(Converter.ReportViewModel)).Dispose();
                
                svm.Dispose();
                await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}