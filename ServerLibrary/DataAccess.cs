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

        static public void CanalHistory(NetworkStream stream, string canalName, byte[] buffer)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var query = databaseConnection.QuerySingleOrDefault(string.Format("SELECT msgID from canals where name = \"{0}\"", canalName));
                string tableName = "";
                if (query != null)
                    tableName = "k" + query.msgID;
                string query2 = string.Format("SELECT * from {0}", tableName);
                var result = databaseConnection.Query(query2);
                string msg = "";
                for (int i = 0; i < result.Count(); i++)
                {

                    msg = "MSG " + result.ElementAt(i).username + "\t" + result.ElementAt(i).time + "\t" + result.ElementAt(i).message + " ENDMSG\r\n";
                    StreamControl.sendText(msg, buffer, stream);
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
                    if (result.islogged)
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

        static public int addMsg(string text, string time, string username, string canalName)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var query = databaseConnection.QuerySingleOrDefault(string.Format("SELECT msgID from canals where name = \"{0}\"", canalName));
                string tableName = "";
                if (query != null)
                    tableName = "k" + query.msgID;
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

        static public int createCanal(string canalName, User user, string type) {

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
                        if (v.max != null)
                            id = (int)v.max;

                        string name = "k" + (id + 1).ToString();
                        string insertCanalNameIntoCanalsOperation = string.Format("INSERT INTO canals(name, msgID, type) VALUES(\"{0}\",{1}, \"{2}\")", canalName, id + 1, type);
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
                           
                            var query = databaseConnection.QuerySingleOrDefault(string.Format("SELECT msgID from canals where name = \"{0}\"", canalName));
                            string tableName = "";
                            if (query != null)
                                tableName = "k" + query.msgID;
                            databaseConnection.Execute(string.Format("DROP TABLE {0}", tableName));
                            databaseConnection.Execute(string.Format("DELETE FROM canals WHERE name = \"{0}\"", canalName));
                            return "RESP DEL OK";
                        }
                        return "RESP DEL AUTH_ERR";
                    }
                    return "RESP DEL AUTH_ERR";
                }
                return "RESP DEL INVALID";

            }
        }

        public static string addtoCanal(string canalName, string username) {

            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {
                var isuser = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM users WHERE username = \"{0}\"", username));
                if (isuser != null) {
                    var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                    if (result != null) {
                        var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, username));
                        if (check == null) {
                            databaseConnection.Execute(string.Format("INSERT INTO {0} (username,administrator) VALUES (\"{1}\", 0)", canalName, username));
                            return "RESP ADU OK";
                        }
                        else
                        {
                            return "RESP ADU ALREADY_MEMBER";
                        }
                    }
                    else
                    {
                        return "RESP ADU INVALID";
                    }
                }
                else
                {
                    return "RESP ADU INVALID_MEMBER";
                }
            }
        }

        static public string joinCanal(string canalName, User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null)
                {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, user.Name));
                    if (check == null)
                    {
                        databaseConnection.Execute(string.Format("INSERT INTO {0} (username,administrator) VALUES (\"{1}\", 0)", canalName, user.Name));
                        return "RESP JOIN OK";
                    }
                    else
                    {
                        return "RESP JOIN ALREADY_MEMBER";
                    }
                }
                else
                {
                    return "RESP JOIN INVALID";
                }
            }
        }

        static public string changeCanal(string canalName, User user){
            
               using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {
                    var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));
                   if (result != null) {
                      var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, user.Name));
                         if(check != null){
                            user.CurrentCanal = canalName;
                            return "RESP SWITCHTO OK";

                         }
                         return "RESP SWITCHTO AUTH_ERROR";
                    }
                   return "RESP SWITCHTO INVALID_ERROR";
               }

        
        }

        public static string removeFromCanal(string canalName, string username, string userName)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var isuser = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM users WHERE username = \"{0}\"", username));
                if (isuser != null)
                {
                    var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                    if (result != null)
                    {
                        var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, username));
                        var admincheck = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\" AND administrator = 1", canalName, userName));

                        if (check != null)
                        {
                            if (admincheck != null)
                            {
                                databaseConnection.Execute(string.Format("DELETE FROM {0} WHERE username = \"{1}\"", canalName, username));
                                return "RESP RMV OK";
                            }
                            else
                            {
                                return "RESP RMV NO_PERM";
                            }
                        }
                        else
                        {
                            return "RESP RMV NO_MEMBER";
                        }
                    }

                    else
                    {
                        return "RESP RMV INVALID_CANAL";
                    }
                }
                else
                {
                    return "RESP RMV INVALID_MEMBER";

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

        public static string leaveCanal(string canalName,User user)
        {
          
           using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                var result = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM canals WHERE name = \"{0}\"", canalName));

                if (result != null)
                {
                    var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", canalName, user.Name));
                    var admincheck = databaseConnection.Query(string.Format("SELECT * FROM {0} WHERE administrator = 1", canalName));


                    if (check != null && admincheck == null)
                    {
                        databaseConnection.Execute(string.Format("DELETE FROM {0} WHERE username = \"{1}\"",canalName,user.Name));
                        return "RESP LEAVE OK";
                    }
                    else if (check != null && admincheck != null)
                    {
                        databaseConnection.Execute(string.Format("DELETE FROM {0} WHERE username = \"{1}\"", canalName, user.Name));
                        var first_user = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} as User LIMIT 1", canalName));
                        if (first_user != null)
                        {
                            makeAdmin(canalName, first_user.User, user);
                            return "RESP LEAVE OK";
                        }
                        else
                        {
                            
                            string insertAdminIntoCanalOperation = string.Format("INSERT INTO {0} (username,administrator) VALUES (@name, 1)", canalName);
                            databaseConnection.Execute(insertAdminIntoCanalOperation, user);
                            string res = deleteCanal(canalName, user);
                            return "RESP LEAVE OK_DELETED";
                        }
                    }
                    else
                    {
                        return "RESP LEAVE NOT_MEMBER";
                    }

                }
                else
                {
                    return "RESP LEAVE ERR";
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

        public static string[] AdminCheck (string[] users, string canalName)
        {
            string[] result = { };
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                for (int i = 0; i < users.Length; i++)
                {
                    result = result.Append(users[i]).ToArray();
                    var admincheck = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\" AND administrator = 1", canalName, users[i]));
                    if(admincheck != null)
                        result = result.Append("1").ToArray();
                    else result = result.Append("0").ToArray();

                }

                return result;
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

        static public string[] selectUserCanals(string username)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                string querypub = string.Format("SELECT * FROM canals WHERE type = \"public\"");
                string querypriv = string.Format("SELECT * FROM canals WHERE type = \"private\"");
                string[] userCanals = databaseConnection.Query<string>(querypub).ToArray();
                string[] privateCanals = databaseConnection.Query<string>(querypriv).ToArray();
            
                for(int i = 0; i < privateCanals.Length; i++)
                {
                   var check = databaseConnection.QuerySingleOrDefault(string.Format("SELECT * FROM {0} WHERE username = \"{1}\"", privateCanals[i], username));
                    if (check != null)
                        userCanals = userCanals.Append(privateCanals[i]).ToArray();
                }

                return userCanals;
                
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
                                                    type VARCHAR(7) NOT NULL,
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
