using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;

namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros
{
    public class RegistroMantenimientoRepository : IRegistroMantenimiento
    {

        private readonly NPGSQLService _npgsqlService;

        public RegistroMantenimientoRepository(NPGSQLService npgsqlService)
        {
            _npgsqlService = npgsqlService;
        }

        public void AddRegistreManteniment(RegistreManteniment registre)
        {
            string query = @"
            INSERT INTO registros_app.registre_manteniment 
            (id, id_tasca_interna, id_grup, nom_tasca, propera_data_programada, observacions, feta_per)
            VALUES (@id, @idTascaInterna, @idGrup, @nomTasca, @properaDataProgramada, @observacions, @fetaPer)";

            var parameters = new Dictionary<string, object>
            {
                { "id", registre.Id },
                { "idTascaInterna", registre.IdTascaInterna },
                { "idGrup", registre.IdGrup },
                { "nomTasca", registre.NomTasca },
                { "properaDataProgramada", registre.ProperaDataProgramada },
                { "observacions", string.IsNullOrEmpty(registre.Observacions) ? (object)DBNull.Value : registre.Observacions },
                { "fetaPer", registre.FetaPer }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public void DeleteRegistreManteniment(string nomTasca, string observacions, DateTime properaDataProgramada)
        {
            string query = @"
            DELETE FROM registros_app.registre_manteniment
            WHERE nom_tasca = @nomTasca
            AND observacions = @observacions
            AND propera_data_programada = @properaDataProgramada";

            var parameters = new Dictionary<string, object>
            {
                { "nomTasca", nomTasca },
                { "observacions", observacions },
                { "properaDataProgramada", properaDataProgramada }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }

        public List<RegistreManteniment> GetAllRegistresManteniment()
        {
            string query = "SELECT id, id_tasca_interna, id_grup, nom_tasca, propera_data_programada, observacions, feta_per FROM registros_app.registre_manteniment";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistreManteniment
            {
                Id = reader.GetString(0),
                IdTascaInterna = reader.GetInt32(1),
                IdGrup = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                ProperaDataProgramada = reader.GetDateTime(4),
                Observacions = reader.IsDBNull(5) ? null : reader.GetString(5),
                FetaPer = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        public int GetMaxIdRegistreManteniment()
        {
            string query = "SELECT MAX(id) FROM registros_app.registre_manteniment";

            var result = _npgsqlService.ExecuteScalar(query, new Dictionary<string, object>());

            return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
        }

        public List<RegistreManteniment> GetTodosRegistrosManteniment()
        {
            string query = @"
            SELECT 
                rm.id,
                rm.id_tasca_interna,
                rm.id_grup,
                rm.nom_tasca,
                rm.propera_data_programada,
                rm.observacions,
                rm.feta_per
            FROM 
                registros_app.registre_manteniment rm
            ORDER BY 
                rm.nom_tasca";

            return _npgsqlService.ExecuteQuery(query, new Dictionary<string, object>(), reader => new RegistreManteniment
            {
                Id = reader.GetString(0),
                IdTascaInterna = reader.GetInt32(1),
                IdGrup = reader.GetInt32(2),
                NomTasca = reader.GetString(3),
                ProperaDataProgramada = reader.GetDateTime(4),
                Observacions = reader.IsDBNull(5) ? null : reader.GetString(5),
                FetaPer = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        public void UpdateRegistreManteniment(RegistreManteniment registre)
        {
            string query = @"
            UPDATE registros_app.registre_manteniment
            SET id_tasca_interna = @idTascaInterna,
                id_grup = @idGrup,
                nom_tasca = @nomTasca,
                propera_data_programada = @properaDataProgramada,
                observacions = @observacions,
                feta_per = @fetaPer
            WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "id", registre.Id },
                { "idTascaInterna", registre.IdTascaInterna },
                { "idGrup", registre.IdGrup },
                { "nomTasca", registre.NomTasca },
                { "properaDataProgramada", registre.ProperaDataProgramada },
                { "observacions", string.IsNullOrEmpty(registre.Observacions) ? (object)DBNull.Value : registre.Observacions },
                { "fetaPer", registre.FetaPer }
            };

            _npgsqlService.ExecuteNonQuery(query, parameters);
        }
    }
}
