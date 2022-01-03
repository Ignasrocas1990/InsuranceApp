using System;
using Xamarin.Forms;
using Plugin.BLE.Abstractions.Contracts;
using Insurance_app.BLE;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        int i = 0;
        
        private BleManager bleManager;
        private InferenceService inferenceService;

        public MainPage()
        {
            InitializeComponent();
            bleManager = new BleManager();
            inferenceService = new InferenceService();
            bleManager.InferEvent += (s, e) =>
            {
                inferenceService.Predict(e);
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
            inferenceService.Predict("");
        }
        
    }
    
}
