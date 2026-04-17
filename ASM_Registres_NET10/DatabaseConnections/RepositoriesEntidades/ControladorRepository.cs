using ASM_Registres_NET10.Services;
using System.Collections.Generic;
using System.Linq;


namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades
{
    public class ControladorRepository : IController
    {
        private readonly NPGSQLService _npgsqlService;

        public ControladorRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }
        public List<int> GetNumerosEnContenido(int id)
        {
            string query = "SELECT contenido FROM registros_app.controlador WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            return _npgsqlService.ExecuteQuery(query, parameters, reader =>
            {
                string contenido = reader["contenido"]?.ToString() ?? "";
                List<int> numeros = new List<int>();

                string[] partes = contenido.Split(',');
                foreach (var parte in partes)
                {
                    if (int.TryParse(parte.Trim(), out int numero))
                    {
                        numeros.Add(numero);
                    }
                }

                return numeros;
            }).FirstOrDefault() ?? new List<int>();
        }
    }
}
