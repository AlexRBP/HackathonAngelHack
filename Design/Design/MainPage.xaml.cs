using Firebase.Storage;
using FireSharp;
using FireSharp.Config;
using FireSharp.EventStreaming;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Design
{
    public sealed partial class MainPage : Page
    {
        int[] PinEntrada = new int[] { 4 };
        int[] PinSalida = new int[] { 2, 3, 17 };
        Dictionary<int, GpioPin> PinesEntrada = new Dictionary<int, GpioPin>();
        Dictionary<int, GpioPin> PinesSalida = new Dictionary<int, GpioPin>();
        MediaCapture mediaCapture;
        List<string> ListaIMEI = new List<string>();
        bool PresionoBoton = false;
        MediaPlayer mediaPlayer;

        public MainPage()
        {
            this.InitializeComponent();
            PuertaMoviendose(false);
            InicioVideo();
            InicioPines();
            InicioAudio();
        }

        public void InicioPines()
        {
            GpioController Gpio = GpioController.GetDefault();

            //Entrada
            if (Gpio == null) { return; }
            int Count = 0;
            foreach (var valor in PinEntrada) { PinesEntrada.Add(Count, Gpio.OpenPin(PinEntrada[Count])); Count++; }
            foreach (var valor in PinesEntrada)
            {
                valor.Value.Write(GpioPinValue.Low);
                valor.Value.SetDriveMode(GpioPinDriveMode.Input);
                valor.Value.ValueChanged += async (s, e) => 
                {
                    if (e.Edge == GpioPinEdge.FallingEdge) 
                    {
                        string Nombre = "Foto_" + System.Guid.NewGuid() + ".png";
                        var LinkFoto = await TomarFoto(Nombre);

                        foreach (var Item in ListaIMEI)
                        {
                            await SubirNube<Object>(JsonConvert.DeserializeObject("{\"" + "Chat" + "\":\"" + LinkFoto + "|" + Nombre + "|4" + "\"}"), "Datos/" + Item + "/Descargas/ChatBot/");
                        }
                        Debug.WriteLine($"Se ha tomado una foto"); PresionoBoton = true;
                    }
                };
            }
            //Salida
            Count = 0;
            foreach (var valor in PinSalida) { PinesSalida.Add(Count, Gpio.OpenPin(PinSalida[Count])); Count++; }
            foreach (var valor in PinesSalida)
            {
                valor.Value.Write(GpioPinValue.Low);
                valor.Value.SetDriveMode(GpioPinDriveMode.Output);
            }
            PinesSalida[2].Write(GpioPinValue.High);
        }

        public async void InicioVideo()
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

            previewElement.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();

            RecepcionDatos();
            UsuariosDinamicos();
        }

        private async Task<string> TomarFoto(string Nombre)
        {
            try
            {
                var photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(Nombre, CreationCollisionOption.ReplaceExisting);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreatePng();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

                IRandomAccessStream photoStream = await photoFile.OpenReadAsync();

                var task = new FirebaseStorage("webserverplc.appspot.com").Child(Nombre).PutAsync(photoStream.AsStream());
                task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");
                var Url = await task;
                return task.TargetUrl;
            }
            catch { return ""; }
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

        private async void PuertaMoviendose(bool Puede)
        {
            if (Puede)
            {
                PinesSalida[0].Write(GpioPinValue.High); //Abrir
                PinesSalida[1].Write(GpioPinValue.Low);
                await Task.Delay(4000);
                PinesSalida[0].Write(GpioPinValue.Low); //Cerrar
                PinesSalida[1].Write(GpioPinValue.High);
                await Task.Delay(4000);
                PinesSalida[0].Write(GpioPinValue.Low); //Apagado
                PinesSalida[1].Write(GpioPinValue.Low);
            }

            await EliminarNube<Object>("Datos/" + "ChatBot" + "/Descargas");
        }

        public async void UsuariosDinamicos()
        {
            int NumUsuario = 1;
            while (true)
            {
                string IMEI = await LeerNube("Tipo/" + NumUsuario.ToString());
                if (IMEI == "null") break;
                else { ListaIMEI.Add(IMEI); }
                NumUsuario++;
            }
        }

        public async void RecepcionDatos()
        {
            await NubeOnline("Datos/" + "ChatBot" + "/Descargas", async (sender, args, ctxt) =>
            {
                switch (args.Data.Split('|')[2])
                {
                    case "6": //Mensajes de Texto
                        if (args.Data.Split('|')[0] == "Si" || args.Data.Split('|')[0] == "si" && PresionoBoton)
                        {
                            Debug.WriteLine($"Respuesta " + args.Data);
                            PuertaMoviendose(true); PresionoBoton = !PresionoBoton;

                            StorageFile storageFile = await KnownFolders.PicturesLibrary.GetFileAsync("Falla.mp3");
                            mediaPlayer.Source = MediaSource.CreateFromStorageFile(storageFile);
                            mediaPlayer.Play();

                            await EliminarNube<Object>("Datos/" + "ChatBot" + "/Descargas");
                        }
                        else if (args.Data.Split('|')[0] == "No" || args.Data.Split('|')[0] == "no" && PresionoBoton)
                        {
                            Debug.WriteLine($"Respuesta " + args.Data); PresionoBoton = !PresionoBoton;

                            StorageFile storageFile = await KnownFolders.PicturesLibrary.GetFileAsync("Correcto.mp3");
                            mediaPlayer.Source = MediaSource.CreateFromStorageFile(storageFile);
                            mediaPlayer.Play();

                            await EliminarNube<Object>("Datos/" + "ChatBot" + "/Descargas");
                        }
                        else { await EliminarNube<Object>("Datos/" + "ChatBot" + "/Descargas"); PresionoBoton = false; }
                        break;
                    case "3": //Mensaje de Voz
                        string Link = args.Data.Split('|')[0];
                        string Nombre = args.Data.Split('|')[1];
                        try
                        {
                            //Descargar Audio
                            Uri Source = new Uri(await LinkDescarga(Link, Nombre));
                            Debug.WriteLine($"Respuesta " + Source.AbsolutePath);
                            StorageFile Files = await KnownFolders.PicturesLibrary.CreateFileAsync(Nombre, CreationCollisionOption.GenerateUniqueName);

                            BackgroundDownloader Downloader = new BackgroundDownloader();
                            DownloadOperation Download = Downloader.CreateDownload(Source, Files);
                            await Download.StartAsync();

                            //Reproducir Audio
                            IRandomAccessStream AudioStream = await Files.OpenReadAsync();
                            var buffer = new byte[AudioStream.Size];
                            var reader = new DataReader(AudioStream);
                            await reader.LoadAsync((uint)buffer.Length);

                            reader.ReadBytes(buffer);
                            var Decode = DecodeA(buffer, 0, buffer.Length);

                            StorageFile sampleFile = await KnownFolders.PicturesLibrary.CreateFileAsync(Nombre.Split('.')[0] + ".wav", CreationCollisionOption.GenerateUniqueName);
                            IRandomAccessStream Audio = await Files.OpenReadAsync();
                            await FileIO.WriteBytesAsync(sampleFile, EncodeWAV.WriteWAV(Decode, (ulong)(Decode.Length), (long)(Decode.Length + 36), 8000, 1, 16 * 8000 * 1 / 8));

                            StorageFile storageFile = await KnownFolders.PicturesLibrary.GetFileAsync(Nombre.Split('.')[0] + ".wav");
                            mediaPlayer.Source = MediaSource.CreateFromStorageFile(storageFile);
                            mediaPlayer.Play();

                            await EliminarNube<Object>("Datos/" + "ChatBot" + "/Descargas"); PresionoBoton = false;
                        }
                        catch { Debug.WriteLine($"Error Audio"); }
                        break;
                    default:
                        await EliminarNube<Object>("Datos/" + "ChatBot" + "/Descargas"); PresionoBoton = false;
                        break;
                }
            });

            await NubeOnline("Tipo", (sender, args, ctxt) =>
            {
                if (!ListaIMEI.Contains(args.Data)) { ListaIMEI.Add(args.Data); }
            });
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

        public async Task<string> LinkDescarga(string Link, string Nombre)
        {
            var Recep1 = await MakeWebRequest(Link);
            var Recep2 = Recep1.Split(new string[] { "downloadTokens" }, StringSplitOptions.None)[1];
            var Token = Recep2.Split('"')[2];

            return "https://firebasestorage.googleapis.com/v0/b/webserverplc.appspot.com/o/" + Nombre + "?alt=media&token=" + Token;
        }

        public async Task<string> MakeWebRequest(string url)
        {
            HttpClient http = new System.Net.Http.HttpClient();
            HttpResponseMessage response = await http.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
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

        public async void InicioAudio()
        {
            mediaPlayer = new Windows.Media.Playback.MediaPlayer();
            ObservableCollection<DeviceInformation> renderDeviceList = new ObservableCollection<DeviceInformation>();

            var devices = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            if (devices.Count > 0)
            {
                for (var i = 0; i < devices.Count; i++)
                {
                    renderDeviceList.Add(devices[i]);
                    Debug.WriteLine(devices[i].Name);
                }
            }
            mediaPlayer.AudioDevice = renderDeviceList[0];
        }
    }

    [Serializable()]
    public class Database
    {
        public string Link { get; set; }
    }
}
