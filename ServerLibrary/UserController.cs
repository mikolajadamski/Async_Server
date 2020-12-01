using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            set
            {
                if(isLogged==true && value==false)
                {
                    DataAccess.logOutUser(user);
                    isLogged = value;
                }
                else
                {
                    isLogged = value;
                }
            }
        }
        public string login()
        {
            string result = DataAccess.selectUser(user);
            if (result == User.Name)
            {
                isLogged = true;
                return "Zalogowano.\r\n";
            }
            else
            {
                return result;
            }
        }
        public string register()
        {
            if (DataAccess.insertUser(user) == 0)
            {
                return "Nazwa użytkownika już zajęta!\r\n";
            }
            else
            {
                return "Rejestracja zakończyła się powodzeniem.\r\n";
            }
        }
        public User User
        {
            get => user;
            set
            {
                if (value != null) user = value;
            }
        }


        public string deleteUser()
        {
            int result = DataAccess.deleteUser(user);
            if (result == 1)
            {
                return "Pomyślnie usunięto użytkownika (5 sekund do zamknięcia).\r\n";
            }
            else
            {
                return "Operacja zakończona niepowodzeniem.\r\n";
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
            if (DataAccess.changeUserPassword(user) == 0)
            {
                return "error";
            }
            else
            {
                return "Pomyślnie zmieniono hasło\r\n";
            }
        }
    }
    
}
