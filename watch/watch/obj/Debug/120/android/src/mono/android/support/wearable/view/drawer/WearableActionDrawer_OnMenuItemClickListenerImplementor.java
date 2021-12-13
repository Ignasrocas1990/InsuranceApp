package mono.android.support.wearable.view.drawer;


public class WearableActionDrawer_OnMenuItemClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.drawer.WearableActionDrawer.OnMenuItemClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMenuItemClick:(Landroid/view/MenuItem;)Z:GetOnMenuItemClick_Landroid_view_MenuItem_Handler:Android.Support.Wearable.View.Drawer.WearableActionDrawer/IOnMenuItemClickListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.View.Drawer.WearableActionDrawer+IOnMenuItemClickListenerImplementor, Xamarin.Android.Wear", WearableActionDrawer_OnMenuItemClickListenerImplementor.class, __md_methods);
	}


	public WearableActionDrawer_OnMenuItemClickListenerImplementor ()
	{
		super ();
		if (getClass () == WearableActionDrawer_OnMenuItemClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.View.Drawer.WearableActionDrawer+IOnMenuItemClickListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public boolean onMenuItemClick (android.view.MenuItem p0)
	{
		return n_onMenuItemClick (p0);
	}

	private native boolean n_onMenuItemClick (android.view.MenuItem p0);

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
