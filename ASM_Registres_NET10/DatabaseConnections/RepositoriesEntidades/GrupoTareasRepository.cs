using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class GrupoTareasRepository : IGrupoTareas
    {
        private readonly NPGSQLService _npgsqlService;

        public GrupoTareasRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<GrupTasques> GetAllGrups()
        {
            string query = @"SELECT g.id, g.nom AS grup_nom, p.nom AS plan_nom, p.id AS plan_id 
                             FROM registros_app.grup_tasques g 
                             JOIN registros_app.pla p ON g.pla_id = p.id";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new GrupTasques
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                PlanId = reader.GetInt32(3)
            });
        }

        public string GetGroupById(int id)
        {
            string query = "SELECT g.nom AS grup_nom FROM registros_app.grup_tasques g WHERE g.id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => reader.GetString(0)).FirstOrDefault() ?? string.Empty;
        }

        public List<string> GetGroupsByPlaId(int plaId)
        {
            string query = @"SELECT g.nom AS grup_nom 
                             FROM registros_app.grup_tasques g 
                             WHERE g.pla_id = @plaId";

            var parameters = new Dictionary<string, object>
            {
                { "@plaId", plaId }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => reader.GetString(0));
        }

        public List<GrupTasques> GetLabGroups()
        {
            string query = @"SELECT g.id, g.nom AS grup_nom, p.nom AS plan_nom, p.id AS plan_id 
                             FROM registros_app.grup_tasques g 
                             JOIN registros_app.pla p ON g.pla_id = p.id
                             WHERE g.id IN (7, 20, 21, 22, 23, 24, 25)";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new GrupTasques
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                PlanId = reader.GetInt32(3)
            });
        }

        public List<GrupTasques> GetSelectedGrupsEsporadicas()
        {
            string query = @"SELECT g.id, g.nom AS grup_nom, p.nom AS plan_nom, p.id AS plan_id 
                             FROM registros_app.grup_tasques g 
                             JOIN registros_app.pla p ON g.pla_id = p.id
                             WHERE g.id IN (2, 4, 5, 6, 8, 14)";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new GrupTasques
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                PlanId = reader.GetInt32(3)
            });
        }

        public List<GrupTasques> GetSelectedGrupsHoy()
        {
            string query = @"SELECT g.id, g.nom AS grup_nom, p.nom AS plan_nom, p.id AS plan_id 
                             FROM registros_app.grup_tasques g 
                             JOIN registros_app.pla p ON g.pla_id = p.id
                             WHERE g.id IN (1, 3, 9, 10, 11, 12, 13, 16)";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new GrupTasques
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                PlanId = reader.GetInt32(3)
            });
        }

        public string ObtenerNombreGrupoPorId(int id)
        {
            string query = @"SELECT nom FROM registros_app.grup_tasques WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => reader.GetString(0)).FirstOrDefault()
                   ?? throw new Exception("No se encontró un grupo con el ID proporcionado.");
        }
    }
}
