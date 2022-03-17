using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Insurance_app.SupportClasses
{
    public static class Msg
    {
        public const string Notice = "Notice";
        public const string Error = "Error";
        public const string Close = "close";
        public static readonly string SameDetailsMsg = "The details provided did not change";
        public const string SuccessUpdateMsg = "The details updated successfully";
        public const string ResetPassMsg = "The temporary password has been send to account email.";
        public const string ApiSendErrorMsg = "Something went wrong.Please try again later";
        public const string NetworkConMsg = "Network connectivity is not available";
        public const string EmailSent = "Customer has been notified by email.";

        public static async Task Alert(string msg)=>
        await Application.Current.MainPage.DisplayAlert(Notice, msg, Close);

        public static async Task AlertError(string msg)=>
            await Application.Current.MainPage.DisplayAlert(Error, msg, Close);
        
    }
}