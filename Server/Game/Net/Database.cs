using System;
using DarkRift.Server;
using MySql.Data.MySqlClient;

namespace Server.Net {

    public class DataBase {
        
        public Mysql mysql;
        private static DataBase instance;
        
        public static DataBase getInstance() {
            if (instance == null)
                instance = new DataBase();
            return instance;
        }
        
        private DataBase() {
            mysql = new Mysql();
        }
    }
    
    
}