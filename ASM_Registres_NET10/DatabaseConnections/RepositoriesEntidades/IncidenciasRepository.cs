using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class IncidenciasRepository : IIncidencias
    {
        private readonly NPGSQLService npgsqlService;
        //private readonly MailSenderService mailSenderService = new MailSenderService();

        public IncidenciasRepository(NPGSQLService npgsqlService)
        {
            this.npgsqlService = npgsqlService;
        }

        public async Task AddIncidencia(Incidencia incidencia)
        {
            string query = @"
            INSERT INTO registros_app.incidencia 
            (id_tasca, id_grup_tasques, data_incidencia, nom_tasca, nom_grup, descripcio_incidencia, resolta, solucio)
            VALUES (@idTasca, @idGrupTasques, @dataIncidencia, @nomTasca, @nomGrup, @descripcioIncidencia, @resolta, @solucio)";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", incidencia.IdTasca },
                { "idGrupTasques", incidencia.IdGrupTasques },
                { "dataIncidencia", incidencia.Data },
                { "nomTasca", incidencia.NomTasca },
                { "nomGrup", incidencia.NomGrup },
                { "descripcioIncidencia", incidencia.DescripcioIncidencia },
                { "resolta", incidencia.Resolta },
                { "solucio", incidencia.Solucio ?? (object)DBNull.Value }
            };

            npgsqlService.ExecuteNonQuery(query, parameters);
            /*
            switch (incidencia.IdGrupTasques)
            {
                case 7:
                case 20:
                case 21:
                case 22:
                    await mailSenderService.SendEmailAsync("montse.berenguer@additius.com", "Incidencia en el laboratorio", "Se ha creado una incidencia en el laboratorio que requiere supervisión: " + incidencia.NomTasca + " " + incidencia.NomGrup + " " + incidencia.DescripcioIncidencia, null, new List<string> { "juancarlos@additius.com", "lluc@additius.com"});
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 9:
                case 10:
                case 18:
                    await mailSenderService.SendEmailAsync("joan@additius.com", "Incidencia en planta", "Se ha creado una incidencia en planta que requiere supervisión: " + incidencia.NomTasca + " " + incidencia.NomGrup + " " + incidencia.DescripcioIncidencia, null, new List<string> { "juancarlos@additius.com", "oneil@additius.com", "montse.berenguer@additius.com", "lluc@additius.com"});
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                case 16:
                    await mailSenderService.SendEmailAsync("oneil@additius.com", "Incidencia en mantenimiento", "Se ha creado una incidencia en el apartado de mantenimiento que requiere supervisión: " + incidencia.NomTasca + " " + incidencia.NomGrup + " " + incidencia.DescripcioIncidencia, null, new List<string> { "juancarlos@additius.com", "montse.berenguer@additius.com", "joan@additius.com", "lluc@additius.com" });
                    break;
            }*/
        }

        public Incidencia GetIncidenciaById(int idIncidencia)
        {
            string query = @"
            SELECT id, id_tasca, id_grup_tasques, data_incidencia, nom_tasca, nom_grup, descripcio_incidencia, resolta, solucio
            FROM registros_app.incidencia
            WHERE id = @idIncidencia";

            var parameters = new Dictionary<string, object>
            {
                { "idIncidencia", idIncidencia }
            };

            return npgsqlService.ExecuteQuery(query, parameters, reader => new Incidencia
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                Data = reader.GetDateTime(3),
                NomTasca = reader.GetString(4),
                NomGrup = reader.GetString(5),
                DescripcioIncidencia = reader.GetString(6),
                Resolta = reader.GetBoolean(7),
                Solucio = reader.IsDBNull(8) ? null : reader.GetString(8)
            }).FirstOrDefault();
        }

        public List<Incidencia> GetIncidenciasByIdGrupTasques(int idGrupTasques)
        {
            string query = @"
            SELECT id, id_tasca, id_grup_tasques, data_incidencia, nom_tasca, nom_grup, descripcio_incidencia, resolta, solucio
            FROM registros_app.incidencia
            WHERE id_grup_tasques = @idGrupTasques";

            var parameters = new Dictionary<string, object>
            {
                { "idGrupTasques", idGrupTasques }
            };

            return npgsqlService.ExecuteQuery(query, parameters, reader => new Incidencia
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                Data = reader.GetDateTime(3),
                NomTasca = reader.GetString(4),
                NomGrup = reader.GetString(5),
                DescripcioIncidencia = reader.GetString(6),
                Resolta = reader.GetBoolean(7),
                Solucio = reader.IsDBNull(8) ? null : reader.GetString(8)
            });
        }

        public List<Incidencia> GetIncidenciasByIdTascaAndDateRange(int idTasca, DateTime startDate, DateTime endDate)
        {
            string query = @"
            SELECT id, id_tasca, id_grup_tasques, data_incidencia, nom_tasca, nom_grup, descripcio_incidencia, resolta, solucio
            FROM registros_app.incidencia
            WHERE id_tasca = @idTasca AND data_incidencia BETWEEN @startDate AND @endDate";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", idTasca },
                { "startDate", startDate },
                { "endDate", endDate }
            };

            return npgsqlService.ExecuteQuery(query, parameters, reader => new Incidencia
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                Data = reader.GetDateTime(3),
                NomTasca = reader.GetString(4),
                NomGrup = reader.GetString(5),
                DescripcioIncidencia = reader.GetString(6),
                Resolta = reader.GetBoolean(7),
                Solucio = reader.IsDBNull(8) ? null : reader.GetString(8)
            });
        }

        public int GetNumeroIncidenciasNoResueltas()
        {
            string query = @"SELECT COUNT(*) FROM registros_app.incidencia WHERE resolta = false";

            return Convert.ToInt32(npgsqlService.ExecuteScalar(query, new Dictionary<string, object>()));
        }

        public bool HasUnresolvedIncidencias(int idGrupTasques)
        {
            string query = @"
            SELECT COUNT(*)
            FROM registros_app.incidencia
            WHERE id_grup_tasques = @idGrupTasques AND resolta = false";

            var parameters = new Dictionary<string, object>
            {
                { "idGrupTasques", idGrupTasques }
            };

            return Convert.ToInt32(npgsqlService.ExecuteScalar(query, parameters)) > 0;
        }

        public void UpdateIncidencia(Incidencia incidencia)
        {
            string query = @"
            UPDATE registros_app.incidencia
            SET id_tasca = @idTasca,
                id_grup_tasques = @idGrupTasques,
                data_incidencia = @dataIncidencia,
                nom_tasca = @nomTasca,
                nom_grup = @nomGrup,
                descripcio_incidencia = @descripcioIncidencia,
                resolta = @resolta,
                solucio = @solucio
            WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", incidencia.IdTasca },
                { "idGrupTasques", incidencia.IdGrupTasques },
                { "dataIncidencia", incidencia.Data },
                { "nomTasca", incidencia.NomTasca },
                { "nomGrup", incidencia.NomGrup },
                { "descripcioIncidencia", incidencia.DescripcioIncidencia },
                { "resolta", incidencia.Resolta },
                { "solucio", incidencia.Solucio ?? (object)DBNull.Value },
                { "id", incidencia.Id }
            };

            npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
