using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;

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

                    return DataAdapterHelper.ExecuteProcedureForDataTable(connection, "pk.uspGetAllCounties");

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
                SqlTransaction transaction = null;
                try
                {
                    connection.Open();

                    // Begin the transaction
                    transaction = DataAdapterHelper.BeginTransaction(connection);

                    SqlCommand command = new SqlCommand("pk.uspDeleteCounty", connection, transaction);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the CountyName parameter to the command
                    command.Parameters.AddWithValue("@CountyName", countyName);

                    // Add a return parameter
                    SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnParameter);

                    command.ExecuteNonQuery();

                    // Commit the transaction
                    DataAdapterHelper.CommitTransaction(transaction);

                    // Get the return value
                    int result = (int)command.Parameters["@ReturnVal"].Value;

                    return result;
                }
                catch (SqlException ex)
                {
                    // Rollback the transaction in case of any errors
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    // Pass the exception to the ErrorHandler
                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling, so it can be caught and managed outside if needed
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of any errors
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    // Handle general exceptions
                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }
        }


        public DataTable GetAllProposals()
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {

                try
                {
                    return DataAdapterHelper.ExecuteProcedureForDataTable(connection, "pk.uspGetProposalPrimaryKeys");
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


        public int CreateProposal(string countyName, string proposal, string info)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                connection.Open();

                // Start the transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("pk.uspCreateProposal", connection, transaction);
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

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Commit the transaction
                        DataAdapterHelper.CommitTransaction(transaction);

                        // Retrieve the output parameter's value
                        if (resultOutputParameter.Value != DBNull.Value)
                        {
                            return (int)resultOutputParameter.Value;
                        }

                        // Handle the case where the output parameter is DBNull.Value (null)
                        return -1; // You can choose an appropriate error code or value here
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Pass the exception to the ErrorHandler
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
                }
            }
        }



        public int DeleteProposal(string countyName, string proposal)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                connection.Open();

                // Start the transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("pk.uspDeleteProposal", connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameters
                        command.Parameters.AddWithValue("@CountyName", countyName);
                        command.Parameters.AddWithValue("@Proposal", proposal);

                        // Execute the stored procedure
                        int rowsAffected = command.ExecuteNonQuery();

                        // Commit the transaction
                        DataAdapterHelper.CommitTransaction(transaction);

                        return rowsAffected; // This should be 1 if the deletion was successful, or 0 if no rows were affected.
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Pass the exception to the ErrorHandler
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
                }
            }
        }


        public int EditProposal(string countyName, string proposal, string newInfo)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                connection.Open();

                // Start the transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("pk.uspEditProposal", connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@CountyName", countyName);
                        command.Parameters.AddWithValue("@Proposal", proposal);
                        command.Parameters.AddWithValue("@NewInfo", newInfo);

                        // Add return parameter
                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add(returnParameter);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Commit the transaction
                        DataAdapterHelper.CommitTransaction(transaction);

                        // Capture the return value from the stored procedure
                        int returnValue = (int)returnParameter.Value;

                        return returnValue; // Return the result
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Pass the exception to the ErrorHandler
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
                }
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
                connection.Open();

                // Start the transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(call, connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter
                        command.Parameters.AddWithValue(Value, userName);

                        // Output parameter
                        SqlParameter saltOutputParameter = new SqlParameter("@Salt", SqlDbType.VarBinary, 64);
                        saltOutputParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(saltOutputParameter);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Commit the transaction
                        DataAdapterHelper.CommitTransaction(transaction);

                        // Retrieve the output parameter value
                        if (saltOutputParameter.Value != DBNull.Value)
                        {
                            salt = (byte[])saltOutputParameter.Value;
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Pass the exception to the ErrorHandler
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
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
                connection.Open();

                // Start the transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(call, connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for BankIdHash
                        command.Parameters.AddWithValue("@BankIdHash", bankIdHash);

                        // Output parameters for Exists and Message
                        SqlParameter existsOutputParameter = new SqlParameter("@Exists", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(existsOutputParameter);

                        SqlParameter messageOutputParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(messageOutputParameter);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Commit the transaction
                        DataAdapterHelper.CommitTransaction(transaction);

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
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Pass the exception to the ErrorHandler
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
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


                return DataAdapterHelper.ExecuteProcedureForDataTable(connection, "pk.uspGetProposalData", param1, param2);

            }
        }


        public string GetCountyByBankIdHash(byte[] bankIdHash)
        {
            string countyName = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                connection.Open();

                // Begin a new transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("pk.uspGetCountyByBankIdHash", connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter
                        command.Parameters.AddWithValue("@BankIdHash", bankIdHash);

                        // Output parameter
                        SqlParameter countyNameOutputParameter = new SqlParameter("@CountyName", SqlDbType.VarChar, 255);
                        countyNameOutputParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(countyNameOutputParameter);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Retrieve the output parameter value
                        if (countyNameOutputParameter.Value != DBNull.Value)
                        {
                            countyName = countyNameOutputParameter.Value.ToString();
                        }

                        // Commit the transaction if all operations are successful
                        DataAdapterHelper.CommitTransaction(transaction);
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle the SQL exception
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
                }
            }

            return countyName; // Return the found county name or null if not found
        }


        public void CreateUser(byte[] bankIdHash, string userName, byte[] salt, string county)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                connection.Open();

                // Begin a new transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("pk.uspCreateUser", connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@BankIdHash", bankIdHash);
                        command.Parameters.AddWithValue("@Salt", salt);
                        command.Parameters.AddWithValue("@UserName", userName);

                        // Check for county value and provide the appropriate value to the command
                        command.Parameters.AddWithValue("@County", county ?? (object)DBNull.Value);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Commit the transaction if all operations are successful
                        DataAdapterHelper.CommitTransaction(transaction);
                    }
                    catch (SqlException ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle the SQL exception
                        ErrorHandler.HandleSqlException(ex);
                        throw; // Rethrow the exception after handling
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Handle general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // Rethrow the exception after handling
                    }
                }
            }
        }


        public void CreateAdmin(byte[] bankIdHash, string adminName, byte[] salt)
        {
            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                connection.Open();

                // Start a new transaction
                using (SqlTransaction transaction = DataAdapterHelper.BeginTransaction(connection))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("pk.uspCreateAdmin", connection, transaction);
                        command.CommandType = CommandType.StoredProcedure;

                        // Set the parameters
                        command.Parameters.AddWithValue("@BankIdHash", bankIdHash);
                        command.Parameters.AddWithValue("@Salt", salt);
                        command.Parameters.AddWithValue("@AdminName", adminName);

                        // Run the stored procedure
                        command.ExecuteNonQuery();

                        // If all operations are successful, commit the transaction
                        DataAdapterHelper.CommitTransaction(transaction);
                    }
                    catch (SqlException ex)
                    {
                        // In the event of an error, rollback the transaction
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Manage the SQL exception
                        ErrorHandler.HandleSqlException(ex);
                        throw; // After handling, rethrow the exception 
                    }
                    catch (Exception ex)
                    {
                        // In the event of an error, rollback the transaction
                        DataAdapterHelper.RollbackTransaction(transaction);

                        // Address general exceptions
                        ErrorHandler.HandleException(ex);
                        throw; // After handling, rethrow the exception 
                    }
                }
            }
        }



        public (string CitizenName, string CountyName) GetCitizenDataByUserHash(byte[] userHash)
        {
            string citizenName = null;
            string countyName = null;
            SqlTransaction transaction = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                try
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

                    // Begin a transaction using the helper method
                    transaction = DataAdapterHelper.BeginTransaction(connection);
                    command.Transaction = transaction; // Set the transaction for the command

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    // Commit the transaction using the helper method
                    DataAdapterHelper.CommitTransaction(transaction);

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
                catch (SqlException ex)
                {
                    // Handle the SQL exception and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }

            return (citizenName, countyName);
        }


        public string GetAdminDataByUserHash(byte[] userHash)
        {
            string adminName = null;
            SqlTransaction transaction = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                try
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

                    // Begin a transaction using the helper method
                    transaction = DataAdapterHelper.BeginTransaction(connection);
                    command.Transaction = transaction; // Set the transaction for the command

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    // Commit the transaction using the helper method
                    DataAdapterHelper.CommitTransaction(transaction);

                    // Retrieve the output parameter's value
                    if (nameOutputParameter.Value != DBNull.Value)
                    {
                        adminName = (string)nameOutputParameter.Value;
                    }
                }
                catch (SqlException ex)
                {
                    // Handle the SQL exception and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }

            return adminName;
        }



        public int DeleteUser(byte[] id)
        {
            if (id == null || id.Length == 0)
            {
                throw new ArgumentNullException(nameof(id), "ID cannot be null or empty.");
            }

            int result = 0;
            SqlTransaction transaction = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                try
                {
                    SqlCommand command = new SqlCommand("pk.uspDeleteCitizen", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the parameters
                    command.Parameters.AddWithValue("@BankIdHash", id);

                    // Add output parameter
                    SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();

                    // Begin a transaction using the helper method
                    transaction = DataAdapterHelper.BeginTransaction(connection);
                    command.Transaction = transaction; // Set the transaction for the command

                    command.ExecuteNonQuery();

                    // Commit the transaction using the helper method
                    DataAdapterHelper.CommitTransaction(transaction);

                    // Retrieve the value of the output parameter
                    if (outputParam.Value != DBNull.Value)
                    {
                        result = (int)outputParam.Value;
                    }
                }
                catch (SqlException ex)
                {
                    // Handle the SQL exception and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }

            return result;
        }



        public int UpdateCountyNameByBankIDHash(byte[] bankIDHash, string newCountyName)
        {
            if (bankIDHash == null || bankIDHash.Length == 0)
            {
                throw new ArgumentNullException(nameof(bankIDHash), "Bank ID hash cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(newCountyName))
            {
                throw new ArgumentException("New county name cannot be null or empty.", nameof(newCountyName));
            }

            int returnValue = 0;
            SqlTransaction transaction = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {

                try
                {
                    

                SqlCommand command = new SqlCommand("pk.uspUpdateCountyNameByBankIDHash", connection);
                command.CommandType = CommandType.StoredProcedure;


                    // Add parameters
                    command.Parameters.AddWithValue("@BankIDHash", bankIDHash);
                    command.Parameters.AddWithValue("@NewCountyName", newCountyName);

                    // Add return parameter
                    SqlParameter returnParameter = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(returnParameter);

                    connection.Open();

                    // Begin a transaction using the helper method
                    transaction = DataAdapterHelper.BeginTransaction(connection);
                    command.Transaction = transaction; // Set the transaction for the command

                    command.ExecuteNonQuery();

                    // Commit the transaction using the helper method
                    DataAdapterHelper.CommitTransaction(transaction);

                    // Capture the return value from the stored procedure
                    if (returnParameter.Value != DBNull.Value)
                    {
                        returnValue = (int)returnParameter.Value;
                    }
                }
                catch (SqlException ex)
                {
                    // Handle the SQL exception and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }

            return returnValue;
        }


        public int SaveOpinion(byte[] bankIDHash, string proposal, string countyName, int voteFor, int voteAgainst)
        {
            ValidateParameters(bankIDHash, proposal, countyName, voteFor, voteAgainst);

            int returnValue = 0;
            SqlTransaction transaction = null;

            using (SqlConnection connection = ConnectionHandler.GetConnection())
            {
                try
                {
                    SqlCommand command = CreateCommand(connection, bankIDHash, proposal, countyName, voteFor, voteAgainst);

                    connection.Open();

                    // Begin a transaction using the helper method
                    transaction = DataAdapterHelper.BeginTransaction(connection);
                    command.Transaction = transaction; // Set the transaction for the command


                    command.ExecuteNonQuery();

                    // Commit the transaction using the helper method
                    DataAdapterHelper.CommitTransaction(transaction);

                    // Capture the return value from the stored procedure
                    returnValue = (int)command.Parameters["@Result"].Value;
                }
                catch (SqlException ex)
                {
                    // Handle the SQL exception and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);



                    ErrorHandler.HandleSqlException(ex);
                    throw; // Rethrow the exception after handling
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions and rollback
                    if (transaction != null)
                        DataAdapterHelper.RollbackTransaction(transaction);

                    ErrorHandler.HandleException(ex);
                    throw; // Rethrow the exception after handling
                }
            }

            return returnValue;
        }

        private void ValidateParameters(byte[] bankIDHash, string proposal, string countyName, int voteFor, int voteAgainst)
        {
            if (bankIDHash == null || bankIDHash.Length == 0)
            {
                throw new ArgumentNullException(nameof(bankIDHash), "Bank ID hash cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(proposal))
            {
                throw new ArgumentException("Proposal cannot be null or empty.", nameof(proposal));
            }

            if (string.IsNullOrEmpty(countyName))
            {
                throw new ArgumentException("County name cannot be null or empty.", nameof(countyName));
            }

            // Additional validations can be added here if needed
        }

        private SqlCommand CreateCommand(SqlConnection connection, byte[] bankIDHash, string proposal, string countyName, int voteFor, int voteAgainst)
        {
            SqlCommand command = new SqlCommand("pk.uspSaveOpinion", connection);
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.AddWithValue("@BankIdHash", bankIDHash);
            command.Parameters.AddWithValue("@Proposal", proposal);
            command.Parameters.AddWithValue("@CountyName", countyName);
            command.Parameters.AddWithValue("@VoteFor", voteFor);
            command.Parameters.AddWithValue("@VoteAgainst", voteAgainst);

            // Add return parameter
            SqlParameter returnParameter = new SqlParameter("@Result", SqlDbType.Int)
            {
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(returnParameter);

            

            return command;
        }

        




    }
}
