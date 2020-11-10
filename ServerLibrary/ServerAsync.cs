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
        public static string[,] sharedbuffer = new string [10,3];
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
                    switch(command[0].ToLower())
                    {
                        case "changepassword":
                            StreamControl.sendText("Podaj stare haslo: ", buffer, stream);
                            string oldPassword = StreamControl.readText(stream, buffer);
                            if (userController.IScorrectPassword(oldPassword))
                            {
                                StreamControl.sendText("Podaj nowe haslo: ", buffer, stream);
                                string newPassword = StreamControl.readText(stream, buffer);
                                StreamControl.sendText(userController.changePassword(newPassword), buffer, stream);
                            }
                            break;

                        case "create":
                            
                            UserDataAccess.createCanal(command[1], userController.User);
                            StreamControl.sendText("Utworzono kanał " + command[1] + "\r\n", buffer, stream);
                            break;

                        case "unregister":
                            StreamControl.sendText(userController.deleteUser(), buffer, stream);
                            System.Threading.Thread.Sleep(5000);
                            userController.IsLogged = false;
                            break;

                        case "delete":
                            UserDataAccess.deleteCanal(command[1], userController.User);
                            StreamControl.sendText("Usunięto kanał " + command[1] + "\r\n", buffer, stream);
                            break;

                        case "list":
                            StreamControl.sendText(string.Join("\r\n", UserDataAccess.selectOpenCanals()) + "\r\n", buffer, stream);
                            break;

                        case "add":
                            UserDataAccess.addtoCanal(command[1], command[2]);
                            break;

                        case "join":                       
                            UserDataAccess.joinCanal(command[1], userController.User);
                            break;

                        case "remove":
                            UserDataAccess.removefromCanal(command[1], command[2]);
                            break;

                        case "removeall":
                            UserDataAccess.removeAllfromCanal(command[1]);
                            break;

                        case "leave":
                            UserDataAccess.leaveCanal(command[1], userController.User);
                            break;

                        case "listofusers":
                            StreamControl.sendText(string.Join("\r\n", UserDataAccess.listuserCanal(command[1])) + "\r\n", buffer, stream);
                            break;
                        
                        case "exit":
                            userController.IsLogged = false;
                            break;

                        case "help":
                            StreamControl.sendText("POMOC\r\n", buffer, stream);
                            StreamControl.sendText("Wpisz\r\n", buffer, stream);
                            StreamControl.sendText("\"changepassword\" aby zmienic haslo\r\n", buffer, stream);
                            StreamControl.sendText("\"create [nazwa kanalu]\" aby stworzyc kanal komunikacyjny\r\n", buffer, stream);
                            StreamControl.sendText("\"delete [nazwa kanalu]\" aby usunac kanal komunikacyjny\r\n", buffer, stream);
                            StreamControl.sendText("\"unregister\" aby usunac uzytkownika\r\n", buffer, stream);
                            StreamControl.sendText("\"add [nazwa kanalu] [nazwa uzytkownika]\" aby dodac uzytkownika do kanalu komunikacyjnego\r\n", buffer, stream);
                            StreamControl.sendText("\"join [nazwa kanalu]\" aby dolaczyc do kanalu komunikacyjnego\r\n", buffer, stream);
                            StreamControl.sendText("\"switchto [nazwa kanalu]\" aby dolaczyc do rozmowy na danym kanale komunikacyjnym\r\n", buffer, stream);
                            StreamControl.sendText("\"remove [nazwa kanalu] [nazwa uzytkownika]\" aby usunac uzytkownika z kanalu komunikacyjnego\r\n", buffer, stream);
                            StreamControl.sendText("\"removeall\" aby usunac wszystkich uzytkownikow z kanalu komunikacyjnego\r\n", buffer, stream);
                            StreamControl.sendText("\"exit\" aby sie wylogowac\r\n", buffer, stream);
                            break;

                        case "switchto":
                            StreamControl.sendText(UserDataAccess.changeCanal(command[1], userController.User), buffer, stream);
                            Canals.addToCanal(command[1], userController.User.Name, stream);
                            if(userController.User.CurrentCanal != "MENU"){
                            Canals.canalCommunication(userController.User, stream);
                            StreamControl.sendText("Opuszczono kanal\n", buffer, stream);
                            userController.User.CurrentCanal = "MENU";}
                            break;

                        default:
                            StreamControl.sendText("Nieznana komenda.\r\n", buffer, stream);
                            break;

                    }
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


        public static int freecolumn(){
            while(true){
                for(int i=0; i<10; i++){
                if(sharedbuffer[i,0] == null){ return i;}

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
            Canals.initializeCanals();
            StartListening();
            AcceptClient();
            

        }



    }

}