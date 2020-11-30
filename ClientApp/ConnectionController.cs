using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class ConnectionController
    {
        Client client;
        byte[] buffer;
        private UTF8Encoding encoder;
        public ConnectionController()
        { }
        public bool initializeConnection()
        {
            try
            {
                client = new Client("127.0.0.1", 3000);
                buffer = new byte[1024];
                encoder = new UTF8Encoding();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
        public void setUsername(string username)
        {
            client.Username = username;
        }
        public string login(string username, string password)
        {
            sendText("login");
            string response = readText();
            if (response != "OK")
            {
                return "Błąd";
            }
            sendText(username);
            response = readText();
            if (response == "ERR_SIZE")
            {
                return "Nieprawidłowa długość nazwy użytkownika";
            }
            sendText(password);
            response = readText();
            if (response == "ERR_SIZE")
            {
                return "Nieprawidłowa długość nazwy użytkownika";
            }
            else if (response == "ERR_AUTH")
            {
                return "Nieprawidłowe dane";
            }
            return response;
        }

        internal string register(string username, string password)
        {
            sendText("register");
            string response = readText();
            if (response != "OK")
            {
                return "Błąd";
            }
            sendText(username);
            response = readText();
            if (response == "ERR_SIZE")
            {
                return "Nieprawidłowa długość nazwy użytkownika";
            }
            sendText(password);
            response = readText();
            if (response == "ERR_SIZE")
            {
                return "Nieprawidłowa długość nazwy użytkownika";
            }
            else if (response == "ERR_EXISTS")
            {
                return "Nazwa użytkownika zajęta";
            }
            return response;
        }

        private void sendText(string str)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(str);
            int length = encodedText.Length;
            Array.Copy(encodedText, buffer, length);
            client.Stream.Write(buffer, 0, length);
        }
        private string readText()
        {
            int message_size = client.Stream.Read(buffer, 0, buffer.Length);
            return encoder.GetString(buffer, 0, message_size);
        }


        public bool IsLogged
        {
            get => client.IsLogged;
            set => client.IsLogged = true;
        }
    }
}
