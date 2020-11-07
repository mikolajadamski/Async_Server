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

            if(UserDataAccess.selectUser(user) == User.Name)
            {
                isLogged = true;
                return "Zalogowano.\r\n";
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
               if(value != null) user = value;
            }
        }
        public string deleteUser()
        {
            int result = UserDataAccess.deleteUser(user);
            if (result == 1)
            {
                return "Pomyślnie usunięto użytkownika (5 sekund do zamknięcia).\r\n";
            }
            else
            {
                return "Operacja zakończona niepowodzeniem.\r\n";
            }
        }
    }
    
}
