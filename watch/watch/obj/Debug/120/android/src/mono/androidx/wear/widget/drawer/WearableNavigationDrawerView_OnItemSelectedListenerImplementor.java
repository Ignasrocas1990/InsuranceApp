package mono.androidx.wear.widget.drawer;


public class WearableNavigationDrawerView_OnItemSelectedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.wear.widget.drawer.WearableNavigationDrawerView.OnItemSelectedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onItemSelected:(I)V:GetOnItemSelected_IHandler:AndroidX.Wear.Widget.Drawer.WearableNavigationDrawerView/IOnItemSelectedListenerInvoker, Xamarin.AndroidX.Wear\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Wear.Widget.Drawer.WearableNavigationDrawerView+IOnItemSelectedListenerImplementor, Xamarin.AndroidX.Wear", WearableNavigationDrawerView_OnItemSelectedListenerImplementor.class, __md_methods);
	}


	public WearableNavigationDrawerView_OnItemSelectedListenerImplementor ()
	{
		super ();
		if (getClass () == WearableNavigationDrawerView_OnItemSelectedListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Wear.Widget.Drawer.WearableNavigationDrawerView+IOnItemSelectedListenerImplementor, Xamarin.AndroidX.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onItemSelected (int p0)
	{
		n_onItemSelected (p0);
	}

	private native void n_onItemSelected (int p0);

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
