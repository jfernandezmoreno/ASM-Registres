using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroFaseMovilRepository : IRegistreFaeMobil
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroFaseMovilRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public bool Insert(RegistreFaseMovil registro)
        {
            string query = @"
                INSERT INTO registros_app.fase_movil 
                (date, laborant_id, hexa_tick, acetat_etil_tick, acetona_tick, hexa_batch, acetat_batch, acetona_batch)
                VALUES (@date, @laborant_id, @hexa_tick, @acetat_etil_tick, @acetona_tick, @hexa_batch, @acetat_batch, @acetona_batch)";

            var parameters = new Dictionary<string, object>
            {
                { "@date", registro.Date },
                { "@laborant_id", registro.LaborantId },
                { "@hexa_tick", registro.HexaTick },
                { "@acetat_etil_tick", registro.AcetatEtilTick },
                { "@acetona_tick", registro.AcetonaTick },
                { "@hexa_batch", registro.HexaBatch },
                { "@acetat_batch", registro.AcetatBatch },
                { "@acetona_batch", registro.AcetonaBatch }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
            return true;
        }

        public bool Update(RegistreFaseMovil registro)
        {
            string query = @"
                UPDATE registros_app.fase_movil SET
                date = @date,
                laborant_id = @laborant_id,
                hexa_tick = @hexa_tick,
                acetat_etil_tick = @acetat_etil_tick,
                acetona_tick = @acetona_tick,
                hexa_batch = @hexa_batch,
                acetat_batch = @acetat_batch,
                acetona_batch = @acetona_batch
                WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", registro.Id },
                { "@date", registro.Date },
                { "@laborant_id", registro.LaborantId },
                { "@hexa_tick", registro.HexaTick },
                { "@acetat_etil_tick", registro.AcetatEtilTick },
                { "@acetona_tick", registro.AcetonaTick },
                { "@hexa_batch", registro.HexaBatch },
                { "@acetat_batch", registro.AcetatBatch },
                { "@acetona_batch", registro.AcetonaBatch }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
            return true;
        }

        public bool Delete(int id)
        {
            string query = "DELETE FROM registros_app.fase_movil WHERE id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };
            _npgsqlService.ExecuteNonQuery(query, parameters);
            return true;
        }

        public List<RegistreFaseMovil> GetAll()
        {
            const string query = @"SELECT id, date, laborant_id, hexa_tick, acetat_etil_tick, acetona_tick,
                                  hexa_batch, acetat_batch, acetona_batch
                           FROM registros_app.fase_movil";

            var results = _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oDate = reader.GetOrdinal("date");
                int oLab = reader.GetOrdinal("laborant_id");
                int oHexaT = reader.GetOrdinal("hexa_tick");
                int oAcetEtT = reader.GetOrdinal("acetat_etil_tick");
                int oAcetonaT = reader.GetOrdinal("acetona_tick");
                int oHexaB = reader.GetOrdinal("hexa_batch");
                int oAcetB = reader.GetOrdinal("acetat_batch");
                int oAcetonaB = reader.GetOrdinal("acetona_batch");

                return new RegistreFaseMovil
                {
                    Id = reader.GetInt32(oId),
                    Date = reader.GetDateTime(oDate),
                    LaborantId = reader.GetInt32(oLab),
                    HexaTick = reader.IsDBNull(oHexaT) ? (bool?)null : reader.GetBoolean(oHexaT),
                    AcetatEtilTick = reader.IsDBNull(oAcetEtT) ? (bool?)null : reader.GetBoolean(oAcetEtT),
                    AcetonaTick = reader.IsDBNull(oAcetonaT) ? (bool?)null : reader.GetBoolean(oAcetonaT),
                    HexaBatch = reader.IsDBNull(oHexaB) ? "" : reader.GetString(oHexaB),
                    AcetatBatch = reader.IsDBNull(oAcetB) ? "" : reader.GetString(oAcetB),
                    AcetonaBatch = reader.IsDBNull(oAcetonaB) ? "" : reader.GetString(oAcetonaB)
                };
            });

            return results ?? new List<RegistreFaseMovil>();
        }

        public RegistreFaseMovil GetById(int id)
        {
            const string query = @"SELECT id, date, laborant_id, hexa_tick, acetat_etil_tick, acetona_tick,
                                  hexa_batch, acetat_batch, acetona_batch
                           FROM registros_app.fase_movil
                           WHERE id = @id";

            var parameters = new Dictionary<string, object> { { "@id", id } };

            var results = _npgsqlService.ExecuteQuery(query, parameters, reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oDate = reader.GetOrdinal("date");
                int oLab = reader.GetOrdinal("laborant_id");
                int oHexaT = reader.GetOrdinal("hexa_tick");
                int oAcetEtT = reader.GetOrdinal("acetat_etil_tick");
                int oAcetonaT = reader.GetOrdinal("acetona_tick");
                int oHexaB = reader.GetOrdinal("hexa_batch");
                int oAcetB = reader.GetOrdinal("acetat_batch");
                int oAcetonaB = reader.GetOrdinal("acetona_batch");

                return new RegistreFaseMovil
                {
                    Id = reader.GetInt32(oId),
                    Date = reader.GetDateTime(oDate),
                    LaborantId = reader.GetInt32(oLab),
                    HexaTick = reader.IsDBNull(oHexaT) ? (bool?)null : reader.GetBoolean(oHexaT),
                    AcetatEtilTick = reader.IsDBNull(oAcetEtT) ? (bool?)null : reader.GetBoolean(oAcetEtT),
                    AcetonaTick = reader.IsDBNull(oAcetonaT) ? (bool?)null : reader.GetBoolean(oAcetonaT),
                    HexaBatch = reader.IsDBNull(oHexaB) ? "" : reader.GetString(oHexaB),
                    AcetatBatch = reader.IsDBNull(oAcetB) ? "" : reader.GetString(oAcetB),
                    AcetonaBatch = reader.IsDBNull(oAcetonaB) ? "" : reader.GetString(oAcetonaB)
                };
            });

            return (results != null && results.Count > 0) ? results[0] : null;
        }

    }
}
