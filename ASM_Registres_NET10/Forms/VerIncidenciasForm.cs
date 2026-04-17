using ASM_Registres_NET10.Modules;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.Forms
{
    public partial class VerIncidenciasForm : Telerik.WinControls.UI.RadForm
    {

        User user;
        List<Incidencia> incidencias;

        public VerIncidenciasForm(User user, List<Incidencia> incidencias)
        {
            InitializeComponent();

            this.user = user;
            this.incidencias = incidencias;

            SetupWindow();
            SetupDock();
            SetupGrid();
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void SetupGrid()
        {
            radGridView1.DataSource = incidencias;

            string[] columnsToHide = { "Id", "IdTasca", "IdGrupTasques" };
            foreach (string columnName in columnsToHide)
            {
                if (radGridView1.Columns[columnName] != null)
                {
                    radGridView1.Columns[columnName].IsVisible = false;
                }
            }
            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

            var columnHeaders = new Dictionary<string, string>
            {
                { "nomtasca", "Tasca" },
                { "nomgrup", "Grup Tasques" },
                { "DescripcioIncidencia", "Descripció" }
            };

            foreach (var column in columnHeaders)
            {
                if (radGridView1.Columns.Contains(column.Key))
                {
                    radGridView1.Columns[column.Key].HeaderText = column.Value;
                }
            }

            radGridView1.CellDoubleClick += RadGridView1_CellClick;

            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableFiltering = true;
            radGridView1.EnableGrouping = true;
            radGridView1.MultiSelect = true;        
        }

        private void RadGridView1_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (e.Row is null || e.RowIndex < 0) return; // Evitar errores si se hace clic en encabezados o filas vacías

            try
            {
                var id = Convert.ToInt32(e.Row.Cells["Id"].Value); 
                var idTasca = Convert.ToInt32(e.Row.Cells["IdTasca"].Value);
                var idGrupTasques = Convert.ToInt32(e.Row.Cells["IdGrupTasques"].Value);
                var data = Convert.ToDateTime(e.Row.Cells["Data"].Value);
                var nomTasca = e.Row.Cells["nomtasca"]?.Value?.ToString() ?? "Desconocido";
                var nomGrup = e.Row.Cells["nomgrup"]?.Value?.ToString() ?? "Desconocido";
                var descripcionIncidencia = e.Row.Cells["DescripcioIncidencia"]?.Value.ToString() ?? "Desconocido";
                var resolta = e.Row.Cells["Resolta"]?.Value != null && Convert.ToBoolean(e.Row.Cells["Resolta"].Value);
                var solucio = e.Row.Cells["Solucio"]?.Value?.ToString();

                Incidencia incidencia = new Incidencia
                {
                    IdTasca = idTasca,
                    IdGrupTasques = idGrupTasques,
                    Data = data,
                    NomTasca = nomTasca,
                    NomGrup = nomGrup,
                    DescripcioIncidencia = descripcionIncidencia,
                    Resolta = resolta,
                    Solucio = solucio
                };

                Form gestionar = new GestionarIncidenciesForm(incidencia, user);
                gestionar.ShowDialog();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Ocurrió un error al ver la incidencia: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }


        private void SetupDock()
        {
            radGridView1.Dock = DockStyle.Fill;
        }
    }
}
