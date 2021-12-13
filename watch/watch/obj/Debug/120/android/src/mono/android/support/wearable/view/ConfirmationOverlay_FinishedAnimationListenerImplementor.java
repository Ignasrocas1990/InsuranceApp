package mono.android.support.wearable.view;


public class ConfirmationOverlay_FinishedAnimationListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.ConfirmationOverlay.FinishedAnimationListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAnimationFinished:()V:GetOnAnimationFinishedHandler:Android.Support.Wearable.Views.ConfirmationOverlay/IFinishedAnimationListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Views.ConfirmationOverlay+IFinishedAnimationListenerImplementor, Xamarin.Android.Wear", ConfirmationOverlay_FinishedAnimationListenerImplementor.class, __md_methods);
	}


	public ConfirmationOverlay_FinishedAnimationListenerImplementor ()
	{
		super ();
		if (getClass () == ConfirmationOverlay_FinishedAnimationListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Views.ConfirmationOverlay+IFinishedAnimationListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onAnimationFinished ()
	{
		n_onAnimationFinished ();
	}

	private native void n_onAnimationFinished ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
