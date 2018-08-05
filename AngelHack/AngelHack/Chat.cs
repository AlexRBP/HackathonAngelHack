using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Speech;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Firebase.Storage;
using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using FireSharp.Response;
using Java.Lang;
using Newtonsoft.Json;
using static Android.App.ActionBar;
using static Android.Widget.AdapterView;

namespace AngelHack
{
    [Activity(Label = "Chat")]
    public class Chat : Activity, IOnItemClickListener, IRecognitionListener
    {
        private Dictionary<string, ChatClase> ListaMensajes = new Dictionary<string, ChatClase>();
        public ImageButton Image1, Image2, Image3, Image4, Image5;
        public ListView Listado;
        public ProgressBar Progress;
        public string IMEI, MiIMEI;
        public Typeface tf;
        public PopupWindow PopUp;

        public static ChatAdaptador Adaptador;
        public AudioTrack audioTrack;
        public AudioRecord audRecorder;
        public bool CreateSound = false, CreateAudio = false, FinGrabacion = false;
        byte[] audioBuffer = new byte[AudioRecord.GetMinBufferSize(8000, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit)];
        SpeechRecognizer voz;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Chat);
            tf = Typeface.CreateFromAsset(Assets, "MuseoSlab500.otf");
            var Titulo1 = FindViewById<TextView>(Resource.Id.Titulo1); Titulo1.SetTypeface(tf, TypefaceStyle.Bold);
            var Titulo2 = FindViewById<TextView>(Resource.Id.Titulo2); Titulo2.SetTypeface(tf, TypefaceStyle.Bold);
            var Estado = FindViewById<TextView>(Resource.Id.Estado); Estado.SetTypeface(tf, TypefaceStyle.Bold);
            var Text = FindViewById<TextView>(Resource.Id.Text); Text.SetTypeface(tf, TypefaceStyle.Bold);

            Titulo1.Text = Intent.GetStringExtra("Nombre") ?? "null";
            Titulo2.Text = "Usuario";
            Estado.Text = "Estado: Disponible";
            IMEI = Intent.GetStringExtra("IMEI") ?? "null";
            MiIMEI = ((TelephonyManager)GetSystemService(Context.TelephonyService)).DeviceId.ToString();

            Listado = FindViewById<ListView>(Resource.Id.Lista);
            Listado.OnItemClickListener = this;

            GenerarLista();

            audioTrack = new AudioTrack(Android.Media.Stream.Music, 8000, Android.Media.ChannelOut.Mono, Android.Media.Encoding.Pcm16bit, audioBuffer.Length, AudioTrackMode.Stream);
            audioTrack.Play();

            Image1 = FindViewById<ImageButton>(Resource.Id.Image1);
            Drawable drawable = GetDrawable(Resource.Drawable.Back);
            drawable.SetBounds(0, 0, 70, 70);
            Image1.SetImageDrawable(drawable);
            Image1.Click += (s, e) => { };

