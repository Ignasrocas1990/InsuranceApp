using System;
using Xamarin.Forms;
using Plugin.BLE.Abstractions.Contracts;
using Insurance_app.BLE;
using Insurance_app.Models;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        int i = 0;
        private MongoSetup mongo;
        
        private BleManager bleManager;
        private InferenceService inferenceService;
        private Customer curCustomer = null;
        public RealmDb Realm = null;

        public MainPage()
        {
            InitializeComponent();
            //mongo = new MongoSetup();
            //curCustomer = mongo.customer;
            //Realm = new RealmDb();
            
            
            
            bleManager = new BleManager();
            inferenceService = new InferenceService();
            
            bleManager.InferEvent += (s, e) =>
            {
                //inferenceService.Predict(e);
            };
        }
        private void test(object sender, EventArgs e)
        {
            testBtn.Text = $"{i++}";
        }

        private void StopBtn_OnClicked(object sender, EventArgs e)
        {
            bleManager.StopDataSend();
        }

        private void InfBtn_OnClicked(object sender, EventArgs e)
        {
            return;
            //inferenceService.Predict("");
            if (curCustomer != null)
            {
                curCustomer.MovData.Add(new MovData()
                {
                    Gyro =
                    {
                        X = 0.0004072435,
                        Y = 0.0,
                        Z = 0.0
                    },
                    Acc =
                    {
                        X = -0.02139569,
                        Y = 0.004148973,
                        Z = 0.9882692
                    }
                });

            }

        }

        private void LogBtn_OnClicked(object sender, EventArgs e)
        {
            mongo = new MongoSetup();
        }
    }
    
}
