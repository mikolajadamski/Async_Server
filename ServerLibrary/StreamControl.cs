﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    static class StreamControl
    {
        private static UTF8Encoding encoder = new UTF8Encoding();
        public static void sendText(string str, byte[] buffer, NetworkStream stream)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(str);
            int length = encodedText.Length;
            Array.Copy(encodedText, buffer, length);
            stream.Write(buffer, 0, length);
        }
        public static string readText(NetworkStream stream, byte[] buffer)
        {
            int message_size = stream.Read(buffer, 0, buffer.Length);
            stream.ReadByte();
            stream.ReadByte();
            return encoder.GetString(buffer, 0, message_size);
        }
    }
}