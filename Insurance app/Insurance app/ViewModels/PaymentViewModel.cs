using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
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
    private double totalRewards;
    private bool rewardOverDraft;

    private readonly string customerId;
    public ICommand PayCommand { get; }
    public ICommand RewardsCommand { get; }

    private readonly UserManager userManager;
    private readonly PolicyManager policyManager;
    private readonly RewardManager rewardManager;
    public PaymentViewModel(string customerId, double price,string zip)
    {
      this.price = price;
      this.customerId = customerId;
      ZipDisplay = zip;
      PriceDisplay = $"{price}";
      PayCommand = new AsyncCommand(Pay);
      userManager = new UserManager();
      policyManager = new PolicyManager();
      rewardManager = new RewardManager();
      RewardsCommand = new Command(UseRewards);
    }
    
    public async Task Setup()
    {
      try
      {
        SetUpWaitDisplay = true;
        RewardsIsVisible = false;
        float earnedRewards = 0;
        if (price>1 && !zip.Equals(""))
        {
          (_,earnedRewards)=  await rewardManager.GetTotalRewards(App.RealmApp.CurrentUser, customerId);
        }
        if (price < 1)
        {
          var (_, policy) = await policyManager.FindPolicy(customerId, App.RealmApp.CurrentUser);
          if (policy.Price != null)
          {
            price = Converter.FloatToDouble(policy.Price.Value);
            PriceDisplay = $"{price}";
          }
        }
        if (zip.Equals(""))
        {
          var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, customerId);
          ZipDisplay = customer.Address.PostCode;
        }

        
        if (earnedRewards > 0)
        {
          totalRewards = Converter.FloatToDouble(earnedRewards);
          RewardsIsVisible = true;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      SetUpWaitDisplay = false;
    }
    private void UseRewards()
    {
      if (IsCheckedDisplay)
      {
        var (newPrice, rewardLeftover)= rewardManager.ChangePrice(totalRewards, price);
        PriceDisplay = $"{newPrice}";
        RewardsDisplay = $"{rewardLeftover}";
        if (newPrice is 1)
        {
          rewardOverDraft = true;
        }
      }
      else
      {
        rewardOverDraft = false;
        PriceDisplay = $"{price}";
        RewardsDisplay = $"{totalRewards}";
      }
    }
    
    private async Task Pay()
    {
      try
      {
        switch (IsCheckedDisplay)
        {
          //MAKE sure we have internet connection here !!!!
          // Take priceDisplay & RewardsDisplay when updating objects/and paying 
          //TODO implement payment service======================
          case true when rewardOverDraft:
            rewardManager.UpdateRewardsWithOverdraft((float)price, App.RealmApp.CurrentUser, customerId);
            break;
          case true:
            rewardManager.UserRewards(App.RealmApp.CurrentUser, customerId);
            break;
        }
        var customer = await policyManager.UpdatePolicyPrice
          (App.RealmApp.CurrentUser,customerId,Converter.StringToDouble(pDisplay));
        //TODO can send an invoice also here... (use customer email etc...s)

        await StaticOpt.Logout();
        await Msg.Alert("Payment Successful,you can log in now");
        await Application.Current.MainPage.Navigation.PopToRootAsync();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }
    //----------------------- Binding/support methods ------------------------------------------
    private string UpdateCardDetails(string value)
    {
      (LengthDisplay, ImageDisplay) = CardDefinitionService.Instance.DetailsFor(value);
      return value;
    }

    string pDisplay;

    public string PriceDisplay
    {
      get => "€"+pDisplay;
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

    public int LengthDisplay
    {
      get => length;
      set => SetProperty(ref length, value);
    }

    public string NumberDisplay
    {
      get => UpdateCardDetails(number);
      set => SetProperty(ref number, value);
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

    public string LengthError => $"Maximum length has to be : {LengthDisplay}";

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

    private bool isChecked;
    public bool IsCheckedDisplay
    {
      get => isChecked;
      set => SetProperty(ref isChecked, value);
    }
    private string rewards;
    public string RewardsDisplay
    {
      get => "€"+rewards;
      set => SetProperty(ref rewards, value);
    }
    private bool circularWait;
    public bool CircularWaitDisplay
    {
      get => circularWait;
      set => SetProperty(ref circularWait, value);
    }

    private bool rewardsIsVisible;
    public bool RewardsIsVisible
    {
      get => rewardsIsVisible;
      set => SetProperty(ref rewardsIsVisible, value);
    }

    public string IsValid()
    {
      if (MonthDisplay.Length < 1 || YearDisplay.Length < 1) return "";
      string error = "";
      var intMonth = int.Parse(MonthDisplay);
      var intYear = int.Parse(YearDisplay);

      if (intMonth < 1 || intMonth > 12) 
        error = "Expiry month's value is not valid";


      intYear += DateTime.Now.Year / 100 * 100;
      
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