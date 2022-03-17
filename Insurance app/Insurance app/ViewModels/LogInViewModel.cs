using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages;
using Insurance_app.Pages.ClientPages;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exception = System.Exception;
using UserManager = Insurance_app.Logic.UserManager;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string email="";
        private string password = "";
        private const string ExpiredCustomerStr = "Accounts policy has been expired" +
                                            "\nPlease select one of the following options.";

        private const string CreateNewAccStr = "Create new account.";
        private const string ResetStr = "Reset the old account.";
        
        private readonly UserManager userManager;

        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }
        public ICommand ClientRegCommand { get; }


        public LogInViewModel()
        {
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(NavigateToQuote);
            ClientRegCommand = new AsyncCommand(ClientRegister);
            Connectivity.ConnectivityChanged += (s, e) =>
            {
                App.Connected =  (e.NetworkAccess ==  NetworkAccess.Internet);
            };
            userManager = new UserManager();
        }

        private async Task ClientRegister()
        {
            CircularWaitDisplay = true;
            await Application.Current.MainPage.Navigation.PushAsync(new ClientRegistration());
            CircularWaitDisplay = false;
        }
        public async Task ExistUser()
        {
            CircularWaitDisplay = false;
            try
            {
                if (App.RealmApp.CurrentUser != null)
                {
                    await App.RealmApp.CurrentUser.LogOutAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task NavigateToQuote()
        {
            try
            {
                CircularWaitDisplay = true;
                await Application.Current.MainPage.Navigation.PushAsync(new QuotePage(""));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task LogIn()
        {
            try
            {
                CircularWaitDisplay = true;
                if (App.NetConnection())
                {
                    if (!emailIsValid || !passIsValid )
                    {
                        throw new Exception("Log in details are not invalid");
                    }

                    var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    if (user is null) throw new Exception("User fail log in");
                    
                    //await CleanDatabase();//TODO remove when submitting -------------------------------------------------
                    
                     var typeUser = await userManager.FindTypeUser(user);
                     if (typeUser.Equals($"{UserType.Customer}"))
                     {
                         Application.Current.MainPage = new AppShell();
                         await Shell.Current.GoToAsync($"//{nameof(HomePage)}?Email={email}&Pass={password}",true);
                     }
                     else if (typeUser.Equals($"{UserType.Client}"))
                     {
                         Application.Current.MainPage = new ClientShell();
                         await Shell.Current.GoToAsync($"//{nameof(ClientMainPage)}",true);
                     }else if (typeUser.Equals($"{UserType.UnpaidCustomer}"))
                     {
                         await Msg.Alert( "Seems like you haven't payed yet.\nDirecting to payment page...");
                         await Application.Current.MainPage.Navigation.PushAsync(new PaymentPage(user.Id,0,""));
                     }
                     else if(typeUser.Equals(""))
                     {
                         await ExistUser();
                     }
                     else
                     {
                         var answer = await Application.Current.MainPage.DisplayAlert(Msg.Notice, ExpiredCustomerStr,
                            CreateNewAccStr,ResetStr );
                         if (answer)
                         {
                             await ExistUser();
                             QuoteCommand.Execute(null);
                         }
                         else
                         {
                             await Application.Current.MainPage.Navigation.PushAsync(new QuotePage(typeUser),true);
                         }
                     }
                }
                else
                {
                    throw new Exception(Msg.NetworkConMsg);
                }
            }
            catch // throws error in-case password is invalid
            {
                await Msg.AlertError("Invalid Credentials");
            }
            CircularWaitDisplay = false;
        }


        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string PasswordDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private bool circularWait;
        

        public bool CircularWaitDisplay
        {
            get => circularWait;
            set => SetProperty(ref circularWait, value);
        }

        private bool passIsValid;
        public bool PassIsValid
        {
            get => passIsValid;
            set => SetProperty(ref passIsValid, value);
        }
        private bool emailIsValid;

        public bool EmailIsValid
        {
            get => emailIsValid;
            set => SetProperty(ref emailIsValid, value);
        }

        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        public void Dispose()
        {
            userManager.Dispose();
        }
        private async Task CleanDatabase()//TODO Remove when submitting
        {
            await userManager.CleanDatabase(App.RealmApp.CurrentUser);
        }
        

    }
}