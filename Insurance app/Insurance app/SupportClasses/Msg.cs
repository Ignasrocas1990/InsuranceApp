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

using System.Threading.Tasks;
using Xamarin.Forms;

namespace Insurance_app.SupportClasses
{
    public static class Msg
    {
        public const string Notice = "Notice";
        public const string Error = "Error";
        private const string Close = "close";
        public const string SuccessUpdateMsg = "The details updated successfully";
        public const string ResetPassMsg = "Password reset Successfull\nThe temporary password has been send to the email.";
        public const string ApiSendErrorMsg = "Something went wrong.Please try again later";
        public const string NetworkConMsg = "Network connectivity is not available";
        public const string EmailSent = "Customer has been notified by email.";

        public static async Task Alert(string msg)=>
        await Application.Current.MainPage.DisplayAlert(Notice, msg, Close);

        public static async Task AlertError(string msg)=>
            await Application.Current.MainPage.DisplayAlert(Error, msg, Close);
        
        
        
    }
}