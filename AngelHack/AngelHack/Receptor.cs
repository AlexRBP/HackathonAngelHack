using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;

namespace AngelHack
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "dk.android.droid.RECEPTOR" })]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    class Receptor : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //Handler handler = new Handler(Looper.MainLooper);
            //handler.Post(new Runnable(() => { Toast.MakeText(Application.Context, "Encendido", ToastLength.Long).Show(); }));
            Servicio Sensor = new Servicio();
            context.StartService(new Intent(context, Sensor.GetType()));
        }
    }
}