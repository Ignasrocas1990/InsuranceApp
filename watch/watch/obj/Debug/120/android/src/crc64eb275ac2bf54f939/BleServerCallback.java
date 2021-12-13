package crc64eb275ac2bf54f939;


public class BleServerCallback
	extends android.bluetooth.BluetoothGattServerCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCharacteristicReadRequest:(Landroid/bluetooth/BluetoothDevice;IILandroid/bluetooth/BluetoothGattCharacteristic;)V:GetOnCharacteristicReadRequest_Landroid_bluetooth_BluetoothDevice_IILandroid_bluetooth_BluetoothGattCharacteristic_Handler\n" +
			"n_onConnectionStateChange:(Landroid/bluetooth/BluetoothDevice;II)V:GetOnConnectionStateChange_Landroid_bluetooth_BluetoothDevice_IIHandler\n" +
			"n_onNotificationSent:(Landroid/bluetooth/BluetoothDevice;I)V:GetOnNotificationSent_Landroid_bluetooth_BluetoothDevice_IHandler\n" +
			"";
		mono.android.Runtime.register ("watch.BleServerCallback, watch", BleServerCallback.class, __md_methods);
	}


	public BleServerCallback ()
	{
		super ();
		if (getClass () == BleServerCallback.class)
			mono.android.TypeManager.Activate ("watch.BleServerCallback, watch", "", this, new java.lang.Object[] {  });
	}


	public void onCharacteristicReadRequest (android.bluetooth.BluetoothDevice p0, int p1, int p2, android.bluetooth.BluetoothGattCharacteristic p3)
	{
		n_onCharacteristicReadRequest (p0, p1, p2, p3);
	}

	private native void n_onCharacteristicReadRequest (android.bluetooth.BluetoothDevice p0, int p1, int p2, android.bluetooth.BluetoothGattCharacteristic p3);


	public void onConnectionStateChange (android.bluetooth.BluetoothDevice p0, int p1, int p2)
	{
		n_onConnectionStateChange (p0, p1, p2);
	}

	private native void n_onConnectionStateChange (android.bluetooth.BluetoothDevice p0, int p1, int p2);


	public void onNotificationSent (android.bluetooth.BluetoothDevice p0, int p1)
	{
		n_onNotificationSent (p0, p1);
	}

	private native void n_onNotificationSent (android.bluetooth.BluetoothDevice p0, int p1);

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
