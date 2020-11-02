using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    class UserService
    {
        private bool isLogged;
        private string username;
        private string password;
        public UserService()
        {
            isLogged = false;
        }
        public bool IsLogged
        {
            get => isLogged;
        }
        public int parseOperation(string op)
        {
            op = op.ToLower();
            if(op == "login")
            {
                return 1;
            }
            else if(op.Equals("register"))
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        public string login(string username, string password)
        {
            using (var fileStream = File.OpenRead("user_db.txt"))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 64))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] user_data = line.Split();
                        if (user_data[0] == username && user_data[1] == password)
                        {
                            isLogged = true;
                            this.username = user_data[0];
                            this.password = user_data[1];
                            return "Zalogowano pomyślnie.\r\n";
                        }
                        if (user_data[0] == username && user_data[1] != password)
                        {
                            return "Błędne hasło.\r\n";
                        }
                    }
                }
            }
            return "Użytkownik nie istnieje";
        }
        public string register(string username, string password)
        {
            using (var fileStream = File.OpenRead("user_db.txt"))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 64))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] user_data = line.Split();
                        if (user_data[0] == username)
                        {
                            return "Taki użytkownik już istnieje!.\r\n";
                        }
                    }
                }
            }
            using(StreamWriter sw = File.AppendText("user_db.txt"))
            {
                sw.WriteLine(username + " " + password);
            }
            return "Pomyślnie utworzono użytkownika.\r\n";
        }
        public string deleteUser()
        {
            List<string> arrLine = new List<string>();
            using (var fileStream = File.OpenRead("user_db.txt"))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 64))
                {
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] user_data = line.Split();
                        if (user_data[0] != username)
                        {
                            arrLine.Add(line);
                        }
                    }
                }
            }
            File.WriteAllLines("user_db.txt", arrLine);
            return "Usunięto użytkownika.\r\n";
        }
    }

}
