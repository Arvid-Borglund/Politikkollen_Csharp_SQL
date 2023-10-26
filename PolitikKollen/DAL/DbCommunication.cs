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

        public byte[] GetSaltByUserName(string userName)
        {
            byte[] salt = null; // Initialize to null. Will store the salt value if found.

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
            {
                SqlCommand command = new SqlCommand("uspGetSaltByUserName", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter
                command.Parameters.AddWithValue("@UserName", userName);

                // Output parameter
                SqlParameter saltOutputParameter = new SqlParameter("@Salt", SqlDbType.VarBinary, 64);
                saltOutputParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(saltOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameter value
                if (saltOutputParameter.Value != DBNull.Value)
                {
                    salt = (byte[])saltOutputParameter.Value;
                }
            }

            return salt; // This will be null if no salt was found for the given UserName
        }




        public bool CheckCitizenExistence(byte[] bankIdHash, out string message)
        {
            bool exists = false; // This flag will be used to store the existence status
            message = ""; // Initialize the message string

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection logic
            {
                SqlCommand command = new SqlCommand("CheckCitizenExistence", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter for BankIdHash
                command.Parameters.AddWithValue("@BankIdHash", bankIdHash);

                // Output parameters for Exists and Message
                SqlParameter existsOutputParameter = new SqlParameter("@Exists", SqlDbType.Bit);
                existsOutputParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(existsOutputParameter);

                SqlParameter messageOutputParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                messageOutputParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(messageOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameter values
                if (existsOutputParameter.Value != DBNull.Value)
                {
                    exists = Convert.ToBoolean(existsOutputParameter.Value);
                }

                if (messageOutputParameter.Value != DBNull.Value)
                {
                    message = Convert.ToString(messageOutputParameter.Value);
                }
            }

            return exists; // This will be true if BankIdHash exists, false otherwise
        }

        public DataTable GetProposalDataAsDataTable(byte[] bankIdHash, string countyName)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection logic
            {
                using (SqlCommand command = new SqlCommand("GetProposalData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    command.Parameters.AddWithValue("@BankIdHash", bankIdHash);
                    command.Parameters.AddWithValue("@CountyName", countyName);

                    // Open the connection
                    connection.Open();

                    // Use SqlDataAdapter to fill DataTable
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public string GetCountyByBankIdHash(byte[] bankIdHash)
        {
            string countyName = null; // Initialize to null. Will store the county name if found.

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
            {
                SqlCommand command = new SqlCommand("GetCountyByBankIdHash", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter
                command.Parameters.AddWithValue("@BankIdHash", bankIdHash);

                // Output parameter
                SqlParameter countyNameOutputParameter = new SqlParameter("@CountyName", SqlDbType.VarChar, 255);
                countyNameOutputParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(countyNameOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameter value
                if (countyNameOutputParameter.Value != DBNull.Value)
                {
                    countyName = countyNameOutputParameter.Value.ToString();
                }
            }

            return countyName; // This will be null if no county was found for the given BankIdHash
        }

        public void CreateUser(byte[] bankIdHash, string userName, byte[] salt, string county)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
            {
                SqlCommand command = new SqlCommand("CreateUser", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@BankIdHash", bankIdHash);
                command.Parameters.AddWithValue("@Salt", salt);
                command.Parameters.AddWithValue("@UserName", userName);

                if (county != null)
                {
                    command.Parameters.AddWithValue("@County", county);
                }
                else
                {
                    command.Parameters.AddWithValue("@County", DBNull.Value);
                }

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();
            }
        }

        public string GetCitizenNameByUserHash(byte[] userHash)
        {
            string citizenName = null; // Initialize to null. Will store the citizen name if found.

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
             {
                SqlCommand command = new SqlCommand("uspGetUserNameByHash", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter
                command.Parameters.Add(new SqlParameter("@UserHash", SqlDbType.VarBinary, 64)).Value = userHash;

                // Output parameter
                SqlParameter nameOutputParameter = new SqlParameter("@UserName", SqlDbType.NVarChar, 255);
                nameOutputParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(nameOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameter value
                if (nameOutputParameter.Value != DBNull.Value)
                {
                    citizenName = (string)nameOutputParameter.Value;
                }
            }

            return citizenName; // This will be null if no citizen name was found for the given UserHash
        }


    }
}
