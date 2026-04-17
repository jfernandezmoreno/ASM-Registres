using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres_NET10.Modules.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class ExportTasks : Telerik.WinControls.UI.RadForm
    {
        ProdTasksRecurrentRepository prodRepository;
        LabTasksRecurrentRepository labRepository;
        ManTasksRecurrentRepository mantRepository;

        public ExportTasks()
        {
            InitializeComponent();

            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            radDateTimePicker1.Value = DateTime.Today;
        }

        private void radButton4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            // LAB

            if (radDateTimePicker1.Value == DateTime.MinValue)
            {
                RadMessageBox.Show("Debes escribir una fecha correcta para generar el PDF", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            labRepository = new LabTasksRecurrentRepository();

            List<Tasks> tareasDelDia = labRepository.GetTareasByFecha(radDateTimePicker1.Value);

            var tareasOrdenadas = tareasDelDia.OrderBy(t => t.Prioridad).ToList();

            var listaTareas = tareasOrdenadas.Where(t => t.Tipo == "Tarea").ToList();
            var listaRecordatorios = tareasOrdenadas.Where(t => t.Tipo == "Recordatorio").ToList();

            try
            {
                string fileName = Path.Combine(Path.GetTempPath(), "Tareas.pdf");

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 36, 36, 36, 36);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    DateTime fechaSeleccionada = radDateTimePicker1.Value;
                    string tituloDocumento = string.Format("Tareas producción       {0}-{1}/{2}/{3}",
                        fechaSeleccionada.ToString("dd"),
                        fechaSeleccionada.AddDays(1).ToString("dd"),
                        fechaSeleccionada.ToString("MM"),
                        fechaSeleccionada.ToString("yyyy"));

                    Paragraph parrafoTitulo = new Paragraph(tituloDocumento, FontFactory.GetFont("Helvetica", 16));
                    parrafoTitulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(parrafoTitulo);
                    doc.Add(new Paragraph(" "));

                    if (listaTareas.Any())
                    {
                        PdfPTable tablaTareas = new PdfPTable(2);
                        tablaTareas.WidthPercentage = 100;
                        float[] widths = new float[] { 85f, 15f };
                        tablaTareas.SetWidths(widths);

                        PdfPCell headerTareas = new PdfPCell(new Phrase("Tareas", FontFactory.GetFont("Helvetica", 14)));
                        headerTareas.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTareas.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTareas.Padding = 5;
                        tablaTareas.AddCell(headerTareas);

                        PdfPCell headerTurno = new PdfPCell(new Phrase("Turno", FontFactory.GetFont("Helvetica", 14)));
                        headerTurno.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTurno.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTurno.Padding = 5;
                        tablaTareas.AddCell(headerTurno);

                        tablaTareas.HeaderRows = 1;

                        foreach (var tarea in listaTareas)
                        {
                            string textoCelda = tarea.Titulo;
                            if (!string.IsNullOrEmpty(tarea.Descripcion))
                            {
                                textoCelda += " - " + tarea.Descripcion;
                            }
                            if (!string.IsNullOrEmpty(tarea.Comentario))
                            {
                                textoCelda += " - " + tarea.Comentario;
                            }
                            if (!string.IsNullOrEmpty(tarea.PersonaAsignada) && tarea.PersonaAsignada != "Cap")
                            {
                                textoCelda += " : " + tarea.PersonaAsignada;
                            }

                            PdfPCell celda = new PdfPCell(new Phrase(textoCelda, FontFactory.GetFont("Helvetica", 12)));
                            celda.Padding = 5;
                            tablaTareas.AddCell(celda);

                            PdfPCell celdaTurno = new PdfPCell(new Phrase(tarea.Turno, FontFactory.GetFont("Helvetica", 12)));
                            celdaTurno.Padding = 5;
                            tablaTareas.AddCell(celdaTurno);
                        }

                        doc.Add(tablaTareas);
                    }

                    Paragraph separation = new Paragraph(" ", FontFactory.GetFont("Helvetica", 12));
                    separation.SpacingBefore = 10;
                    separation.SpacingAfter = 10;
                    doc.Add(separation);

                    if (listaRecordatorios.Any())
                    {
                        PdfPTable tablaRecordatorios = new PdfPTable(2);
                        tablaRecordatorios.WidthPercentage = 100;
                        float[] widthsRecordatorios = new float[] { 85f, 15f };
                        tablaRecordatorios.SetWidths(widthsRecordatorios);

                        PdfPCell headerRecordatorios = new PdfPCell(new Phrase("Recordatorios", FontFactory.GetFont("Helvetica", 14)));
                        headerRecordatorios.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerRecordatorios.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerRecordatorios.Padding = 5;
                        tablaRecordatorios.AddCell(headerRecordatorios);

                        PdfPCell headerTurnoRec = new PdfPCell(new Phrase("Turno", FontFactory.GetFont("Helvetica", 14)));
                        headerTurnoRec.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTurnoRec.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTurnoRec.Padding = 5;
                        tablaRecordatorios.AddCell(headerTurnoRec);

                        tablaRecordatorios.HeaderRows = 1;

                        foreach (var recordatorio in listaRecordatorios)
                        {
                            string textoCelda = recordatorio.Titulo;
                            if (!string.IsNullOrEmpty(recordatorio.Descripcion))
                            {
                                textoCelda += " - " + recordatorio.Descripcion;
                            }
                            if (!string.IsNullOrEmpty(recordatorio.Comentario))
                            {
                                textoCelda += " - " + recordatorio.Comentario;
                            }
                            if (!string.IsNullOrEmpty(recordatorio.PersonaAsignada) && recordatorio.PersonaAsignada != "Cap")
                            {
                                textoCelda += " : " + recordatorio.PersonaAsignada;
                            }

                            PdfPCell celda = new PdfPCell(new Phrase(textoCelda, FontFactory.GetFont("Helvetica", 12)));
                            celda.Padding = 5;
                            tablaRecordatorios.AddCell(celda);


                            PdfPCell celdaTurno = new PdfPCell(new Phrase(recordatorio.Turno, FontFactory.GetFont("Helvetica", 12)));
                            celdaTurno.Padding = 5;
                            tablaRecordatorios.AddCell(celdaTurno);
                        }

                        doc.Add(tablaRecordatorios);
                    }

                    doc.Close();
                    writer.Close();
                }

                PdfViewerForm viewerForm = new PdfViewerForm(fileName);
                viewerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("Error generando el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            // PROD

            if (radDateTimePicker1.Value == DateTime.MinValue)
            {
                RadMessageBox.Show("Has d'escriure una data correcta per generar el PDF", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);

                return;
            }

            prodRepository = new ProdTasksRecurrentRepository();

            List<Tasks> tareasDelDia = prodRepository.GetTareasByFecha(radDateTimePicker1.Value);

            // Ordenar tareas en orden ASCENDENTE según la propiedad 'Prioridad'
            var tareasOrdenadas = tareasDelDia.OrderBy(t => t.Prioridad).ToList();

            var listaTareas = tareasOrdenadas.Where(t => t.Tipo == "Tarea").ToList();
            var listaRecordatorios = tareasOrdenadas.Where(t => t.Tipo == "Recordatorio").ToList();

            try
            {
                string fileName = Path.Combine(Path.GetTempPath(), "Tareas.pdf");

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Crear documento en orientación horizontal (paisaje)
                    Document doc = new Document(PageSize.A4.Rotate(), 36, 36, 36, 36);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    DateTime fechaSeleccionada = radDateTimePicker1.Value;
                    string tituloDocumento = string.Format("Tareas producción       {0}-{1}/{2}/{3}",
                        fechaSeleccionada.ToString("dd"),
                        fechaSeleccionada.AddDays(1).ToString("dd"),
                        fechaSeleccionada.ToString("MM"),
                        fechaSeleccionada.ToString("yyyy"));

                    Paragraph parrafoTitulo = new Paragraph(tituloDocumento, FontFactory.GetFont("Helvetica", 16));
                    parrafoTitulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(parrafoTitulo);
                    doc.Add(new Paragraph(" "));

                    if (listaTareas.Any())
                    {
                        // Tabla con 2 columnas: Tareas (85%) y Turno (15%)
                        PdfPTable tablaTareas = new PdfPTable(2);
                        tablaTareas.WidthPercentage = 100;
                        float[] widths = new float[] { 85f, 15f };
                        tablaTareas.SetWidths(widths);

                        // Cabeceras de la tabla
                        PdfPCell headerTareas = new PdfPCell(new Phrase("Tareas", FontFactory.GetFont("Helvetica", 14)));
                        headerTareas.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTareas.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTareas.Padding = 5;
                        tablaTareas.AddCell(headerTareas);

                        PdfPCell headerTurno = new PdfPCell(new Phrase("Turno", FontFactory.GetFont("Helvetica", 14)));
                        headerTurno.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTurno.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTurno.Padding = 5;
                        tablaTareas.AddCell(headerTurno);

                        tablaTareas.HeaderRows = 1;

                        foreach (var tarea in listaTareas)
                        {
                            string textoCelda = tarea.Titulo;
                            if (!string.IsNullOrEmpty(tarea.Descripcion))
                            {
                                textoCelda += " - " + tarea.Descripcion;
                            }
                            if (!string.IsNullOrEmpty(tarea.Comentario))
                            {
                                textoCelda += " - " + tarea.Comentario;
                            }
                            if (!string.IsNullOrEmpty(tarea.PersonaAsignada) && tarea.PersonaAsignada != "Cap")
                            {
                                textoCelda += " : " + tarea.PersonaAsignada;
                            }

                            PdfPCell celda = new PdfPCell(new Phrase(textoCelda, FontFactory.GetFont("Helvetica", 12)));
                            celda.Padding = 5;
                            tablaTareas.AddCell(celda);

                            // Se añade la celda para el turno
                            // Suponiendo que la propiedad 'Turno' existe en el objeto tarea
                            PdfPCell celdaTurno = new PdfPCell(new Phrase(tarea.Turno, FontFactory.GetFont("Helvetica", 12)));
                            celdaTurno.Padding = 5;
                            tablaTareas.AddCell(celdaTurno);
                        }

                        doc.Add(tablaTareas);
                    }

                    // Párrafo de separación entre las tablas
                    Paragraph separation = new Paragraph(" ", FontFactory.GetFont("Helvetica", 12));
                    separation.SpacingBefore = 10;
                    separation.SpacingAfter = 10;
                    doc.Add(separation);

                    if (listaRecordatorios.Any())
                    {
                        // Tabla con 2 columnas: Recordatorios (85%) y Turno (15%)
                        PdfPTable tablaRecordatorios = new PdfPTable(2);
                        tablaRecordatorios.WidthPercentage = 100;
                        float[] widthsRecordatorios = new float[] { 85f, 15f };
                        tablaRecordatorios.SetWidths(widthsRecordatorios);

                        PdfPCell headerRecordatorios = new PdfPCell(new Phrase("Recordatorios", FontFactory.GetFont("Helvetica", 14)));
                        headerRecordatorios.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerRecordatorios.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerRecordatorios.Padding = 5;
                        tablaRecordatorios.AddCell(headerRecordatorios);

                        PdfPCell headerTurnoRec = new PdfPCell(new Phrase("Turno", FontFactory.GetFont("Helvetica", 14)));
                        headerTurnoRec.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTurnoRec.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTurnoRec.Padding = 5;
                        tablaRecordatorios.AddCell(headerTurnoRec);

                        tablaRecordatorios.HeaderRows = 1;

                        foreach (var recordatorio in listaRecordatorios)
                        {
                            string textoCelda = recordatorio.Titulo;
                            if (!string.IsNullOrEmpty(recordatorio.Descripcion))
                            {
                                textoCelda += " - " + recordatorio.Descripcion;
                            }
                            if (!string.IsNullOrEmpty(recordatorio.Comentario))
                            {
                                textoCelda += " - " + recordatorio.Comentario;
                            }
                            if (!string.IsNullOrEmpty(recordatorio.PersonaAsignada) && recordatorio.PersonaAsignada != "Ninguna")
                            {
                                textoCelda += " : " + recordatorio.PersonaAsignada;
                            }

                            PdfPCell celda = new PdfPCell(new Phrase(textoCelda, FontFactory.GetFont("Helvetica", 12)));
                            celda.Padding = 5;
                            tablaRecordatorios.AddCell(celda);

                            // Se añade la celda para el turno
                            // Suponiendo que la propiedad 'Turno' existe en el objeto recordatorio
                            PdfPCell celdaTurno = new PdfPCell(new Phrase(recordatorio.Turno, FontFactory.GetFont("Helvetica", 12)));
                            celdaTurno.Padding = 5;
                            tablaRecordatorios.AddCell(celdaTurno);
                        }

                        doc.Add(tablaRecordatorios);
                    }

                    doc.Close();
                    writer.Close();
                }

                PdfViewerForm viewerForm = new PdfViewerForm(fileName);
                viewerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("Error generando el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            // MANT

            if (radDateTimePicker1.Value == DateTime.MinValue)
            {
                RadMessageBox.Show("Has d'escriure una data correcta per generar el PDF", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);

                return;
            }

            mantRepository = new ManTasksRecurrentRepository();

            List<Tasks> tareasDelDia = mantRepository.GetTareasByFecha(radDateTimePicker1.Value);

            // Ordenar tareas en orden ASCENDENTE según la propiedad 'Prioridad'
            var tareasOrdenadas = tareasDelDia.OrderBy(t => t.Prioridad).ToList();

            var listaTareas = tareasOrdenadas.Where(t => t.Tipo == "Tarea").ToList();
            var listaRecordatorios = tareasOrdenadas.Where(t => t.Tipo == "Recordatorio").ToList();

            try
            {
                string fileName = Path.Combine(Path.GetTempPath(), "Tasques.pdf");

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Crear documento en orientación horizontal (paisaje)
                    Document doc = new Document(PageSize.A4.Rotate(), 36, 36, 36, 36);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    DateTime fechaSeleccionada = radDateTimePicker1.Value;
                    string tituloDocumento = string.Format("Tasques de producció       {0}-{1}/{2}/{3}",
                        fechaSeleccionada.ToString("dd"),
                        fechaSeleccionada.AddDays(1).ToString("dd"),
                        fechaSeleccionada.ToString("MM"),
                        fechaSeleccionada.ToString("yyyy"));


                    Paragraph parrafoTitulo = new Paragraph(tituloDocumento, FontFactory.GetFont("Helvetica", 16));
                    parrafoTitulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(parrafoTitulo);
                    doc.Add(new Paragraph(" "));

                    if (listaTareas.Any())
                    {
                        // Tabla con 2 columnas: Tareas (85%) y Turno (15%)
                        PdfPTable tablaTareas = new PdfPTable(2);
                        tablaTareas.WidthPercentage = 100;
                        float[] widths = new float[] { 85f, 15f };
                        tablaTareas.SetWidths(widths);

                        // Cabeceras de la tabla
                        PdfPCell headerTareas = new PdfPCell(new Phrase("Tasques", FontFactory.GetFont("Helvetica", 14)));
                        headerTareas.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTareas.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTareas.Padding = 5;
                        tablaTareas.AddCell(headerTareas);

                        PdfPCell headerTurno = new PdfPCell(new Phrase("Torn", FontFactory.GetFont("Helvetica", 14)));
                        headerTurno.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTurno.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTurno.Padding = 5;
                        tablaTareas.AddCell(headerTurno);

                        tablaTareas.HeaderRows = 1;

                        foreach (var tarea in listaTareas)
                        {
                            string textoCelda = tarea.Titulo;
                            if (!string.IsNullOrEmpty(tarea.Descripcion))
                            {
                                textoCelda += " - " + tarea.Descripcion;
                            }
                            if (!string.IsNullOrEmpty(tarea.Comentario))
                            {
                                textoCelda += " - " + tarea.Comentario;
                            }
                            if (!string.IsNullOrEmpty(tarea.PersonaAsignada) && tarea.PersonaAsignada != "Ninguno")
                            {
                                textoCelda += " : " + tarea.PersonaAsignada;
                            }

                            PdfPCell celda = new PdfPCell(new Phrase(textoCelda, FontFactory.GetFont("Helvetica", 12)));
                            celda.Padding = 5;
                            tablaTareas.AddCell(celda);

                            // Se añade la celda para el turno
                            // Suponiendo que la propiedad 'Turno' existe en el objeto tarea
                            PdfPCell celdaTurno = new PdfPCell(new Phrase(tarea.Turno, FontFactory.GetFont("Helvetica", 12)));
                            celdaTurno.Padding = 5;
                            tablaTareas.AddCell(celdaTurno);
                        }

                        doc.Add(tablaTareas);
                    }

                    // Párrafo de separación entre las tablas
                    Paragraph separation = new Paragraph(" ", FontFactory.GetFont("Helvetica", 12));
                    separation.SpacingBefore = 10;
                    separation.SpacingAfter = 10;
                    doc.Add(separation);

                    if (listaRecordatorios.Any())
                    {
                        // Tabla con 2 columnas: Recordatorios (85%) y Turno (15%)
                        PdfPTable tablaRecordatorios = new PdfPTable(2);
                        tablaRecordatorios.WidthPercentage = 100;
                        float[] widthsRecordatorios = new float[] { 85f, 15f };
                        tablaRecordatorios.SetWidths(widthsRecordatorios);

                        PdfPCell headerRecordatorios = new PdfPCell(new Phrase("Recordatoris", FontFactory.GetFont("Helvetica", 14)));
                        headerRecordatorios.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerRecordatorios.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerRecordatorios.Padding = 5;
                        tablaRecordatorios.AddCell(headerRecordatorios);

                        PdfPCell headerTurnoRec = new PdfPCell(new Phrase("Torn", FontFactory.GetFont("Helvetica", 14)));
                        headerTurnoRec.BackgroundColor = BaseColor.LIGHT_GRAY;
                        headerTurnoRec.HorizontalAlignment = Element.ALIGN_LEFT;
                        headerTurnoRec.Padding = 5;
                        tablaRecordatorios.AddCell(headerTurnoRec);

                        tablaRecordatorios.HeaderRows = 1;

                        foreach (var recordatorio in listaRecordatorios)
                        {
                            string textoCelda = recordatorio.Titulo;
                            if (!string.IsNullOrEmpty(recordatorio.Descripcion))
                            {
                                textoCelda += " - " + recordatorio.Descripcion;
                            }
                            if (!string.IsNullOrEmpty(recordatorio.Comentario))
                            {
                                textoCelda += " - " + recordatorio.Comentario;
                            }
                            if (!string.IsNullOrEmpty(recordatorio.PersonaAsignada) && recordatorio.PersonaAsignada != "Ninguna")
                            {
                                textoCelda += " : " + recordatorio.PersonaAsignada;
                            }

                            PdfPCell celda = new PdfPCell(new Phrase(textoCelda, FontFactory.GetFont("Helvetica", 12)));
                            celda.Padding = 5;
                            tablaRecordatorios.AddCell(celda);

                            // Se añade la celda para el turno
                            // Suponiendo que la propiedad 'Turno' existe en el objeto recordatorio
                            PdfPCell celdaTurno = new PdfPCell(new Phrase(recordatorio.Turno, FontFactory.GetFont("Helvetica", 12)));
                            celdaTurno.Padding = 5;
                            tablaRecordatorios.AddCell(celdaTurno);
                        }

                        doc.Add(tablaRecordatorios);
                    }

                    doc.Close();
                    writer.Close();
                }

                PdfViewerForm viewerForm = new PdfViewerForm(fileName);
                viewerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("Error en generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }
    }
}
