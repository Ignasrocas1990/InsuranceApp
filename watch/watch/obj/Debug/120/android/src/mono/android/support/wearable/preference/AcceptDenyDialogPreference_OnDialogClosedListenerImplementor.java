package mono.android.support.wearable.preference;


public class AcceptDenyDialogPreference_OnDialogClosedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.preference.AcceptDenyDialogPreference.OnDialogClosedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDialogClosed:(Z)V:GetOnDialogClosed_ZHandler:Android.Support.Wearable.Preference.AcceptDenyDialogPreference/IOnDialogClosedListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Preference.AcceptDenyDialogPreference+IOnDialogClosedListenerImplementor, Xamarin.Android.Wear", AcceptDenyDialogPreference_OnDialogClosedListenerImplementor.class, __md_methods);
	}


	public AcceptDenyDialogPreference_OnDialogClosedListenerImplementor ()
	{
		super ();
		if (getClass () == AcceptDenyDialogPreference_OnDialogClosedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Preference.AcceptDenyDialogPreference+IOnDialogClosedListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onDialogClosed (boolean p0)
	{
		n_onDialogClosed (p0);
	}

	private native void n_onDialogClosed (boolean p0);

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
