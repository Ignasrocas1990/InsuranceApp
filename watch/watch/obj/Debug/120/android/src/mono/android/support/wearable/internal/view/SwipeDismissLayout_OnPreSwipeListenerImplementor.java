package mono.android.support.wearable.internal.view;


public class SwipeDismissLayout_OnPreSwipeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.internal.view.SwipeDismissLayout.OnPreSwipeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPreSwipe:(FF)Z:GetOnPreSwipe_FFHandler:Android.Support.Wearable.Internal.View.SwipeDismissLayout/IOnPreSwipeListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Internal.View.SwipeDismissLayout+IOnPreSwipeListenerImplementor, Xamarin.Android.Wear", SwipeDismissLayout_OnPreSwipeListenerImplementor.class, __md_methods);
	}


	public SwipeDismissLayout_OnPreSwipeListenerImplementor ()
	{
		super ();
		if (getClass () == SwipeDismissLayout_OnPreSwipeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Internal.View.SwipeDismissLayout+IOnPreSwipeListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public boolean onPreSwipe (float p0, float p1)
	{
		return n_onPreSwipe (p0, p1);
	}

	private native boolean n_onPreSwipe (float p0, float p1);

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
