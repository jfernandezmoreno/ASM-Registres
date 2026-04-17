using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Modules.Tasks;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks
{
    public class TaskParticipationRepository : ITaskParticipationRepository
    {
        public void EndAllOpenByTask(int taskId, DateTime? fin = null)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    UPDATE tasks_schema.task_participations
                       SET fecha_fin = COALESCE(@fin, NOW())
                     WHERE id_tasca = @tid
                       AND fecha_fin IS NULL;", conn))
                {
                    cmd.Parameters.AddWithValue("tid", taskId);
                    cmd.Parameters.AddWithValue("fin", (object)(fin.HasValue ? (object)fin.Value : DBNull.Value));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EndOpenByTaskAndEmployee(int taskId, int employeeId, DateTime? fin = null, string observacionesAppend = null)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            UPDATE tasks_schema.task_participations
               SET fecha_fin = COALESCE(@fin, NOW()),
                   observaciones =
                       CASE
                           WHEN COALESCE(@obs, '') = '' THEN observaciones
                           WHEN observaciones IS NULL OR observaciones = '' THEN @obs
                           ELSE observaciones || E'\n' || @obs
                       END
             WHERE id_tasca = @tid
               AND id_empleado = @eid
               AND fecha_fin IS NULL;", conn))
                {
                    // Tipado explícito para evitar 42P08
                    var pTid = cmd.Parameters.Add("tid", NpgsqlDbType.Integer);
                    pTid.Value = taskId;

                    var pEid = cmd.Parameters.Add("eid", NpgsqlDbType.Integer);
                    pEid.Value = employeeId;

                    var pFin = cmd.Parameters.Add("fin", NpgsqlDbType.Timestamp);
                    pFin.Value = fin.HasValue ? (object)fin.Value : DBNull.Value;

                    var pObs = cmd.Parameters.Add("obs", NpgsqlDbType.Text);
                    pObs.Value = (object)observacionesAppend ?? DBNull.Value;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<TaskParticipationView> GetByTaskId(int taskId)
        {
            var list = new List<TaskParticipationView>();

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT p.id,
                   p.id_empleado,                        
                   e.nombre_usuario AS empleado,
                   p.fecha_inicio,
                   p.fecha_fin
              FROM tasks_schema.task_participations p
              JOIN businesscentralsync.employees e
                ON e.id_usuario = p.id_empleado
             WHERE p.id_tasca = @tid
             ORDER BY p.fecha_inicio ASC;", conn))
                {
                    cmd.Parameters.AddWithValue("tid", taskId);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var v = new TaskParticipationView
                            {
                                Id = rd.GetInt64(0),
                                IdEmpleado = rd.GetInt32(1),
                                Empleado = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                                FechaInicio = rd.GetDateTime(3),
                                FechaFin = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4)
                            };
                            list.Add(v);
                        }
                    }
                }
            }

            return list;
        }

        public bool HasOpenParticipation(int taskId, int employeeId)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    SELECT EXISTS(
                        SELECT 1
                          FROM tasks_schema.task_participations
                         WHERE id_tasca = @tid
                           AND id_empleado = @eid
                           AND fecha_fin IS NULL
                    );", conn))
                {
                    cmd.Parameters.AddWithValue("tid", taskId);
                    cmd.Parameters.AddWithValue("eid", employeeId);
                    var o = cmd.ExecuteScalar();
                    return (o is bool) && (bool)o;
                }
            }
        }

        public TaskParticipation Start(int taskId, int employeeId, string observaciones, DateTime? inicio = null)
        {
            if (HasOpenParticipation(taskId, employeeId))
            {
                using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(@"
                        SELECT id, id_tasca, id_empleado, fecha_inicio, fecha_fin, observaciones
                          FROM tasks_schema.task_participations
                         WHERE id_tasca = @tid AND id_empleado = @eid AND fecha_fin IS NULL
                         ORDER BY id DESC
                         LIMIT 1;", conn))
                    {
                        cmd.Parameters.AddWithValue("tid", taskId);
                        cmd.Parameters.AddWithValue("eid", employeeId);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                return new TaskParticipation
                                {
                                    Id = rd.GetInt64(0),
                                    IdTasca = rd.GetInt32(1),
                                    IdEmpleado = rd.GetInt32(2),
                                    FechaInicio = rd.GetDateTime(3),
                                    FechaFin = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                                    Observaciones = rd.IsDBNull(5) ? null : rd.GetString(5)
                                };
                            }
                        }
                    }
                }

                return null;
            }

            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    INSERT INTO tasks_schema.task_participations
                        (id_tasca, id_empleado, fecha_inicio, observaciones)
                    VALUES (@tid, @eid, COALESCE(@inicio, NOW()), @obs)
                    RETURNING id, fecha_inicio;", conn))
                {
                    cmd.Parameters.AddWithValue("tid", taskId);
                    cmd.Parameters.AddWithValue("eid", employeeId);
                    cmd.Parameters.AddWithValue("inicio", (object)(inicio.HasValue ? (object)inicio.Value : DBNull.Value));
                    cmd.Parameters.AddWithValue("obs", (object)(observaciones ?? (object)DBNull.Value));

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            return new TaskParticipation
                            {
                                Id = rd.GetInt64(0),
                                IdTasca = taskId,
                                IdEmpleado = employeeId,
                                FechaInicio = rd.GetDateTime(1),
                                FechaFin = null,
                                Observaciones = observaciones
                            };
                        }
                    }
                }
            }
            return null;
        }
        
        public void DeleteById(long participationId)
        {
            using (var conn = new NpgsqlConnection(DatabaseCredentials.ConnectionStringLocal))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            DELETE FROM tasks_schema.task_participations
             WHERE id = @id;", conn))
                {
                    var pId = cmd.Parameters.Add("id", NpgsqlDbType.Bigint);
                    pId.Value = participationId;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
