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
            UserController userController = new UserController();
            while (!userController.IsLogged)
            {
                try
                {
                    if (CommunicationProtocol.LogIn(stream, buffer, userController) == -1) break;
                }
                catch (IOException)
                {
                    break;
                }
            }
            while (userController.IsLogged)
            {
                try
                {
                    CommunicationProtocol.CommandExecution(stream, buffer, userController);
                }
                catch (System.IndexOutOfRangeException)
                {
                    StreamControl.sendText("Za mało argumentów!\r\n", buffer, stream);
                }
                catch (IOException)
                {
                    break;
                }

            }
            userController.IsLogged = false;
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