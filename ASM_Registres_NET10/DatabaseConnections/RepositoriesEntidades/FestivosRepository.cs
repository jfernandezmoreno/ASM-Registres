using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class FestivosRepository : IFestivo
    {
        private readonly NPGSQLService _npgsqlService;

        public FestivosRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<DateTime> GetFestivos()
        {
            string query = @"SELECT data FROM registros_app.festius";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => reader.GetDateTime(0));
        }
    }
}
