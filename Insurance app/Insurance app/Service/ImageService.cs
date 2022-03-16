using System.Reflection;
using Xamarin.Forms;

namespace Insurance_app.Service {
  internal class ImageService {
    private ImageService() { }

    public static ImageService Instance { get; } = new ImageService();

    public ImageSource CardFront { get; } = Load("stp_card_form_front@3x.png");

    public ImageSource CardBack { get; } = Load("stp_card_form_back@3x.png");
    
    public ImageSource Discover { get; } = Load("stp_card_discover@3x.png");
    public ImageSource Jcb { get; } = Load("stp_card_jcb@3x.png");
    public ImageSource Mastercard { get; } = Load("stp_card_mastercard@3x.png");
    public ImageSource Unionpay { get; } = Load("stp_card_unionpay_en@3x.png");
    public ImageSource Visa { get; } = Load("stp_card_visa@3x.png");

    public ImageSource CardError { get; } = Load("stp_card_error@3x.png");

    public ImageSource CardUnknown { get; } = Load("stp_card_unknown@3x.png");

    private static ImageSource Load(string name) {
      return ImageSource.FromResource($"Insurance_app.Resources.{name}",
        typeof(ImageService).GetTypeInfo().Assembly);
    }
  }
}