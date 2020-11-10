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
        /// <summary>
        /// Canal zawiera listę par(nazwa użytkownika i wiadomość)
        /// </summary>
        private Dictionary<string, NetworkStream> canalUsers;
        string name;

        public Canal (string canalName)
        {
            name = canalName;
            canalUsers = new Dictionary<string, NetworkStream>();

        }
        public string Name
        {
            get => name;
            set => name = value;
        }
        public void addToCanal(string username, NetworkStream stream)
        {
                canalUsers.Add(username, stream);
        }
        public void stayInCanal(string username, byte[] buffer)
        {
            string text = "";
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
                        foreach (KeyValuePair<string, NetworkStream> canalUser in canalUsers)
                        {
                            if (canalUser.Key != username && text.Length != 0)
                            {
                                StreamControl.sendText(username + ": " + text + "\r\n", buffer, canalUser.Value);
                                canalUser.Value.Flush();
                            }
                        }
                    }
                }
                catch (IOException){ }

            }

        }
        public void removeFromCanal(string username)
        {
            canalUsers.Remove(username);
        }
        public Dictionary<string, NetworkStream> CanalUsers
        {
            get => canalUsers;
        }

    }
    public static class Canals
    {
        private static Dictionary<string, Canal> canals = new Dictionary<string, Canal>();
        //public static Mutex mutex = new Mutex();

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
