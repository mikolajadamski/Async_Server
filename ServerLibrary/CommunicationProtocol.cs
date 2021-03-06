﻿using System;
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
        static public void CommandExecution(NetworkStream stream, byte[] buffer, UserController userController)
        {
            //StreamControl.sendText(userController.User.CurrentCanal + "\r\n", buffer, stream);
            //StreamControl.sendText("Wpisz \"help\" aby uzyskac pomoc\r\n", buffer, stream);
            string[] command = StreamControl.readText(stream, buffer).Split();
            CommunicationProtocol.execute(stream, buffer, userController, command);
        }

        static public int LogIn(NetworkStream stream, byte[] buffer, UserController userController)
        {
            string message;
            message = StreamControl.readText(stream, buffer);
            if (message == "login" || message == "register")
            {
                StreamControl.sendText("OK", buffer, stream);
                userController.User = getUser(stream, buffer);
                if (userController.User == null) return 0;
                if (message == "login")
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
                return -1;
            }
            else
            {
                StreamControl.sendText("Nieprawidłowa operacja", buffer, stream);
            }
            return 0;
        }

        static private void execute(NetworkStream stream, byte[] buffer, UserController userController, string[] cmd)
        {
            switch (cmd[0].ToLower())
            {
                case "changepassword": changepassword(stream, buffer, userController, cmd[1], cmd[2]); break;
                case "create": create(stream, buffer, userController, cmd[1], cmd[2]); break;
                case "unregister": unregister(stream, buffer, userController); break;
                case "delete": delete(stream, buffer, userController, cmd[1]); break;
                case "help": help(stream, buffer); break;
                case "list": list(stream, buffer, cmd[1]); break;
                case "add": DataAccess.addtoCanal(cmd[1], cmd[2]); break;
                case "join": join(stream, buffer, userController, cmd[1]); break;
                case "remove": DataAccess.removeFromCanal(cmd[1], cmd[2], userController.User.Name); break;
                case "removeall": DataAccess.removeAllfromCanal(cmd[1]); break;
                case "leave": leaveCanal(stream, buffer, userController, cmd[1]); break;
                case "listofusers": StreamControl.sendText(string.Join("\r\n", DataAccess.listuserCanal(cmd[1])) + "\r\n", buffer, stream); break;
                case "exit": userController.IsLogged = false; break;
                case "mkadmin": DataAccess.makeAdmin(cmd[1], cmd[2], userController.User.Name); break;
                case "tkadmin": DataAccess.takeAdmin(cmd[1], cmd[2], userController.User.Name); break;
                case "switchto": switchto(stream, buffer, userController, cmd[1]); break;
                default:
                    StreamControl.sendText("Nieznana komenda.\r\n", buffer, stream);
                    break;
            }

        }

        static private void leaveCanal(NetworkStream stream, byte[] buffer, UserController userController, string canalName)
        {
            string result = DataAccess.leaveCanal(canalName, userController.User);
            StreamControl.sendText(result, buffer, stream);
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

        static private void changepassword(NetworkStream stream, byte[] buffer, UserController userController, string oldPassword, string newPassword)
        {
            if (userController.IScorrectPassword(oldPassword))
            {
                if (newPassword.Length < 8 || newPassword.Length > 25)
                {
                    StreamControl.sendText("RESP CPW SIZE_ERROR", buffer, stream);
                    return;
                }
                userController.User.setPassword(newPassword);
                StreamControl.sendText("RESP CPW OK", buffer, stream);
                return;
            }
            else
            {
                StreamControl.sendText("RESP CPW OLD_ERROR", buffer, stream);
            }
        }

        private static void create(NetworkStream stream, byte[] buffer, UserController userController, string name, string type)
        {
            string result = DataAccess.createCanal(name, userController.User, type);
            if (result == "RESP CREATE OK")
            {
                CanalsController.addCanal(name);
            }
            StreamControl.sendText(result, buffer, stream);
        }

        private static void unregister(NetworkStream stream, byte[] buffer, UserController userController)
        {
            StreamControl.sendText(userController.deleteUser(), buffer, stream);
            System.Threading.Thread.Sleep(5000);
            userController.IsLogged = false;
        }

        private static void delete(NetworkStream stream, byte[] buffer, UserController userController, string name)
        {
            string resp = DataAccess.deleteCanal(name, userController.User);
            StreamControl.sendText(resp, buffer, stream);
        }

        private static void list(NetworkStream stream, byte[] buffer, string username)
        {
            StreamControl.sendText("CANALS\r\n" + string.Join("\r\n", DataAccess.selectUserCanals(username)) + "\r\n", buffer, stream);
        }

        private static void join(NetworkStream stream, byte[] buffer, UserController userController, string name)
        {
            StreamControl.sendText(DataAccess.joinCanal(name, userController.User) + " " + name, buffer, stream);
        }

        private static void switchto(NetworkStream stream, byte[] buffer, UserController userController, string name)
        {
            StreamControl.sendText(DataAccess.changeCanal(name, userController.User), buffer, stream);
            if (userController.User.CurrentCanal != "MENU")
            {
                CanalsController.joinCanal(name, userController.User.Name, stream, buffer);
                userController.User.CurrentCanal = "MENU";
            }
        }

        public static User getUser(NetworkStream stream, byte[] buffer)
        {
            string username = StreamControl.readText(stream, buffer);
            if (username.Length < 8 || username.Length > 25)
            {
                StreamControl.sendText("ERR_SIZE", buffer, stream);
                return null;
            }
            StreamControl.sendText("OK", buffer, stream);
            string password = StreamControl.readText(stream, buffer);
            if (password.Length < 8 || password.Length > 25)
            {
                StreamControl.sendText("ERR_SIZE", buffer, stream);
                return null;
            }

            return new User(username, password);
        }
    }
}
