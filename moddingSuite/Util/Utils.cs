﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using moddingSuite.BL;

namespace moddingSuite.Util
{
    public static class Utils
    {
        public static string Int32ToBigEndianHexByteString(Int32 i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            string format = BitConverter.IsLittleEndian
                ? "{0:X2} {1:X2} {2:X2} {3:X2}"
                : "{3:X2} {2:X2} {1:X2} {0:X2}";
            return String.Format(format, bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static string ByteArrayToBigEndianHexByteString(byte[] data)
        {
            if (data == null)
                return string.Empty;

            var stringBuilderb = new StringBuilder();

            stringBuilderb.Append(string.Empty);

            foreach (var b in data)
                stringBuilderb.Append(string.Format("{0:X2}", b));

            return stringBuilderb.ToString();
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            var arr = new byte[hex.Length >> 1];

            for (int i = 0; i < (hex.Length >> 1); ++i)
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static void SaveDebug(string fileName, byte[] contentData)
        {
            var path = SettingsManager.Load().SavePath;
            var file = Path.Combine(path, string.Format("{0}_{1}", DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ff"), fileName));

            if (!File.Exists(file))
                using (File.Create(file))
                { }

            using (var fs = new FileStream(file, FileMode.Truncate))
                fs.Write(contentData, 0, contentData.Length);
        }

        public static string GenerateCoupon(int length, Random random)
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                result.Append(characters[random.Next(characters.Length)]);

            return result.ToString();
        }

        public static byte[] CreateLocalisationHash(string text, int maxSize = 8)
        {
            long hash = 0;
            for (int i = 0; i < maxSize; ++i)
            {
                int value;
                ushort chr = text[i];

                if (chr == 0)
                    break;

                if ('0' <= chr && chr <= '9')
                    value = 1 + chr - '0';
                else if ('A' <= chr && chr <= 'Z')
                    value = 2 + '9' - '0' + chr - 'A';
                else if (chr == '_')
                    value = 3 + '9' - '0' + 'Z' - 'A';
                else if ('a' <= chr && chr <= 'z')
                    value = 4 + '9' - '0' + 'Z' - 'A' + chr - 'a';
                else
                    throw new InvalidDataException("");

                hash = (hash << 6) | value;
            }

            return BitConverter.GetBytes(hash);
        }
    }
}