using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class Canal
    {
        /// <summary>
        /// Canal zawiera listę par(nazwa użytkownika i wiadomość)
        /// </summary>
        Dictionary<string, NetworkStream> canalUsers;
        string name;
        int userCount;
        int maxUserCount;

        public Canal (string canalName)
        {
            name = canalName;
            canalUsers = new Dictionary<string, NetworkStream>();
            userCount = 0;
            maxUserCount = 10;
        }
        public string Name
        {
            get => name;
            set => name = value;
        }
        public void addToCanal(string username, NetworkStream stream)
        {
            if (userCount < maxUserCount)
            {
                canalUsers.Add(username, stream);
                userCount++;
            }
        }
        public void stayInCanal(string username, byte[] buffer)
        {
            while (true)
            {
                try
                {
                    string text = StreamControl.readText(canalUsers[username], buffer);
                    if (text == "//leave") break;
                    foreach(KeyValuePair<string, NetworkStream> canalUser in canalUsers)
                    {
                        if (canalUser.Key != username && text.Length != 0)
                        {
                            StreamControl.sendText(username + ": " + text + "\r\n", buffer, canalUser.Value);
                        }
                    }

                }
                catch (IOException){ }

            }

        }
        public void removeFromCanal(string username)
        {
            canalUsers.Remove(username);
            userCount--;
        }
        public Dictionary<string, NetworkStream> CanalUsers
        {
            get => canalUsers;
        }

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

    }
}
