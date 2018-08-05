package md58365709f2a5565b1e7c9c318618e70f7;


public class Servicio
	extends android.app.Service
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBind:(Landroid/content/Intent;)Landroid/os/IBinder;:GetOnBind_Landroid_content_Intent_Handler\n" +
			"n_onCreate:()V:GetOnCreateHandler\n" +
			"n_onStartCommand:(Landroid/content/Intent;II)I:GetOnStartCommand_Landroid_content_Intent_IIHandler\n" +
			"n_onTaskRemoved:(Landroid/content/Intent;)V:GetOnTaskRemoved_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("AngelHack.Servicio, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Servicio.class, __md_methods);
	}


	public Servicio ()
	{
		super ();
		if (getClass () == Servicio.class)
			mono.android.TypeManager.Activate ("AngelHack.Servicio, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public Servicio (android.content.Context p0)
	{
		super ();
		if (getClass () == Servicio.class)
			mono.android.TypeManager.Activate ("AngelHack.Servicio, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public android.os.IBinder onBind (android.content.Intent p0)
	{
		return n_onBind (p0);
	}

	private native android.os.IBinder n_onBind (android.content.Intent p0);


	public void onCreate ()
	{
		n_onCreate ();
	}

	private native void n_onCreate ();


	public int onStartCommand (android.content.Intent p0, int p1, int p2)
	{
		return n_onStartCommand (p0, p1, p2);
	}

	private native int n_onStartCommand (android.content.Intent p0, int p1, int p2);


	public void onTaskRemoved (android.content.Intent p0)
	{
		n_onTaskRemoved (p0);
	}

	private native void n_onTaskRemoved (android.content.Intent p0);

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
