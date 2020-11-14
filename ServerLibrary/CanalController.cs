using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class Canal
    {
        string name;
        Dictionary<string, NetworkStream> canalUsers;
        Mutex mutex;
        public Canal (string canalName)
        {
            name = canalName;
            canalUsers = new Dictionary<string, NetworkStream>();
            mutex = new Mutex();
        }
        public string Name { get; set; }
        public void addToCanal(string username, NetworkStream stream)
        {
            mutex.WaitOne();
            canalUsers.Add(username, stream);
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
                                canalUser.Value.Flush();
                            }
                        }
                        mutex.ReleaseMutex();
                    }
                }
                catch (IOException){ }

            }

        }
        public void removeFromCanal(string username)
        {
            mutex.WaitOne();
            canalUsers.Remove(username);
            mutex.ReleaseMutex();
        }
        public Dictionary<string, NetworkStream> CanalUsers { get; }

    }
    public static class Canals
    {
        private static Dictionary<string, Canal> canals = new Dictionary<string, Canal>();

        public static void initializeCanals()
        {
            string [] canalNames = UserDataAccess.selectOpenCanals();
            foreach(string canalName in canalNames)
            {
                canals.Add(canalName, new Canal(canalName));
            }

        }
        public static void joinCanal(string canalName, string username, NetworkStream stream, byte[] buffer)
        {
            canals[canalName].addToCanal(username, stream);
            canals[canalName].stayInCanal(username, buffer);
            canals[canalName].removeFromCanal(username);
        }
        public static UTF8Encoding encoder = new UTF8Encoding();

        public static string add(string canalName)
        {
            try
            {
                canals.Add(canalName, new Canal(canalName));
                return "okejka";
            }
            catch(ArgumentException)
            {
                return "Już istnieje";
            }
        }
    }
}
