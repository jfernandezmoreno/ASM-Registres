using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroHAETRepository : IRegistreHAET
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroHAETRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public bool Insert(RegistreHAET registro)
        {
            string query = @"
                INSERT INTO registros_app.haet 
                (date, laborant_id, hexa_tick, acetona_tick, etanol_tick, toule_tick, hexa_batch, acetona_batch, etanol_batch, tolue_batch)
                VALUES (@date, @laborant_id, @hexa_tick, @acetona_tick, @etanol_tick, @toule_tick, @hexa_batch, @acetona_batch, @etanol_batch, @toule_batch)";

            var parameters = new Dictionary<string, object>
            {
                { "@date", registro.Date },
                { "@laborant_id", registro.LaborantId },
                { "@hexa_tick", registro.HexaTick },
                { "@acetona_tick", registro.AcetonaTick },
                { "@etanol_tick", registro.EtanolTick },
                { "@toule_tick", registro.TouleTick },
                { "@hexa_batch", registro.HexaBatch },
                { "@acetona_batch", registro.AcetonaBatch },
                { "@etanol_batch", registro.EtanolBatch },
                { "@toule_batch", registro.TolueBatch }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
            return true;
        }

        public bool Update(RegistreHAET registro)
        {
            string query = @"
                UPDATE registros_app.haet SET 
                date = @date,
                laborant_id = @laborant_id,
                hexa_tick = @hexa_tick,
                acetona_tick = @acetona_tick,
                etanol_tick = @etanol_tick,
                toule_tick = @toule_tick,
                hexa_batch = @hexa_batch,
                acetona_batch = @acetona_batch,
                etanol_batch = @etanol_batch,
                toule_batch = @toule_batch
                WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", registro.Id },
                { "@date", registro.Date },
                { "@laborant_id", registro.LaborantId },
                { "@hexa_tick", registro.HexaTick },
                { "@acetona_tick", registro.AcetonaTick },
                { "@etanol_tick", registro.EtanolTick },
                { "@toule_tick", registro.TouleTick },
                { "@hexa_batch", registro.HexaBatch },
                { "@acetona_batch", registro.AcetonaBatch },
                { "@etanol_batch", registro.EtanolBatch },
                { "@toule_batch", registro.TolueBatch }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
            return true;
        }

        public bool Delete(int id)
        {
            string query = "DELETE FROM registros_app.haet WHERE id = @id";
            var parameters = new Dictionary<string, object> { { "@id", id } };
            _npgsqlService.ExecuteNonQuery(query, parameters);
            return true;
        }

        public List<RegistreHAET> GetAll()
        {
            const string query = @"
        SELECT id, date, laborant_id, hexa_tick, acetona_tick, etanol_tick,
               toule_tick, hexa_batch, acetona_batch, etanol_batch, tolue_batch
        FROM registros_app.haet";

            var results = _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oDate = reader.GetOrdinal("date");
                int oLab = reader.GetOrdinal("laborant_id");
                int oHexaT = reader.GetOrdinal("hexa_tick");
                int oAcetonaT = reader.GetOrdinal("acetona_tick");
                int oEtanolT = reader.GetOrdinal("etanol_tick");
                int oTouleT = reader.GetOrdinal("toule_tick");
                int oHexaB = reader.GetOrdinal("hexa_batch");
                int oAcetonaB = reader.GetOrdinal("acetona_batch");
                int oEtanolB = reader.GetOrdinal("etanol_batch");
                int oTolueB = reader.GetOrdinal("tolue_batch"); // <-- aquí el fix

                return new RegistreHAET
                {
                    Id = reader.GetInt32(oId),
                    Date = reader.GetDateTime(oDate),
                    LaborantId = reader.GetInt32(oLab),
                    HexaTick = !reader.IsDBNull(oHexaT) && reader.GetBoolean(oHexaT),
                    AcetonaTick = !reader.IsDBNull(oAcetonaT) && reader.GetBoolean(oAcetonaT),
                    EtanolTick = !reader.IsDBNull(oEtanolT) && reader.GetBoolean(oEtanolT),
                    TouleTick = !reader.IsDBNull(oTouleT) && reader.GetBoolean(oTouleT),
                    HexaBatch = reader.IsDBNull(oHexaB) ? "" : reader.GetString(oHexaB),
                    AcetonaBatch = reader.IsDBNull(oAcetonaB) ? "" : reader.GetString(oAcetonaB),
                    EtanolBatch = reader.IsDBNull(oEtanolB) ? "" : reader.GetString(oEtanolB),
                    TolueBatch = reader.IsDBNull(oTolueB) ? "" : reader.GetString(oTolueB) // <-- aquí también
                };
            });

            return results ?? new List<RegistreHAET>();
        }


        public RegistreHAET GetById(int id)
        {
            const string query = @"
        SELECT id, date, laborant_id, hexa_tick, acetona_tick, etanol_tick,
               toule_tick, hexa_batch, acetona_batch, etanol_batch, tolue_batch
        FROM registros_app.haet
        WHERE id = @id";

            var parameters = new Dictionary<string, object> { { "@id", id } };

            var results = _npgsqlService.ExecuteQuery(query, parameters, reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oDate = reader.GetOrdinal("date");
                int oLab = reader.GetOrdinal("laborant_id");
                int oHexaT = reader.GetOrdinal("hexa_tick");
                int oAcetonaT = reader.GetOrdinal("acetona_tick");
                int oEtanolT = reader.GetOrdinal("etanol_tick");
                int oTouleT = reader.GetOrdinal("toule_tick");
                int oHexaB = reader.GetOrdinal("hexa_batch");
                int oAcetonaB = reader.GetOrdinal("acetona_batch");
                int oEtanolB = reader.GetOrdinal("etanol_batch");
                int oTolueB = reader.GetOrdinal("tolue_batch"); // <-- fix

                return new RegistreHAET
                {
                    Id = reader.GetInt32(oId),
                    Date = reader.GetDateTime(oDate),
                    LaborantId = reader.GetInt32(oLab),
                    HexaTick = !reader.IsDBNull(oHexaT) && reader.GetBoolean(oHexaT),
                    AcetonaTick = !reader.IsDBNull(oAcetonaT) && reader.GetBoolean(oAcetonaT),
                    EtanolTick = !reader.IsDBNull(oEtanolT) && reader.GetBoolean(oEtanolT),
                    TouleTick = !reader.IsDBNull(oTouleT) && reader.GetBoolean(oTouleT),
                    HexaBatch = reader.IsDBNull(oHexaB) ? "" : reader.GetString(oHexaB),
                    AcetonaBatch = reader.IsDBNull(oAcetonaB) ? "" : reader.GetString(oAcetonaB),
                    EtanolBatch = reader.IsDBNull(oEtanolB) ? "" : reader.GetString(oEtanolB),
                    TolueBatch = reader.IsDBNull(oTolueB) ? "" : reader.GetString(oTolueB) // <-- fix
                };
            });

            return (results != null && results.Count > 0) ? results[0] : null;
        }


    }
}
