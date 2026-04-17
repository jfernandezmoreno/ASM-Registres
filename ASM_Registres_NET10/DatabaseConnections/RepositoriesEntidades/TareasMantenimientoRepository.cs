using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class TareasMantenimientoRepository : ITareasMantenimiento
    {
        private readonly NPGSQLService _npgsqlService;

        public TareasMantenimientoRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public List<TManteniment> GetAllTManteniment()
        {
            string query = @"SELECT id, nom, periodicitat, zona 
                             FROM registros_app.t_manteniment";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new TManteniment
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                Periodicitat = reader.GetString(2),
                Zona = reader.GetString(3)
            });
        }

        public int GetIdInternByNombreTasca(string nomTasca)
        {
            string query = @"SELECT id 
                             FROM registros_app.t_manteniment 
                             WHERE nom = @nomTasca";

            var parameters = new Dictionary<string, object>
            {
                { "@nomTasca", nomTasca }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => reader.GetInt32(0)).FirstOrDefault();
        }
    }
}
