using Xamarin.Forms;

namespace Insurance_app.Pages
{
    public class LoadingPage: ContentPage
    {
        public static readonly BindableProperty RootViewModelProperty =
            BindableProperty.Create(
                "RootViewModel", typeof(object), typeof(LoadingPage),
                defaultValue: default(object));

        public object RootViewModel
        {
            get { return (object)GetValue(RootViewModelProperty); }
            set { SetValue(RootViewModelProperty, value); }
        }
    }
}