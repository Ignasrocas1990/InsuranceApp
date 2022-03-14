using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels.Popups;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditorPopup: Popup<string>
    {
        public EditorPopup()
        {
            InitializeComponent();
            BindingContext = new EditorViewModel(this);
        }

        private async void  Button_OnClicked(object sender, EventArgs e)
        {
   
                var vm = (EditorViewModel)BindingContext;
                
                if (ExtraInfoValidator.IsValid)
                {
                    vm.CloseCommand.Execute(null);
                }
                else
                {
                    var errBuilder = new StringBuilder();
                    if (ExtraInfoValidator.IsNotValid)
                    {
                        if (ExtraInfoValidator.Errors != null)
                            foreach (var err in ExtraInfoValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    await Application.Current.MainPage.DisplayAlert(Msg.Error, errBuilder.ToString(), "close");
                } 
        }
    }
}