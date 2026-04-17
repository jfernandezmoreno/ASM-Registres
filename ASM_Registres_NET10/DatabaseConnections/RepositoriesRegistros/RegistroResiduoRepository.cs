using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroResiduoRepository : IRegistroResiduo
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroResiduoRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public async Task AddRegistreAsync(RegistreResidusLaboratori registre)
        {
            string query = @"INSERT INTO registros_app.registre_residus_laboratori 
                             (data, laborant, residu, quantitat, quantitat_litres) 
                             VALUES (@data, @laborant, @residu, @quantitat, @quantitatLitres)";

            var parameters = new Dictionary<string, object>
            {
                { "data", registre.Data },
                { "laborant", registre.Laborant },
                { "residu", registre.Residu },
                { "quantitat", registre.Quantitat },
                { "quantitatLitres", registre.QuantitatLitres != null ? (object)registre.QuantitatLitres : DBNull.Value }
            };

            await Task.Run(() => _npgsqlService.ExecuteNonQuery(query, parameters));
        }

        public List<RegistreResidusLaboratori> GetAll()
        {
            const string query = @"SELECT id, data, laborant, residu, quantitat, quantitat_litres
                           FROM registros_app.registre_residus_laboratori
                           ORDER BY data DESC";

            var results = _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oData = reader.GetOrdinal("data");
                int oLaborant = reader.GetOrdinal("laborant");
                int oResidu = reader.GetOrdinal("residu");
                int oQuantitat = reader.GetOrdinal("quantitat");
                int oQL = reader.GetOrdinal("quantitat_litres");

                return new RegistreResidusLaboratori
                {
                    Id = reader.GetInt32(oId),
                    Data = reader.GetDateTime(oData),
                    Laborant = reader.IsDBNull(oLaborant) ? "" : reader.GetString(oLaborant),
                    Residu = reader.IsDBNull(oResidu) ? "" : reader.GetString(oResidu),
                    Quantitat = reader.IsDBNull(oQuantitat) ? "" : reader.GetString(oQuantitat),
                    QuantitatLitres = reader.IsDBNull(oQL) ? null : Convert.ToString(reader.GetValue(oQL))
                };
            });

            return results ?? new List<RegistreResidusLaboratori>();
        }

        public List<RegistreResidusLaboratori> GetByDateRange(DateTime from, DateTime to)
        {
            const string query = @"SELECT id, data, laborant, residu, quantitat, quantitat_litres
                           FROM registros_app.registre_residus_laboratori
                           WHERE data BETWEEN @d1 AND @d2
                           ORDER BY data DESC";

            var parameters = new Dictionary<string, object> { { "d1", from }, { "d2", to } };

            var results = _npgsqlService.ExecuteQuery(query, parameters, reader =>
            {
                int oId = reader.GetOrdinal("id");
                int oData = reader.GetOrdinal("data");
                int oLaborant = reader.GetOrdinal("laborant");
                int oResidu = reader.GetOrdinal("residu");
                int oQuantitat = reader.GetOrdinal("quantitat");
                int oQL = reader.GetOrdinal("quantitat_litres");

                return new RegistreResidusLaboratori
                {
                    Id = reader.GetInt32(oId),
                    Data = reader.GetDateTime(oData),
                    Laborant = reader.IsDBNull(oLaborant) ? "" : reader.GetString(oLaborant),
                    Residu = reader.IsDBNull(oResidu) ? "" : reader.GetString(oResidu),
                    Quantitat = reader.IsDBNull(oQuantitat) ? "" : reader.GetString(oQuantitat),
                    QuantitatLitres = reader.IsDBNull(oQL) ? null : Convert.ToString(reader.GetValue(oQL))
                };
            });

            return results ?? new List<RegistreResidusLaboratori>();
        }

    }
}
