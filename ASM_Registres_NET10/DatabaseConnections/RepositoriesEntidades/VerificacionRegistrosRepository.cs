using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class VerificacionRegistrosRepository : IVerificacionRegistros
    {
        private readonly NPGSQLService _npgsqlService;

        public VerificacionRegistrosRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public List<VRegistres> GetVRegistresByGrup()
        {
            string query = @"SELECT id, grup_tasques_id, ambit, item, criteris, metode, responsable 
                             FROM registros_app.v_registres";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new VRegistres
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                Ambit = reader.GetString(2),
                Item = reader.GetString(3),
                Criteris = reader.IsDBNull(4) ? null : reader.GetString(4),
                Metode = reader.GetString(5),
                Responsable = reader.GetString(6)
            });
        }
    }
}
