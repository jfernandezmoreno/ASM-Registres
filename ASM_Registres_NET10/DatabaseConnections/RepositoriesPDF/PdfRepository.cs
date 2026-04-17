using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
namespace ASM_Registres_NET10.DatabaseConnections.RepositoriesPDF
{
    public class PdfRepository : IPdfRepository
    {
        private readonly NPGSQLService _db;

        public PdfRepository(NPGSQLService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public int InsertPdf(byte[] fileData)
        {
            if (fileData == null || fileData.Length == 0)
                throw new ArgumentException("El PDF no puede estar vacío.", nameof(fileData));

            const string sql = @"
                INSERT INTO registros_app.pdf_files (file_data)
                VALUES (@fileData)
                RETURNING id;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@fileData", fileData }
            };

            var result = _db.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(result);
        }

        public byte[] GetPdfById(int id)
        {
            const string sql = @"
                SELECT file_data
                FROM registros_app.pdf_files
                WHERE id = @id;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            var results = _db.ExecuteQuery(
                sql,
                parameters,
                reader => (byte[])reader["file_data"]
            );

            return results.Count > 0 ? results[0] : null;
        }

        public void UpdatePdf(int id, byte[] fileData)
        {
            if (fileData == null || fileData.Length == 0)
                throw new ArgumentException("El PDF no puede estar vacío.", nameof(fileData));

            const string sql = @"
                UPDATE registros_app.pdf_files
                SET file_data = @fileData
                WHERE id = @id;
            ";

            var parameters = new Dictionary<string, object>
            {
                { "@fileData", fileData },
                { "@id", id }
            };

            _db.ExecuteNonQuery(sql, parameters);
        }
    }
}
