package mono.android.support.wearable.view.drawer;


public class FlingWatcher_FlingListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.drawer.FlingWatcher.FlingListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onFlingComplete:(Landroid/view/View;)V:GetOnFlingComplete_Landroid_view_View_Handler:Android.Support.Wearable.View.Drawer.FlingWatcher/IFlingListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.View.Drawer.FlingWatcher+IFlingListenerImplementor, Xamarin.Android.Wear", FlingWatcher_FlingListenerImplementor.class, __md_methods);
	}


	public FlingWatcher_FlingListenerImplementor ()
	{
		super ();
		if (getClass () == FlingWatcher_FlingListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.View.Drawer.FlingWatcher+IFlingListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onFlingComplete (android.view.View p0)
	{
		n_onFlingComplete (p0);
	}

	private native void n_onFlingComplete (android.view.View p0);

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
