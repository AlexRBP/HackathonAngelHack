using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

namespace AngelHack
{
    [Activity(Label = "Registro")]
    public class Registro : Activity
    {
        public int NumUsuario = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Registro);

            Typeface tf = Typeface.CreateFromAsset(Assets, "MuseoSlab500.otf");
            var Titulo = FindViewById<TextView>(Resource.Id.Titulo); Titulo.SetTypeface(tf, TypefaceStyle.Bold);

            //Nombre
            var Label1 = FindViewById<EditText>(Resource.Id.Label1); Label1.SetTypeface(tf, TypefaceStyle.Bold);
            var Text1 = FindViewById<TextView>(Resource.Id.Text1); Text1.SetTypeface(tf, TypefaceStyle.Bold);

            //DNI
            var Label2 = FindViewById<EditText>(Resource.Id.Label2); Label2.SetTypeface(tf, TypefaceStyle.Bold);
            var Text2 = FindViewById<TextView>(Resource.Id.Text2); Text2.SetTypeface(tf, TypefaceStyle.Bold);

            //Celular
            var Label3 = FindViewById<EditText>(Resource.Id.Label3); Label3.SetTypeface(tf, TypefaceStyle.Bold);
            var Text3 = FindViewById<TextView>(Resource.Id.Text3); Text3.SetTypeface(tf, TypefaceStyle.Bold);

            //Email
            var Label4 = FindViewById<EditText>(Resource.Id.Label4); Label4.SetTypeface(tf, TypefaceStyle.Bold);
            var Text4 = FindViewById<TextView>(Resource.Id.Text4); Text4.SetTypeface(tf, TypefaceStyle.Bold);

            //IMEI
            string IMEI = ((TelephonyManager)GetSystemService(TelephonyService)).DeviceId.ToString();

            var Progress = FindViewById<ProgressBar>(Resource.Id.Progress);

            var Button1 = FindViewById<Button>(Resource.Id.Button); Button1.SetTypeface(tf, TypefaceStyle.Bold);
            Button1.Click += (s, e) =>
            {
                new Thread(async () =>
                {
                    if (Label1.Text == "" || Label2.Text == "" || Label3.Text == "" || Label4.Text == "") { RunOnUiThread(() => Toast.MakeText(Application.Context, "Rellene todos los campos", ToastLength.Long).Show()); }
                    else
                    {
                        HabilitadorLabel(Label1, Label2, Label3, Label4, false);
                        RunOnUiThread(() => Button1.Visibility = ViewStates.Invisible); RunOnUiThread(() => Progress.Visibility = ViewStates.Visible);
                        if (HayInternet())
                        {
                            await SubirNube<Database>(new Database { Nombre = Label1.Text, Dni = Label2.Text, Celular = Label3.Text, Email = Label4.Text }, "Datos/" + IMEI);

                            while (true)
                            {
                                if (await LeerNube("Tipo/" + NumUsuario.ToString()) == "null" || await LeerNube("Tipo/" + NumUsuario.ToString()) == IMEI) break;
                                NumUsuario++;
                            }

                            await SubirNube<Object>(JsonConvert.DeserializeObject("{\"" + NumUsuario.ToString() + "\":\"" + IMEI + "\"}"), "Tipo/");

                            CrearFile("Archivos");
                            CrearFile("Archivos/Usuarios");
                            CrearFile("Archivos/Fotos");
                            CrearTxt("Archivos/Usuarios", "Lista.txt");

                            //Crear Contacto del ChatBot
                            GuardarUsuario("ChatBot"+ "|" + "1" + "|" + "ChatBot", "Archivos/Usuarios", "Lista.txt");
                            CrearTxt("Archivos/Usuarios", "ChatBot" + ".txt");
                            CrearFile("Archivos/Fotos/" + "ChatBot");

                            StartActivity(typeof(Directo));
                        }
                        else { RunOnUiThread(() => Toast.MakeText(Application.Context, "Ha ocurrido un error, compruebe su conexión a Internet", ToastLength.Long).Show()); RunOnUiThread(() => Button1.Visibility = ViewStates.Visible); RunOnUiThread(() => Progress.Visibility = ViewStates.Invisible); HabilitadorLabel(Label1, Label2, Label3, Label4, true); }
                    }
                }).Start();
            };
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }

        public bool HayInternet()
        {
            Android.Net.ConnectivityManager connectivityManager = (Android.Net.ConnectivityManager)GetSystemService(ConnectivityService);
            Android.Net.NetworkInfo activeNetworkInfo = connectivityManager.ActiveNetworkInfo;
            return activeNetworkInfo != null && activeNetworkInfo.IsConnected;
        }

        public void HabilitadorLabel(EditText L1, EditText L2, EditText L3, EditText L4, bool Habilita)
        {
            RunOnUiThread(() => L1.Enabled = Habilita);
            RunOnUiThread(() => L2.Enabled = Habilita);
            RunOnUiThread(() => L3.Enabled = Habilita);
            RunOnUiThread(() => L4.Enabled = Habilita);
        }

        public void CrearFile(string Nombre)
        {
            string Direccion = this.FilesDir + "/" + Nombre;
            Directory.CreateDirectory(Direccion);
        }

        public void CrearTxt(string Direccion, string Nombre)
        {
            Java.IO.File Path = new Java.IO.File(FilesDir, Direccion);
            Java.IO.File Txt = new Java.IO.File(Path, Nombre);
            File.WriteAllText(Txt.Path, "");
            System.Console.WriteLine(Txt.Path);
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

        private async Task SubirNube<Clase>(Clase Database, string Direccion)
        {
            IFirebaseConfig Configuracion = new FirebaseConfig
            {
                AuthSecret = "0egwc0yZZOzkfVSeUCMTqvszblcZvQphJJisK2qO",
                BasePath = "https://webserverplc.firebaseio.com/"
            };
            IFirebaseClient Cliente = new FirebaseClient(Configuracion);
            FirebaseResponse Response = await Cliente.UpdateAsync(Direccion, Database);
            Clase Result = Response.ResultAs<Clase>();
        }

        private async Task<string> LeerNube(string Direccion)
        {
            IFirebaseConfig Configuracion = new FirebaseConfig
            {
                AuthSecret = "0egwc0yZZOzkfVSeUCMTqvszblcZvQphJJisK2qO",
                BasePath = "https://webserverplc.firebaseio.com/"
            };
            IFirebaseClient Cliente = new FirebaseClient(Configuracion);
            FirebaseResponse Response = await Cliente.GetAsync(Direccion);
            Object Todo = Response.ResultAs<Object>();
            if (Response.Body != "null") { return Response.Body.Split('"')[1]; }
            else return "null";
        }
    }

    [Serializable()]
    public class Database
    {
        public string Nombre { get; set; }
        public string Dni { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
    }

    [Serializable()]
    public class Data
    {
        public string IMEI { get; set; }
    }
}