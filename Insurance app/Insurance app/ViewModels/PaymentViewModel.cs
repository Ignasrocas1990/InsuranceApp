using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Stripe;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;
using Customer = Insurance_app.Models.Customer;

namespace Insurance_app.ViewModels {
  internal class PaymentViewModel: ObservableObject,IDisposable
  {
    private ImageSource image = ImageService.Instance.CardUnknown;
    private int length = 16;
    private string month = "";
    private string number = "";
    private bool useCardFront;
    private string verificationCode = "";
    private string year = "";
    private string zip = "";
    private double price;
    private string email;
    private string name;
    private double totalRewards;
    private bool rewardOverDraft;
    private Customer customer;
    public ICommand PayCommand { get; }
    public ICommand RewardsCommand { get; }

    private readonly UserManager userManager;
    private readonly RewardManager rewardManager;
    private readonly PolicyManager policyManager;
    public PaymentViewModel(Customer customer)
    {
      this.customer = customer;
      PayCommand = new AsyncCommand(Pay);
      userManager = new UserManager();
      rewardManager = new RewardManager();
      policyManager = new PolicyManager();
      RewardsCommand = new Command(UseRewards);
    }
    
    public async Task Setup()
    {
      try
      {
        SetUpWaitDisplay = true;
        RewardsIsVisible = false;
        if (customer is null)
        {
          var user = App.RealmApp.CurrentUser;
          customer = await userManager.GetCustomer(user,user.Id);
          if (customer is null) throw new Exception("Customer is null at set up");
          var rewardList = new List<Reward>(customer.Reward);
          var earnedRewards = rewardManager.GetRewardSum(rewardList);
          if (earnedRewards > 0)
          {
            totalRewards = Converter.FloatToDouble(earnedRewards);
            RewardsIsVisible = true;
          }
         
        }
        ZipDisplay = customer.Address.PostCode;
        email = customer.Email;
        name = customer.Name;
        //var policyPrice = //notNewCustomer.Policy.FirstOrDefault(p => p.PayedPrice == 0 && p.DelFlag == false)!.Price;
        var policy = policyManager.FindUnpayedPolicy(customer);
        if (policy != null)
        {
          price = Converter.FloatToDouble(policy.Price);
          PriceDisplay = $"{price}";
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
        if (!App.NetConnection())
        {
          await Msg.Alert(Msg.NetworkConMsg);
        }
        CircularWaitDisplay = true;
        
        if (!await PaymentService.PaymentAsync(number, Int32.Parse(year), Int32.Parse(month), verificationCode, zip, price, name, email))
            throw new Exception("payment failed");
        var user = App.RealmApp.CurrentUser;
        switch (IsCheckedDisplay)
        {
          case true when rewardOverDraft:
            rewardManager.UpdateRewardsWithOverdraft((float)price, user, user.Id);
            break;
          case true:
            rewardManager.UserRewards(user, user.Id);
            break;
        }

        var currentPolicy = policyManager.FindUnpayedPolicy(customer);
        if (currentPolicy is null) throw new Exception("Current policy is null");
          
        await policyManager.UpdatePolicyPrice(currentPolicy,user,Converter.StringToFloat(pDisplay));
        
        // can send an invoice also here... (use customer email etc...s)
        await StaticOpt.Logout();
        await Msg.Alert("Payment completed successfully\nYou can log in now");
        await Application.Current.MainPage.Navigation.PopToRootAsync();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        await Msg.AlertError("Failed to complete\nPlease try again later...");
      }
      CircularWaitDisplay = false;
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
      get => SetError(length);
      set => SetProperty(ref length, value);
    }

    private int SetError(int value)
    {
      LengthError = $"{value}";
      return value;
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

    private string lenError;
    public string LengthError
    {
      get => $"\nCard has to be exactly {LengthDisplay} digits long\n";
      set => SetProperty(ref lenError, value);
    }

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

    public string Valid()
    {
      var error = "";
      if (NumberDisplay.Length < LengthDisplay)
      {
        error += $"{LengthError}\n";
      }
      
      if (MonthDisplay.Length < 1 || YearDisplay.Length < 2 || MonthDisplay.Length > 2 || YearDisplay.Length > 2)
      {
        error += "Month & Year values must be between 1 & 2 digits long\n";
      }
      else
      {
        var intMonth = int.Parse(MonthDisplay);
        var intYear = int.Parse(YearDisplay);
        if (intMonth < 1 || intMonth > 12) 
          error += "Expiry month's value is not valid\n";

        var thisYear = DateTime.Now.Year % 100;
        if (intYear  < thisYear || intYear  > thisYear+16)
          error += "Expiry year's value is not valid\n";
      }
      return error;
    }

    public void Dispose()
    {
      userManager.Dispose();
    }
  }
}