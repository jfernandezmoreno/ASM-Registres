using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class BasculaGrandeRepository : IBasculaGrande
    {
        private readonly NPGSQLService _npgsqlService;

        public BasculaGrandeRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<CBascGranKoh> SelectCBascGranKoh()
        {
            string query = @"SELECT id, grup_tasques_id, equip, item, alies, frequencia, valor_min, valor_max, criteris_acceptacio, metode, responsable 
                             FROM registros_app.c_basc_gran_koh";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new CBascGranKoh
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                Equip = reader.GetString(2),
                Item = reader.GetString(3),
                Alies = reader.GetString(4),
                Frequencia = reader.GetString(5),
                ValorMin = reader.GetInt32(6),
                ValorMax = reader.GetInt32(7),
                CriterisAcceptacio = reader.GetString(8),
                Metode = reader.GetString(9),
                Responsable = reader.GetString(10)
            });
        }
    }
}
