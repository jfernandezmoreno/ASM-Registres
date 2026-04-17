using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Services;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesPDF;

namespace ASM_Registres_NET10.Forms
{
    public partial class VerActualizacion : Telerik.WinControls.UI.RadForm
    {
        private readonly PdfRepository _pdfRepository;

        public VerActualizacion()
        {
            InitializeComponent();

            var npgsqlService = new NPGSQLService(DatabaseCredentials.ConnectionStringAWS);
            _pdfRepository = new PdfRepository(npgsqlService);

            radPdfViewer1.Dock = DockStyle.Fill;

            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Load += VerActualizacion_Load;
        }

        private void VerActualizacion_Load(object sender, EventArgs e)
        {
            //InsertPDF();
            CargarPdf(1);
        }

        private void InsertPDF()
        {
            string path = @"C:\Users\lluc\Desktop\PDF_Registros.pdf";

            if (!File.Exists(path))
            {
                RadMessageBox.Show("No s'ha trobat el fitxer PDF.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            byte[] pdfBytes = File.ReadAllBytes(path);
            int id = _pdfRepository.InsertPdf(pdfBytes);

            RadMessageBox.Show("PDF inserit correctament amb ID: " + id,
                                "Correcte", MessageBoxButtons.OK, RadMessageIcon.Info);
        }

        private void CargarPdf(int id)
        {
            try
            {
                byte[] pdfBytes = _pdfRepository.GetPdfById(id);

                if (pdfBytes == null)
                {
                    RadMessageBox.Show(
                        $"No se ha encontrado ningún PDF con id {id}.",
                        "Información",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Info
                    );
                    return;
                }

                var provider = new PdfFormatProvider();

                using (var ms = new MemoryStream(pdfBytes))
                {
                    RadFixedDocument document = provider.Import(ms);
                    radPdfViewer1.Document = document;
                }
            }

            catch (Exception ex)
            {
                RadMessageBox.Show(
                    "Error al cargar el PDF: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error
                );
            }
        }
    }
}
