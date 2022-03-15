using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels {
  internal class PaymentViewModel: ObservableObject,IDisposable
  {
    private ImageSource image = ImageService.Instance.CardUnknown;
    //public ImageSource CardFront => ImageService.Instance.CardFront;
    //public ImageSource CardBack => ImageService.Instance.CardBack;
    private int length = 16;
    private string month = "";
    private string number = "";
    private bool useCardFront;
    private string verificationCode = "";
    private string year = "";
    private string zip = "";
    private double price;
    private readonly string customerId;
    public ICommand PayCommand { get; }
    private readonly UserManager userManager;
    private readonly PolicyManager policyManager;

    public PaymentViewModel(string customerId, double price)
    {
      this.price = price;
      this.customerId = customerId;
      PriceDisplay = $"€{price}";
      PayCommand = new AsyncCommand(Pay);
      userManager = new UserManager();
      policyManager = new PolicyManager();
    }
    public async Task Setup()
    {
      try
      {
        if (price is 0.0)
        {
          SetUpWaitDisplay = true;
          var (_, policy) = await policyManager.FindPolicy(customerId, App.RealmApp.CurrentUser);
          if (policy.Price != null)
          {
            var priceFloat = policy.Price.Value;
            price = Math.Round(Convert.ToDouble(priceFloat),2);
            PriceDisplay = "€"+priceFloat.ToString("F");
          }
        }
        var customer =await userManager.GetCustomer(App.RealmApp.CurrentUser, customerId);
        ZipDisplay = customer.Address.PostCode;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      SetUpWaitDisplay = false;
    }
    
    private void UpdateCardDetails()
    {
      (Length, ImageDisplay) = CardDefinitionService.Instance.DetailsFor(NumberDisplay);
    }

    private async Task Pay()
    {
      try
      {
        //MAKE sure we have internet connection here !!!!
        //TODO implement payment service======================
        //update customer Realm object (with payed price)
        
        policyManager.UpdatePolicyPrice(App.RealmApp.CurrentUser,customerId,price);
        await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
        await Application.Current.MainPage.DisplayAlert(Msg.Notice, "Payment Successful,you can log in now", "close");
        //can send an invoice also here...
        if (App.RealmApp.CurrentUser != null)
        {
          await App.RealmApp.CurrentUser.LogOutAsync();
        }
        else
        {
          Console.WriteLine("user longed out");
        }
        await Application.Current.MainPage.Navigation.PopToRootAsync();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    string pDisplay;

    public string PriceDisplay
    {
      get => pDisplay;
      set => SetProperty(ref pDisplay, value);
    }

    public bool UseCardFrontDisplay
    {
      get => useCardFront;
      set => SetProperty(ref useCardFront, value);
    }

    public ImageSource ImageDisplay
    {
      get => image;
      set => SetProperty(ref image, value);
    }

    public int Length
    {
      get => length;
      set => SetProperty(ref length, value);
    }

    public string NumberDisplay
    {
      get => number;
      set => SetProperty(ref number, value, "", UpdateCardDetails);
    }

    public string MonthDisplay
    {
      get => month;
      set => SetProperty(ref month, value);
    }

    public string YearDisplay
    {
      get => year;
      set => SetProperty(ref year, value);
    }

    public string LengthError => $"Maximum length has to be : {Length}";

    public string VerificationCodeDisplay
    {
      get => verificationCode;
      set => SetProperty(ref verificationCode, value);
    }

    public string ZipDisplay
    {
      get => zip;
      set => SetProperty(ref zip, value);
    }

    private bool setUpWait;

    public bool SetUpWaitDisplay
    {
      get => setUpWait;
      set => SetProperty(ref setUpWait, value);
    }

    private bool circularWait;

    public bool CircularWaitDisplay
    {
      get => circularWait;
      set => SetProperty(ref circularWait, value);
    }

    public string IsValid()
    {
      string error = "";
      var intMonth = int.Parse(MonthDisplay);
      var intYear = int.Parse(YearDisplay);

      if (intMonth < 1 || intMonth > 12) 
        error = "Expiry month's value is not valid";


      year += DateTime.Now.Year / 100 * 100;
      
      if (intYear < DateTime.Now.Year || intYear > DateTime.Now.AddYears(16).Year)
        error += "\nExpiry year's value is not valid";

      return error;
    }

    public void Dispose()
    {
      userManager.Dispose();
    }
  }
}