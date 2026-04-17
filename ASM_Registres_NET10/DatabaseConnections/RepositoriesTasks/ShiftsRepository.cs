using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Modules;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks
{
    public class ShiftsRepository
    {

        public ShiftsRepository() { }

        public void InsertarOActualizarRegistro(RegistroTurno registro)
        {
            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();

                string checkQuery = @"
                SELECT id_shift 
                FROM tasks_schema.shift_log 
                WHERE day = @dia AND id_user = @id_usuario";

                using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.Add(new NpgsqlParameter("@dia", NpgsqlTypes.NpgsqlDbType.Date) { Value = registro.date });
                    checkCommand.Parameters.Add(new NpgsqlParameter("@id_usuario", NpgsqlTypes.NpgsqlDbType.Integer) { Value = registro.IdUsuario });

                    var result = checkCommand.ExecuteScalar();

                    if (result == null || result.ToString() == "Sin asignar")
                    {
                        string insertQuery = @"
                        INSERT INTO tasks_schema.shift_log (day, id_user, id_shift, user_name)
                        VALUES (@dia, @id_usuario, @id_turno, @nombre_usuario)";

                        using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.Add(new NpgsqlParameter("@dia", NpgsqlTypes.NpgsqlDbType.Date) { Value = registro.date });
                            insertCommand.Parameters.Add(new NpgsqlParameter("@id_usuario", NpgsqlTypes.NpgsqlDbType.Integer) { Value = registro.IdUsuario });
                            insertCommand.Parameters.Add(new NpgsqlParameter("@id_turno", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = registro.IdTurno });
                            insertCommand.Parameters.Add(new NpgsqlParameter("@nombre_usuario", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = registro.NombreUsuario });

                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string updateQuery = @"
                        UPDATE tasks_schema.shift_log 
                        SET id_shift = @id_turno, user_name = @nombre_usuario
                        WHERE day = @dia AND id_user = @id_usuario";

                        using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.Add(new NpgsqlParameter("@dia", NpgsqlTypes.NpgsqlDbType.Date) { Value = registro.date });
                            updateCommand.Parameters.Add(new NpgsqlParameter("@id_usuario", NpgsqlTypes.NpgsqlDbType.Integer) { Value = registro.IdUsuario });
                            updateCommand.Parameters.Add(new NpgsqlParameter("@id_turno", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = registro.IdTurno });
                            updateCommand.Parameters.Add(new NpgsqlParameter("@nombre_usuario", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = registro.NombreUsuario });

                            int rowsAffected = updateCommand.ExecuteNonQuery();

                        }
                    }
                }
            }
        }
        public RegistroTurno GetRegistroById(int idRegistro)
        {
            RegistroTurno registro = null;

            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();

                string query = @"
                SELECT id_register, day, id_user, id_shift, user_name
                FROM tasks_schema.shift_log 
                WHERE id_register = @idRegistro";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idRegistro", idRegistro);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            registro = new RegistroTurno(
                                reader.GetInt32(reader.GetOrdinal("id_register")),
                                reader.GetDateTime(reader.GetOrdinal("day")),
                                reader.GetInt32(reader.GetOrdinal("id_user")),
                                reader.GetString(reader.GetOrdinal("id_shift")),
                                reader.IsDBNull(reader.GetOrdinal("user_name")) ? null : reader.GetString(reader.GetOrdinal("user_name"))
                            );
                        }
                    }
                }
            }

            return registro;
        }
        public void EliminarRegistrosPorFecha(DateTime fecha)
        {
            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();
                string query = "DELETE FROM tasks_schema.shift_log WHERE id_shift IN (SELECT id_shift FROM tasks_schema.shift WHERE day = @dia)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dia", fecha);
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<RegistroTurno> GetRegistrosPorFecha(DateTime fecha)
        {
            List<RegistroTurno> registros = new List<RegistroTurno>();

            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();

                string query = @"
                SELECT id_register, day, id_user, id_shift, user_name 
                FROM tasks_schema.shift_log 
                WHERE day = @fecha";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.Add(new NpgsqlParameter("@fecha", NpgsqlTypes.NpgsqlDbType.Date) { Value = fecha });

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RegistroTurno registro = new RegistroTurno(
                                reader.GetInt32(reader.GetOrdinal("id_register")),
                                reader.GetDateTime(reader.GetOrdinal("day")),
                                reader.GetInt32(reader.GetOrdinal("id_user")),
                                reader.GetString(reader.GetOrdinal("id_shift")),
                                reader.GetString(reader.GetOrdinal("user_name"))
                            );

                            registros.Add(registro);
                        }
                    }
                }
            }

            return registros;
        }
        public List<Empleados> GetAllUsuarios()
        {
            var usuarios = new List<Empleados>();

            using (var connection = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT id_usuario, nombre_usuario, password, iniciales, activo FROM businesscentralsync.employees WHERE departamento = 'Producció'", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new Empleados(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetString(4)
                            );

                            if (usuario.Activo == "Activo")
                            {
                                usuarios.Add(usuario);
                            }
                        }
                    }
                }
            }
            return usuarios;
        }
    }
}
