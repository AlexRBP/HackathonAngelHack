package md58365709f2a5565b1e7c9c318618e70f7;


public class ChatAdaptador_ViewHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AngelHack.ChatAdaptador+ViewHolder, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ChatAdaptador_ViewHolder.class, __md_methods);
	}


	public ChatAdaptador_ViewHolder ()
	{
		super ();
		if (getClass () == ChatAdaptador_ViewHolder.class)
			mono.android.TypeManager.Activate ("AngelHack.ChatAdaptador+ViewHolder, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
