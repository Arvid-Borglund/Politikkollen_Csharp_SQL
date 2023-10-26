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
                SqlCommand command = new SqlCommand("pk.uspAddCounty", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CountyName", county);

                // Add return parameter
                SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnParameter);

                connection.Open();

                // Start the transaction
                SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection);
                command.Transaction = transaction;

                try
                {
                    command.ExecuteNonQuery();

                    // Capture the return value from the stored procedure
                    int returnValue = (int)returnParameter.Value;

                    // Commit the transaction
                    DataAdapterHelper.CommitTransaction(transaction);

                    // Optional: Handle the return value here, or you can handle it where this method is called
                    return returnValue;
                }
                catch (SqlException ex)
                {
                    // Rollback the transaction in case of any errors
                    DataAdapterHelper.RollbackTransaction(transaction);

                    // Pass the exception to the ErrorHandler
                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling, so it can be caught and managed outside if needed
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of any other kind of errors
                    DataAdapterHelper.RollbackTransaction(transaction);

                    // Handle general exceptions
                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }
        }


        public DataTable GetAllCounties()
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                try
                {
                    return DataAdapterHelper.ExecuteProcedureForDataTable(connection, "uspGetAllCounties");
                }
                catch (SqlException ex)
                {
                    // Pass the exception to the ErrorHandler
                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling, so it can be caught and managed outside if needed
                }
                catch (Exception ex)
                {
                    // Handle general exceptions
                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }
        }


        public int DeleteCounty(string countyName)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("pk.uspDeleteCounty", connection);
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
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                return DataAdapterHelper.ExecuteProcedureForDataTable(connection, "uspGetProposalPrimaryKeys");
            }
        }

        public int CreateProposal(string countyName, string proposal, string info)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("pk.uspCreateProposal", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameters
                command.Parameters.AddWithValue("@CountyName", countyName);
                command.Parameters.AddWithValue("@Proposal", proposal);
                command.Parameters.AddWithValue("@Info", info);

                // Output parameter for @Result
                SqlParameter resultOutputParameter = new SqlParameter("@Result", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameter's value
                if (resultOutputParameter.Value != DBNull.Value)
                {
                    return (int)resultOutputParameter.Value;
                }

                // Handle the case where the output parameter is DBNull.Value (null)
                return -1; // You can choose an appropriate error code or value here
            }
        }


        public int DeleteProposal(string countyName, string proposal)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("pk.uspDeleteProposal", connection);
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
                SqlCommand command = new SqlCommand("pk.uspEditProposal", connection);
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

        public byte[] GetSaltByUserName(string userName, String type)
        {
            byte[] salt = null; // Initialize to null. Will store the salt value if found.

            String call = "";
            String Value = "";

            if (type.Equals("User"))
            {
                call = "pk.uspGetSaltByUserName";
                Value = "@UserName";
            }
            else if (type.Equals("Admin"))
            {
                call = "pk.uspGetSaltByAdminName";
                Value = "@AdminName";
            }

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
            {
                SqlCommand command = new SqlCommand(call, connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter
                command.Parameters.AddWithValue(Value, userName);

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




        public bool CheckCitizenExistence(byte[] bankIdHash, String type, out string message)
        {
            bool exists = false; // This flag will be used to store the existence status
            message = ""; // Initialize the message string

            String call = "";

            if (type.Equals("User"))
            {
                call = "pk.uspCheckCitizenExistence";
            }
            else if (type.Equals("Admin"))
            {
                call = "pk.uspCheckAdminExistence";
            }

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection logic
            {
                SqlCommand command = new SqlCommand(call, connection);
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
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlParameter param1 = new SqlParameter("@BankIdHash", bankIdHash);
                SqlParameter param2 = new SqlParameter("@CountyName", countyName);

                return DataAdapterHelper.ExecuteProcedureForDataTable(connection, "GetProposalData", param1, param2);
            }
        }

        public string GetCountyByBankIdHash(byte[] bankIdHash)
        {
            string countyName = null; // Initialize to null. Will store the county name if found.

            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
            {
                SqlCommand command = new SqlCommand("pk.uspGetCountyByBankIdHash", connection);
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
                SqlCommand command = new SqlCommand("pk.uspCreateUser", connection);
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

        public void CreateAdmin(byte[] bankIdHash, string AdminName, byte[] salt)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection()) // Replace with your actual connection handling logic
            {
                SqlCommand command = new SqlCommand("pk.uspCreateAdmin", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@BankIdHash", bankIdHash);
                command.Parameters.AddWithValue("@Salt", salt);
                command.Parameters.AddWithValue("@AdminName", AdminName);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();
            }
        }


        public (string CitizenName, string CountyName) GetCitizenDataByUserHash(byte[] userHash)
        {
            string citizenName = null;
            string countyName = null; // Initialize countyName at the start of the method

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("pk.uspGetUserDetailsByHash", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter
                command.Parameters.Add(new SqlParameter("@UserHash", SqlDbType.VarBinary, 64)).Value = userHash;

                // Output parameters
                SqlParameter nameOutputParameter = new SqlParameter("@UserName", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(nameOutputParameter);

                SqlParameter countyNameOutputParameter = new SqlParameter("@County", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(countyNameOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameters' values
                if (nameOutputParameter.Value != DBNull.Value)
                {
                    citizenName = (string)nameOutputParameter.Value;
                }
                if (countyNameOutputParameter.Value != DBNull.Value)
                {
                    countyName = (string)countyNameOutputParameter.Value;
                }
            }

            return (citizenName, countyName);
        }

        public string GetAdminDataByUserHash(byte[] userHash)
        {
            string adminName = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("pk.uspGetAdminDetailsByHash", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Input parameter
                SqlParameter userHashParameter = new SqlParameter("@UserHash", SqlDbType.VarBinary, 64)
                {
                    Value = userHash
                };
                command.Parameters.Add(userHashParameter);

                // Output parameter for Admin Name
                SqlParameter nameOutputParameter = new SqlParameter("@UserName", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(nameOutputParameter);

                connection.Open();

                // Execute the stored procedure
                command.ExecuteNonQuery();

                // Retrieve the output parameter's value
                if (nameOutputParameter.Value != DBNull.Value)
                {
                    adminName = (string)nameOutputParameter.Value;
                }
            }

            return adminName;
        }


        public int deleteUser(byte[] id)
        {

            int success = 0;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("pk.uspDeleteCitizen", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add the parameters
                command.Parameters.AddWithValue("@BankIdHash", id);

                connection.Open();
                success = command.ExecuteNonQuery();
            }

            return success;
        }

        public int UpdateCountyNameByBankIDHash(byte[] bankIDHash, string newCountyName)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                SqlCommand command = new SqlCommand("uspUpdateCountyNameByBankIDHash", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@BankIDHash", bankIDHash);
                command.Parameters.AddWithValue("@NewCountyName", newCountyName);

                // Add return parameter
                SqlParameter returnParameter = new SqlParameter("@Result", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(returnParameter);

                connection.Open();
                command.ExecuteNonQuery();

                // Capture the return value from the stored procedure
                int returnValue = (int)returnParameter.Value;

                return returnValue;
            }
        }



    }
}
