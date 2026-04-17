using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class PlanRepository : IPlan
    {
        private readonly NPGSQLService _npgsqlService;

        public PlanRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<string> GetAllPlanNames()
        {
            string query = "SELECT nom FROM registros_app.pla WHERE id != 2";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => reader.GetString(0));
        }
        public List<Pla> GetAllPlans()
        {
            string query = "SELECT id, nom FROM registros_app.pla";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new Pla
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1)
            });
        }
    }
}
