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

            if (UserDataAccess.selectUser(user) != null)
            {
                isLogged = true;
                return "Zalogowano pomyślnie.\r\n";
            }
            else
            {
                return "Nieprawidłowe dane!\r\n";
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
        public bool IScorrectPassword(string oldPassword)
        {
            if (oldPassword == user.Password)
                return true;
            else
                return false;
        }
        public string changePassword(string newpassword)
        {

            user.setPassword(newpassword);
            if (UserDataAccess.changeUserPassword(user) == 0)
            {
                return "error";
            }
            else
            {
                return "Pomyślnie zmieniono hasło\r\n";
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
