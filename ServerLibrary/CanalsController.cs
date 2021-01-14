using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ServerLibrary
{
    public static class CanalsController
    {
        private static Dictionary<string, Canal> canals = new Dictionary<string, Canal>();

        public static void initializeCanals()
        {
            string [] canalNames = DataAccess.selectOpenCanals();
            foreach(string canalName in canalNames)
            {
                canals.Add(canalName, new Canal(canalName));
            }

        }
        
        public static void joinCanal(string canalName, string username, NetworkStream stream, byte[] buffer)
        {
            canals[canalName].addToCanal(username, stream, buffer);
            canals[canalName].stayInCanal(username, buffer);
            canals[canalName].removeFromCanal(username, buffer);
        }
        
        public static UTF8Encoding encoder = new UTF8Encoding();

        public static string addCanal(string canalName)
        {
            try
            {
                canals.Add(canalName, new Canal(canalName));
                return "Utworzono";
            }
            catch(ArgumentException)
            {
                return "Już istnieje";
            }
        }



    }
}
