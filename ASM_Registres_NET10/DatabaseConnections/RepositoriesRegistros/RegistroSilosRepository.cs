using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroSilosRepository : IRegistroSilos
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroSilosRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void InsertRegistroSilos(RegistroSilos registro)
        {
            string query = @"INSERT INTO registros_app.registro_silos 
                             (fecha, precinto, precintado_por, nombre_tarea, nombre_grupo) 
                             VALUES (@fecha, @precinto, @precintado_por, @nombre_tarea, @nombre_grupo)";

            var parameters = new Dictionary<string, object>
            {
                { "fecha", registro.Fecha },
                { "precinto", registro.Precinto },
                { "precintado_por", registro.PrecintadoPor },
                { "nombre_tarea", registro.NombreTarea },
                { "nombre_grupo", registro.NombreGrupo }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
