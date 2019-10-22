package md583db3c0f0ac8d2f3447d8354b77f7186;


public class EditActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("RPICoin_Rewards.EditActivity, RPICoin_Rewards", EditActivity.class, __md_methods);
	}


	public EditActivity ()
	{
		super ();
		if (getClass () == EditActivity.class)
			mono.android.TypeManager.Activate ("RPICoin_Rewards.EditActivity, RPICoin_Rewards", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
