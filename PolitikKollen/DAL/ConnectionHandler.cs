using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal class ConnectionHandler
    {

        public static SqlConnection GetConnection()
        {
            string connectionString =
            ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnectionStringBuilder builder = new(connectionString);
            SqlConnection connection = new(builder.ConnectionString);
            return connection;
        }
    }
}
