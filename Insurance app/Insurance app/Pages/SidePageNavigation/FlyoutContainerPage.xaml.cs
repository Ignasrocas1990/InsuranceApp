using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.SidePageNavigation
{
    public partial class FlyoutContainerPage : FlyoutPage
    {
        public FlyoutContainerPage()
        {
            InitializeComponent();
            Flyout.ListView.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is FlyoutContainerItem item)
            {
                if (item.Title.Equals("LogOut"))
                {
                    new LogOut(Navigation).Logout();
                    
                }
                else
                {
                    var page = (Page)Activator.CreateInstance(item.TargetType);
                    Detail = new NavigationPage(page);
                }
                Flyout.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}