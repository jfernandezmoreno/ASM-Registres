using System;
using System.Collections.Generic;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class ReprocessatMostresLaboratoriRepository : IReprocessatMostresLaboratori
    {

        private readonly NPGSQLService _npgsqlService;

        public ReprocessatMostresLaboratoriRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void AddReprocesatMostraLaboratori(RegistreReprocesatMostresLaboratori registre)
        {
            string query = @"
                INSERT INTO registros_app.reprocessat_mostres_laboratori 
                    (lot, data, pes, laborant, producte)
                VALUES 
                    (@lot, @data, @pes, @laborant, @producte)";

            var parameters = new Dictionary<string, object>
            {
                { "lot", registre.Lot },
                { "data", registre.Data },
                { "pes", registre.Pes },
                { "laborant", registre.Laborant },
                { "producte", registre.Producte }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<RegistreReprocesatMostresLaboratori> getRegistresReprocessatMostraLaboratoris()
        {
            const string query = @"SELECT id, lot, data, pes, laborant, producte
                           FROM registros_app.reprocessat_mostres_laboratori";

            var results = _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oLot = reader.GetOrdinal("lot");
                int oData = reader.GetOrdinal("data");
                int oPes = reader.GetOrdinal("pes");
                int oLaborant = reader.GetOrdinal("laborant");
                int oProducte = reader.GetOrdinal("producte");

                return new RegistreReprocesatMostresLaboratori
                {
                    Id = reader.GetInt32(oId),
                    Lot = reader.IsDBNull(oLot) ? "" : reader.GetString(oLot),
                    Data = reader.GetDateTime(oData),
                    // Postgres numeric -> decimal: Convert.ToDouble es más robusto
                    Pes = reader.IsDBNull(oPes) ? 0.0 : Convert.ToDouble(reader.GetValue(oPes)),
                    Laborant = reader.IsDBNull(oLaborant) ? "" : reader.GetString(oLaborant),
                    Producte = reader.IsDBNull(oProducte) ? "" : reader.GetString(oProducte)
                };
            });

            return results ?? new List<RegistreReprocesatMostresLaboratori>();
        }

        public List<RegistreReprocesatMostresLaboratori> getRegistresReprocessatMostraLaboratorisByDates(DateTime d1, DateTime d2)
        {
            const string query = @"SELECT id, lot, data, pes, laborant, producte
                           FROM registros_app.reprocessat_mostres_laboratori
                           WHERE data BETWEEN @d1 AND @d2";

            var parameters = new Dictionary<string, object> { { "d1", d1 }, { "d2", d2 } };

            var results = _npgsqlService.ExecuteQuery(query, parameters, reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oLot = reader.GetOrdinal("lot");
                int oData = reader.GetOrdinal("data");
                int oPes = reader.GetOrdinal("pes");
                int oLaborant = reader.GetOrdinal("laborant");
                int oProducte = reader.GetOrdinal("producte");

                return new RegistreReprocesatMostresLaboratori
                {
                    Id = reader.GetInt32(oId),
                    Lot = reader.IsDBNull(oLot) ? "" : reader.GetString(oLot),
                    Data = reader.GetDateTime(oData),
                    Pes = reader.IsDBNull(oPes) ? 0.0 : Convert.ToDouble(reader.GetValue(oPes)),
                    Laborant = reader.IsDBNull(oLaborant) ? "" : reader.GetString(oLaborant),
                    Producte = reader.IsDBNull(oProducte) ? "" : reader.GetString(oProducte)
                };
            });

            return results ?? new List<RegistreReprocesatMostresLaboratori>();
        }

    }
}
