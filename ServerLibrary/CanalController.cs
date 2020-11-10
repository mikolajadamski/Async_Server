using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class Canal
    {
        Dictionary<string, NetworkStream> canalUsers;
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
        public void sendToUsers(string username, string message)
        {

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

        public static void initializeCanals()
        {
            string [] canalNames = UserDataAccess.selectOpenCanals();
            foreach(string canalName in canalNames)
            {
                canals.Add(canalName, new Canal(canalName));
            }

        }

        public static void addToCanal(string canalName, string username, NetworkStream stream)
        {
            canals[canalName].addToCanal(username, stream);
        }

        public static void removeFromCanal(string canalName, string username)
        {
            canals[canalName].removeFromCanal(username);
        }
        private static UTF8Encoding encoder = new UTF8Encoding();
        public static void canalCommunication(User user, NetworkStream stream)
        {
            string message = "";
            byte[] buffer = new byte[1024];
            while (true)
            {
                int message_size = 0;
                stream.ReadTimeout = 300;
                try
                {
                    message_size = stream.Read(buffer, 0, buffer.Length);
                    stream.ReadByte();
                    stream.ReadByte();
                    message = encoder.GetString(buffer, 0, message_size);
                    if (message == "//leave") { stream.ReadTimeout = 3600000; break; }
                    if (message_size != 0)
                        canals[user.CurrentCanal].setMessage(user.Name, message);
                }
                catch (IOException e) { }

                foreach (KeyValuePair<string, string> canalUser in canals[user.CurrentCanal].CanalUsers)
                {
                    if (canalUser.Key != user.Name && canalUser.Value != null)
                    {
                        StreamControl.sendText(canalUser.Value, buffer, stream);
                        try
                        {
                            message_size = stream.Read(buffer, 0, buffer.Length);
                        }
                        catch (IOException e) { }
                        if (message_size != 0)
                        {
                            stream.ReadByte();
                            stream.ReadByte();
                        }
                        canals[user.CurrentCanal].setMessage(canalUser.Key, null);
                    }
                }
            }


        }

    }
}
