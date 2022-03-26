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
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels
{
    /// <summary>
    /// Class used to store and manipulate ChangePasswordPage UI inputs in real time via BindingContext and its properties
    /// </summary>
    public class ChangePassViewModel : ObservableObject,IDisposable
    {
        private string password;

        public ICommand ChangePassCommand { get; }
        private readonly UserManager userManager;
        
        public ChangePassViewModel()
        {
            ChangePassCommand = new AsyncCommand(ChangePassword);
            userManager = new UserManager();
        }
        /// <summary>
        /// updates user password, if successful redirects to log in page
        /// </summary>
        private async Task ChangePassword()
        {
            try
            {
                if (!App.NetConnection())
                {
                    await Msg.Alert(Msg.NetworkConMsg);
                    return;
                }
                CircularWaitDisplay = true;

                var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, App.RealmApp.CurrentUser.Id);
                if (customer != null)
                {
                    await App.RealmApp.EmailPasswordAuth
                        .CallResetPasswordFunctionAsync(customer.Email,password);
                }
                
                await Msg.Alert("Password changed successfully.\nPlease login again...");
                await StaticOpt.Logout();
            }
            catch (Exception e)
            {
                await Msg.Alert("Password change failed.\nTry again later.");
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
        }
        //---------------------- Bindable properties below ----------------------------
        public string PassDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
        }
        private string password2;
        public string PassDisplay2
        {
            get => password2;
            set => SetProperty(ref password2, value);
        }
        private bool wait;
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

        public void Dispose()
        {
            userManager.Dispose();
        }
    }
}