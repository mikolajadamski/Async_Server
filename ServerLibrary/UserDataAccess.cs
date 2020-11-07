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
        static private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

    }
}
