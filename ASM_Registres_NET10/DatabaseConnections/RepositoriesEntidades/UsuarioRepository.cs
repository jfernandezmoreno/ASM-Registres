using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class UsuarioRepository : IUsuario
    {
        private readonly NPGSQLService _npgsqlService;

        public UsuarioRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public int getActualLevel(int usuario_id)
        {
            string query = "SELECT nivel FROM businesscentralsync.niveles WHERE usuario_id = @usuario_id AND aplicacion_id = 3";

            var parameters = new Dictionary<string, object>
            {
                { "@usuario_id", usuario_id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => (int?)reader.GetInt32(0)).FirstOrDefault() ?? 0;
        }

        public string GetPasswordHashByEmail(string email)
        {
            string query = "SELECT password FROM businesscentralsync.usuarios WHERE email = @Email";

            var parameters = new Dictionary<string, object>
            {
                { "@Email", email }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => reader.GetString(0)).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            string query = "SELECT id, name, email, departamento FROM businesscentralsync.usuarios WHERE email = @Email";

            var parameters = new Dictionary<string, object>
            {
                { "@Email", email }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Departament = reader.GetString(reader.GetOrdinal("departamento"))
            }).FirstOrDefault();
        }

        public bool UpdateUserPassword(int userId, string newPassword)
        {
            string query = "UPDATE businesscentralsync.usuarios SET password = @password WHERE id = @id";

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            var parameters = new Dictionary<string, object>
            {
                { "@password", hashedPassword },
                { "@id", userId }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);

            return true;
        }

        public List<string> GetIniciales()
        {
            List<string> inicialesList = new List<string>();

            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT iniciales FROM businesscentralsync.employees", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string iniciales = reader.GetString(0);
                            inicialesList.Add(iniciales);
                        }
                    }
                }
            }

            return inicialesList;
        }

        public string GetNameByUserId(int id)
        {
            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            using (var command = new NpgsqlCommand(
                "SELECT name FROM businesscentralsync.usuarios WHERE id = @id", connection))
            {
                command.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                connection.Open();

                var result = command.ExecuteScalar();
                return (result == null || result == DBNull.Value) ? null : Convert.ToString(result);
            }
        }

    }
}