            Image2 = FindViewById<ImageButton>(Resource.Id.Image2);
            drawable = GetDrawable(Resource.Drawable.Camara);
            drawable.SetBounds(0, 0, 70, 70);
            Image2.SetImageDrawable(drawable);
            Image2.Click += (s, e) =>
            {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 0);
            };

            Image3 = FindViewById<ImageButton>(Resource.Id.Image3);
            drawable = GetDrawable(Resource.Drawable.Microphone);
            drawable.SetBounds(0, 0, 70, 70);
            Image3.SetImageDrawable(drawable);
            Image3.Touch += (s, e) =>
            {
                if (MotionEventActions.Down == e.Event.Action) { Image3.SetBackgroundColor(Color.Rgb(66, 66, 66)); if (!CreateAudio) { GrabarAudio(); CreateAudio = !CreateAudio; } }
                if (MotionEventActions.Up == e.Event.Action) { FinGrabacion = !FinGrabacion; CreateAudio = !CreateAudio; Image3.SetBackgroundColor(Color.Rgb(2, 94, 115)); }
            };

            Image4 = FindViewById<ImageButton>(Resource.Id.Image4);
            drawable = GetDrawable(Resource.Drawable.Sheldon);
            drawable.SetBounds(0, 0, 70, 70);
            Image4.SetImageDrawable(drawable);
            Image4.Touch += (s, e) =>
            {
                if (MotionEventActions.Down == e.Event.Action)
                {
                    Image4.SetBackgroundColor(Color.Rgb(66, 66, 66));
                    Intent almacenar = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                    almacenar.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                    almacenar.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

                    //Timer
                    almacenar.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                    almacenar.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 15000);
                    almacenar.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 15000);
                    almacenar.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                    voz.StartListening(almacenar);
                }
                if (MotionEventActions.Up == e.Event.Action) { Image4.SetBackgroundColor(Color.Rgb(2, 94, 115)); }
            };

            voz = SpeechRecognizer.CreateSpeechRecognizer(this);
            voz.SetRecognitionListener(this);

            Image5 = FindViewById<ImageButton>(Resource.Id.Image5);
            drawable = GetDrawable(Resource.Drawable.Sent);
            drawable.SetBounds(0, 0, 70, 70);
            Image5.SetImageDrawable(drawable);
            Image5.Click += (s, e) =>
            {
                RunOnUiThread(() => Progress.Visibility = ViewStates.Visible);

                new Java.Lang.Thread(new Runnable(async () =>
                {
                    //Subida a la Nube (Mensaje Texto)
                    bool bandera = true;
                    while (bandera)
                    {
                        await SubirNube<System.Object>(JsonConvert.DeserializeObject("{\"" + MiIMEI + "\":\"" + Text.Text + "|" + "null" + "|6" + "\"}"), "Datos/" + IMEI + "/Descargas");

                        RunOnUiThread(() => Progress.Visibility = ViewStates.Invisible);
                        RunOnUiThread(() => Listado.Adapter = null);
                        RunOnUiThread(() => GuardarUsuario("5|" + Text.Text, "Archivos/Usuarios", IMEI + ".txt")); bandera = !bandera;
                        RunOnUiThread(() => Text.Text = "");
                    }
                })).Start();
            };

            Progress = FindViewById<ProgressBar>(Resource.Id.Progress);

            await NubeOnline("Datos/" + MiIMEI + "/Descargas/ChatBot", (sender, args, ctxt) =>
            {
                Handler handler = new Handler(Looper.MainLooper);
                handler.Post(new Runnable(() =>
                {
                    GenerarLista(); //Actualiza Pantalla de Inicio
                }));
            });
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            ChatClase mensaje = Adaptador.GetItem(position);
            var Key = ListaMensajes.FirstOrDefault(x => x.Value == mensaje).Key;
            if (ListaMensajes[Key] == mensaje)
            {
                switch (ListaMensajes[Key].GetEsMio())
                {
                    case 1:
                    case 3:
                        Toast.MakeText(Application.Context, "Click", ToastLength.Long).Show();
                        Java.IO.File music = new Java.IO.File(ListaMensajes[Key].GetContenido());
                        byte[] byteData = new byte[(int)music.Length()];
                        var fos = new FileStream(music.Path, FileMode.OpenOrCreate);
                        fos.Read(byteData, 0, byteData.Length);
                        fos.Close();
                        ReproducirAudio(byteData);
                        break;
                    case 2:
                    case 4:
                        LayoutInflater Inflate = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
                        View Layout = Inflate.Inflate(Resource.Layout.PopUpFoto, null);

                        PopUp = new PopupWindow(this); //Crear PopUpWindow Foto
                        PopUp.ContentView = Layout;
                        PopUp.Width = LayoutParams.MatchParent;
                        PopUp.Height = LayoutParams.MatchParent;
                        PopUp.Focusable = true;
                        PopUp.SetBackgroundDrawable(new BitmapDrawable());
                        PopUp.ShowAtLocation(Layout, GravityFlags.Center, 0, 0);

                        var ImagePop = Layout.FindViewById<ImageView>(Resource.Id.FotoZoom);
                        Java.IO.File Image = new Java.IO.File(ListaMensajes[Key].GetContenido());
                        Bitmap myBitmap = BitmapFactory.DecodeFile(Image.AbsolutePath);
                        ImagePop.SetImageBitmap(myBitmap);
                        ImagePop.Click += (s, e) => { PopUp.Dismiss(); };
                        break;
                }
            }
        }

        //Reproducir Audio
        void ReproducirAudio(byte[] buffer)
        {
            var Decode = DecodeA(buffer, 0, buffer.Length);
            audioTrack.Write(Decode, 0, Decode.Length);
        }

        //Grabar Audio
        public void GrabarAudio()
        {
            Java.Lang.Thread stream = new Java.Lang.Thread(new Runnable(async () =>
            {
                Java.IO.File Dir = new Java.IO.File(FilesDir, "Archivos/Fotos/" + IMEI);
                string Nombre = "Audio_" + System.Guid.NewGuid() + ".pcm";
                Java.IO.File Audio = new Java.IO.File(Dir, Nombre);
                audRecorder = new AudioRecord(AudioSource.VoiceCommunication, 8000, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit, audioBuffer.Length);
                bool bandera = true, Llave = true;

                var fs = new FileStream(Audio.Path, FileMode.OpenOrCreate);
                audRecorder.StartRecording();
                while (bandera)
                {
                    int audioData = audRecorder.Read(audioBuffer, 0, audioBuffer.Length);
                    var Encode = EncodeA(audioBuffer, 0, audioBuffer.Length);
                    fs.Write(Encode, 0, Encode.Length);
                    if (FinGrabacion) { FinGrabacion = !FinGrabacion; bandera = !bandera; }
                }
                audRecorder.Stop();
                fs.Close();
                RunOnUiThread(() => Progress.Visibility = ViewStates.Visible);

                //Subida a la Nube (Voz)
                bandera = true;
                var music = new FileStream(Audio.Path, FileMode.OpenOrCreate);
                var task = new FirebaseStorage("webserverplc.appspot.com").Child(Nombre).PutAsync(music);
                task.Progress.ProgressChanged += (s, se) => { if (se.Percentage >= 100) { Llave = false; } };
                while (bandera)
                {
                    if (!Llave)
                    {
                        await SubirNube<System.Object>(JsonConvert.DeserializeObject("{\"" + MiIMEI + "\":\"" + task.TargetUrl + "|" + Nombre + "|3" + "\"}"), "Datos/" + IMEI + "/Descargas");

                        RunOnUiThread(() => Progress.Visibility = ViewStates.Invisible);
                        RunOnUiThread(() => Listado.Adapter = null);
                        RunOnUiThread(() => GuardarUsuario("1|" + Audio.Path, "Archivos/Usuarios", IMEI + ".txt")); bandera = !bandera; music.Close();
                    }
                }
            }));
            stream.Start();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                Bitmap bitmap = (Bitmap)data.Extras.Get("data");

                Java.IO.File Dir = new Java.IO.File(FilesDir, "Archivos/Fotos/" + IMEI);
                string Nombre = "Foto_" + System.Guid.NewGuid() + ".png";
                Java.IO.File Image = new Java.IO.File(Dir, Nombre);

                var fs = new FileStream(Image.Path, FileMode.OpenOrCreate);
                bitmap.Compress(Bitmap.CompressFormat.Png, 90, fs);
                fs.Close();
                RunOnUiThread(() => Progress.Visibility = ViewStates.Visible);

                new Java.Lang.Thread(new Runnable(async () =>
                {
                    //Subida a la Nube (Foto)
                    bool bandera = true, Llave = true;
                    var photo = new FileStream(Image.Path, FileMode.OpenOrCreate);
                    var task = new FirebaseStorage("webserverplc.appspot.com").Child(Nombre).PutAsync(photo);
                    task.Progress.ProgressChanged += (s, se) => { if (se.Percentage >= 100) { Llave = false; } };
                    while (bandera)
                    {
                        if (!Llave)
                        {
                            await SubirNube<System.Object>(JsonConvert.DeserializeObject("{\"" + MiIMEI + "\":\"" + task.TargetUrl + "|" + Nombre + "|4" + "\"}"), "Datos/" + IMEI + "/Descargas");

                            RunOnUiThread(() => Progress.Visibility = ViewStates.Invisible);
                            RunOnUiThread(() => Listado.Adapter = null);
                            RunOnUiThread(() => GuardarUsuario("2|" + Image.Path, "Archivos/Usuarios", IMEI + ".txt")); bandera = !bandera; photo.Close();
                        }
                    }
                })).Start();
            }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }

        public void AgregarLista(string Contenido, int EsMio)
        {
            ChatClase Message = new ChatClase(Contenido, EsMio);
            Message.SetLetra(tf);
            ListaMensajes.Add(Message.getId(), Message);
        }

        public void GenerarLista()
        {
            Adaptador = new ChatAdaptador(this, 0, ListaCompleta());
            Handler handler = new Handler(Looper.MainLooper);
            handler.Post(new Runnable(() => { Listado.Adapter = Adaptador; }));
            Adaptador.NotifyDataSetChanged();
        }

        private async Task NubeOnline(string Direccion, ValueRemovedEventHandler Remove)
        {
            IFirebaseConfig Configuracion = new FirebaseConfig
            {
                AuthSecret = "0egwc0yZZOzkfVSeUCMTqvszblcZvQphJJisK2qO",
                BasePath = "https://webserverplc.firebaseio.com/"
            };
            IFirebaseClient Cliente = new FirebaseClient(Configuracion);
            EventStreamResponse response = await Cliente.OnAsync(Direccion, (sender, args, ctxt) => { }, (sender, args, ctxt) => { }, Remove);
        }

        private ChatClase[] ListaCompleta()
        {
            ListaMensajes.Clear();
            foreach (var Lectura in LeerUsuarios("Archivos/Usuarios", IMEI + ".txt").ToArray())
            {
                switch (Lectura.Split('|')[0])
                {
                    case "1":
                        AgregarLista(Lectura.Split('|')[1], 1);
                        break;
                    case "2":
                        AgregarLista(Lectura.Split('|')[1], 2);
                        break;
                    case "3":
                        AgregarLista(Lectura.Split('|')[1], 3);
                        break;
                    case "4":
                        AgregarLista(Lectura.Split('|')[1], 4);
                        break;
                    case "5":
                        AgregarLista(Lectura.Split('|')[1], 5);
                        break;
                    case "6":
                        AgregarLista(Lectura.Split('|')[1], 6);
                        break;
                }
            }

            return ListaMensajes.Values.ToList().ToArray();
        }

        private void GuardarUsuario(string Data, string Direccion, string Archivo)
        {
            Java.IO.File File = new Java.IO.File(FilesDir, Direccion);
            Java.IO.File Txt = new Java.IO.File(File, Archivo);
            using (var streamWriter = new System.IO.StreamWriter(Txt.Path, true))
            {
                streamWriter.WriteLine(Data);
            }
            GenerarLista();
        }

        private string[] LeerUsuarios(string Direccion, string Archivo)
        {
            Java.IO.File File = new Java.IO.File(FilesDir, Direccion);
            Java.IO.File Txt = new Java.IO.File(File, Archivo);
            string Line = ""; List<string> Lista = new List<string>();
            using (var streamReader = new System.IO.StreamReader(Txt.Path))
            {
                while ((Line = streamReader.ReadLine()) != null) { Lista.Add(Line); }
            }
            return Lista.ToArray();
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

        public byte[] EncodeA(byte[] data, int offset, int length) //Codificador Ley A
        {
            byte[] encoded = new byte[length / 2];
            int outIndex = 0;
            for (int n = 0; n < length; n += 2)
            {
                encoded[outIndex++] = EncodeALaw.LinearToALawSample(BitConverter.ToInt16(data, offset + n));
            }
            return encoded;
        }

        public byte[] DecodeA(byte[] data, int offset, int length) //Decodificador Ley A
        {
            byte[] decoded = new byte[length * 2];
            int outIndex = 0;
            for (int n = 0; n < length; n++)
            {
                short decodedSample = DecodeALaw.ALawToLinearSample(data[n + offset]);
                decoded[outIndex++] = (byte)(decodedSample & 0xFF);
                decoded[outIndex++] = (byte)(decodedSample >> 8);
            }
            return decoded;
        }

        public void OnBeginningOfSpeech()
        {
            //throw new NotImplementedException(); //Empieza
        }

        public void OnBufferReceived(byte[] buffer)
        {
            //throw new NotImplementedException();
        }

        public void OnEndOfSpeech()
        {
            //throw new NotImplementedException();
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            //throw new NotImplementedException();
        }

        public void OnEvent(int eventType, Bundle @params)
        {
            //throw new NotImplementedException();
        }

        public void OnPartialResults(Bundle partialResults)
        {
            //throw new NotImplementedException();
        }

        public void OnReadyForSpeech(Bundle @params)
        {
            //throw new NotImplementedException();
        }

        public void OnResults(Bundle results)
        {
            var Datos = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            StringBuilder Agrupar = new StringBuilder();
            for (int i = 0; i < 1; i++) { Agrupar.Append(Datos[i]); }
            RunOnUiThread(() => Progress.Visibility = ViewStates.Visible);

            new Java.Lang.Thread(new Runnable(async () =>
            {
                //Subida a la Nube (Mensaje Texto)
                bool bandera = true;
                while (bandera)
                {
                    await SubirNube<System.Object>(JsonConvert.DeserializeObject("{\"" + MiIMEI + "\":\"" + Agrupar.ToString() + "|" + "null" + "|6" + "\"}"), "Datos/" + IMEI + "/Descargas");

                    RunOnUiThread(() => Progress.Visibility = ViewStates.Invisible);
                    RunOnUiThread(() => Listado.Adapter = null);
                    RunOnUiThread(() => GuardarUsuario("5|" + Agrupar.ToString(), "Archivos/Usuarios", IMEI + ".txt")); bandera = !bandera;
                }
            })).Start();
        }

        public void OnRmsChanged(float rmsdB)
        {
            //throw new NotImplementedException();
        }
    }
}