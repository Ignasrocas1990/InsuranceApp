using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Logic;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.ClientViewModels
{
    public class ClientRegViewModel : ObservableObject
    {
        private string email="";
        private string pass="";
        private string fname="";
        private bool wait;
        private string lname="";
        private InferenceService inf;
        private int attempt = 0;
        private bool codeIsValid;
        private readonly UserManager userManager;


        public ClientRegViewModel()
        {
            inf = new InferenceService();
            userManager = new UserManager();
        }

        public async Task Register()
        {
            CircularWaitDisplay = true;
          
            if (codeIsValid)
            {
                //TODO need to change the API so takes over the code when used 
                try
                {
                    await userManager.Register(email, pass);
                    var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, pass));
                    if (user is null) throw new Exception("Registration failed");
                    var saved = await userManager.CreateClient(user, email, fname, lname, code);
                    if (!saved)
                    {
                        throw new Exception("Registration failed");
                    }
                    await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                    if (App.RealmApp.CurrentUser !=null)
                    {
                        await App.RealmApp.CurrentUser.LogOutAsync();
                    }
                    userManager.Dispose();
                    
                    await Application.Current.MainPage.DisplayAlert("notice", "Successfully registered", "close");
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("notice", e.Message, "close");
                }
            }
            CircularWaitDisplay = false;
        }

        public async Task ValidateCode()
        {
            try
            {
                if (!App.NetConnection())
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", StaticOpt.NCE, "close");
                    return;
                }
                if (attempt >= 3)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "You have been blocked for 3min\nToo many attempts", "close");
                    return;
                }
                var response = await inf.CheckCompanyCode(code);
                var sResponse = await response.Content.ReadAsStringAsync();
                if (sResponse.Equals("ok"))
                {
                    CodeReadOnly = true;
                }
                else
                {
                    CheckAttempt();
                    await Application.Current.MainPage.DisplayAlert("Error", "The code provided is inValid",
                        "close");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 
        private void CheckAttempt()
        {
            attempt++;
            codeIsValid = false;
            if (attempt == 3)
            {
                Task.Run( async () =>
                {
                    await Task.Delay(1000);
                    attempt = 0;
                });
            }
        }
        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string PassDisplay
        {
            get => pass;
            set => SetProperty(ref pass, value);
        }

        public string FNameDisplay
        {
            get => fname;
            set => SetProperty(ref fname, value);
        }

        public string LNameDisplay
        {
            get => lname;
            set => SetProperty(ref lname, value);
        }

        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }

        private string code;
        public string CodeDisplay
        {
            get => code;
            set => SetProperty(ref code, value);
        }

        public bool CodeReadOnly
        {
            get => codeIsValid;
            set => SetProperty(ref codeIsValid, value);
        }
    }
}