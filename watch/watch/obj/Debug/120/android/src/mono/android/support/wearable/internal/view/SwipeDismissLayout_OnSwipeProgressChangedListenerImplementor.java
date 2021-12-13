package mono.android.support.wearable.internal.view;


public class SwipeDismissLayout_OnSwipeProgressChangedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.internal.view.SwipeDismissLayout.OnSwipeProgressChangedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSwipeCancelled:(Landroid/support/wearable/internal/view/SwipeDismissLayout;)V:GetOnSwipeCancelled_Landroid_support_wearable_internal_view_SwipeDismissLayout_Handler:Android.Support.Wearable.Internal.View.SwipeDismissLayout/IOnSwipeProgressChangedListenerInvoker, Xamarin.Android.Wear\n" +
			"n_onSwipeProgressChanged:(Landroid/support/wearable/internal/view/SwipeDismissLayout;FF)V:GetOnSwipeProgressChanged_Landroid_support_wearable_internal_view_SwipeDismissLayout_FFHandler:Android.Support.Wearable.Internal.View.SwipeDismissLayout/IOnSwipeProgressChangedListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Internal.View.SwipeDismissLayout+IOnSwipeProgressChangedListenerImplementor, Xamarin.Android.Wear", SwipeDismissLayout_OnSwipeProgressChangedListenerImplementor.class, __md_methods);
	}


	public SwipeDismissLayout_OnSwipeProgressChangedListenerImplementor ()
	{
		super ();
		if (getClass () == SwipeDismissLayout_OnSwipeProgressChangedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Internal.View.SwipeDismissLayout+IOnSwipeProgressChangedListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onSwipeCancelled (android.support.wearable.internal.view.SwipeDismissLayout p0)
	{
		n_onSwipeCancelled (p0);
	}

	private native void n_onSwipeCancelled (android.support.wearable.internal.view.SwipeDismissLayout p0);


	public void onSwipeProgressChanged (android.support.wearable.internal.view.SwipeDismissLayout p0, float p1, float p2)
	{
		n_onSwipeProgressChanged (p0, p1, p2);
	}

	private native void n_onSwipeProgressChanged (android.support.wearable.internal.view.SwipeDismissLayout p0, float p1, float p2);

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
