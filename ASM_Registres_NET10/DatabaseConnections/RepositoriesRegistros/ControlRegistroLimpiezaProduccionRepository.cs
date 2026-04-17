using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;
using System.Linq;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class ControlRegistroLimpiezaProduccionRepository : IControlRegistroLimpiezaProduccionRepository
    {
        private readonly NPGSQLService _npgsqlService;

        public ControlRegistroLimpiezaProduccionRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public void DeleteControl(int id)
        {
            string query = "DELETE FROM registros_app.control_registro_limpieza_produccion WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<ControlRegistroLimpiezaProduccion> GetAllControles()
        {
            string query = @"SELECT id, id_registro_limpieza_produccion, fecha, hora_inicio, hora_final, accion, iniciales,
                             kg_recogidos, kg_silice, lote_silice, kg_carbonato, lote_carbonato, lote, integridad, finalizada, observaciones
                             FROM registros_app.control_registro_limpieza_produccion";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new ControlRegistroLimpiezaProduccion
            {
                Id = reader.GetInt32(0),
                IdRegistroLimpiezaProduccion = reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                HoraInicio = reader.GetString(3),
                HoraFinal = reader.GetString(4),
                Accion = reader.GetString(5),
                Iniciales = reader.GetString(6),
                KgRecogidos = reader.GetInt32(7),
                KgSilice = reader.GetInt32(8),
                LoteSilice = reader.GetString(9),
                KgCarbonato = reader.GetInt32(10),
                LoteCarbonato = reader.GetString(11),
                Lote = reader.GetString(12),
                Integridad = reader.GetString(13),
                Finalizada = reader.GetBoolean(14),
                Observaciones = reader.GetString(15)
            });
        }

        public ControlRegistroLimpiezaProduccion GetControlById(int id)
        {
            string query = @"SELECT id, id_registro_limpieza_produccion, fecha, hora_inicio, hora_final, accion, iniciales,
                             kg_recogidos, kg_silice, lote_silice, kg_carbonato, lote_carbonato, lote, integridad, finalizada, observaciones
                             FROM registros_app.control_registro_limpieza_produccion
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new ControlRegistroLimpiezaProduccion
            {
                Id = reader.GetInt32(0),
                IdRegistroLimpiezaProduccion = reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                HoraInicio = reader.GetString(3),
                HoraFinal = reader.GetString(4),
                Accion = reader.GetString(5),
                Iniciales = reader.GetString(6),
                KgRecogidos = reader.GetInt32(7),
                KgSilice = reader.GetInt32(8),
                LoteSilice = reader.GetString(9),
                KgCarbonato = reader.GetInt32(10),
                LoteCarbonato = reader.GetString(11),
                Lote = reader.GetString(12),
                Integridad = reader.GetString(13),
                Finalizada = reader.GetBoolean(14),
                Observaciones = reader.GetString(15)


            }).FirstOrDefault();
        }

        public List<ControlRegistroLimpiezaProduccion> GetControlesByRegistroLimpiezaId(int idRegistroLimpiezaProduccion)
        {
            string query = @"SELECT id, id_registro_limpieza_produccion, fecha, hora_inicio, hora_final, accion, iniciales,
                             kg_recogidos, kg_silice, lote_silice, kg_carbonato, lote_carbonato, lote, integridad, finalizada, observaciones
                             FROM registros_app.control_registro_limpieza_produccion
                             WHERE id_registro_limpieza_produccion = @idRegistro";

            var parameters = new Dictionary<string, object>
            {
                { "idRegistro", idRegistroLimpiezaProduccion }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new ControlRegistroLimpiezaProduccion
            {
                Id = reader.GetInt32(0),
                IdRegistroLimpiezaProduccion = reader.GetInt32(1),
                Fecha = reader.GetDateTime(2),
                HoraInicio = reader.GetString(3),
                HoraFinal = reader.GetString(4),
                Accion = reader.GetString(5),
                Iniciales = reader.GetString(6),
                KgRecogidos = reader.GetInt32(7),
                KgSilice = reader.GetInt32(8),
                LoteSilice = reader.GetString(9),
                KgCarbonato = reader.GetInt32(10),
                LoteCarbonato = reader.GetString(11),
                Lote = reader.GetString(12),
                Integridad = reader.GetString(13),
                Finalizada = reader.GetBoolean(14),
                Observaciones = reader.GetString(15)
            });
        }

        public void InsertControl(ControlRegistroLimpiezaProduccion control)
        {
            string query = @"INSERT INTO registros_app.control_registro_limpieza_produccion 
                            (id_registro_limpieza_produccion, fecha, hora_inicio, hora_final, accion, iniciales, 
                             kg_recogidos, kg_silice, lote_silice, kg_carbonato, lote_carbonato, lote, integridad, finalizada, observaciones)
                             VALUES (@id_registro, @fecha, @hora_inicio, @hora_final, @accion, @iniciales, 
                                     @kg_recogidos, @kg_silice, @lote_silice, @kg_carbonato, @lote_carbonato, @lote, @integridad, @finalizada, @observaciones)";

            var parameters = new Dictionary<string, object>
            {
                { "id_registro", control.IdRegistroLimpiezaProduccion },
                { "fecha", control.Fecha },
                { "hora_inicio", control.HoraInicio },
                { "hora_final", control.HoraFinal },
                { "accion", control.Accion },
                { "iniciales", control.Iniciales },
                { "kg_recogidos", control.KgRecogidos },
                { "kg_silice", control.KgSilice },
                { "lote_silice", control.LoteSilice },
                { "kg_carbonato", control.KgCarbonato },
                { "lote_carbonato", control.LoteCarbonato },
                { "lote", control.Lote },
                { "integridad", control.Integridad },
                { "finalizada", control.Finalizada },
                { "observaciones", control.Observaciones}
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateControl(ControlRegistroLimpiezaProduccion control)
        {
            string query = @"UPDATE registros_app.control_registro_limpieza_produccion
                             SET id_registro_limpieza_produccion = @id_registro,
                                 fecha = @fecha,
                                 hora_inicio = @hora_inicio,
                                 hora_final = @hora_final,
                                 accion = @accion,
                                 iniciales = @iniciales,
                                 kg_recogidos = @kg_recogidos,
                                 kg_silice = @kg_silice,
                                 lote_silice = @lote_silice,
                                 kg_carbonato = @kg_carbonato,
                                 lote_carbonato = @lote_carbonato,
                                 lote = @lote,
                                 integridad = @integridad,
                                 finalizada = @finalizada,
                                 observaciones = @observaciones
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", control.Id },
                { "id_registro", control.IdRegistroLimpiezaProduccion },
                { "fecha", control.Fecha },
                { "hora_inicio", control.HoraInicio },
                { "hora_final", control.HoraFinal },
                { "accion", control.Accion },
                { "iniciales", control.Iniciales },
                { "kg_recogidos", control.KgRecogidos },
                { "kg_silice", control.KgSilice },
                { "lote_silice", control.LoteSilice },
                { "kg_carbonato", control.KgCarbonato },
                { "lote_carbonato", control.LoteCarbonato },
                { "lote", control.Lote },
                { "integridad", control.Integridad },
                { "finalizada", control.Finalizada },
                { "observaciones", control.Observaciones }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void UpdateFinalizadaEstado(int idControl, bool finalizada)
        {
            string query = @"UPDATE registros_app.control_registro_limpieza_produccion
                             SET finalizada = @finalizada
                             WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", idControl },
                { "finalizada", finalizada }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<string> GetAllAccionesLimpiezaProduccion()
        {
            string query = "SELECT accion FROM registros_app.acciones_limpieza_produccion ORDER BY accion";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => reader.GetString(0));
        }
    }
}
