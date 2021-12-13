package mono.android.support.wearable.view;


public class AcceptDenyDialogFragment_OnDismissListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.AcceptDenyDialogFragment.OnDismissListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDismiss:(Landroid/support/wearable/view/AcceptDenyDialogFragment;)V:GetOnDismiss_Landroid_support_wearable_view_AcceptDenyDialogFragment_Handler:Android.Support.Wearable.Views.AcceptDenyDialogFragment/IOnDismissListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Views.AcceptDenyDialogFragment+IOnDismissListenerImplementor, Xamarin.Android.Wear", AcceptDenyDialogFragment_OnDismissListenerImplementor.class, __md_methods);
	}


	public AcceptDenyDialogFragment_OnDismissListenerImplementor ()
	{
		super ();
		if (getClass () == AcceptDenyDialogFragment_OnDismissListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Views.AcceptDenyDialogFragment+IOnDismissListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onDismiss (android.support.wearable.view.AcceptDenyDialogFragment p0)
	{
		n_onDismiss (p0);
	}

	private native void n_onDismiss (android.support.wearable.view.AcceptDenyDialogFragment p0);

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
