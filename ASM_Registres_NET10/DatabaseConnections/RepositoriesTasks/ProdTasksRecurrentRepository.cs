using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Tasks;
using Npgsql;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks
{
    public class ProdTasksRecurrentRepository : ITasksRecurrent
    {
        public List<Tasks> GetAllTareas()
        {
            var tareas = new List<Tasks>();

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(
                    @"SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift,
                     tasca_id, estado, employee_id
              FROM tasks_schema.tasks 
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
                                Turno = reader.GetString(9),
                                Tasca_Id = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                                Estado = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Employee_Id = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12)
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
                    @"SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift, 
                     tasca_id, estado, employee_id
              FROM tasks_schema.tasks 
              WHERE id = @id", conn))
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
                                Turno = reader.GetString(9),
                                Tasca_Id = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                                Estado = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Employee_Id = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12)
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
                    @"INSERT INTO tasks_schema.tasks 
              (day, type, title, description, comments, assigned_person, priority, completed, shift, tasca_id, estado) 
              VALUES (@dia, @tipo, @titulo, @descripcion, @comentario, @persona_asignada, @prioridad, false, @turno, @tasca_id, @estado)", conn))
                {
                    cmd.Parameters.AddWithValue("dia", newTask.Dia);
                    cmd.Parameters.AddWithValue("tipo", newTask.Tipo);
                    cmd.Parameters.AddWithValue("titulo", newTask.Titulo);
                    cmd.Parameters.AddWithValue("descripcion", newTask.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("comentario", newTask.Comentario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("persona_asignada", newTask.PersonaAsignada ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("prioridad", newTask.Prioridad);
                    cmd.Parameters.AddWithValue("turno", newTask.Turno);
                    cmd.Parameters.AddWithValue("tasca_id", (object)newTask.Tasca_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("estado", (object)(newTask.Estado ?? "No empezada"));
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
                    "DELETE FROM tasks_schema.tasks WHERE id = @id", conn))
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
                    @"UPDATE tasks_schema.tasks 
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
                    @"SELECT id, day, type, title, description, comments, assigned_person, priority, completed, shift,
                     tasca_id, estado, employee_id
              FROM tasks_schema.tasks 
              WHERE day = @fecha", conn))
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
                                Turno = reader.GetString(9),
                                Tasca_Id = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                                Estado = reader.IsDBNull(11) ? null : reader.GetString(11),
                                Employee_Id = reader.IsDBNull(12) ? (int?)null : reader.GetInt32(12)
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
                using (var cmd = new NpgsqlCommand("UPDATE tasks_schema.tasks SET completed = @completada WHERE id = @id", conn))
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
                      WHERE departamento = 'Producció' AND activo = 'Activo'", conn))
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
        public void EmpezarTarea(Tasks task, string emp)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                UPDATE tasks_schema.tasks
                SET estado = 'Empezada',
                   assigned_person = @emp,
                   completed = FALSE
                WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", task.Id);
                    cmd.Parameters.AddWithValue("emp", (object)emp ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void FinalizarTarea(Tasks task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                UPDATE tasks_schema.tasks
                SET estado = 'Terminada',
                    completed = TRUE
                WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", task.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AbordarTarea(Tasks task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            UPDATE tasks_schema.tasks
               SET estado = 'No acabada',
                   assigned_person = COALESCE(@emp, assigned_person),
                   completed = FALSE
             WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", task.Id);
                    cmd.Parameters.AddWithValue("emp", (object)task.PersonaAsignada ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void EstablecerTareaComoNoRealizada(Tasks task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            UPDATE tasks_schema.tasks
               SET estado = 'No realizada',
                   completed = FALSE
             WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", task.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool CheckEmpleadoExisteEnGesAnextia(string emp)
        {
            /* Se deja para Additius ya que aqui no tengo el archivo */
            return true;
        }

        public string GetEmployeeNameById(int employeeId)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"SELECT nombre_usuario
                          FROM businesscentralsync.employees
                          WHERE id_usuario = @id
                          LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("id", employeeId);
                    var result = cmd.ExecuteScalar();
                    return result == null || result is DBNull ? null : result.ToString();
                }
            }
        }

        public string GetRegistroTextoByTascaId(int tascaId)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    @"SELECT titol, zona_o_equip
                          FROM registros_app.tasques
                          WHERE id = @id
                          LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("id", tascaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string titol = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            string zona = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            return string.IsNullOrWhiteSpace(zona) ? titol : $"{titol} - {zona}";
                        }
                    }
                }
            }
            return null;
        }

        public bool CheckEmpleadoExisteEnBBDD(string emp)
        {
            if (string.IsNullOrWhiteSpace(emp)) return false;
            var valor = emp.Trim();

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT EXISTS(SELECT 1 FROM businesscentralsync.employees WHERE clave = @clave);", conn))
                {
                    cmd.Parameters.AddWithValue("clave", valor);
                    var result = cmd.ExecuteScalar();
                    return result is bool b && b;
                }
            }
        }

        public void AsignarRegistroATarea(int idRegistro, int idTarea)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    UPDATE tasks_schema.tasks
                       SET tasca_id = @idRegistro
                     WHERE id = @idTarea;", conn))
                {
                    cmd.Parameters.AddWithValue("idRegistro", idRegistro);
                    cmd.Parameters.AddWithValue("idTarea", idTarea);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Empleados GetEmployeeByClave(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                return null;

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    SELECT id_usuario, nombre_usuario, password, iniciales, activo, clave
                      FROM businesscentralsync.employees
                     WHERE clave = @clave
                       AND activo = 'Activo'
                     LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("clave", clave.Trim());

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Ajusta los índices/nombres si tu tabla usa otros campos
                            int idUsuario = reader.GetInt32(0);
                            string nombre = reader.IsDBNull(1) ? null : reader.GetString(1);
                            string password = reader.IsDBNull(2) ? null : reader.GetString(2);
                            string iniciales = reader.IsDBNull(3) ? null : reader.GetString(3);
                            string activo = reader.IsDBNull(4) ? null : reader.GetString(4);
                            string claveValue = reader.IsDBNull(5) ? null : reader.GetString(5);

                            return new Empleados(idUsuario, nombre, password, iniciales, activo, claveValue);
                        }
                    }
                }
            }

            return null;
        }

        public void EmpezarTareaConEmpleado(int idTarea, int employeeId, string nombreEmpleado)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    UPDATE tasks_schema.tasks
                       SET estado = 'Empezada',
                           assigned_person = @emp_name,
                           employee_id = @emp_id,
                           completed = FALSE
                     WHERE id = @task_id;", conn))
                {
                    cmd.Parameters.AddWithValue("emp_name", (object)nombreEmpleado ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("emp_id", employeeId);
                    cmd.Parameters.AddWithValue("task_id", idTarea);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void FinalizarTareaSoloEstado(int idTarea)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    UPDATE tasks_schema.tasks
                       SET estado = 'Terminada',
                           completed = TRUE
                     WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", idTarea);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EmpezarTareaSoloEstado(int idTarea)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            UPDATE tasks_schema.tasks
               SET estado = 'Empezada',
                   completed = FALSE,
                   assigned_person = NULL,  -- limpiar asignación
                   employee_id = NULL       -- limpiar asignación
             WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", idTarea);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void FinalizarTareaConEmpleado(int idTarea, int employeeId, string nombreEmpleado)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            UPDATE tasks_schema.tasks
               SET estado = 'Terminada',
                   completed = TRUE,
                   assigned_person = @emp_name,
                   employee_id = @emp_id
             WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("id", idTarea);
                    cmd.Parameters.AddWithValue("emp_id", employeeId);
                    cmd.Parameters.AddWithValue("emp_name", (object)nombreEmpleado ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Empleados GetEmployeeById(int employeeId)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT id_usuario, nombre_usuario, password, iniciales, activo, clave
              FROM businesscentralsync.employees
             WHERE id_usuario = @id
             LIMIT 1;", conn))
                {
                    cmd.Parameters.AddWithValue("id", employeeId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int idUsuario = reader.GetInt32(0);
                            string nombre = reader.IsDBNull(1) ? null : reader.GetString(1);
                            string password = reader.IsDBNull(2) ? null : reader.GetString(2);
                            string iniciales = reader.IsDBNull(3) ? null : reader.GetString(3);
                            string activo = reader.IsDBNull(4) ? null : reader.GetString(4);
                            string clave = reader.IsDBNull(5) ? null : reader.GetString(5);

                            return new Empleados(idUsuario, nombre, password, iniciales, activo, clave);
                        }
                    }
                }
            }

            return null;
        }




    }
}
