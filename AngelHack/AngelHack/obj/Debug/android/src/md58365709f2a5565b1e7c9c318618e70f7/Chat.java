package md58365709f2a5565b1e7c9c318618e70f7;


public class Chat
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer,
		android.widget.AdapterView.OnItemClickListener,
		android.speech.RecognitionListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onActivityResult:(IILandroid/content/Intent;)V:GetOnActivityResult_IILandroid_content_Intent_Handler\n" +
			"n_onBackPressed:()V:GetOnBackPressedHandler\n" +
			"n_onItemClick:(Landroid/widget/AdapterView;Landroid/view/View;IJ)V:GetOnItemClick_Landroid_widget_AdapterView_Landroid_view_View_IJHandler:Android.Widget.AdapterView/IOnItemClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onBeginningOfSpeech:()V:GetOnBeginningOfSpeechHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onBufferReceived:([B)V:GetOnBufferReceived_arrayBHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onEndOfSpeech:()V:GetOnEndOfSpeechHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onError:(I)V:GetOnError_IHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onEvent:(ILandroid/os/Bundle;)V:GetOnEvent_ILandroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onPartialResults:(Landroid/os/Bundle;)V:GetOnPartialResults_Landroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onReadyForSpeech:(Landroid/os/Bundle;)V:GetOnReadyForSpeech_Landroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onResults:(Landroid/os/Bundle;)V:GetOnResults_Landroid_os_Bundle_Handler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"n_onRmsChanged:(F)V:GetOnRmsChanged_FHandler:Android.Speech.IRecognitionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("AngelHack.Chat, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Chat.class, __md_methods);
	}


	public Chat ()
	{
		super ();
		if (getClass () == Chat.class)
			mono.android.TypeManager.Activate ("AngelHack.Chat, AngelHack, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public void onActivityResult (int p0, int p1, android.content.Intent p2)
	{
		n_onActivityResult (p0, p1, p2);
	}

	private native void n_onActivityResult (int p0, int p1, android.content.Intent p2);


	public void onBackPressed ()
	{
		n_onBackPressed ();
	}

	private native void n_onBackPressed ();


	public void onItemClick (android.widget.AdapterView p0, android.view.View p1, int p2, long p3)
	{
		n_onItemClick (p0, p1, p2, p3);
	}

	private native void n_onItemClick (android.widget.AdapterView p0, android.view.View p1, int p2, long p3);


	public void onBeginningOfSpeech ()
	{
		n_onBeginningOfSpeech ();
	}

	private native void n_onBeginningOfSpeech ();


	public void onBufferReceived (byte[] p0)
	{
		n_onBufferReceived (p0);
	}

	private native void n_onBufferReceived (byte[] p0);


	public void onEndOfSpeech ()
	{
		n_onEndOfSpeech ();
	}

	private native void n_onEndOfSpeech ();


	public void onError (int p0)
	{
		n_onError (p0);
	}

	private native void n_onError (int p0);


	public void onEvent (int p0, android.os.Bundle p1)
	{
		n_onEvent (p0, p1);
	}

	private native void n_onEvent (int p0, android.os.Bundle p1);


	public void onPartialResults (android.os.Bundle p0)
	{
		n_onPartialResults (p0);
	}

	private native void n_onPartialResults (android.os.Bundle p0);


	public void onReadyForSpeech (android.os.Bundle p0)
	{
		n_onReadyForSpeech (p0);
	}

	private native void n_onReadyForSpeech (android.os.Bundle p0);


	public void onResults (android.os.Bundle p0)
	{
		n_onResults (p0);
	}

	private native void n_onResults (android.os.Bundle p0);


	public void onRmsChanged (float p0)
	{
		n_onRmsChanged (p0);
	}

	private native void n_onRmsChanged (float p0);

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
