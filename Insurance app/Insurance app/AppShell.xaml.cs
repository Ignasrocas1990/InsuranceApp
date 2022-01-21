using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute($"Pages/{nameof(QuotePage)}", typeof(QuotePage));
            //Routing.RegisterRoute($"Pages/{nameof(LogInPage)}", typeof(LogInPage));
        }
    }
}