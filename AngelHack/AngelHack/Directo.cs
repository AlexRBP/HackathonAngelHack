using Android.App;
using Android.Content;
using Android.OS;

namespace AngelHack
{
    [Activity(Label = "Directo", MainLauncher = true)]
    public class Directo : Activity
    {
        public Intent ServicioIntent;
        public Servicio Sensor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (ExisteArchivo())
            {
                Sensor = new Servicio();
                ServicioIntent = new Intent(this, Sensor.GetType());
                if (!ServicioOn(Sensor.GetType())) { StartService(ServicioIntent); }

                Intent intento = new Intent(this, typeof(Chat));
                intento.PutExtra("Nombre", "ChatBot");
                intento.PutExtra("IMEI", "ChatBot");
                this.StartActivity(intento);
            }
            else { this.StartActivity(typeof(Registro)); }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }

        public bool ServicioOn(System.Type Tipo)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(Tipo).CanonicalName)) { return true; }
            }
            return false;
        }

        public bool ExisteArchivo()
        {
            Java.IO.File folder = new Java.IO.File(FilesDir, "Archivos");
            if (!folder.Exists()) { return false; }
            else { return true; }
        }
    }
}