using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class Canal
    {
        List<KeyValuePair<string, NetworkStream>> canalUsers;
        string name;

        public Canal (string canalName)
        {
            name = canalName;
        }
        public string Name
        {
            get => name;
            set => name = value;
        }
        public void addToCanal(string username, NetworkStream stream)
        {
            canalUsers.Add(new KeyValuePair<string, NetworkStream>(username, stream));
        }
        public void sendToUsers(string username, string message)
        {
            byte[] buffer = new byte[1024];
            foreach (KeyValuePair<string, NetworkStream> userAndStream in canalUsers)
            {
                if (username != userAndStream.Key)
                {
                    StreamControl.sendText(username + ": " + message, buffer, userAndStream.Value);
                }
            }
        }
        public void leaveCanal(string username)
        {
            for (int i = 0; i < canalUsers.Count; i++)
            {
                if (username == canalUsers.ElementAt(i).Key)
                {
                    canalUsers.RemoveAt(i);
                    break;
                }
            }
        }
    }
    public static class Canals
    {
        public static List<Canal> canals;

        public static void addToCanal(string canalName, string userName, NetworkStream stream)
        {
            canals.ElementAt(findCanalIndex(canalName)).addToCanal(userName, stream);
        }
        public static int findCanalIndex(string canalName)
        {
            for(int i = 0; i<canals.Count; i++)
            {
                if (canals.ElementAt(i).Name == canalName) return i;
            }
            return -1;
        }

    }
    class CanalController
    {

    }
}
