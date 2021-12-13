package mono.android.support.wearable.view;


public class ActionChooserView_ActionChooserListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.support.wearable.view.ActionChooserView.ActionChooserListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onOptionChosen:(I)V:GetOnOptionChosen_IHandler:Android.Support.Wearable.Views.ActionChooserView/IActionChooserListenerInvoker, Xamarin.Android.Wear\n" +
			"n_onOptionProgress:(F)V:GetOnOptionProgress_FHandler:Android.Support.Wearable.Views.ActionChooserView/IActionChooserListenerInvoker, Xamarin.Android.Wear\n" +
			"";
		mono.android.Runtime.register ("Android.Support.Wearable.Views.ActionChooserView+IActionChooserListenerImplementor, Xamarin.Android.Wear", ActionChooserView_ActionChooserListenerImplementor.class, __md_methods);
	}


	public ActionChooserView_ActionChooserListenerImplementor ()
	{
		super ();
		if (getClass () == ActionChooserView_ActionChooserListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Support.Wearable.Views.ActionChooserView+IActionChooserListenerImplementor, Xamarin.Android.Wear", "", this, new java.lang.Object[] {  });
	}


	public void onOptionChosen (int p0)
	{
		n_onOptionChosen (p0);
	}

	private native void n_onOptionChosen (int p0);


	public void onOptionProgress (float p0)
	{
		n_onOptionProgress (p0);
	}

	private native void n_onOptionProgress (float p0);

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
