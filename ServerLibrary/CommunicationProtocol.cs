using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
     public class CommunicationProtocol
    {

       static  public void executeCmd(NetworkStream stream, byte[] buffer, UserController userController, string[] cmd)
        {
                switch (cmd[0].ToLower())
                {
                    case "changepassword": changepassword(stream, buffer, userController); break;
                    case "create": create(stream, buffer, userController, cmd[1]); break;
                    case "unregister": unregister(stream, buffer, userController); break;
                    case "delete": delete(stream, buffer, userController, cmd[1]); break;
                    case "help": help(stream, buffer); break;
                    case "list": list(stream, buffer); break;
                    case "add": DataAccess.addtoCanal(cmd[1], cmd[2]); break;
                    case "join": DataAccess.joinCanal(cmd[1], userController.User); break;
                    case "remove": DataAccess.removefromCanal(cmd[1], cmd[2], userController.User); break;
                    case "removeall": DataAccess.removeAllfromCanal(cmd[1]); break;
                    case "leave": DataAccess.leaveCanal(cmd[1], userController.User); break;
                    case "listofusers": StreamControl.sendText(string.Join("\r\n", DataAccess.listuserCanal(cmd[1])) + "\r\n", buffer, stream); break;
                    case "exit": userController.IsLogged = false; break;
                    case "mkadmin": DataAccess.makeAdmin(cmd[1], cmd[2], userController.User); break;
                    case "switchto": switchto(stream, buffer, userController, cmd[1]); break;
                    default:
                        StreamControl.sendText("Nieznana komenda.\r\n", buffer, stream);
                        break;
                }

        }

        static private void help(NetworkStream stream, byte[] buffer)
        {
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
            StreamControl.sendText("\"leave\" aby zrezygnować z bycia czlonkiem kanalu\r\n", buffer, stream);
            StreamControl.sendText("\"list\" aby uzyskac liste wszystkich dostepnych na serwerze kanalow\r\n", buffer, stream);
            StreamControl.sendText("\"listofusers [nazwa kanalu]\" aby uzyskac liste wszystkich czlonkow kanalu\r\n", buffer, stream);
            StreamControl.sendText("\"mkadmin [nazwa kanalu] [nazwa uzytkownika]\" aby nadac prawa admina kanalu danemu uzytkownikowi\r\n", buffer, stream);
        }

      
        static private void changepassword(NetworkStream stream, byte[] buffer, UserController userController)
        {
            StreamControl.sendText("Podaj stare haslo: ", buffer, stream);
                            string oldPassword = StreamControl.readText(stream, buffer);
                            if (userController.IScorrectPassword(oldPassword))
                            {
                                StreamControl.sendText("Podaj nowe haslo: ", buffer, stream);
                                string newPassword = StreamControl.readText(stream, buffer);
        StreamControl.sendText(userController.changePassword(newPassword), buffer, stream);
                            }
        }

        private static void create(NetworkStream stream, byte[] buffer, UserController userController, string name)
        {
            int result = DataAccess.createCanal(name, userController.User);
            if (result == 1)
            {
                CanalsController.addCanal(name);
                StreamControl.sendText("Utworzono kanał " +name + "\r\n", buffer, stream);
            }
            else
            {
                StreamControl.sendText("Nie można utworzyć kanału \r\n", buffer, stream);
            }
        }

        private static void unregister(NetworkStream stream, byte[] buffer, UserController userController)
        {
            StreamControl.sendText(userController.deleteUser(), buffer, stream);
            System.Threading.Thread.Sleep(5000);
            userController.IsLogged = false;
        }

        private static void delete(NetworkStream stream, byte[] buffer, UserController userController, string name)
        {
            DataAccess.deleteCanal(name, userController.User);
            StreamControl.sendText("Usunięto kanał " + name + "\r\n", buffer, stream);
        }

        private static void list(NetworkStream stream, byte[] buffer)
        {
            StreamControl.sendText(string.Join("\r\n", DataAccess.selectOpenCanals()) + "\r\n", buffer, stream);
           
        }


        private static void switchto(NetworkStream stream, byte[] buffer, UserController userController, string name)
        {
            StreamControl.sendText(DataAccess.changeCanal(name, userController.User), buffer, stream);
            if (userController.User.CurrentCanal != "MENU")
            {
                CanalsController.joinCanal(name, userController.User.Name, stream, buffer);
                StreamControl.sendText("Opuszczono kanal\n", buffer, stream);
                userController.User.CurrentCanal = "MENU";
            }
        }

    }
}
