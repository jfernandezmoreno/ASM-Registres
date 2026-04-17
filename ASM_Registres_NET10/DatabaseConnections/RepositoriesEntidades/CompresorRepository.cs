using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class CompresorRepository : ICompresor
    {
        private readonly NPGSQLService _npgsqlService;

        public CompresorRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<RsCompresor> SelectRsCompresor()
        {
            string query = @"SELECT id, grup_tasques_id, equip, item, alies, frequencia, criteris_acceptacio, metode, responsable, acc_correctores 
                             FROM registros_app.r_s_compresor";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RsCompresor
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                Equip = reader.GetString(2),
                Item = reader.GetString(3),
                Alies = reader.GetString(4),
                Frequencia = reader.GetString(5),
                CriterisAcceptacio = reader.GetString(6),
                Metode = reader.GetString(7),
                Responsable = reader.GetString(8),
                AccCorrectores = reader.GetInt32(9)
            });
        }
    }
}
