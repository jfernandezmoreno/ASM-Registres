using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroLimpiezaProduccionRepository : IRegistroLimpiezaProduccionRepository
    {

        private readonly NPGSQLService _npgsqlService;

        public RegistroLimpiezaProduccionRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void DeleteRegistro(int id)
        {
            string query = "DELETE FROM registros_app.registro_limpieza_produccion WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<RegistroLimpiezaProduccion> GetAllRegistros()
        {
            string query = @"SELECT id, operario, fecha, finalizada, tipo
                             FROM registros_app.registro_limpieza_produccion";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistroLimpiezaProduccion
            {
                Id = reader.GetInt32(0),
                Operario = reader.GetString(1),
                Fecha = reader.GetDateTime(2),
                Finalizada = reader.GetBoolean(3),
                Tipo = reader.GetString(4),
            });
        }

        public RegistroLimpiezaProduccion GetRegistroById(int id)
        {
            string query = @"SELECT id, operario, fecha, finalizada, tipo
                             FROM registros_app.registro_limpieza_produccion
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new RegistroLimpiezaProduccion
            {
                Id = reader.GetInt32(0),
                Operario = reader.GetString(1),
                Fecha = reader.GetDateTime(2),
                Finalizada = reader.GetBoolean(3),
                Tipo = reader.GetString(4)

            }).FirstOrDefault();
        }

        public void InsertRegistro(RegistroLimpiezaProduccion registro)
        {
            string query = @"INSERT INTO registros_app.registro_limpieza_produccion (operario, fecha, finalizada, tipo)
                             VALUES (@operario, @fecha, @finalizada, @tipo)";

            var parameters = new Dictionary<string, object>
            {
                { "operario", registro.Operario },
                { "fecha", registro.Fecha },
                { "finalizada", false },
                { "tipo", registro.Tipo}
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateRegistro(RegistroLimpiezaProduccion registro)
        {
            string query = @"UPDATE registros_app.registro_limpieza_produccion
                             SET operario = @operario, fecha = @fecha
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", registro.Id },
                { "operario", registro.Operario },
                { "fecha", registro.Fecha }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateFinalizadaEstado(int id, bool finalizada)
        {
            string query = @"UPDATE registros_app.registro_limpieza_produccion
                     SET finalizada = @finalizada
                     WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id },
                { "finalizada", finalizada }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public bool ExisteRegistroNoFinalizado()
        {
            string query = @"SELECT COUNT(*) FROM registros_app.registro_limpieza_produccion
                     WHERE finalizada = FALSE";

            int count = Convert.ToInt32(_npgsqlService.ExecuteScalar(query, new Dictionary<string, object>()));

            return count > 0;
        }


    }
}
