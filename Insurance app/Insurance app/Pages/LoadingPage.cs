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
    Code based from : https://stackoverflow.com/questions/62876229/xamarin-forms-how-to-show-activityindicator-in-every-page
 */

using Xamarin.Forms;

namespace Insurance_app.Pages
{
    /// <summary>
    /// For each page set current page as root page.
    /// (Used one Activity indicator throughout the project)
    /// </summary>
    public class LoadingPage: ContentPage
    {
        public static readonly BindableProperty RootViewModelProperty =
            BindableProperty.Create(
                "RootViewModel", typeof(object), typeof(LoadingPage));

        public object RootViewModel
        {
            get => GetValue(RootViewModelProperty);
            set => SetValue(RootViewModelProperty, value);
        }
    }
}