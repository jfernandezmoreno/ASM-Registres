using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroTemperaturaCamaraRepository : IRegistroTemperatura
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroTemperaturaCamaraRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void AddRegistroTermico(RegistroTemperaturaCamara registro)
        {
            string query = @"
            INSERT INTO registros_app.registro_temperatura_camara 
            (camara, dia_hora, paro, marcha, cambio_temperatura, temperatura_camara, operario) 
            VALUES (@camara, @diaHora, @paro, @marcha, @cambioTemperatura, @temperaturaCamara, @operario)";

            var parameters = new Dictionary<string, object>
            {
                { "camara", registro.Camara },
                { "diaHora", registro.DiaHora },
                { "paro", registro.Paro },
                { "marcha", registro.Marcha },
                { "cambioTemperatura", registro.CambioTemperatura },
                { "temperaturaCamara", registro.TemperaturaCamara },
                { "operario", registro.Operario }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
