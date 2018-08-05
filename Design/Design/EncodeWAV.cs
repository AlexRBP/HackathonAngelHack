using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Design
{
    public class EncodeWAV
    {
        public static int RECORDER_BPP = 16;

        public static byte[] WriteWAV(byte[] Salida, ulong totalAudioLen, long totalDataLen, long longSampleRate, int channels, long byteRate)
        {
            byte[] header = new byte[44];

            header[0] = (byte)'R';
            header[1] = (byte)'I';
            header[2] = (byte)'F';
            header[3] = (byte)'F';
            header[4] = (byte)(totalDataLen & 0xff);
            header[5] = (byte)((totalDataLen >> 8) & 0xff);
            header[6] = (byte)((totalDataLen >> 16) & 0xff);
            header[7] = (byte)((totalDataLen >> 24) & 0xff);
            header[8] = (byte)'W';
            header[9] = (byte)'A';
            header[10] = (byte)'V';
            header[11] = (byte)'E';
            header[12] = (byte)'f';
            header[13] = (byte)'m';
            header[14] = (byte)'t';
            header[15] = (byte)' ';
            header[16] = 16;
            header[17] = 0;
            header[18] = 0;
            header[19] = 0;
            header[20] = 1;
            header[21] = 0;
            header[22] = (byte)channels;
            header[23] = 0;
            header[24] = (byte)(longSampleRate & 0xff);
            header[25] = (byte)((longSampleRate >> 8) & 0xff);
            header[26] = (byte)((longSampleRate >> 16) & 0xff);
            header[27] = (byte)((longSampleRate >> 24) & 0xff);
            header[28] = (byte)(byteRate & 0xff);
            header[29] = (byte)((byteRate >> 8) & 0xff);
            header[30] = (byte)((byteRate >> 16) & 0xff);
            header[31] = (byte)((byteRate >> 24) & 0xff);
            header[32] = (2 * 16 / 8);
            header[33] = 0;
            header[34] = (byte)RECORDER_BPP;
            header[35] = 0;
            header[36] = (byte)'d';
            header[37] = (byte)'a';
            header[38] = (byte)'t';
            header[39] = (byte)'a';
            header[40] = (byte)(totalAudioLen & 0xff);
            header[41] = (byte)((totalAudioLen >> 8) & 0xff);
            header[42] = (byte)((totalAudioLen >> 16) & 0xff);
            header[43] = (byte)((totalAudioLen >> 24) & 0xff);

            byte[] Output = new byte[header.Length + Salida.Length];
            for (int i = 0; i < header.Length + Salida.Length; i++)
            {
                if (i < 44) { Output[i] = header[i]; }
                else { Output[i] = Salida[i - 44]; }
            }

            return Output;
        }
    }
}