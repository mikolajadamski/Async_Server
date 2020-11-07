using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class UserController
    {
        User user;
        bool isLogged;

        public UserController()
        {
            isLogged = false;
        }
        public bool IsLogged
        {
            get => isLogged;
            set => isLogged = value;
        }
        public string login()
        {
            if(UserDataAccess.selectUser(user) == null)
            {
                return "Nieprawidłowe dane!\r\n";
            }
            else
            {
                return "Zalogowano pomyślnie.";
            }
        }
        public string register()
        {
            if(UserDataAccess.insertUser(user) == 0)
            {
                return "Nazwa użytkownika już zajęta!";
            }
            else
            {
                return "Zarejestrowano.";
            }
        }
        public User User
        {
            get => user;
            set
            {
               if(value != null) user = value;
            }
        }
    }
    
}
