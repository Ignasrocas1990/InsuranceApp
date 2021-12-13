package mono.android.support.wearable.internal.view;


public class SwipeDismissLayout_OnDismissedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.internal.view.SwipeDismissLayout.OnDismissedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDismissed:(Landroid/support/wearable/internal/view/SwipeDismissLayout;)V:GetOnDismissed_Landroid_support_wearable_internal_view_SwipeDismissLayout_Handler:Android.Support.Wearable.Internal.View.SwipeDismissLayout/IOnDismissedListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Internal.View.SwipeDismissLayout+IOnDismissedListenerImplementor, Xamarin.Android.Wear", SwipeDismissLayout_OnDismissedListenerImplementor.class, __md_methods);
	}


	public SwipeDismissLayout_OnDismissedListenerImplementor ()
	{
		super ();
		if (getClass () == SwipeDismissLayout_OnDismissedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Internal.View.SwipeDismissLayout+IOnDismissedListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onDismissed (android.support.wearable.internal.view.SwipeDismissLayout p0)
	{
		n_onDismissed (p0);
	}

	private native void n_onDismissed (android.support.wearable.internal.view.SwipeDismissLayout p0);

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
