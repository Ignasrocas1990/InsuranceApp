/*
    Copyright 2020,Ignas Rocas

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
using System.Linq;
using System.Text;
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
        public EditorPopup(string heading, bool readOnly, string popupDisplayText)
        {
            InitializeComponent();
            BindingContext = new EditorViewModel(this,heading,readOnly,popupDisplayText);
        }

        private async void  Button_OnClicked(object sender, EventArgs e)
        {
   
                var vm = (EditorViewModel)BindingContext;
                
                if (ExtraInfoValidator.IsValid)
                {
                    vm.SubmitCommand.Execute(null);
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