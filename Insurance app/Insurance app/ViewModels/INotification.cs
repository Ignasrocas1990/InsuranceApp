using System.Threading.Tasks;
using Android.OS;

namespace Insurance_app.ViewModels
{
    public interface INotification
    {
        Task Notify(string title, string message, string close);
        Task<bool> NotifyOption(string title, string message, string accept, string close);
    }
}