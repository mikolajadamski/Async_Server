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
        
        public User(string name, string password)
        {
            this.name = name;
            this.password = password;
        }
        public string Name
        {
            get => name;
        }
        public string Password
        {
            get => password;
        }
    }
}
