using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DbCommunication
    {

        public void test()
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new("GetAllCounties", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    System.Diagnostics.Debug.WriteLine($"{reader["CountyName"]}");
                }
            }
        }
    }
}
