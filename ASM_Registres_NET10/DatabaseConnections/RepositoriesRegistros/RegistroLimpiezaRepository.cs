using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroLimpiezaRepository : IRegistroLimpieza
    {
        private readonly NPGSQLService _npgsqlService;

        public RegistroLimpiezaRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void InsertRegistroLimpieza(RegistroLimpieza registro)
        {
            string query = @"INSERT INTO registros_app.registro_limpieza 
                    (fecha, producto, cantidad) 
                    VALUES (@fecha, @producto, @cantidad)";

            var parameters = new Dictionary<string, object>
            {
                { "fecha", registro.Fecha },
                { "producto", registro.Producto },
                { "cantidad", registro.Cantidad }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
