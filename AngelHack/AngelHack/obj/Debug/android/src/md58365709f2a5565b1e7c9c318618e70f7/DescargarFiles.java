package md58365709f2a5565b1e7c9c318618e70f7;


public class DescargarFiles
	extends android.os.AsyncTask
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_doInBackground:([Ljava/lang/Object;)Ljava/lang/Object;:GetDoInBackground_arrayLjava_lang_Object_Handler\n" +
			"";
		mono.android.Runtime.register ("AngelHack.DescargarFiles, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DescargarFiles.class, __md_methods);
	}


	public DescargarFiles ()
	{
		super ();
		if (getClass () == DescargarFiles.class)
			mono.android.TypeManager.Activate ("AngelHack.DescargarFiles, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public java.lang.Object doInBackground (java.lang.Object[] p0)
	{
		return n_doInBackground (p0);
	}

	private native java.lang.Object n_doInBackground (java.lang.Object[] p0);

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
