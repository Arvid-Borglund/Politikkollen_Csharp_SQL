using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class ErrorHandler
    {
        // Handle SQL exceptions
        public static void HandleSqlException(SqlException ex)
        {
            // You can add more sophisticated logging or other handling mechanisms here

            // Example: Differentiate message based on error number
            switch (ex.Number)
            {
                case 2627:  // Unique constraint error
                    throw new ApplicationException("The record already exists in the database.", ex);
                case 547:   // Constraint check violation
                    throw new ApplicationException("A constraint was violated while processing your request.", ex);
                default:
                    throw new ApplicationException($"A database error occurred: {ex.Message}", ex);
            }
        }

        // Handle general exceptions
        public static void HandleException(Exception ex)
        {
            // Again, you can add logging or other handling mechanisms here
            throw new ApplicationException($"An error occurred: {ex.Message}", ex);
        }
    }
}
