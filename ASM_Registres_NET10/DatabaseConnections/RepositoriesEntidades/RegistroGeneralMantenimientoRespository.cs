using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class RegistroGeneralMantenimientoRespository : IRegistroGeneralMantenimiento
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroGeneralMantenimientoRespository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<RGeneralManteniment> GetRGeneralMantenimentByGrup()
        {
            string query = @"SELECT id, id_grup, ambit, item, alies, metode, responsable 
                             FROM registros_app.r_general_manteniment";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RGeneralManteniment
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                Ambit = reader.GetString(2),
                Item = reader.GetString(3),
                Alies = reader.GetString(4),
                Metode = reader.GetString(5),
                Responsable = reader.GetString(6)
            });
        }
    }
}
