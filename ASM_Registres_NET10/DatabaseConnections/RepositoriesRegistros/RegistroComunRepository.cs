using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroComunRepository : IRegistroComun
    {
        private readonly NPGSQLService npgsqlService;

        public RegistroComunRepository(NPGSQLService npgsqlService)
        {
            this.npgsqlService = npgsqlService;
        }

        public bool ExisteRegistroComun(int id)
        {
            string query = "SELECT COUNT(*) FROM registros_app.registre_comu WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            return Convert.ToInt32(npgsqlService.ExecuteScalar(query, parameters)) > 0;
        }

        public RegistroComun GetRegistroComun(int id)
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, estat, observacions, feta_per
                             FROM registros_app.registre_comu 
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            return npgsqlService.ExecuteQuery(query, parameters, reader => new RegistroComun
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Estat = reader.GetString(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                FetaPer = reader.GetString(8)
            }).FirstOrDefault();
        }

        public List<RegistroComun> GetRegistroComunesPorIdTasca(DateTime fechaInicio, DateTime fechaFin, int idTasca)
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, estat, observacions, feta_per
                             FROM registros_app.registre_comu
                             WHERE idtasca = @idTasca AND data BETWEEN @fechaInicio AND @fechaFin";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", idTasca },
                { "fechaInicio", fechaInicio },
                { "fechaFin", fechaFin }
            };

            return npgsqlService.ExecuteQuery(query, parameters, reader => new RegistroComun
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Estat = reader.GetString(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                FetaPer = reader.GetString(8)
            });
        }

        public List<RegistroComun> GetRegistrosIncomplet()
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, estat, observacions, feta_per
                             FROM registros_app.registre_comu
                             WHERE estat = 'Incompleta'";

            return npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistroComun
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Estat = reader.GetString(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                FetaPer = reader.GetString(8)
            });
        }

        public List<RegistroComun> GetRegistrosPendent()
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, estat, observacions, feta_per
                             FROM registros_app.registre_comu
                             WHERE estat = 'Pendent'";

            return npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistroComun
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Estat = reader.GetString(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                FetaPer = reader.GetString(8)
            });
        }

        public void InsertRegistroComun(RegistroComun registro)
        {
            string query = @"INSERT INTO registros_app.registre_comu 
                         (idtasca, idgruptasques, nomtasca, nomgrup, data, estat, observacions, feta_per) 
                         VALUES (@idTasca, @idGrupTasques, @nomTasca, @nomGrup, @data, @estat, @observacions, @feta_per)";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", registro.IdTasca },
                { "idGrupTasques", registro.IdGrupTasques },
                { "nomTasca", registro.NomTasca },
                { "nomGrup", registro.NomGrup },
                { "data", registro.Data },
                { "estat", registro.Estat },
                { "observacions", string.IsNullOrEmpty(registro.Observacions) ? (object)DBNull.Value : registro.Observacions },
                { "feta_per", registro.FetaPer }
            };

            npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateRegistroComun(int id, string estat)
        {
            string query = "UPDATE registros_app.registre_comu SET estat = @estat WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "estat", estat },
                { "id", id }
            };

            npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateRegistroComun(int id, string estat, string obv)
        {
            string query = "UPDATE registros_app.registre_comu SET estat = @estat, observacions = @observacions WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "estat", estat },
                { "observacions", string.IsNullOrEmpty(obv) ? DBNull.Value : (object)obv },
                { "id", id }
            };

            npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
