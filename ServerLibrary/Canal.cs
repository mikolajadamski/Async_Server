﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ServerLibrary
{
    public class Canal
    {
        string name;
        Dictionary<string, NetworkStream> canalUsers;
        Mutex mutex;
        public Canal(string canalName)
        {
            name = canalName;
            canalUsers = new Dictionary<string, NetworkStream>();
            mutex = new Mutex();
        }
        public string Name { get; set; }
        public Dictionary<string, NetworkStream> CanalUsers { get; }
        public void addToCanal(string username, NetworkStream stream, byte[] buffer)
        {
            mutex.WaitOne();
            canalUsers.Add(username, stream);
            sendCommand(buffer, prepareUpdateMsg());
            DataAccess.CanalHistory(stream, name, buffer);
            mutex.ReleaseMutex();
        }
        public void stayInCanal(string username, byte[] buffer)
        {
            string text;
            while (true)
            {
                try
                {
                    text = StreamControl.readText(canalUsers[username], buffer);
                    if (text == "//leave")
                    {
                        break;
                    }
                    else if (text.Length>=5 && text.Substring(0, 5) == "//add")
                    {
                        string[] data = text.Substring(5).Split();
                        DataAccess.addtoCanal(data[1], data[2]);
                    }
                    else if (text == "/r/n")
                        continue;
                    else
                    {
                        mutex.WaitOne();
                        if (text.Length != 0)
                        {
                            sendToOthers(username, buffer, text);
                        }
                        mutex.ReleaseMutex();
                    }
                }
                catch (IOException) { }

            }

        }

        private void sendToOthers(string username, byte[] buffer, string text)
        {
            foreach (KeyValuePair<string, NetworkStream> canalUser in canalUsers)
            {
                if (text.Length != 0)
                {
                    string time = DateTime.Now.ToString("h:mm:ss tt");
                    DataAccess.addMsg(text, time, username, name);
                    StreamControl.sendText("MSG " + username + "\t" + time + "\t" + text + " ENDMSG\r\n", buffer, canalUser.Value);
                    canalUser.Value.Flush();
                }
            }
        }

        private void sendCommand(byte[] buffer, string text)
        {
            foreach (KeyValuePair<string, NetworkStream> canalUser in canalUsers)
            {
                if (text.Length != 0)
                {
                    StreamControl.sendText( text, buffer, canalUser.Value);
                    canalUser.Value.Flush();
                }
            }
        }

        public void removeFromCanal(string username, byte[] buffer)
        {
            mutex.WaitOne();
            canalUsers.Remove(username);
            sendCommand( buffer, prepareUpdateMsg());
            mutex.ReleaseMutex();
        }

        private string prepareUpdateMsg()
        {
            string[] users = DataAccess.AdminCheck(canalUsers.Keys.ToArray(), name);
            return "UPDATE " + string.Join(" ", users);
        }

    }
}
