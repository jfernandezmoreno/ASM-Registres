using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class BasculaPequenaRepository : IBasculaPequena
    {
        private readonly NPGSQLService _npgsqlService;

        public BasculaPequenaRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<CBascPetites> SelectCBascPetites()
        {
            string query = @"SELECT id, grup_tasques_id, tipus_equip, nom_equip, valor_min, valor_max, caracteristiques_control, periodicitat, acc_correctores, responsable 
                             FROM registros_app.c_basc_petites";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new CBascPetites
            {
                Id = reader.GetInt32(0),
                GrupTasquesId = reader.GetInt32(1),
                TipusEquip = reader.GetString(2),
                NomEquip = reader.GetString(3),
                ValorMin = reader.GetDouble(4),
                ValorMax = reader.GetDouble(5),
                CaracteristiquesControl = reader.GetString(6),
                Periodicitat = reader.GetString(7),
                AccCorrectores = reader.GetInt32(8),
                Responsable = reader.GetString(9)
            });
        }
    }
}
