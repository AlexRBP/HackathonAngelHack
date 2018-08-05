using Android.Graphics;
using Java.Util;

namespace AngelHack
{
    public class ChatClase
    {
        public string Contenido;
        public int EsMio;
        public string Id;
        public Typeface Letra;

        public ChatClase(string Contenido, int EsMio)
        {
            this.Contenido = Contenido;
            this.EsMio = EsMio;
            Id = UUID.RandomUUID().ToString();
        }

        public string getId() { return Id; }

        public void setId(string Id) { this.Id = Id; }

        public string GetContenido() { return Contenido; }

        public int GetEsMio() { return EsMio; }

        public void SetLetra(Typeface Letra) { this.Letra = Letra; }

        public Typeface GetLetra() { return Letra; }
    }
}