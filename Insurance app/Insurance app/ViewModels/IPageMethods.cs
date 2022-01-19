using System.Threading.Tasks;
using Android.OS;

namespace Insurance_app.ViewModels
{
    public interface IPageMethods
    {
        Task NavigateToMainPage();
        Task NavigateToQuotePage();

        Task Notify(string title, string message, string button);
    }
}