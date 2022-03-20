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
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    
    public class EditorViewModel : ObservableObject
    {
        private readonly EditorPopup popup;
        public ICommand CloseCommand { get; }
        public ICommand SubmitCommand { get; }
        public EditorViewModel(EditorPopup popup, string heading, bool readOnly, string popupDisplayText)
        {
            this.popup = popup;
            CloseCommand = new Command(Close);
            SubmitCommand = new Command(Submit);
            HeadingDisplay = heading;
            ReadOnlyDisplay = readOnly;
            ExtraInfoDisplay = popupDisplayText;
        }

        private void Submit()
        {
            popup.Dismiss(extraInfo);

        }
        private void Close()
        {
            popup.Dismiss("");
        }

        private string heading;
        public string HeadingDisplay
        {
            get => heading;
            set => SetProperty(ref heading, value);
        }

        private string extraInfo;
        public string ExtraInfoDisplay
        {
            get => extraInfo;
            set => SetProperty(ref extraInfo, value);
        }

        private bool readOnly;
        public bool ReadOnlyDisplay
        {
            get => readOnly;
            set => SetProperty(ref readOnly, value);
        }




    }
}