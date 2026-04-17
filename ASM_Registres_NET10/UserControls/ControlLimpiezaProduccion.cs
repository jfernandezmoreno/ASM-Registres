using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.UserControls
{
    public partial class ControlLimpiezaProduccion : UserControl
    {

        RegistroLimpiezaProduccion registro;
        ControlRegistroLimpiezaProduccionRepository repositoryControles;
        RegistroLimpiezaProduccionRepository repositoryLimpieza;

        NPGSQLService service = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        List<ControlRegistroLimpiezaProduccion> controles = new List<ControlRegistroLimpiezaProduccion>();

        User user;

        public ControlLimpiezaProduccion(RegistroLimpiezaProduccion registro, User user)
        {
            InitializeComponent();
            this.registro = registro;
            this.user = user;
            repositoryControles = new ControlRegistroLimpiezaProduccionRepository(service);
            repositoryLimpieza = new RegistroLimpiezaProduccionRepository(service);

            radLabel1.Text = radLabel1.Text + registro.Id.ToString();

            InitDocks();
            SetupGrid();

            radGridView1.CellDoubleClick += RadGridView1_CellDoubleClick;
            radGridView1.UserDeletingRow += RadGridView1_UserDeletingRow;
        }

        private void RadGridView1_UserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {
            if (e.Rows[0] is GridViewDataRowInfo row)
            {
                var confirm = RadMessageBox.Show(
                    "Segur que vols eliminar aquest registre?",
                    "Confirmar eliminació",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Exclamation
                );

                if (confirm != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                if (row.Cells["Id"].Value != null && int.TryParse(row.Cells["Id"].Value.ToString(), out int id))
                {
                    try
                    {
                        repositoryControles.DeleteControl(id);
                        controles.RemoveAll(c => c.Id == id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error en eliminar el registre: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("ID no vàlid. No s'ha pogut eliminar el registre.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            SetupGrid();
        }



        private void RadGridView1_CellDoubleClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            if (radGridView1.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                ControlRegistroLimpiezaProduccion registroActual = new ControlRegistroLimpiezaProduccion
                {
                    Id = Convert.ToInt32(selectedRow.Cells["Id"].Value),
                    IdRegistroLimpiezaProduccion = Convert.ToInt32(selectedRow.Cells["IdRegistroLimpiezaProduccion"].Value),
                    Fecha = Convert.ToDateTime(selectedRow.Cells["Fecha"].Value),
                    HoraInicio = selectedRow.Cells["HoraInicio"].Value?.ToString(),
                    HoraFinal = selectedRow.Cells["HoraFinal"].Value?.ToString(),
                    Accion = selectedRow.Cells["Accion"].Value?.ToString(),
                    Iniciales = selectedRow.Cells["Iniciales"].Value?.ToString(),
                    KgRecogidos = Convert.ToInt32(selectedRow.Cells["KgRecogidos"].Value),
                    KgSilice = Convert.ToInt32(selectedRow.Cells["KgSilice"].Value),
                    LoteSilice = selectedRow.Cells["LoteSilice"].Value?.ToString(),
                    KgCarbonato = Convert.ToInt32(selectedRow.Cells["KgCarbonato"].Value),
                    LoteCarbonato = selectedRow.Cells["LoteCarbonato"].Value?.ToString(),
                    Lote = selectedRow.Cells["Lote"].Value?.ToString(),
                    Integridad = selectedRow.Cells["Integridad"].Value?.ToString(),
                    Finalizada = Convert.ToBoolean(selectedRow.Cells["Finalizada"].Value),
                    Observaciones = selectedRow.Cells["Observaciones"].Value?.ToString()
                };

                var form = new AddEditControlLimpiezaProduccion(registro.Id, registroActual);
                form.FormClosed += Form_FormClosed1;
                form.ShowDialog();
            }
        }

        private void SetupGrid()
        {
            controles = repositoryControles
            .GetControlesByRegistroLimpiezaId(registro.Id)
            .OrderBy(c => c.Fecha.Date)
            .ThenBy(c => c.HoraInicio)
            .ToList();

            radGridView1.DataSource = null;
            radGridView1.DataSource = controles;

            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = true;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableGrouping = false;

            SetIdsVisibleFalse();
            RenameGridColumns();
            ActualizarColoresFinalizadaEnGrid();

            // Formatear columna Fecha
            if (radGridView1.Columns.Contains("Fecha"))
            {
                radGridView1.Columns["Fecha"].FormatString = "{0:dd/MM/yyyy}";
                radGridView1.Columns["Fecha"].FormatInfo = System.Globalization.CultureInfo.InvariantCulture;
            }
        }

        private void InitDocks()
        {
            radPanel1.Dock = DockStyle.Top;
            radPanel3.Dock = DockStyle.Fill;
            radPanel2.Dock = DockStyle.Bottom;

            radGridView1.Dock = DockStyle.Fill;
            radGridView1.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            ControlRegistroLimpiezaProduccion registro = new ControlRegistroLimpiezaProduccion();
            registro.Id = 0;
            AddEditControlLimpiezaProduccion form = new AddEditControlLimpiezaProduccion(this.registro.Id, registro);
            form.FormClosed += Form_FormClosed1;
            form.ShowDialog();
        }

        private void Form_FormClosed1(object sender, FormClosedEventArgs e)
        {
            SetupGrid();
        }

        private void radButton3_Click(object sender, EventArgs e)
        {   
            if (radGridView1.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                if (selectedRow.Cells["Id"].Value == null) return;

                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);

                bool finalizadaActual = Convert.ToBoolean(selectedRow.Cells["Finalizada"].Value);

                bool nuevoEstado = !finalizadaActual;

                repositoryControles.UpdateFinalizadaEstado(id, nuevoEstado);

                var control = controles.FirstOrDefault(c => c.Id == id);

                if (control != null)
                {
                    control.Finalizada = nuevoEstado;
                }

                selectedRow.Cells["Finalizada"].Value = nuevoEstado;

                SetupGrid();
            }
            else
            {
                RadMessageBox.Show("Selecciona un registre per canviar l'estat.", "Avís", MessageBoxButtons.OK, RadMessageIcon.Info);
            }

        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            if (repositoryLimpieza.ExisteRegistroNoFinalizado())
            {
                RadMessageBox.Show("Hi ha neteges no finalitzades; és impossible crear-ne una de nova.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            var form = new NuevaLimpiezaProduccionForm(user);

            form.LimpiezaCreada += (s, args) =>
            {
                var parent = this.Parent;
                while (parent != null && !(parent is ProductionTasksUC))
                    parent = parent.Parent;

                if (parent is ProductionTasksUC productionUC)
                    productionUC.RecargarArbol();

                controles = repositoryControles.GetControlesByRegistroLimpiezaId(registro.Id);
                SetupGrid();
            };

            form.ShowDialog();
        }


        private void ActualizarColoresFinalizadaEnGrid()
        {
            foreach (var row in radGridView1.Rows)
            {
                if (row is GridViewDataRowInfo dataRow && dataRow.Cells["Finalizada"].Value != null)
                {
                    bool finalizada = Convert.ToBoolean(dataRow.Cells["Finalizada"].Value);

                    if (finalizada)
                    {
                        foreach (var cell in dataRow.Cells)
                        {
                            cell.Style.ForeColor = Color.Green;
                        }
                    }
                    else
                    {
                        foreach (var cell in dataRow.Cells)
                        {
                            cell.Style.ForeColor = Color.Blue;
                        }
                    }
                }
            }
        }


        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            // GESTIONAR EL CIERRE DE UN NUEVO FORMULARIO
        }

        private void SetIdsVisibleFalse()
        {
            var columnsToHide = new[] { "Id", "IdRegistroLimpiezaProduccion", "IdIntern", "Darrera" };
            foreach (var column in columnsToHide)
            {
                if (radGridView1.Columns.Contains(column))
                {
                    radGridView1.Columns[column].IsVisible = false;
                }
            }
        }

        private void RenameGridColumns()
        {
            var columnHeaders = new Dictionary<string, string>
                {
                    { "HoraInicio", "Hora d'inici" },
                    { "HoraFinal", "Hora final" },
                    { "KgRecogidos", "Kg recollits" },
                    { "KgSilice", "Kg de sílice" },
                    { "LoteSilice", "Lot de sílice" },
                    { "KgCarbonato", "Kg de carbonat" },
                    { "LoteCarbonato", "Lot de carbonat" },
                    { "Integridad", "Integritat del tamís" },
                };

            foreach (var column in columnHeaders)
            {
                if (radGridView1.Columns.Contains(column.Key))
                    radGridView1.Columns[column.Key].HeaderText = column.Value;
            }
        }


        private void radButton4_Click(object sender, EventArgs e)
        {
            bool nuevoEstado = !registro.Finalizada;

            repositoryLimpieza.UpdateFinalizadaEstado(registro.Id, nuevoEstado);
            registro.Finalizada = nuevoEstado; 

            var parent = this.Parent;

            while (parent != null && !(parent is ProductionTasksUC))
            {
                parent = parent.Parent;
            }

            if (parent is ProductionTasksUC productionUC)
            {
                productionUC.RecargarArbol();
            }

            controles = repositoryControles.GetControlesByRegistroLimpiezaId(registro.Id);
            SetupGrid();
        }
    }
}
