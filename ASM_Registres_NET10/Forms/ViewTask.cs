using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Tasks;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.Forms
{
    /// <summary>
    /// Formulario de visualización y gestión de una tarea.
    /// </summary>
    public partial class ViewTask : Telerik.WinControls.UI.RadForm
    {
        #region Constantes

        private const string EstadoEnProgreso = "En progreso";
        private const string EstadoFinalizada = "Finalizada";
        private const string EstadoTerminada = "Terminada";
        private const string EstadoPendent = "Pendent";

        private const string TituloInfo = "Informació";
        private const string TituloError = "Error";
        private const string TituloAviso = "Avís";

        #endregion

        #region Campos

        private Tasks _task;
        private readonly ProdTasksRecurrentRepository _repo;
        private readonly TaskParticipationRepository _partRepo;

        private RadContextMenu _gridMenu;

        private RadMenuItem _endItem;
        private RadMenuItem _deleteItem;

        private RadContextMenuManager _gridMenuManager;


        #endregion

        #region Constructor

        public ViewTask(Tasks task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            InitializeComponent();

            _task = task;
            _repo = new ProdTasksRecurrentRepository();
            _partRepo = new TaskParticipationRepository();

            SetupWindow();
            ReloadTaskFromDb();

            _gridMenu = BuildGridContextMenu();

            radGridView1.ContextMenuStrip = null;
            radGridView1.ContextMenuOpening += (s, e) => e.Cancel = true;

            radGridView1.MouseUp += RadGridView1_MouseUp;


            UpdateForm();
        }

        private void RadGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var cell = radGridView1.ElementTree.GetElementAtPoint(e.Location) as GridDataCellElement;
            if (cell == null) return; 

            radGridView1.CurrentRow = cell.RowInfo;
            _gridMenu.Show(radGridView1, e.Location);
        }

        private RadContextMenu BuildGridContextMenu()
        {
            var menu = new RadContextMenu();

            _endItem = new RadMenuItem("Finalitzar participació");
            _endItem.Click += FinalizarParticipacion_Click;

            _deleteItem = new RadMenuItem("Eliminar participació");
            _deleteItem.Click += EliminarParticipacion_Click;

            menu.Items.Add(_endItem);
            menu.Items.Add(_deleteItem);
            return menu;
        }

        private void EliminarParticipacion_Click(object sender, EventArgs e)
        {
            try
            {
                var row = radGridView1.CurrentRow as GridViewDataRowInfo;
                if (row == null) return;

                var idObj = row.Cells["Id"].Value;
                if (idObj == null || idObj == DBNull.Value)
                {
                    RadMessageBox.Show("No s'ha trobat l'ID de la participació.", TituloError,
                        MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                long participationId = Convert.ToInt64(idObj);

                var confirm = RadMessageBox.Show("Vols eliminar aquesta participació?",
                    "Confirmació", MessageBoxButtons.YesNo, RadMessageIcon.Question);

                if (confirm != DialogResult.Yes) return;

                _partRepo.DeleteById(participationId);
                LoadParticipationsGrid();

                RadMessageBox.Show("Participació eliminada.", "Correcte",
                    MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"No s'ha pogut eliminar la participació: {ex.Message}",
                    TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }
        private void RadGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var element = radGridView1.ElementTree.GetElementAtPoint(e.Location) as GridDataCellElement;
            if (element != null)
            {
                // Selecciona la fila bajo el cursor para que el menú actúe sobre ella
                radGridView1.CurrentRow = element.RowInfo;
            }
        }
        private void FinalizarParticipacion_Click(object sender, EventArgs e)
        {
            try
            {
                var row = radGridView1.CurrentRow as GridViewDataRowInfo;
                if (row == null) return;

                var fechaFinObj = row.Cells["FechaFin"].Value;
                if (fechaFinObj != null && fechaFinObj != DBNull.Value)
                {
                    RadMessageBox.Show("Aquesta participació ja està finalitzada.", TituloInfo,
                        MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                int idEmpleado = Convert.ToInt32(row.Cells["IdEmpleado"].Value);

                _partRepo.EndOpenByTaskAndEmployee(_task.Id, idEmpleado, DateTime.Now, null);

                LoadParticipationsGrid();
                RadMessageBox.Show("Participació finalitzada.", "Correcte",
                    MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"No s'ha pogut finalitzar la participació: {ex.Message}",
                    TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }


        #endregion

        #region Eventos UI

        private void radButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadParticipationsGrid()
        {
            try
            {
                if (radGridView1 == null || _task == null) return;

                var data = _partRepo.GetByTaskId(_task.Id);
                radGridView1.DataSource = data;

                radGridView1.ReadOnly = true;
                radGridView1.AllowAddNewRow = false;
                radGridView1.AllowDeleteRow = false;
                radGridView1.AllowEditRow = false;
                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

                if (radGridView1.Columns.Contains("Empleado"))
                    radGridView1.Columns["Empleado"].HeaderText = "Empleat";
                if (radGridView1.Columns.Contains("FechaInicio"))
                    radGridView1.Columns["FechaInicio"].HeaderText = "Inici";
                if (radGridView1.Columns.Contains("FechaFin"))
                    radGridView1.Columns["FechaFin"].HeaderText = "Fi";
                if (radGridView1.Columns.Contains("Id"))
                    radGridView1.Columns["Id"].IsVisible = false;
                if (radGridView1.Columns.Contains("IdEmpleado"))
                    radGridView1.Columns["IdEmpleado"].IsVisible = false;


            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"No s'han pogut carregar les participacions: {ex.Message}",
                    TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void empezarButton(object sender, EventArgs e)
        {
            try
            {
                if (IsFinalizadaOEquivalente(_task))
                {
                    RadMessageBox.Show("La tasca ja està finalitzada.", TituloInfo, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                if (IsEnProgreso(_task))
                {
                    RadMessageBox.Show("La tasca ja està en curs.", TituloInfo, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                var clave = PromptClave("Introdueix la teva clau per participar a la tasca:");
                if (clave == null) return;

                var empleado = _repo.GetEmployeeByClave(clave);
                if (empleado == null)
                {
                    RadMessageBox.Show("Clau incorrecta o empleat no trobat.", "Accés denegat", MessageBoxButtons.OK, RadMessageIcon.Exclamation);

                    return;
                }

                var actual = _repo.GetTareaById(_task.Id);
                if (actual == null)
                {
                    RadMessageBox.Show("No s'ha pogut carregar la tasca.", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                if (IsFinalizadaOEquivalente(actual))
                {
                    RadMessageBox.Show("La tasca ja està finalitzada.", TituloInfo, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                if (IsEnProgreso(actual))
                {
                    RadMessageBox.Show("La tarea ja està en progres.", TituloInfo, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                _repo.EmpezarTareaSoloEstado(actual.Id);

                _partRepo.Start(actual.Id, empleado.IdUsuario, null, DateTime.Now);

                _task = _repo.GetTareaById(actual.Id);
                UpdateForm();

                RadMessageBox.Show("Tasca posada en curs correctament.", "Correcte", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error en iniciar la tasca: {ex.Message}", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void finalizarButton(object sender, EventArgs e)
        {
            try
            {
                if (IsFinalizadaOEquivalente(_task))
                {
                    RadMessageBox.Show("La tasca ja estava finalitzada.", TituloInfo, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                var actual = _repo.GetTareaById(_task.Id);
                if (actual == null)
                {
                    RadMessageBox.Show("No s'ha pogut carregar la tasca.", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                if (IsFinalizadaOEquivalente(actual))
                {
                    RadMessageBox.Show("La tasca ja estava finalitzada.", TituloInfo, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                var clave = PromptClave("Introdueix la teva clau per finalitzar la tasca:");
                if (clave == null) return;

                var empleado = _repo.GetEmployeeByClave(clave);
                if (empleado == null)
                {
                    RadMessageBox.Show("Clau incorrecta o empleat no trobat.", "Accés denegat",
                        MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                    return;
                }

                _repo.FinalizarTareaConEmpleado(actual.Id, empleado.IdUsuario, empleado.NombreUsuario);
                _partRepo.EndAllOpenByTask(actual.Id, DateTime.Now);

                CompletarRegistroSiProcede(actual, empleado.NombreUsuario);

                ReloadTaskFromDb();
                _task.Completada = true;
                _task.Estado = EstadoFinalizada;
                UpdateForm();

                RadMessageBox.Show("Tasca finalitzada i registre actualitzat (si escau).", "Correcte", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error en finalitzar la tasca: {ex.Message}", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);

            }
        }

        #endregion

        #region Métodos privados (UI)

        private void SetupWindow()
        {
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
        }

        private void UpdateForm()
        {
            SetReadOnly(radTextBoxControl1, true);
            SetReadOnly(radTextBoxControl2, true, multiline: true, wordWrap: true);
            SetReadOnly(radTextBoxControl3, true);
            SetReadOnly(radTextBoxControl4, true);
            SetReadOnly(radTextBoxControl5, true, multiline: true, wordWrap: true);

            radTextBoxControl1.Text = _task.Titulo ?? string.Empty;
            radTextBoxControl2.Text = _task.Descripcion ?? string.Empty;
            radTextBoxControl3.Text = _task.Turno ?? string.Empty;
            radTextBoxControl4.Text = _task.PersonaAsignada ?? string.Empty;
            radTextBoxControl5.Text = _task.Comentario ?? string.Empty;

            if (radTextBoxControl6 != null)
            {
                SetReadOnly(radTextBoxControl6, true);
                string empleadoNombre = string.Empty;
                if (_task.Employee_Id.HasValue)
                {
                    empleadoNombre = _repo.GetEmployeeNameById(_task.Employee_Id.Value) ?? string.Empty;
                }
                radTextBoxControl6.Text = empleadoNombre;
            }

            if (radTextBoxControl7 != null)
            {
                SetReadOnly(radTextBoxControl7, true);
                radTextBoxControl7.Text = _task.Estado ?? string.Empty;
            }

            if (radTextBoxControl8 != null)
            {
                SetReadOnly(radTextBoxControl8, true);
                string registroTexto = string.Empty;
                if (_task.Tasca_Id.HasValue)
                {
                    registroTexto = _repo.GetRegistroTextoByTascaId(_task.Tasca_Id.Value) ?? string.Empty;
                }
                radTextBoxControl8.Text = registroTexto;
            }

            LoadParticipationsGrid();

        }

        private static void SetReadOnly(Telerik.WinControls.UI.RadTextBoxControl tb, bool readOnly, bool multiline = false, bool wordWrap = false)
        {
            if (tb == null) return;
            tb.IsReadOnly = readOnly;
            tb.Multiline = multiline;
            tb.WordWrap = wordWrap;
        }

        #endregion

        #region Métodos privados (Dominio/Repos)

        private void ReloadTaskFromDb()
        {
            try
            {
                if (_task == null) return;
                var fresh = _repo.GetTareaById(_task.Id);
                if (fresh != null)
                {
                    _task = fresh;
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"No s'ha pogut tornar a carregar la tasca: {ex.Message}", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void CompletarRegistroSiProcede(Tasks tarea, string empleado)
        {
            try
            {
                if (tarea == null || !tarea.Tasca_Id.HasValue) return;

                var npg = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
                var tasquesRepo = new TareasRepository(npg);
                var registroRepo = new RegistroComunRepository(npg);

                var tasca = tasquesRepo.GetTaskById(tarea.Tasca_Id.Value);
                if (tasca == null) return;

                DateTime inicio = tarea.Dia.Date;
                DateTime fin = inicio.AddDays(1).AddTicks(-1);

                var existentes = registroRepo.GetRegistroComunesPorIdTasca(inicio, fin, tasca.Id);
                if (existentes != null && existentes.Count > 0)
                {
                    registroRepo.UpdateRegistroComun(existentes[0].Id, EstadoFinalizada);
                }
                else
                {
                    string fetaPer = null;
                    if (tarea.Employee_Id.HasValue)
                        fetaPer = _repo.GetEmployeeNameById(tarea.Employee_Id.Value);
                    if (string.IsNullOrWhiteSpace(fetaPer))
                        fetaPer = tarea.PersonaAsignada ?? string.Empty;

                    var nuevo = new ASM_Registres_NET10.Modules.Registros.RegistroComun
                    {
                        IdTasca = tasca.Id,
                        IdGrupTasques = tasca.IdGrup,
                        NomTasca = tasca.Titol,
                        NomGrup = tasca.Zona,
                        Data = DateTime.Now,
                        Estat = EstadoPendent,
                        Observacions = tarea.Comentario,
                        FetaPer = empleado
                    };

                    registroRepo.InsertRegistroComun(nuevo);
                }

                tasquesRepo.UpdateTasquesDarrera(tasca.Id, DateTime.Now);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"La tasca s'ha finalitzat, però hi ha hagut un problema en actualitzar el registre: {ex.Message}",
                    TituloAviso, MessageBoxButtons.OK, RadMessageIcon.Info);

            }
        }

        #endregion

        #region Helpers

        private static bool EqualsIgnoreCase(string a, string b)
            => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        private static bool IsFinalizadaOEquivalente(Tasks t)
            => t != null && (t.Completada || EqualsIgnoreCase(t.Estado, EstadoFinalizada) || EqualsIgnoreCase(t.Estado, EstadoTerminada));

        private static bool IsEnProgreso(Tasks t)
            => t != null && EqualsIgnoreCase(t.Estado, EstadoEnProgreso);

        private string PromptClave(string title)
        {
            using (var form = new Form())
            using (var label = new Label { Left = 12, Top = 12, Width = 360, Text = title })
            using (var textBox = new TextBox { Left = 12, Top = 36, Width = 360, UseSystemPasswordChar = true })
            using (var ok = new Button { Text = "OK", Left = 212, Width = 75, Top = 68, DialogResult = DialogResult.OK })
            using (var cancel = new Button { Text = "Cancelar", Left = 297, Width = 75, Top = 68, DialogResult = DialogResult.Cancel })
            {
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new System.Drawing.Size(384, 110);
                form.Controls.AddRange(new Control[] { label, textBox, ok, cancel });
                form.AcceptButton = ok;
                form.CancelButton = cancel;

                form.Text = "Participar";
                label.Text = title;
                ok.Text = "D'acord";
                cancel.Text = "Cancel·la";


                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    var value = (textBox.Text ?? string.Empty).Trim();
                    if (string.IsNullOrEmpty(value))
                    {
                        RadMessageBox.Show("La clau no pot estar buida.", "Atenció",
                            MessageBoxButtons.OK, RadMessageIcon.Exclamation);

                        return null;
                    }
                    return value;
                }

                return null;
            }
        }

        #endregion
    }
}
