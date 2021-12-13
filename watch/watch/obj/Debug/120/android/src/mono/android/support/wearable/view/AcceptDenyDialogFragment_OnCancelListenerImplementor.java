package mono.android.support.wearable.view;


public class AcceptDenyDialogFragment_OnCancelListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.AcceptDenyDialogFragment.OnCancelListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCancel:(Landroid/support/wearable/view/AcceptDenyDialogFragment;)V:GetOnCancel_Landroid_support_wearable_view_AcceptDenyDialogFragment_Handler:Android.Support.Wearable.Views.AcceptDenyDialogFragment/IOnCancelListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Views.AcceptDenyDialogFragment+IOnCancelListenerImplementor, Xamarin.Android.Wear", AcceptDenyDialogFragment_OnCancelListenerImplementor.class, __md_methods);
	}


	public AcceptDenyDialogFragment_OnCancelListenerImplementor ()
	{
		super ();
		if (getClass () == AcceptDenyDialogFragment_OnCancelListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Views.AcceptDenyDialogFragment+IOnCancelListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onCancel (android.support.wearable.view.AcceptDenyDialogFragment p0)
	{
		n_onCancel (p0);
	}

	private native void n_onCancel (android.support.wearable.view.AcceptDenyDialogFragment p0);

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
