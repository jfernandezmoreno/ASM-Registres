using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class EstadoInstalacionRepository : IEstadoInstalacion
    {
        private readonly NPGSQLService _npgsqlService;

        public EstadoInstalacionRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<REstatInstalacio> SelectREstatInstalacio()
        {
            string query = @"SELECT id, grup_tasques_id, ambit, item, metode, acc_correctores, frequencia, responsable 
                             FROM registros_app.r_estat_instalacio";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new REstatInstalacio
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                Ambit = reader.GetString(2),
                Item = reader.GetString(3),
                Metode = reader.GetString(4),
                AccCorrectores = reader.GetInt32(5),
                Frequencia = reader.GetString(6),
                Responsable = reader.GetString(7)
            });
        }
    }
}
