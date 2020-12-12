using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    class Client
    {
        TcpClient client;
        NetworkStream stream;
        string username;
        bool isLogged;

        public Client(string serverIp, int port)
        {
            try
            {
                client = new TcpClient(serverIp, port);
                stream = client.GetStream();
                isLogged = false;
            }
            catch (SocketException e)
            {
                throw e;
            }
        }
        public string Username
        {
            get => username;
            set => username = value;
        }
        public NetworkStream Stream
        {
            get => stream;
        }
        public bool IsLogged
        {
            get => isLogged;
            set => isLogged = value;
        }
        public void setTimeout(int milis)
        {
            stream.ReadTimeout = 1000;
        }
        public void disconnect()
        {
            client.Close();
        }
    }
}
