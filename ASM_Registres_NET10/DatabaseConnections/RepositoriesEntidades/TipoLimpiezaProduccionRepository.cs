using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class TipoLimpiezaProduccionRepository : ITipoLimpiezaProduccion
    {
        private readonly NPGSQLService _npgsqlService;

        public TipoLimpiezaProduccionRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public List<string> GetTipos()
        {
            string query = @"SELECT nombre
                             FROM registros_app.tipo_limpieza_produccion
                             ORDER BY id";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => reader.GetString(0));
        }
    }
}
