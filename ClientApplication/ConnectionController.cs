using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    public class ConnectionController
    {
        Client client;
        byte[] buffer;
        private UTF8Encoding encoder;
        public ConnectionController()
        {}

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

        internal void setTimeout(int milis)
        {
            client.setTimeout(milis);
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

        internal string Username
        {
            get{ return client.Username; }
        }

        public void createCanal(string canalName)
        {
            sendText("create " + canalName);
        }

        public void deleteCanal(string canalName)
        {
            sendText("delete " + canalName);
        }

        public void switchToCanal(string canalName)
        {
            sendText("switchto " + canalName);
        }

        public void getListOfUsers(string canalName)
        {
            sendText("listofusers" + canalName);
        }

        public void leaveCanal()
        {
            sendText("//leave");
        }

        public void joinCanal(string canalName)
        {
            sendText("join " + canalName);
        }

        public void getCanals()
        {
            sendText("list " + Username);
            /*
             * string response = readText();
             * if (response == "OK")
             * {
             * return "Błąd";
             * }
             * return response;
            */
        }

        public void addUserToCanal(string canalName, string userName)
        {
            string mess = "//add " + canalName + " " + userName;
            sendText(mess);
            
        }

        public void removeUserFromCanal(string canalName, string userName)
        {
            string mess = "//remove " + canalName + " " + userName;
            sendText(mess);
        }

        public void makeAdminUser(string canalName, string userName)
        {
            string mess = "//mkadmin " + canalName + " " + userName;
            sendText(mess);
        }

        public void takeAdminUser(string canalName, string userName)
        {
            string mess = "//tkadmin " + canalName + " " + userName;
            sendText(mess);
        }

        public void changePassword(string oldPassword, string newPassword)
        {
            string mess = "changepassword " + oldPassword + " " + newPassword;
            sendText(mess);
        }

        public void sendText(string str)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(str);
            int length = encodedText.Length;
            Array.Copy(encodedText, buffer, length);
            client.Stream.Write(buffer, 0, length);
        }
        
        public string readText()
        {
            int message_size = client.Stream.Read(buffer, 0, buffer.Length);
            return encoder.GetString(buffer, 0, message_size);
        }


        public bool IsLogged
        {
            get => client.IsLogged;
            set => client.IsLogged = true;
        }
        public void disconnectClient()
        {
            client.disconnect();
        }

        internal void leaveCommunity(string canalName)
        {
            sendText("leave " + canalName);
        }
    }
}
