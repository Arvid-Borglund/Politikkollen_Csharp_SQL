using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DataAdapterHelper
    {

        public static DataTable ExecuteProcedureForDataTable(SqlConnection connection, string procedureName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }


        public static SqlTransaction BeginTransaction(SqlConnection connection)
        {
            return connection.BeginTransaction();
        }

        public static void CommitTransaction(SqlTransaction transaction)
        {
            transaction.Commit();
        }

        public static void RollbackTransaction(SqlTransaction transaction)
        {
            transaction.Rollback();
        }

        public static SqlTransaction BeginTransactionWithIsolationLevel(SqlConnection connection, IsolationLevel isolationLevel)
        {
            return connection.BeginTransaction(isolationLevel);
        }



    }
}
