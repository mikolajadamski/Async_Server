using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    static public class UserDataAccess
    {

       static public int insertUser(User user)
        {
            using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString()))
            {
                int result = databaseConnection.Execute(
                    @"
                    INSERT INTO users (username, password)
                    VALUES (@name, @password)"
                    , user);
                return result;
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

        static public void createCanal(string canalName,User user){
           
               using (IDbConnection databaseConnection = new SQLiteConnection(LoadConnectionString())) {

                string pom4 = String.Format("SELECT name FROM canals WHERE name = \"{0}\"", canalName);
                 var result = databaseConnection.QuerySingleOrDefault(@pom4);


                if (result == null)
                     {
                    string pom = String.Format("CREATE TABLE {0} ( username VARCHAR(25) UNIQUE NOT NULL, administrator BOOLEAN NOT NULL)", canalName);
                    string pom2 = String.Format("INSERT INTO {0} (username,administrator) VALUES (@name, 1)", canalName);
                    string pom3 = String.Format("INSERT INTO canals(name) VALUES(\"{0}\")", canalName);
                int result1 = databaseConnection.Execute(@pom3);
                int result2 = databaseConnection.Execute(@pom);
                int result3 =  databaseConnection.Execute(@pom2, user);
               }
               }

           }

    
        static private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }
}
