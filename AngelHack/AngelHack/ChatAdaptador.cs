using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Sql;

namespace AngelHack
{
    public class ChatAdaptador : ArrayAdapter<ChatClase>
    {
        public override int GetItemViewType(int position) { return position % 10000; }

        public override int ViewTypeCount => 10000;

        public ChatAdaptador(Context context, int textViewResourceId, ChatClase[] objects) : base(context, textViewResourceId, objects) { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            int viewType = GetItemViewType(position);

            ViewHolder Holder = new ViewHolder();
            Holder.position = position;
            ChatClase Mensaje = GetItem(Holder.position);
            int Resources = 0;

            switch (Mensaje.EsMio)
            {
                case 1:
                    Resources = Resource.Layout.ChatDerVoz;
                    if (convertView == null)
                    {
                        convertView = inflater.Inflate(Resources, parent, false);
                        Holder.Icon = convertView.FindViewById<ImageView>(Resource.Id.ImageDerVoz);
                        Holder.Text = convertView.FindViewById<TextView>(Resource.Id.TextDerVoz);
                        Holder.Text.SetTypeface(Mensaje.GetLetra(), TypefaceStyle.Bold);
                        Date Date = new Date(new Java.IO.File(Mensaje.GetContenido()).LastModified());
                        Holder.Text.Text = Date.ToString();
                        convertView.Tag = Holder;
                    }
                    else { Holder = (ViewHolder)convertView.Tag; }
                    break;
                case 2:
                    Resources = Resource.Layout.ChatDerFoto;
                    if (convertView == null)
                    {
                        convertView = inflater.Inflate(Resources, parent, false);
                        Holder.Icon = convertView.FindViewById<ImageView>(Resource.Id.ImageDerFoto);
                        Java.IO.File Image = new Java.IO.File(Mensaje.GetContenido());
                        Bitmap myBitmap = BitmapFactory.DecodeFile(Image.AbsolutePath);
                        Holder.Icon.SetImageBitmap(myBitmap);
                        convertView.Tag = Holder;
                    }
                    else { Holder = (ViewHolder)convertView.Tag; }
                    break;
                case 3:
                    Resources = Resource.Layout.ChatIzqVoz;
                    if (convertView == null)
                    {
                        convertView = inflater.Inflate(Resources, parent, false);
                        Holder.Icon = convertView.FindViewById<ImageView>(Resource.Id.ImageIzqVoz);
                        Holder.Text = convertView.FindViewById<TextView>(Resource.Id.TextIzqVoz);
                        Holder.Text.SetTypeface(Mensaje.GetLetra(), TypefaceStyle.Bold);
                        Date Date = new Date(new Java.IO.File(Mensaje.GetContenido()).LastModified());
                        Holder.Text.Text = Date.ToString();
                        convertView.Tag = Holder;
                    }
                    else { Holder = (ViewHolder)convertView.Tag; }
                    break;
                case 4:
                    Resources = Resource.Layout.ChatIzqFoto;
                    if (convertView == null)
                    {
                        convertView = inflater.Inflate(Resources, parent, false);
                        Holder.Icon = convertView.FindViewById<ImageView>(Resource.Id.ImageIzqFoto);
                        Java.IO.File Image = new Java.IO.File(Mensaje.GetContenido());
                        Bitmap myBitmap = BitmapFactory.DecodeFile(Image.AbsolutePath);
                        Holder.Icon.SetImageBitmap(myBitmap);
                        convertView.Tag = Holder;
                    }
                    else { Holder = (ViewHolder)convertView.Tag; }
                    break;
                case 5:
                    Resources = Resource.Layout.ChatDerMensajes;
                    if (convertView == null)
                    {
                        convertView = inflater.Inflate(Resources, parent, false);
                        Holder.Text = convertView.FindViewById<TextView>(Resource.Id.TextDerMens);
                        Holder.Text.Text = Mensaje.GetContenido();
                        Holder.Text.SetTypeface(Mensaje.GetLetra(), TypefaceStyle.Bold);
                        convertView.Tag = Holder;
                    }
                    else { Holder = (ViewHolder)convertView.Tag; }
                    break;
                case 6:
                    Resources = Resource.Layout.ChatIzqMensajes;
                    if (convertView == null)
                    {
                        convertView = inflater.Inflate(Resources, parent, false);
                        Holder.Text = convertView.FindViewById<TextView>(Resource.Id.TextIzqMens);
                        Holder.Text.SetTypeface(Mensaje.GetLetra(), TypefaceStyle.Bold);
                        Holder.Text.Text = Mensaje.GetContenido();
                        convertView.Tag = Holder;
                    }
                    else { Holder = (ViewHolder)convertView.Tag; }
                    break;
            }

            return convertView;
        }
    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView Text;
        public ImageView Icon;
        public int position;
    }
}