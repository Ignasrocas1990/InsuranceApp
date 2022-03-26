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

using System.Windows.Input;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    /// <summary>
    /// Used to set UI viewable elements of InfoPopup Page
    /// </summary>
    public class InfoPopupViewModel : ObservableObject
    {
        private readonly InfoPopup infoPopup;
        public ICommand CloseCommand { get; }


        public InfoPopupViewModel(InfoPopup infoPopup, string type)
        {
            this.infoPopup = infoPopup;
            CloseCommand = new Command(Close);
            InfoPicker(type);
        }
        /// <summary>
        /// close the pop up
        /// </summary>
        private void Close()
        {
            infoPopup.Dismiss("");
        }
        /// <summary>
        /// Splits info into columns so it
        /// can be displayed
        /// </summary>
        /// <param name="type">What type of info selected string</param>
        private void InfoPicker(string type)
        {
            var longInfoString = StaticOpt.InfoTest(type);
            var splitInfo = longInfoString.Split('~');
            InfoDisplayH1 = splitInfo[0];
            InfoDisplayH2 = splitInfo[1];
            InfoDisplayH3 = splitInfo[2];
            InfoDisplayC1 = splitInfo[3];
            InfoDisplayC2 = splitInfo[4];
            InfoDisplayC3 = splitInfo[5];
            
        }
        //------------------ Bindable properties below --------------------------
        private string column1Head;
        public string InfoDisplayH1
        {
            get => column1Head;
            set => SetProperty(ref column1Head, value);
        }
        private string column2Head;
        public string InfoDisplayH2
        {
            get => column2Head;
            set => SetProperty(ref column2Head, value);
        }
        private string column3Head;
        public string InfoDisplayH3
        {
            get => column3Head;
            set => SetProperty(ref column3Head, value);
        }
        
        private string column1;
        public string InfoDisplayC1
        {
            get => column1;
            set => SetProperty(ref column1, value);
        }
        private string column2;
        public string InfoDisplayC2
        {
            get => column2;
            set => SetProperty(ref column2, value);
        }

        private string column3;

        public string InfoDisplayC3
        {
            get => column3;
            set => SetProperty(ref column3, value);
        }
    }
}