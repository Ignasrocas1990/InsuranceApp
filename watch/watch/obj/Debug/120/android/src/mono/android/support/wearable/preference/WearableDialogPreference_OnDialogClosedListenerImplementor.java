package mono.android.support.wearable.preference;


public class WearableDialogPreference_OnDialogClosedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.preference.WearableDialogPreference.OnDialogClosedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDialogClosed:(I)V:GetOnDialogClosed_IHandler:Android.Support.Wearable.Preference.WearableDialogPreference/IOnDialogClosedListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Preference.WearableDialogPreference+IOnDialogClosedListenerImplementor, Xamarin.Android.Wear", WearableDialogPreference_OnDialogClosedListenerImplementor.class, __md_methods);
	}


	public WearableDialogPreference_OnDialogClosedListenerImplementor ()
	{
		super ();
		if (getClass () == WearableDialogPreference_OnDialogClosedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Preference.WearableDialogPreference+IOnDialogClosedListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onDialogClosed (int p0)
	{
		n_onDialogClosed (p0);
	}

	private native void n_onDialogClosed (int p0);

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
