using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLibrary
{
    static public class DataAccess
    {
        private static Mutex addUserMutex = new Mutex();
        private static Mutex createCanalMutex = new Mutex();
        private static Mutex loginUserMutex = new Mutex();
        static public int insertUser(User user)
        {
            try
            {
                addUserMutex.WaitOne();
                using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
                {
                    int result = databaseConnection.Execute(
                        @"
                    INSERT INTO users (username, password, islogged)
                    VALUES (@name, @password, 0)"
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

       static public void CanalHistory(NetworkStream stream, string canalName, byte[] buffer){
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                 var query = databaseConnection.QuerySingleOrDefault(string.Format("SELECT msgID from canals where name = \"{0}\"", canalName));
                 string tableName = "";
            if(query != null)
                 tableName = "k" + query.msgID;
            string query2 = string.Format("SELECT * from {0}", tableName);
                var result = databaseConnection.Query(query2);
                string msg = "";
            for(int i=0; i < result.Count(); i++){
                   
             msg =  result.ElementAt(i).username + "(" + result.ElementAt(i).time + "): " + result.ElementAt(i).message + "\r\n";
                    StreamControl.sendText(msg,buffer,stream);
            }

}         
        

       }

        internal static void initUsers()
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                databaseConnection.QuerySingleOrDefault(
                            @"
                            UPDATE users
                            SET islogged = 0
                            WHERE islogged = 1");
            }
        }

        static public string selectUser(User user)
        {
            loginUserMutex.WaitOne();
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(
                       @"
                        SELECT * FROM users
                        WHERE username = @name
                        AND password = @password
                    ", user);

                if (result != null)
                {
                    if(result.islogged)
                    {
                        loginUserMutex.ReleaseMutex();
                        return "Ktoś już jest zalogowany na tym koncie.\r\n";
                    }
                    else
                    {
                        databaseConnection.QuerySingleOrDefault(
                            @"
                            UPDATE users
                            SET islogged = 1
                            WHERE username = @name
                            AND password = @password", user);
                        loginUserMutex.ReleaseMutex();
                        return result.username;
                    }
                }
                else
                {
                    loginUserMutex.ReleaseMutex();
                    return "Nie ma takiego użytkownika.\r\n";
                }
                
            }
        }

        static public int addMsg(String text, String username, String canalName){
                  using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {   
            var query = databaseConnection.QuerySingleOrDefault(string.Format("SELECT msgID from canals where name = \"{0}\"", canalName));
                 string tableName = "";
            if(query != null)
                 tableName = "k" + query.msgID;
            string time = DateTime.Now.ToString("h:mm:ss tt");
                    string query2 = string.Format(
                        " INSERT INTO {0} (username, time, message) VALUES (\"{1}\", \"{2}\",\"{3}\")", tableName, username, time, text);
                  
   int result = databaseConnection.Execute(query2);
                return result;
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
        static public void logOutUser(User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                databaseConnection.QuerySingleOrDefault(
                    @"
                    UPDATE users
                    SET islogged = 0
                    WHERE username = @name
                    AND password = @password", user);
            }
        }

        static public int createCanal(string canalName, User user) {

            createCanalMutex.WaitOne();
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {

                try
                {
                    string pom4 = string.Format("SELECT name FROM canals WHERE name = \"{0}\"", canalName);
                    var result = databaseConnection.QuerySingleOrDefault(@pom4);


                    if (result == null)
                    {
                    
                            string createCanalTableOperation = string.Format("CREATE TABLE {0} ( username VARCHAR(25) UNIQUE NOT NULL, administrator BOOLEAN NOT NULL)", canalName);
                            string insertAdminIntoCanalOperation = string.Format("INSERT INTO {0} (username,administrator) VALUES (@name, 1)", canalName);
                        string idcheck = "SELECT max(msgID) as max FROM canals";
                            var v = databaseConnection.QuerySingleOrDefault(@idcheck);
                        int id = -1;
                        if(v.max != null)
                             id =(int)v.max;
                      
                              string name =  "k"+(id+1).ToString();
                            string insertCanalNameIntoCanalsOperation = string.Format("INSERT INTO canals(name, msgID) VALUES(\"{0}\",{1})", canalName, id+1 );
                              string initMsgTable = string.Format(@"CREATE TABLE IF NOT EXISTS {0} (
                                                    username VARCHAR (25) NOT NULL, 
                                                    time  TEXT,
                                                    message TEXT)", name);
                        
                            databaseConnection.Execute(@insertCanalNameIntoCanalsOperation);
                            databaseConnection.Execute(@createCanalTableOperation);
                            databaseConnection.Execute(insertAdminIntoCanalOperation, user);
                             databaseConnection.Execute(initMsgTable);
                            createCanalMutex.ReleaseMutex();
                            return 1;
                    }
                   
                }
                catch (SQLiteException)
                {
                    createCanalMutex.ReleaseMutex();
                    return 0;
                }
                createCanalMutex.ReleaseMutex();
                return 0;
            }

        }

        static public string deleteCanal(string canalName, User user) {

            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));
                if (result != null) 
                {
                    string command = String.Format("SELECT * FROM {0} WHERE username = @name", canalName);
                    var result2 = databaseConnection.QuerySingleOrDefault(@command, user);

                    if (result2 != null) 
                    {
                        if (result2.administrator) {
                            databaseConnection.Execute(string.Format("DROP TABLE {0}", canalName));
                            databaseConnection.Execute(string.Format("DELETE FROM canals WHERE name = \"{0}\"", canalName));
                            return "Usunięto kanał.\r\n";
                        }
                        return "Nie masz uprawnień.\r\n";
                    }
                    return "Nie jesteś członkiem tego kanału.\r\n";
                }
                return "Kanał nie istnieje.\r\n";

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



        public static void removefromCanal(string canalName, string username, User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null)
                {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, username));
                    var admincheck =  databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\" AND administrator = 1", canalName, user.Name));

                    if (check != null && admincheck != null)
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
        public static void leaveCanal(string canalName,User user)
        {
          
           using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null)
                {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, user.Name));
                   
                   
                    
                    if (check != null)
                    {
                        databaseConnection.Execute(string.Format("DELETE FROM {0} WHERE username = \"{1}\"",canalName,user.Name));
                    }


                }
            }
        }

        public static void makeAdmin (string canalName, string username, User user){
                
             using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) 
             {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));
                 if (result != null)
                 {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, username));
                    var admincheck =  databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\" AND administrator = 1", canalName, user.Name));
                    if (check != null && admincheck != null)
                    {
                         databaseConnection.Execute(string.Format("UPDATE {0} SET administrator = 1 WHERE username = \"{1}\"",canalName,username));
                    }
                 }

             }
        
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

        static public void initTables()
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                string createUsersTableOperation = @"CREATE TABLE IF NOT EXISTS users (
                                                    username VARCHAR (25) PRIMARY KEY UNIQUE NOT NULL, 
                                                    password VARCHAR (25) NOT NULL,
                                                    islogged BOOL DEFAULT 0)";
                string createCanalsTableOperation = @"CREATE TABLE IF NOT EXISTS canals (
                                                    name VARCHAR(25) NOT NULL UNIQUE,
                                                    msgID INT UNIQUE NOT NULL,
                                                    PRIMARY KEY(name, msgID))";
                databaseConnection.Execute(createUsersTableOperation);
                databaseConnection.Execute(createCanalsTableOperation);
            }
        }
        static private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }

    
}
