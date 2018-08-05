using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Telephony;
using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using FireSharp.Response;
using Java.Lang;
using Java.Net;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AngelHack
{
    [Service(Enabled = true)]
    public class Servicio : Service
    {
        public Servicio(Context Contexto) { Handler handler = new Handler(Looper.MainLooper); }

        public Servicio() { Handler handler = new Handler(Looper.MainLooper); }

        public override IBinder OnBind(Intent intent) { return null; }

        public override async void OnCreate()
        {
            //MI IMEI
            string IMEI = ((TelephonyManager)GetSystemService(TelephonyService)).DeviceId.ToString();

            //Recepción de Mensajes
            await NubeOnline("Datos/" + IMEI + "/Descargas/ChatBot", (sender, args, ctxt) =>
            {
                Handler handler = new Handler(Looper.MainLooper);
                handler.Post(new Runnable(async () =>
                {
                    //Foto
                    System.Console.WriteLine(args.Data);
                    new DescargarFiles().Execute(LinkDescarga(args.Data.Split('|')[0], args.Data.Split('|')[1]));

                    Notificacion("¡ChatBot!", "¡Acabas de recibir un mensaje!");

                    await EliminarNube<Object>("Datos/" + IMEI + "/Descargas/ChatBot");
                }));
            });
            /*
            //Notificación Siempre presente
            Intent notificationIntent = new Intent(this, typeof(Directo));
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            Notification NotificacionP = new NotificationCompat.Builder(this)
                    .SetSmallIcon(Resource.Drawable.Microphone)
                    .SetContentTitle("ChatBot")
                    .SetContentText("Estado: Encendido")
                    .SetContentIntent(pendingIntent).Build();
            StartForeground(1234, NotificacionP);*/
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
            //Handler handler = new Handler(Looper.MainLooper);
            //handler.Post(new Runnable(() => { Toast.MakeText(Application.Context, "Reinicio", ToastLength.Long).Show(); }));
            Intent BroadcastIntent = new Intent("dk.android.droid.RECEPTOR");
            SendBroadcast(BroadcastIntent);
        }

        private async Task NubeOnline(string Direccion, ValueAddedEventHandler Add)
        {
            IFirebaseConfig Configuracion = new FirebaseConfig
            {
                AuthSecret = "0egwc0yZZOzkfVSeUCMTqvszblcZvQphJJisK2qO",
                BasePath = "https://webserverplc.firebaseio.com/"
            };
            IFirebaseClient Cliente = new FirebaseClient(Configuracion);
            EventStreamResponse response = await Cliente.OnAsync(Direccion, Add, (sender, args, ctxt) => { }, (sender, args, ctxt) => { });
        }

        private async Task EliminarNube<Clase>(string Direccion)
        {
            IFirebaseConfig Configuracion = new FirebaseConfig
            {
                AuthSecret = "0egwc0yZZOzkfVSeUCMTqvszblcZvQphJJisK2qO",
                BasePath = "https://webserverplc.firebaseio.com/"
            };
            IFirebaseClient Cliente = new FirebaseClient(Configuracion);
            FirebaseResponse Response = await Cliente.DeleteAsync(Direccion);
        }

        public void Notificacion(string Titulo, string Texto)
        {
            Intent notificationIntent = new Intent(this, typeof(Directo));
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            Notification Notificacion = new NotificationCompat.Builder(this)
                    .SetSmallIcon(Resource.Drawable.Sent)
                    .SetContentTitle(Titulo)
                    .SetContentText(Texto)
                    .SetAutoCancel(true)
                    .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                    .SetContentIntent(pendingIntent).Build();

            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(12345, Notificacion);
        }

        private void GuardarUsuario(string Data, string Direccion, string Archivo)
        {
            Java.IO.File File = new Java.IO.File(FilesDir, Direccion);
            Java.IO.File Txt = new Java.IO.File(File, Archivo);
            using (var streamWriter = new System.IO.StreamWriter(Txt.Path, true))
            {
                streamWriter.WriteLine(Data);
            }
        }

        public void GuardarFile(byte[] buffer, string File)
        {
            Java.IO.File Dir = new Java.IO.File(FilesDir, "Archivos/Fotos/" + "Chatbot");
            Java.IO.File Files = new Java.IO.File(Dir, File);

            var fs = new FileStream(Files.Path, FileMode.OpenOrCreate);
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush(); fs.Close();

            GuardarUsuario("4|" + Files.Path, "Archivos/Usuarios", "Chatbot.txt");
        }

        public string LinkDescarga(string Link, string Nombre)
        {
            WebClient client = new WebClient();
            string Html = client.DownloadString(Link).Replace("<br>", "\r\n");
            string Token = Html.Split(new[] { "downloadTokens" }, System.StringSplitOptions.None)[1].Split('"')[2];

            //Handler handler = new Handler(Looper.MainLooper);
            //handler.Post(new Runnable(() => { Toast.MakeText(Application.Context, Directorio, ToastLength.Long).Show(); }));

            return "https://firebasestorage.googleapis.com/v0/b/webserverplc.appspot.com/o/" + Nombre + "?alt=media&token=" + Token + "|" + FilesDir.Path + "|" + Nombre;
        }

        public byte[] DescargarFile(string Url)
        {
            URL u = new URL(Url);
            URLConnection conexion = u.OpenConnection();
            int LongBytes = conexion.ContentLength;

            Java.IO.DataInputStream stream = new Java.IO.DataInputStream(u.OpenStream());
            byte[] buffer = new byte[LongBytes];
            stream.ReadFully(buffer);
            stream.Close();

            return buffer;
        }
    }

    public class DescargarFiles : AsyncTask<string, Integer, byte[]>
    {
        protected override byte[] RunInBackground(params string[] @params)
        {
            URL u = new URL(@params[0].Split('|')[0]);
            URLConnection conexion = u.OpenConnection();
            int LongBytes = conexion.ContentLength;

            Java.IO.DataInputStream stream = new Java.IO.DataInputStream(u.OpenStream());
            byte[] buffer = new byte[LongBytes];

            System.Console.WriteLine(buffer.Length);

            stream.ReadFully(buffer);
            stream.Close();

            Handler handlerF = new Handler(Looper.MainLooper);
            handlerF.Post(new Runnable(() => { GuardarFile(buffer, @params[0].Split('|')[2] + "|" + (@params[0].Split('|')[1])); }));

            return buffer;
        }

        protected override void OnPostExecute(byte[] result) { }

        public void GuardarFile(byte[] buffer, string File)
        {
            Java.IO.File Dir = new Java.IO.File(File.Split('|')[1] + "/Archivos/Fotos/" + "ChatBot");
            Java.IO.File Files = new Java.IO.File(Dir, File.Split('|')[0]);

            var fs = new FileStream(Files.Path, FileMode.OpenOrCreate);
            fs.Write(buffer, 0, buffer.Length);
            fs.Flush(); fs.Close();

            //Foto
            GuardarUsuario("4|" + Files.Path, File.Split('|')[1] + "/Archivos/Usuarios", "ChatBot.txt");

            //Preguntas
            GuardarUsuario("6|" + "Alguien ha tocado la puerta ¿Desea abrirla?", File.Split('|')[1] + "/Archivos/Usuarios", "ChatBot.txt");
            GuardarUsuario("6|" + "Indique Si o No", File.Split('|')[1] + "/Archivos/Usuarios", "ChatBot.txt");
        }

        private void GuardarUsuario(string Data, string Direccion, string Archivo)
        {
            Java.IO.File File = new Java.IO.File(Direccion);
            Java.IO.File Txt = new Java.IO.File(File, Archivo);
            using (var streamWriter = new System.IO.StreamWriter(Txt.Path, true))
            {
                streamWriter.WriteLine(Data);
            }
        }
    }
}