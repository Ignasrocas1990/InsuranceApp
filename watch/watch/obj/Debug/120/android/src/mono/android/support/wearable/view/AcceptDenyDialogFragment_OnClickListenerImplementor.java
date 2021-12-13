package mono.android.support.wearable.view;


public class AcceptDenyDialogFragment_OnClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.AcceptDenyDialogFragment.OnClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/support/wearable/view/AcceptDenyDialogFragment;I)V:GetOnClick_Landroid_support_wearable_view_AcceptDenyDialogFragment_IHandler:Android.Support.Wearable.Views.AcceptDenyDialogFragment/IOnClickListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Views.AcceptDenyDialogFragment+IOnClickListenerImplementor, Xamarin.Android.Wear", AcceptDenyDialogFragment_OnClickListenerImplementor.class, __md_methods);
	}


	public AcceptDenyDialogFragment_OnClickListenerImplementor ()
	{
		super ();
		if (getClass () == AcceptDenyDialogFragment_OnClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Views.AcceptDenyDialogFragment+IOnClickListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onClick (android.support.wearable.view.AcceptDenyDialogFragment p0, int p1)
	{
		n_onClick (p0, p1);
	}

	private native void n_onClick (android.support.wearable.view.AcceptDenyDialogFragment p0, int p1);

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
