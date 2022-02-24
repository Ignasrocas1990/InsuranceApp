using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels.ClientViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.ClientPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomersPage : ContentPage
    {
        public CustomersPage()
        {
            InitializeComponent();
            //BindingContext = new CustomersViewModel();
        }

        protected  override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (CustomersViewModel)BindingContext;
            await vm.Setup();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                ((ListView) sender).SelectedItem = null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}