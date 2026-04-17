using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.UserControls
{
    public partial class ShiftsManager : UserControl
    {
        ShiftsRepository ShiftsRepository;
        private List<Empleados> empleados;
        private GridViewRowInfo selectedRow;
        List<RegistroTurno> registros;

        public ShiftsManager()
        {
            InitializeComponent();

            ShiftsRepository = new ShiftsRepository();
            registros = new List<RegistroTurno>();
            empleados = ShiftsRepository.GetAllUsuarios();

            InitGrid();
            ConfigureGridView();

        }

        private void ConfigureGridView()
        {
            radGridView1.Columns.Clear();
            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowDragToGroup = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.AllowAddNewRow = false;
            radGridView1.Dock = DockStyle.Fill;

            GridViewTextBoxColumn nombreColumn = new GridViewTextBoxColumn
            {
                Name = "NombreUsuario",
                HeaderText = "Nombre",
                FieldName = "NombreUsuario"
            };
            radGridView1.Columns.Add(nombreColumn);

            GridViewTextBoxColumn turnoColumn = new GridViewTextBoxColumn
            {
                Name = "Turno",
                HeaderText = "Turno",
                FieldName = "Turno"
            };
            radGridView1.Columns.Add(turnoColumn);

            radGridView1.CellClick += RadGridView1_CellClick1;
        }

        private void RadGridView1_CellClick1(object sender, GridViewCellEventArgs e)
        {
            GridCellElement cell = (GridCellElement)sender;
            if (cell.ColumnInfo.Name == "NombreUsuario")
            {
                DateTime selectedDate = radCalendar5.SelectedDate;

                var row = cell.RowInfo.DataBoundItem as dynamic;

                string nombreUsuario = row.NombreUsuario;
                int idRegistro = row.IdRegistro;
                int idUsuario = row.IdUsuario;
                string tipoTurno = row.Turno;

                EscogerTurno escoger;

                if (tipoTurno == "Sin turno asignado")
                {
                    tipoTurno = "Sin asignar";
                    escoger = new EscogerTurno(nombreUsuario, tipoTurno, idUsuario, idRegistro, this);
                    escoger.Show();
                }
                else
                {
                    selectedRow = cell.RowInfo;
                    escoger = new EscogerTurno(nombreUsuario, tipoTurno, idUsuario, idRegistro, this);
                    escoger.Show();
                }
            }
        }

        public void getInfo(string tipoTurno, int idUser, int idRegistro, string nombre)
        {

            DateTime selectedDate = radCalendar5.SelectedDate;

            if (idRegistro == 0)
            {

                RegistroTurno registroNuevo = new RegistroTurno(selectedDate, idUser, tipoTurno, nombre);
                ShiftsRepository.InsertarOActualizarRegistro(registroNuevo);
            }

            else
            {
                RegistroTurno registro = ShiftsRepository.GetRegistroById(idRegistro);
                if (registro == null)
                {
                    RadMessageBox.Show("No se encontró el registro especificado.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                registro.NombreUsuario = nombre;
                registro.IdTurno = tipoTurno;

                ShiftsRepository.InsertarOActualizarRegistro(registro);
            }

            registros = ShiftsRepository.GetRegistrosPorFecha(selectedDate);


            /* DUDOSO ⊂(◉‿◉)つ */

            var combinedList = empleados.Select(u => new
            {
                u.IdUsuario,
                u.NombreUsuario,
                Turno = registros.FirstOrDefault(r => r.IdUsuario == u.IdUsuario)?.IdTurno ?? "Sin asignar",
                IdRegistro = registros.FirstOrDefault(r => r.IdUsuario == u.IdUsuario)?.IdRegistro ?? 0
            }).ToList();

            radGridView1.DataSource = combinedList;
        }

        private void InitGrid()
        {
            RadSplitContainer verticalSplitContainer = new RadSplitContainer();
            verticalSplitContainer.Orientation = Orientation.Horizontal;
            verticalSplitContainer.Dock = DockStyle.Fill;

            RadSplitContainer horizontalSplitContainer = new RadSplitContainer();
            horizontalSplitContainer.Orientation = Orientation.Vertical;
            horizontalSplitContainer.Dock = DockStyle.Fill;

            SplitPanel panel1 = new SplitPanel();
            SplitPanel panel2 = new SplitPanel();
            SplitPanel panel3 = new SplitPanel();

            horizontalSplitContainer.SplitPanels.Add(panel1);
            horizontalSplitContainer.SplitPanels.Add(panel2);

            verticalSplitContainer.SplitPanels.Add(horizontalSplitContainer);
            verticalSplitContainer.SplitPanels.Add(panel3);

            panel1.Controls.Add(radCalendar5);
            panel2.Controls.Add(radGridView1);
            panel3.Controls.Add(radPanel1);

            radCalendar5.Dock = DockStyle.Fill;
            radGridView1.Dock = DockStyle.Fill;
            radPanel1.Dock = DockStyle.Fill;

            panel3.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
            panel3.SizeInfo.AbsoluteSize = new System.Drawing.Size(0, 75);

            radPanel2.Dock = DockStyle.Right;
            radPanel2.Controls.Add(radButton1);
            radButton1.Dock = DockStyle.Fill;

            Controls.Add(verticalSplitContainer);
        }

        private void CopiarTurnosASemana(DateTime selectedDate)
        {
            DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek + (int)DayOfWeek.Monday);

            List<RegistroTurno> turnosDelDiaSeleccionado = ShiftsRepository.GetRegistrosPorFecha(selectedDate);

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = startOfWeek.AddDays(i);

                if (currentDate.Date != selectedDate.Date)
                {
                    ShiftsRepository.EliminarRegistrosPorFecha(currentDate);

                    for (int j = 0; j < turnosDelDiaSeleccionado.Count; j++)
                    {
                        RegistroTurno registro = turnosDelDiaSeleccionado[j];

                        string nombre = this.empleados[j].NombreUsuario;
                        string turnoID = registro.IdTurno;

                        RegistroTurno nuevoRegistro = new RegistroTurno
                        (
                            currentDate,
                            registro.IdUsuario,
                            turnoID,
                            nombre
                        );
                        ShiftsRepository.InsertarOActualizarRegistro(nuevoRegistro);
                    }
                }
            }

            RadMessageBox.Show("Turnos copiados a toda la semana correctamente.", "Información", MessageBoxButtons.OK, RadMessageIcon.Info);

            registros = ShiftsRepository.GetRegistrosPorFecha(selectedDate);

            var combinedList = empleados.Select(u => new
            {
                u.IdUsuario,
                u.NombreUsuario,
                Turno = registros.FirstOrDefault(r => r.IdUsuario == u.IdUsuario)?.IdTurno ?? "Sin asignar",
                IdRegistro = registros.FirstOrDefault(r => r.IdUsuario == u.IdUsuario)?.IdRegistro ?? 0
            }).ToList();

            radGridView1.DataSource = combinedList;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            DialogResult result = RadMessageBox.Show("¿Estás seguro de que quieres realizar esta acción?", "Confirmación", MessageBoxButtons.YesNo, RadMessageIcon.Question);

            if (result == DialogResult.Yes)
            {
                DateTime selectedDate = radCalendar5.SelectedDate;
                CopiarTurnosASemana(selectedDate);
            }
        }

        private void radCalendar5_SelectionChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = radCalendar5.SelectedDate;
            registros = ShiftsRepository.GetRegistrosPorFecha(selectedDate);

            /* DUDOSO (ㆆ _ ㆆ) */
            var combinedList = empleados.Select(u => new
            {
                u.IdUsuario,
                u.NombreUsuario,
                Turno = registros.FirstOrDefault(r => r.IdUsuario == u.IdUsuario)?.IdTurno ?? "Sin asignar",
                IdRegistro = registros.FirstOrDefault(r => r.IdUsuario == u.IdUsuario)?.IdRegistro ?? 0
            }).ToList();

            radGridView1.DataSource = combinedList;
        }
    }
}
