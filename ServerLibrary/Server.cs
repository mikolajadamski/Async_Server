using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerLibrary

{
    public abstract class Server

    {
        IPAddress ipAddress;
        int port;
        protected int bufferSize = 1024;
        protected bool running;
        protected TcpListener listener;
        protected TcpClient client;
        protected NetworkStream stream;


        public IPAddress IPAddress { get => ipAddress; set { if (!running) ipAddress = value; else throw new Exception("nie można zmienić adresu IP kiedy serwer jest uruchomiony"); } }

        public int Port
        {
            get => port; set
            {
                int tmp = port;
                if (!running) port = value; else throw new Exception("nie można zmienić portu kiedy serwer jest uruchomiony");
                if (!checkPort())
                {
                    port = tmp;
                    throw new Exception("błędna wartość portu");
                }
            }
        }
        public int BufferSize
        {
            get => bufferSize; set
            {
                if (value < 0 || value > 1024 * 1024 * 64) throw new Exception("błędny rozmiar pakietu");
                if (!running) bufferSize = value; else throw new Exception("nie można zmienić rozmiaru pakietu kiedy serwer jest uruchomiony");
            }
        }
        protected TcpListener TcpListener { get => listener; set => listener = value; }
        protected TcpClient TcpClient { get => client; set => client = value; }
        protected NetworkStream Stream { get => stream; set => stream = value; }
        public Server(IPAddress IP, int port)
        {
            running = false;
            IPAddress = IP;
            Port = port;
            if (!checkPort())
            {
                Port = 8000;
                throw new Exception("błędna wartość portu, ustawiam port na 8000");
            }
        }
        protected bool checkPort()
        {
            if (port < 1024 || port > 49151) return false;
            return true;
        }
        protected void StartListening()
        {
            TcpListener = new TcpListener(IPAddress, Port);
            TcpListener.Start();
        }
        protected abstract void AcceptClient();
        protected abstract void BeginDataTransmission(NetworkStream stream);
        public abstract void Start();
    }

}