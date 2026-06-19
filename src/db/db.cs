using System.Configuration;
using System.Data.SqlClient;

namespace src.db
{
    public static class Db
    {
        public static SqlConnection OpenConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["SimsDb"].ConnectionString;
            var conn = new SqlConnection(cs);
            conn.Open();
            return conn;
        }
    }
}
