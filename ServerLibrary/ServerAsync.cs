using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ServerLibrary

{

    public class ServerAsync : Server
    {
        public delegate void TransmissionDataDelegate(NetworkStream stream);
        public ServerAsync(IPAddress IP, int port) : base(IP, port)
        {
        }

        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                stream = client.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, client);
            }
        }



        private void TransmissionCallback(IAsyncResult operationState)
        {
            TcpClient client = (TcpClient)operationState.AsyncState;
            client.Close();
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            stream.ReadTimeout = 3600000;
            
            byte[] buffer = new byte[bufferSize];
            string message;
            UserController userController = new UserController();
            while (!userController.IsLogged)
            {
                try
                {
                    StreamControl.sendText("Wpisz register lub login:", buffer, stream);
                    message = StreamControl.readText(stream, buffer);
                    if (message == "login" || message == "register")
                    {
                        userController.User = getUser(stream, buffer);
                        if (userController.User == null) continue;
                        if(message == "login")
                        {
                            StreamControl.sendText(userController.login(), buffer, stream);
                        }
                        else
                        {
                            StreamControl.sendText(userController.register(), buffer, stream);
                        }
                    }
                    else if (message == "exit")
                    {
                        break;
                    }
                    else
                    {
                        StreamControl.sendText("Nieprawidłowa operacja\r\n", buffer, stream);
                    }
                }
                catch (IOException e)
                {
                    // e.Message;
                    break;
                }
            }
            while (userController.IsLogged)
            {
                try
                {
                    StreamControl.sendText(userController.User.CurrentCanal+"\r\n", buffer, stream);
                    StreamControl.sendText("Wpisz \"help\" aby uzyskac pomoc\r\n", buffer, stream);
                    string[] command = StreamControl.readText(stream, buffer).Split();
                    CommunicationProtocol.executeCmd(stream, buffer, userController, command);
                 
                }
                catch (System.IndexOutOfRangeException)
                {
                    StreamControl.sendText("Za mało argumentów!\r\n", buffer, stream);
                }
                catch (IOException e)
                {
                    // e.Message;
                    break;
                }
            }
        }




        public User getUser(NetworkStream stream, byte [] buffer)
        {
            StreamControl.sendText("nazwa użytkownika(8-25 znaków):", buffer, stream);
            string username = StreamControl.readText(stream, buffer);
            if(username.Length<8 || username.Length>25)
                {
                StreamControl.sendText("Nieprawidłowa długość nazwy użytkownika!\r\n", buffer, stream);
                return null;
            }
            StreamControl.sendText("hasło(8-25 znaków):", buffer, stream);
            string password = StreamControl.readText(stream, buffer);
            if (password.Length < 8 || password.Length > 25)
            {
                StreamControl.sendText("Nieprawidłowa długość hasła!\r\n", buffer, stream);
                return null;
            }
            return new User(username, password);
        }

       

        public override void Start()
        {
            running = true;
            DataAccess.initTables();
            CanalsController.initializeCanals();
            StartListening();
            AcceptClient();
            

        }



    }

}