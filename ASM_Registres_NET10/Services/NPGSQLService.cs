using ASM_Registres_NET10.Constants;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.Services
{
    public class NPGSQLService
    {
        private readonly string _connectionString;

        public NPGSQLService(string ConnectionStringLocal)
        {
            this._connectionString = ConnectionStringLocal;
        }

        public List<T> ExecuteQuery<T>(string query, Dictionary<string, object> parameters, Func<NpgsqlDataReader, T> mapFunction)
        {
            var results = new List<T>();

            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(mapFunction(reader));
                        }
                    }
                }
            }

            return results;
        }

        public void ExecuteNonQuery(string query, Dictionary<string, object> parameters, NpgsqlTransaction transaction = null)
        {
            var connection = transaction?.Connection ?? new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal);

            try
            {
                if (transaction == null) connection.Open();

                using (var command = new NpgsqlCommand(query, connection, transaction))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                    }

                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                if (transaction == null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public void ExecuteTransaction(Action<NpgsqlTransaction> transactionalOperations)
        {
            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        transactionalOperations(transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public object ExecuteScalar(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                    }

                    return command.ExecuteScalar();
                }
            }
        }
    }
}
