package mono.androidx.wear.widget;


public class CircularProgressLayout_OnTimerFinishedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		androidx.wear.widget.CircularProgressLayout.OnTimerFinishedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onTimerFinished:(Landroidx/wear/widget/CircularProgressLayout;)V:GetOnTimerFinished_Landroidx_wear_widget_CircularProgressLayout_Handler:AndroidX.Wear.Widget.CircularProgressLayout/IOnTimerFinishedListenerInvoker, Xamarin.AndroidX.Wear\n" +
			"";
		mono.android.Runtime.register ("AndroidX.Wear.Widget.CircularProgressLayout+IOnTimerFinishedListenerImplementor, Xamarin.AndroidX.Wear", CircularProgressLayout_OnTimerFinishedListenerImplementor.class, __md_methods);
	}


	public CircularProgressLayout_OnTimerFinishedListenerImplementor ()
	{
		super ();
		if (getClass () == CircularProgressLayout_OnTimerFinishedListenerImplementor.class)
			mono.android.TypeManager.Activate ("AndroidX.Wear.Widget.CircularProgressLayout+IOnTimerFinishedListenerImplementor, Xamarin.AndroidX.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onTimerFinished (androidx.wear.widget.CircularProgressLayout p0)
	{
		n_onTimerFinished (p0);
	}

	private native void n_onTimerFinished (androidx.wear.widget.CircularProgressLayout p0);

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
