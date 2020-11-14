﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLibrary
{
    static public class DataAccess
    {
        private static Mutex addUserMutex = new Mutex();
        private static Mutex createCanalMutex = new Mutex();
        static public int insertUser(User user)
        {
            try
            {
                addUserMutex.WaitOne();
                using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
                {
                    int result = databaseConnection.Execute(
                        @"
                    INSERT INTO users (username, password)
                    VALUES (@name, @password)"
                        , user);
                    addUserMutex.ReleaseMutex();
                    return result;
                }
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                addUserMutex.ReleaseMutex();
                return 0;
            }

        }
        static public int deleteUser(User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                int result = databaseConnection.Execute(
                    @"
                    DELETE FROM users
                    WHERE username = @name
                    AND password = @password
                    ", user);
                return result;
            }
        }
        static public string selectUser(User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(
                       @"
                        SELECT * FROM users
                        WHERE username = @name
                        AND password = @password
                    ", user);
                string name = null;
                if (result != null)
                {
                    name = result.username;
                }
                return name;
            }
        }
        static public int changeUserPassword(User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                int result = databaseConnection.Execute(
                    @"
                     UPDATE users
                     SET password = @password
                     WHERE username = @name
                 ", user);
                return result;
            };
        }

        static public int createCanal(string canalName, User user) {

            createCanalMutex.WaitOne();
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {

                string pom4 = string.Format("SELECT name FROM canals WHERE name = \"{0}\"", canalName);
                var result = databaseConnection.QuerySingleOrDefault(@pom4);


                if (result == null)
                {
                    string createCanalTableOperation = string.Format("CREATE TABLE {0} ( username VARCHAR(25) UNIQUE NOT NULL, administrator BOOLEAN NOT NULL)", canalName);
                    string insertAdminOperation = string.Format("INSERT INTO {0} (username,administrator) VALUES (@name, 1)", canalName);
                    string insertCanalNameIntoTableOperation = string.Format("INSERT INTO canals(name) VALUES(\"{0}\")", canalName);
                    int insertCanalResult = databaseConnection.Execute(@insertCanalNameIntoTableOperation);
                    int createCanalResult = databaseConnection.Execute(@createCanalTableOperation);
                    int insertAdminResult = databaseConnection.Execute(@insertAdminOperation, user);
                    createCanalMutex.ReleaseMutex();
                    return createCanalResult;
                }
                createCanalMutex.ReleaseMutex();
                return 0;
            }

        }

        static public void deleteCanal(string canalName, User user) {

            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null) {
                    string command = String.Format("SELECT * FROM {0} WHERE username = @name", canalName);
                    var result2 = databaseConnection.QuerySingleOrDefault(@command, user);

                    if (result2 != null) {
                        if (result2.administrator) {
                            databaseConnection.Execute(string.Format("DROP TABLE {0}", canalName));
                            databaseConnection.Execute(string.Format("DELETE FROM canals WHERE name = \"{0}\"", canalName));
                        }
                    }
                }

            }
        }


        public static void addtoCanal(string canalName, string username) {

            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null) {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, username));
                    if (check == null) {
                        databaseConnection.Execute(string.Format("INSERT INTO {0} (username,administrator) VALUES (\"{1}\", 0)", canalName, username));
                    }
                }
            }
        }
        static public void joinCanal(string canalName, User user)
        {
            addtoCanal(canalName, user.Name);
        }


        static public string changeCanal(string canalName, User user){
            
               using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {
                    var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));
                   if (result != null) {
                      var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, user.Name));
                         if(check != null){
                            user.CurrentCanal = canalName;
                            return canalName + "\r\n Wpisz \"//leave\" by wrocic do menu glownego \r\n";

                         }
                         return "Uzytkownik nie ma dostepu do tego kanalu\r\n";
                    }
                   return "Kanal o takiej nazwie nie istnieje\r\n";
               }

        
        }



        public static void removefromCanal(string canalName, string username)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null)
                {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, username));

                    if (check != null)
                    {
                        databaseConnection.Execute(string.Format("DELETE FROM {0} WHERE username = \"{1}\"",canalName,username));
                    }
                }
            }
        }
        public static void removeAllfromCanal(string canalName)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals", canalName));
                if (result != null)
                {
                    databaseConnection.Execute(string.Format("DELETE FROM {0}", canalName));
                }
            }
        }
        public static void leaveCanal(string canalname,User user)
        {
            removefromCanal(canalname, user.Name);
        }

        public static string[] listuserCanal(string canalName)
        {
            using(IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                return databaseConnection.Query<string>(String.Format(
                    @"
                    SELECT *
                    FROM {0}
                    ",canalName)).ToArray();
            }
        }

        

        static public string[] selectOpenCanals() 
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                return databaseConnection.Query<string>(
                    @"
                    SELECT * 
                    FROM canals
                    ").ToArray();
            }
        }
        static private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }

    
}