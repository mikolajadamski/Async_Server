using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerLibrary
{

    public class ServerAsync
    {
        /// <summary>
        /// Serwer asynchroniczny
        /// </summary>
        IPAddress iPAddress;
        int port;
        int buffer_size = 1024;
        bool running;
        string firstmsg = "Wpisz register lub login aby zacząć lub exit:";
        TcpListener tcpListener;
        private delegate void TransmissionDataDelegate(NetworkStream stream);
        public ServerAsync(IPAddress IP, int port)
        {
            this.running = false;
            this.iPAddress = IP;
            if (port > 1024 && port < 49151) this.port = port;
            else this.port = 1024;
        }
        /// <summary>
        /// Akceptuje klienta
        /// </summary>
        private void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                NetworkStream stream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(stream, TransmissionCallback, tcpClient);
            }

        }
        /// <summary>
        /// rozpoczyna pracę serwera
        /// </summary>
        public void Start()
        {
            running = true;
            StartListening();
            AcceptClient();
        }
        /// <summary>
        /// Rozpoczyna nasłuchiwanie
        /// </summary>
        private void StartListening()
        {
            tcpListener = new TcpListener(iPAddress, port);
            tcpListener.Start();
        }
        /// <summary>
        /// Zakańcza pozwolenie
        /// </summary>
        /// <param name="ar"></param>
        private void TransmissionCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            client.Close();
        }
        /// <summary>
        /// Konwertuje bufor bajtów na łańcuch znaków
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns>Zwraca łańcuch znaków</returns>
        string ReadString(NetworkStream stream, byte[]buffer)
        {
            int message_size = stream.Read(buffer, 0, buffer_size);
            stream.ReadByte();
            stream.ReadByte();
            return new ASCIIEncoding().GetString(buffer, 0, message_size);
        }
        /// <summary>
        /// Obsługuje dialog z klientem
        /// </summary>
        /// <param name="stream"></param>
        public void BeginDataTransmission(NetworkStream stream)
        {
            stream.ReadTimeout = 60000;
            byte[] buffer = new byte[buffer_size];
            UserService usrService = new UserService();
            while (!usrService.IsLogged)
            {
                try
                {
                    stream.Write(Encoding.ASCII.GetBytes(firstmsg), 0, firstmsg.Length);
                    string str = ReadString(stream, buffer);
                    int op = usrService.parseOperation(str);
                    if(op>0)
                    {
                        sendString("nazwa użytkownika(8-25 znaków):", buffer, stream);
                        string username = ReadString(stream, buffer);
                        if (username.Length<8 || username.Length > 25)
                        {
                            sendString("Zły rozmiar nazwy użytkownika\r\n", buffer, stream);
                            continue;
                        }
                        sendString("hasło(8-25 znaków):", buffer, stream);
                        string password = ReadString(stream, buffer);
                        if (username.Length < 8 || username.Length > 25)
                        {
                            sendString("Zły rozmiar hasła\r\n", buffer, stream);
                            continue;
                        }
                        if(op == 1)
                        {
                            sendString(usrService.login(username, password), buffer,stream);
                        }
                        else if(op == 2)
                        {
                            sendString(usrService.register(username, password),buffer, stream);
                        }
                        else
                        {
                            sendString("Błąd\r\n", buffer, stream);
                        }
                    }
                    else if(op == 0)
                    {
                        break;
                    }
                    else
                    {
                        sendString("Nieprawidłowa operacja\r\n", buffer, stream);
                    }
                }
                catch (IOException e)
                {
                   // e.Message;
                    break;
                }
            }
            while(usrService.IsLogged)
            {
                try {
                    sendString("Wpisz exit aby wyjść lub delete aby usunąć konto\r\n", buffer, stream);
                    string str = ReadString(stream, buffer);
                    if (str.ToLower() == "exit") break;
                    else if(str.ToLower() == "delete")
                    {
                        sendString(usrService.deleteUser(), buffer, stream);
                        sendString("Wpisz cokolwiek aby wyjść.\r\n", buffer, stream);
                        ReadString(stream, buffer);
                        break;
                    }
                    else sendString(str, buffer, stream);
                }
                catch (IOException e)
                {
                    // e.Message;
                    break;
                }
            }
        }
        /// <summary>
        /// wysyła łańcuch znaków do klienta w postaci tablicy bajtów
        /// </summary>
        /// <param name="str">łańcuch znaków do wysłania</param>
        /// <param name="buffer">bufor wiadomości</param>
        /// <param name="stream"></param>
        private void sendString(string str,byte[] buffer, NetworkStream stream)
        {
            buffer = Encoding.ASCII.GetBytes(str);
            stream.Write(buffer, 0, str.Length);
        }
    }
}
