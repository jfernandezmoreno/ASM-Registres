using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Modules.Tasks;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks
{
    public class LabTasksRecurrentRepository : ITasksRecurrentLab
    {
        public List<Tasks> GetAllTareas()
        {
            var tareas = new List<Tasks>();

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    @"SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift 
                      FROM tasks_schema.laboratory_tasks 
                      WHERE day BETWEEN @startDate AND @endDate", conn))
                {
                    var startDate = DateTime.Today.AddDays(-7);
                    var endDate = DateTime.Today.AddDays(7);

                    cmd.Parameters.AddWithValue("startDate", startDate);
                    cmd.Parameters.AddWithValue("endDate", endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tarea = new Tasks
                            {
                                Id = reader.GetInt32(0),
                                Dia = reader.GetDateTime(1),
                                Tipo = reader.GetString(2),
                                Titulo = reader.GetString(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Comentario = reader.IsDBNull(5) ? null : reader.GetString(5),
                                PersonaAsignada = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Prioridad = reader.GetInt32(7),
                                Completada = reader.GetBoolean(8),
                                Turno = reader.GetString(9)
                            };
                            tareas.Add(tarea);
                        }
                    }
                }
            }
            return tareas;
        }
        public Tasks GetTareaById(int id)
        {
            Tasks tarea = null;

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift FROM tasks_schema.laboratory_tasks WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tarea = new Tasks
                            {
                                Id = reader.GetInt32(0),
                                Dia = reader.GetDateTime(1),
                                Tipo = reader.GetString(2),
                                Titulo = reader.GetString(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Comentario = reader.IsDBNull(5) ? null : reader.GetString(5),
                                PersonaAsignada = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Prioridad = reader.GetInt32(7),
                                Completada = reader.GetBoolean(8),
                                Turno = reader.GetString(9)

                            };
                        }
                    }
                }
            }
            return tarea;
        }
        public void AddTask(Tasks newTask)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO tasks_schema.laboratory_tasks
              (day, type, title, description, comments, assigned_person, priority, completed, shift) 
              VALUES (@dia, @tipo, @titulo, @descripcion, @comentario, @persona_asignada, @prioridad, false, @turno)", conn))
                {
                    cmd.Parameters.AddWithValue("dia", newTask.Dia);
                    cmd.Parameters.AddWithValue("tipo", newTask.Tipo);
                    cmd.Parameters.AddWithValue("titulo", newTask.Titulo);
                    cmd.Parameters.AddWithValue("descripcion", newTask.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("comentario", newTask.Comentario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("persona_asignada", newTask.PersonaAsignada ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("prioridad", newTask.Prioridad);
                    cmd.Parameters.AddWithValue("turno", newTask.Turno);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteTask(int id)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "DELETE FROM tasks_schema.laboratory_tasks WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateTask(Tasks updatedTask)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"UPDATE tasks_schema.laboratory_tasks 
                     SET day = @dia, 
                      type = @tipo,     
                      title = @titulo, 
                      description = @descripcion, 
                      comments = @comentario, 
                      assigned_person = @persona_asignada, 
                      priority = @prioridad,
                      completed = @completada,
                      shift = @turno
                     WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("dia", updatedTask.Dia);
                    cmd.Parameters.AddWithValue("tipo", updatedTask.Tipo);
                    cmd.Parameters.AddWithValue("titulo", updatedTask.Titulo);
                    cmd.Parameters.AddWithValue("descripcion", updatedTask.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("comentario", updatedTask.Comentario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("persona_asignada", updatedTask.PersonaAsignada ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("prioridad", updatedTask.Prioridad);
                    cmd.Parameters.AddWithValue("id", updatedTask.Id);
                    cmd.Parameters.AddWithValue("completada", updatedTask.Completada);
                    cmd.Parameters.AddWithValue("turno", updatedTask.Turno);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Tasks> GetTareasByFecha(DateTime fecha)
        {
            var tareas = new List<Tasks>();

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift FROM tasks_schema.laboratory_tasks WHERE day = @fecha", conn))
                {
                    cmd.Parameters.AddWithValue("fecha", fecha);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tarea = new Tasks
                            {
                                Id = reader.GetInt32(0),
                                Dia = reader.GetDateTime(1),
                                Tipo = reader.GetString(2),
                                Titulo = reader.GetString(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Comentario = reader.IsDBNull(5) ? null : reader.GetString(5),
                                PersonaAsignada = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Prioridad = reader.GetInt32(7),
                                Completada = reader.GetBoolean(8),
                                Turno = reader.GetString(9)
                            };
                            tareas.Add(tarea);
                        }
                    }
                }
            }
            return tareas;
        }
        public void UpdateCompletadaEstado(int taskId, bool completada)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE tasks_schema.laboratory_tasks SET completed = @completada WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.Parameters.AddWithValue("@completada", completada);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<string> GetActiveEmployees()
        {
            var activeEmployees = new List<string>();

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"SELECT nombre_usuario 
                      FROM businesscentralsync.employees 
                      WHERE departamento = 'Laboratori' AND activo = 'Activo'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activeEmployees.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return activeEmployees;
        }
        public DateTime GetTareaMasReciente()
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT MAX(day) FROM tasks_schema.laboratory_tasks", conn))
                {
                    var result = cmd.ExecuteScalar();
                    return (result == null || result == DBNull.Value)
                        ? DateTime.MinValue
                        : (DateTime)result;
                }
            }
        }
        public List<Tasks> GetAllTasksByDateTime()
        {
            var tareas = new List<Tasks>();

            var targetDate = GetTareaMasReciente();

            if (targetDate == DateTime.MinValue)
                return tareas;

            var start = targetDate.Date;
            var end = start.AddDays(1);

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift
                      FROM tasks_schema.laboratory_tasks
                      WHERE day >= @start AND day < @end
                      ORDER BY day ASC, priority DESC", conn))
                {
                    cmd.Parameters.AddWithValue("start", start);
                    cmd.Parameters.AddWithValue("end", end);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tarea = new Tasks
                            {
                                Id = reader.GetInt32(0),
                                Dia = reader.GetDateTime(1),
                                Tipo = reader.GetString(2),
                                Titulo = reader.GetString(3),
                                Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Comentario = reader.IsDBNull(5) ? null : reader.GetString(5),
                                PersonaAsignada = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Prioridad = reader.GetInt32(7),
                                Completada = reader.GetBoolean(8),
                                Turno = reader.GetString(9)
                            };
                            tareas.Add(tarea);
                        }
                    }
                }
            }
            return tareas;
        }
    }
}
