using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class TareasRepository : ITareas
    {
        private readonly NPGSQLService _npgsqlService;

        public TareasRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public Tasques GetTaskById(int taskId)
        {
            string query = @"SELECT id, idintern, idgrup, titol, periodicitat, darrera, zona_o_equip 
                             FROM registros_app.tasques 
                             WHERE id = @taskId";

            var parameters = new Dictionary<string, object>
            {
                { "@taskId", taskId }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new Tasques
            {
                Id = reader.GetInt32(0),
                IdIntern = reader.GetInt32(1),
                IdGrup = reader.GetInt32(2),
                Titol = reader.GetString(3),
                Periodicitat = reader.GetString(4),
                Darrera = reader.GetDateTime(5),
                Zona = reader.GetString(6)
            }).FirstOrDefault();
        }

        public List<Tasques> GetTasks()
        {
            string query = @"SELECT id, idintern, idgrup, titol, periodicitat, darrera, zona_o_equip 
                             FROM registros_app.tasques";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new Tasques
            {
                Id = reader.GetInt32(0),
                IdIntern = reader.GetInt32(1),
                IdGrup = reader.GetInt32(2),
                Titol = reader.GetString(3),
                Periodicitat = reader.GetString(4),
                Darrera = reader.GetDateTime(5),
                Zona = reader.GetString(6)
            });
        }

        public List<Tasques> GetTasksByGroupId(int idGroup)
        {
            string query = @"SELECT id, idintern, idgrup, titol, periodicitat, darrera, zona_o_equip 
                             FROM registros_app.tasques 
                             WHERE idgrup = @idGroup";

            var parameters = new Dictionary<string, object>
            {
                { "@idGroup", idGroup }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new Tasques
            {
                Id = reader.GetInt32(0),
                IdIntern = reader.GetInt32(1),
                IdGrup = reader.GetInt32(2),
                Titol = reader.GetString(3),
                Periodicitat = reader.GetString(4),
                Darrera = reader.GetDateTime(5),
                Zona = reader.GetString(6)
            });
        }

        public List<Tasques> GetTasquesLab()
        {
            string query = @"SELECT id, idintern, idgrup, titol, periodicitat, darrera, zona_o_equip 
                             FROM registros_app.tasques 
                             WHERE idgrup = 7";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new Tasques
            {
                Id = reader.GetInt32(0),
                IdIntern = reader.GetInt32(1),
                IdGrup = reader.GetInt32(2),
                Titol = reader.GetString(3),
                Periodicitat = reader.GetString(4),
                Darrera = reader.GetDateTime(5),
                Zona = reader.GetString(6)
            });
        }

        public void UpdateTasquesDarrera(int tascaId, DateTime nuevaFechaDarrera)
        {
            string query = @"UPDATE registros_app.tasques 
                             SET darrera = @nuevaFechaDarrera 
                             WHERE id = @tascaId";

            var parameters = new Dictionary<string, object>
            {
                { "@nuevaFechaDarrera", nuevaFechaDarrera },
                { "@tascaId", tascaId }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
