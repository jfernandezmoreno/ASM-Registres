using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class ControlAguasRepository : IControlAguas
    {

        private readonly NPGSQLService _npgsqlService;

        public ControlAguasRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void AddControlAguas(ControlAguas controlAguas)
        {
            string query = @"
            INSERT INTO registros_app.control_aguas 
            (fecha, control_aguas, cloro_libre, nitritos, ph, dureza, olor, turbidez, operario, dqs, observaciones) 
            VALUES (@Fecha, @ControlAguasValue, @CloroLibre, @Nitritos, @Ph, @Dureza, @Olor, @Turbidez, @Operario, @Dqs, @Observaciones)";

            var parameters = new Dictionary<string, object>
            {
                { "@Fecha", controlAguas.Fecha },
                { "@ControlAguasValue", controlAguas.ControlAguasValue },
                { "@CloroLibre", controlAguas.CloroLibre },
                { "@Nitritos", controlAguas.Nitritos },
                { "@Ph", controlAguas.Ph },
                { "@Dureza", controlAguas.Dureza },
                { "@Olor", controlAguas.Olor },
                { "@Turbidez", controlAguas.Turbidez },
                { "@Operario", controlAguas.Operario },
                { "@Dqs", controlAguas.DQS },
                { "@Observaciones", controlAguas.Observaciones }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<ControlAguas> GetControlAguasPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            string query = @"
            SELECT fecha, control_aguas, cloro_libre, nitritos, ph, dureza, olor, turbidez, operario, dqs, observaciones
            FROM registros_app.control_aguas
            WHERE fecha BETWEEN @FechaInicio AND @FechaFin";

            var parameters = new Dictionary<string, object>
            {
                { "@FechaInicio", fechaInicio },
                { "@FechaFin", fechaFin }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader => new ControlAguas
            {
                Fecha = reader.GetDateTime(0),
                ControlAguasValue = reader.GetDouble(1),
                CloroLibre = reader.GetDouble(2),
                Nitritos = reader.GetDouble(3),
                Ph = reader.GetDouble(4),
                Dureza = reader.GetString(5),
                Olor = reader.GetString(6),
                Turbidez = reader.GetString(7),
                Operario = reader.GetString(8),
                DQS = reader.GetString(9),
                Observaciones = !reader.IsDBNull(10) ? reader.GetString(10) : null
            });
        }
    }
}
