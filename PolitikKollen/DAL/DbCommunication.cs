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


        public int AddCounty(string county)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspAddCounty", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CountyName", county);

                // Add return parameter
                SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnParameter);

                connection.Open();
                command.ExecuteNonQuery();

                // Capture the return value from the stored procedure
                int returnValue = (int)returnParameter.Value;
                // Optional: Handle the return value here, or you can handle it where this method is called
                return returnValue;
            }
        }

        public DataTable GetAllCounties()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspGetAllCounties", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Load the data into the DataTable
                dt.Load(reader);
                reader.Close();
            }

            return dt;
        }

        public int DeleteCounty(string countyName)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspDeleteCounty", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add the CountyName parameter to the command
                command.Parameters.AddWithValue("@CountyName", countyName);

                // Add a return parameter
                SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnParameter);

                connection.Open();
                command.ExecuteNonQuery();

                // Get the return value
                int result = (int)command.Parameters["@ReturnVal"].Value;

                return result;
            }
        }

        public DataTable GetAllProposals()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspGetProposalPrimaryKeys", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Load the data into the DataTable
                dt.Load(reader);
                reader.Close();
            }

            return dt;
        }

        public int CreateProposal(string countyName, string proposal, string info)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspCreateProposal", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameters
                command.Parameters.AddWithValue("@CountyName", countyName);
                command.Parameters.AddWithValue("@Proposal", proposal);
                command.Parameters.AddWithValue("@Info", info);

                connection.Open();

                // Since you're inserting data, you can use ExecuteNonQuery which returns the number of rows affected.
                // If the insertion is successful, it should return 1. Otherwise, it might return 0.
                return command.ExecuteNonQuery();
            }
        }

        public int DeleteProposal(string countyName, string proposal)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspDeleteProposal", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameters
                command.Parameters.AddWithValue("@CountyName", countyName);
                command.Parameters.AddWithValue("@Proposal", proposal);

                connection.Open();

                // Since you're deleting data, you can use ExecuteNonQuery which returns the number of rows affected.
                // If the deletion is successful, it should return 1. Otherwise, it might return 0.
                return command.ExecuteNonQuery();
            }
        }

        public int EditProposal(string countyName, string proposal, string newInfo)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspEditProposal", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@CountyName", countyName);
                command.Parameters.AddWithValue("@Proposal", proposal);
                command.Parameters.AddWithValue("@NewInfo", newInfo);

                // Add return parameter
                SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnParameter);

                connection.Open();
                command.ExecuteNonQuery();

                // Capture the return value from the stored procedure
                int returnValue = (int)returnParameter.Value;
                // Optional: Handle the return value here, or you can handle it where this method is called
                return returnValue;
            }
        }




    }
}
