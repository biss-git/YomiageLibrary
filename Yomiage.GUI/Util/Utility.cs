using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Util
{
    public static class Utility
    {
        /// <summary>
        /// 特定のファイルを一定の深さまで探索する。
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="searchDepth"></param>
        /// <param name="files"></param>
        public static void SearchFile(string directory, string fileName, int searchDepth, List<string> files)
        {
            if(string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) { return; }
            foreach (var f in Directory.GetFiles(directory))
            {
                if (Path.GetFileName(f) == fileName)
                {
                    files.Add(f);
                }
            }
            if (searchDepth <= 0) { return; }
            foreach (var d in Directory.GetDirectories(directory))
            {
                SearchFile(d, fileName, searchDepth - 1, files);
            }
        }

        private static readonly double pow2_7 = Math.Pow(2, 7);
        private static readonly double pow2_15 = Math.Pow(2, 15);
        private static readonly double pow2_31 = Math.Pow(2, 31);

        /// <summary>
        /// Double から Byte の配列に変換する。
        /// </summary>
        /// <param name="Dbuffer"></param>
        /// <param name="bitPerSample"></param>
        /// <returns></returns>
        public static byte[] DoubleToByte(double[] Dbuffer, int bitPerSample)
        {
            if(Dbuffer == null) { return new byte[0]; }
            byte[] Bbuffer;
            byte[] temp;
            switch (bitPerSample)
            {
                case 8:
                    {
                        Bbuffer = new byte[Dbuffer.Length];
                        for (int i = 0; i < Dbuffer.Length; i++)
                        {
                            temp = BitConverter.GetBytes((byte)(Dbuffer[i] * pow2_7) + 128);  // byteデータに変換
                            Bbuffer[i] = temp[0];
                        }
                        return Bbuffer;
                    }
                case 16:
                    {
                        Bbuffer = new byte[2 * Dbuffer.Length];
                        for (int i = 0; i < Dbuffer.Length; i++)
                        {
                            temp = BitConverter.GetBytes((Int16)(Dbuffer[i] * pow2_15));  // byteデータに変換
                            Array.Copy(temp, 0, Bbuffer, 2 * i, 2);
                        }
                        return Bbuffer;
                    }
                case 24:
                    {
                        Bbuffer = new byte[3 * Dbuffer.Length];
                        for (int i = 0; i < Dbuffer.Length; i++)
                        {
                            temp = BitConverter.GetBytes((Int32)(Dbuffer[i] * pow2_31));  // byteデータに変換
                            Array.Copy(temp, 0, Bbuffer, 3 * i, 3);
                        }
                        return Bbuffer;
                    }
                default: // 32のはず
                    {
                        Bbuffer = new byte[4 * Dbuffer.Length];
                        for (int i = 0; i < Dbuffer.Length; i++)
                        {
                            temp = BitConverter.GetBytes((Int32)(Dbuffer[i] * pow2_31));  // byteデータに変換
                            Array.Copy(temp, 0, Bbuffer, 4 * i, 4);
                        }
                        return Bbuffer;
                    }
            }
        }

    }
}
