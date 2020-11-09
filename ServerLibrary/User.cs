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
        
        public User(string username, string password)
        {
            this.name = username;
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
        public void setPassword(string password)
        {
            this.password = password;
        }
    }
}
