using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroBasculaRepository : IRegistroBascula
    {

        private readonly NPGSQLService _npgsqlService;

        public RegistroBasculaRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void AddRegistreBascula(RegistroBascula registre)
        {
            string query = @"INSERT INTO registros_app.registre_bascula 
                         (idtasca, idgruptasques, nomtasca, nomgrup, data, valor, observacions, estat, feta_per)
                         VALUES (@idTasca, @idGrupTasques, @nomTasca, @nomGrup, @data, @valor, @observacions, @estat, @feta_per)";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", registre.IdTasca },
                { "idGrupTasques", registre.IdGrupTasques },
                { "nomTasca", registre.NomTasca },
                { "nomGrup", registre.NomGrup },
                { "data", registre.Data },
                { "valor", registre.Valor },
                { "observacions", string.IsNullOrEmpty(registre.Observacions) ? (object)DBNull.Value : registre.Observacions },
                { "estat", registre.Estat },
                { "feta_per", registre.FetaPer }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public bool ExisteRegistroBascula(int id)
        {
            string query = "SELECT COUNT(*) FROM registros_app.registre_bascula WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            return Convert.ToInt32(_npgsqlService.ExecuteScalar(query, parameters)) > 0;
        }

        public RegistroBascula GetRegistro(int id)
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, valor, observacions, estat, feta_per 
                             FROM registros_app.registre_bascula 
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new RegistroBascula
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Valor = reader.GetDouble(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                Estat = reader.GetString(8),
                FetaPer = reader.GetString(9)
            }).FirstOrDefault();
        }

        public List<RegistroBascula> GetRegistrosBasculaIncomplet()
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, valor, observacions, estat, feta_per
                             FROM registros_app.registre_bascula
                             WHERE estat = 'Incompleta'";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistroBascula
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Valor = reader.GetDouble(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                Estat = reader.GetString(8),
                FetaPer = reader.GetString(9)
            });
        }

        public List<RegistroBascula> GetRegistrosBasculaPendent()
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, valor, observacions, estat, feta_per
                             FROM registros_app.registre_bascula
                             WHERE estat = 'Pendent'";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistroBascula
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Valor = reader.GetDouble(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                Estat = reader.GetString(8),
                FetaPer = reader.GetString(9)
            });
        }

        public List<RegistroBascula> GetRegistrosBasculaPorFechaYIdTasca(DateTime fechaInicio, DateTime fechaFin, int idTasca)
        {
            string query = @"SELECT id, idtasca, idgruptasques, nomtasca, nomgrup, data, valor, observacions, estat, feta_per
                             FROM registros_app.registre_bascula
                             WHERE idtasca = @idTasca AND data >= @fechaInicio AND data <= @fechaFin";

            var parameters = new Dictionary<string, object>
            {
                { "idTasca", idTasca },
                { "fechaInicio", fechaInicio },
                { "fechaFin", fechaFin }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new RegistroBascula
            {
                Id = reader.GetInt32(0),
                IdTasca = reader.GetInt32(1),
                IdGrupTasques = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                NomGrup = reader.GetString(4),
                Data = reader.GetDateTime(5),
                Valor = reader.GetDouble(6),
                Observacions = !reader.IsDBNull(7) ? reader.GetString(7) : null,
                Estat = reader.GetString(8),
                FetaPer = reader.GetString(9)
            });
        }

        public void UpdateRegistroBascula(int id, string estat)
        {
            string query = "UPDATE registros_app.registre_bascula SET estat = @estat WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "estat", estat },
                { "id", id }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateRegistroBascula(int id, string estat, string obv)
        {
            string query = "UPDATE registros_app.registre_bascula SET estat = @estat, observacions = @observacions WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "estat", estat },
                { "observacions", string.IsNullOrEmpty(obv) ? (object)DBNull.Value : obv },
                { "id", id }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
