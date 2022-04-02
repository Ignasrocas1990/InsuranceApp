/*   Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.ClientViewModels
{
    /// <summary>
    /// Class used to store and manipulate ClientRegistration Page UI
    /// inputs in real time via BindingContext & its properties
    /// </summary>
    public class ClientRegViewModel : ObservableObject,IDisposable
    {
        private string email="";
        private string pass="";
        private string fName="";
        private bool wait;
        private string lname="";
        private int attempt = 0;
        private bool codeIsValid;
        private readonly UserManager userManager;


        public ClientRegViewModel()
        {
            userManager = new UserManager();
        }
        /// <summary>
        /// Loads in data using manager classes via database
        /// and set it to Bindable properties(UI)
        /// </summary>
        public async Task Register()
        {
            CircularWaitDisplay = true;
          
            if (codeIsValid)
            {
                try
                {
                    await userManager.Register(email, pass);
                    var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, pass));
                    if (user is null) throw new Exception("Registration failed");
                    var saved = await userManager.CreateClient(user, email, fName, lname, code);
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
                    
                    await Msg.Alert("Successfully registered");
                    
                    await Application.Current.MainPage.Navigation.PopToRootAsync();
                }
                catch (Exception e)
                {
                   await Msg.AlertError(e.Message);
                }
            }
            CircularWaitDisplay = false;
        }
        /// <summary>
        /// Verifies that client code is valid via HttpService/email
        /// </summary>
        public async Task ValidateCode()
        {
            try
            {
                if (!App.NetConnection())
                {
                    await Msg.AlertError(Msg.NetworkConMsg);
                    return;
                }
                if (attempt >= 3)
                {
                    await Msg.AlertError("You have been blocked for 3min\nToo many attempts");
                    return;
                }
                var response = await HttpService.CheckCompanyCode(code);
                var sResponse = await response.Content.ReadAsStringAsync();
                if (sResponse.Equals("ok"))
                {
                    CodeReadOnly = true;
                }
                else
                {
                    CheckAttempt();
                    await Msg.AlertError( "The code provided is inValid");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Security measure which prevents too many attempts. 
        /// </summary>
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
        //------------------ Bindable properties below ----------------------
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
            get => fName;
            set => SetProperty(ref fName, value);
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

        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
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

        public void Dispose()
        {
            userManager.Dispose();
        }
    }
}