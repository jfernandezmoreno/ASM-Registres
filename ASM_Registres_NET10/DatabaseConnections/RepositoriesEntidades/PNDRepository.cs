using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class PNDRepository : IPND
    {
        private readonly NPGSQLService _npgsqlService;

        public PNDRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<PndTasques> SelectPndTasques(int idGrup)
        {
            string query = @"SELECT id, grup_tasques_id, zona, element, operacio, acc_correctores, frequencia, responsable 
                             FROM registros_app.p_nd_tasques 
                             WHERE grup_tasques_id = @idGrup";

            var parameters = new Dictionary<string, object>
            {
                { "@idGrup", idGrup }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new PndTasques
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                Zona = reader.GetString(2),
                Element = reader.GetString(3),
                Operacio = reader.GetString(4),
                AccCorrectores = reader.GetInt32(5),
                Frequencia = reader.GetString(6),
                Responsable = reader.GetString(7)
            });
        }
    }
}
