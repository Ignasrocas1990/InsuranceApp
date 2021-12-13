package mono.android.support.wearable.watchface.decompositionface;


public class DecompositionConfigView_OnComplicationTapListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.watchface.decompositionface.DecompositionConfigView.OnComplicationTapListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onComplicationTap:(I[I)V:GetOnComplicationTap_IarrayIHandler:Android.Support.Wearable.Watchface.Decompositionface.DecompositionConfigView/IOnComplicationTapListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Watchface.Decompositionface.DecompositionConfigView+IOnComplicationTapListenerImplementor, Xamarin.Android.Wear", DecompositionConfigView_OnComplicationTapListenerImplementor.class, __md_methods);
	}


	public DecompositionConfigView_OnComplicationTapListenerImplementor ()
	{
		super ();
		if (getClass () == DecompositionConfigView_OnComplicationTapListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Watchface.Decompositionface.DecompositionConfigView+IOnComplicationTapListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onComplicationTap (int p0, int[] p1)
	{
		n_onComplicationTap (p0, p1);
	}

	private native void n_onComplicationTap (int p0, int[] p1);

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
