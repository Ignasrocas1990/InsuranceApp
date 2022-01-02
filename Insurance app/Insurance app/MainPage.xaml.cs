using System;
using Xamarin.Forms;
using Plugin.BLE.Abstractions.Contracts;
using Insurance_app.BLE;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        private readonly Guid SERVER_GUID;
        private const string uuidString = "a3bb5442-5b61-11ec-bf63-0242ac130002";

        private readonly IBluetoothLE ble;
        private readonly IAdapter adapter;
        
        private readonly EventHandler<byte[]> readCompleted;
        private bool canRead = false;
        int i = 0;
        private ICharacteristic chara=null;
        private int serviceDelay = 0;
        private int readingDelay = 0;
        private int charaErrDelay = 0;


        private BleManager bleManager;

        public MainPage()
        {
            InitializeComponent();
            bleManager = new BleManager();
        }
        private void test(object sender, EventArgs e)
        {
            testBtn.Text = $"{i++}";
        }

        private async void StopBtn_OnClicked(object sender, EventArgs e)
        {
            bleManager.StopDataSend();
        }
    }
}
