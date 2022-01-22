using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.BLE.Abstractions.Contracts;
using Insurance_app.Communications;
using Insurance_app.Models;
using Insurance_app.Pages;
using Realms;
using Task = System.Threading.Tasks.Task;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        int i = 0;
        private BleManager bleManager;
        private InferenceService inferenceService;
        private RealmDb db = null;
        private Customer currCustomer;
        private ObservableCollection<MovData> _mov = new ObservableCollection<MovData>();
        public ObservableCollection<MovData> myData
        {
            get => _mov;
        }

        public MainPage()
        {
            InitializeComponent();
        }
        protected override  void OnAppearing()
        {
            /*

            //currCustomer = await App.RealmDb.FindCustomer(App.RealmApp.CurrentUser.Id);
                if (currCustomer is null)
                {
                    Console.WriteLine("customer not found");

                    await DisplayAlert("No User object","Customer not found", "OK");
                }
                else
                {
                    Console.WriteLine($"we logged in with {currCustomer.Email}");
                    GetAllMovData();
                    SaveMoveData();
                }
                
                bleManager = new BleManager();
            
            base.OnAppearing();
            */
        }

        private void GetAllMovData()
        {
            //db.addMovData(currCustomer);------------------------------------
        }

        private void SaveMoveData()
        {
            //db.addMovData();----------------------------------------
        }

        private void StopBtn_OnClicked(object sender, EventArgs e)
        {
            bleManager.StopDataSend();
        }
    }
    
}
