package mono.androidx.wear.widget;


public class ConfirmationOverlay_OnAnimationFinishedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.wear.widget.ConfirmationOverlay.OnAnimationFinishedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAnimationFinished:()V:GetOnAnimationFinishedHandler:AndroidX.Wear.Widget.ConfirmationOverlay/IOnAnimationFinishedListenerInvoker, Xamarin.AndroidX.Wear\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Wear.Widget.ConfirmationOverlay+IOnAnimationFinishedListenerImplementor, Xamarin.AndroidX.Wear", ConfirmationOverlay_OnAnimationFinishedListenerImplementor.class, __md_methods);
	}


	public ConfirmationOverlay_OnAnimationFinishedListenerImplementor ()
	{
		super ();
		if (getClass () == ConfirmationOverlay_OnAnimationFinishedListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Wear.Widget.ConfirmationOverlay+IOnAnimationFinishedListenerImplementor, Xamarin.AndroidX.Wear", "", this, new java.lang.Object[] {  });
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
