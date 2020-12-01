using System;
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
                    if (text == "//leave") break;
                    else if (text == "/r/n")
                        continue;
                    else
                    {
                        mutex.WaitOne();
                        foreach (KeyValuePair<string, NetworkStream> canalUser in canalUsers)
                        {
                            if (canalUser.Key != username && text.Length != 0)
                            {
                                StreamControl.sendText(username + ": " + text + "\r\n", buffer, canalUser.Value);
                                DataAccess.addMsg(text, username, name);

                                canalUser.Value.Flush();
                            }
                            else if(text.Length != 0){
                                  DataAccess.addMsg(text, username, name);
                                }
                        }
                        mutex.ReleaseMutex();
                    }
                }
                catch (IOException) { }

            }

        }
        public void removeFromCanal(string username)
        {
            mutex.WaitOne();
            canalUsers.Remove(username);
            mutex.ReleaseMutex();
        }

    }
}
