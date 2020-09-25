using System;
using System.Collections.Generic;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Server.Net {
    public class Mysql {
        
        public MySqlConnection descriptor;
        
        public void Connect(string connectionString) {
            try {
                descriptor = new MySqlConnection(connectionString);
                descriptor.Open();
            }
            catch (Exception e) {
                Console.WriteLine("MySQL: Connection failed");
                Console.WriteLine(e);
                throw;
            }
        }

        public int Count(string sql, Dictionary<string, string> placeholders) {
            try {

                var sql2 = "SELECT COUNT(*) FROM users WHERE login = @login AND password = @password LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(sql2, descriptor);
                
                foreach (var placeholder in placeholders)
                    cmd.Parameters.AddWithValue(placeholder.Key, placeholder.Value);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            return 0;
        }

    }
}