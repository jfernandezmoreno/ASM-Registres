using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroGeneralRepository : IRegistroGeneral
    {

        private readonly NPGSQLService _npgsqlService;

        public RegistroGeneralRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public void AddRegistroGeneral(RegistroGeneral registro)
        {
            string query = @"INSERT INTO registros_app.registre_general 
                         (acc_realitzada, data, tipus_mto, duracio_h, empresa, observacions, nateja, feta_per) 
                         VALUES (@acc_realitzada, @data, @tipus_mto, @duracio_h, @empresa, @observacions, @nateja, @feta_per)";

            var parameters = new Dictionary<string, object>
            {
                { "acc_realitzada", registro.AccRealitzada },
                { "data", registro.Data },
                { "tipus_mto", registro.TipusMto },
                { "duracio_h", registro.DuracioH },
                { "empresa", registro.Empresa },
                { "observacions", string.IsNullOrEmpty(registro.Observacions) ? (object)DBNull.Value : registro.Observacions },
                { "nateja", registro.Nateja },
                { "feta_per", registro.FetaPer }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<RegistroGeneral> GetAllRegistrosGenerales()
        {
            string query = @"SELECT id, acc_realitzada, data, tipus_mto, duracio_h, empresa, observacions, nateja, feta_per 
                             FROM registros_app.registre_general";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistroGeneral
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                AccRealitzada = reader.GetString(reader.GetOrdinal("acc_realitzada")),
                Data = reader.GetDateTime(reader.GetOrdinal("data")),
                TipusMto = reader.GetString(reader.GetOrdinal("tipus_mto")),
                DuracioH = reader.GetDouble(reader.GetOrdinal("duracio_h")),
                Empresa = reader.GetString(reader.GetOrdinal("empresa")),
                Observacions = reader.IsDBNull(reader.GetOrdinal("observacions")) ? null : reader.GetString(reader.GetOrdinal("observacions")),
                Nateja = reader.GetBoolean(reader.GetOrdinal("nateja")),
                FetaPer = reader.GetString(reader.GetOrdinal("feta_per"))
            });
        }
    }
}
