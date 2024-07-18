using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BTL_C_
{
    internal class DBConnection
    {
        public SQLiteConnection conn = null;
        private string strConn = ConfigurationManager.AppSettings["strConn"].ToString();

        public SQLiteConnection GetConnection()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(strConn);
            }
            if(conn != null && conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        public SQLiteConnection CloseConnection()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            return conn;
        }
    }
}
