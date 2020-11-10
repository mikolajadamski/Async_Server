using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class User
    {
        private string name;
        private string password;
        private bool isLogged;
        string currentCanal;

        public User(string username, string password)
        {
            this.name = username;
            this.password = password;
            currentCanal = "MENU";
        }

        
        public string CurrentCanal
        {
            get => currentCanal;
            set => currentCanal = value;
        }

        public string Name
        {
            get => name;
        }
        public string Password
        {
            get => password;
        }
        public void setPassword(string password)
        {
            this.password = password;
        }
    }
}
