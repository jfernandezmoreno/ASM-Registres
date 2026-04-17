using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Tasks;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.Drawing;

namespace ASM_Registres.UserControls
{
    public partial class ProdTasksRecurrentSpecific : UserControl
    {
        #region Constantes

        private const string EstadoFinalizada = "Finalizada";
        private const string EstadoTerminada = "Terminada";
        private const string EstadoEnProgreso = "En progreso";
        private const string EstadoEmpezada = "Empezada";

        private const string TituloError = "Error";

        private static readonly string[] TurnosValidos = { "Noche", "Mañana", "Tarde", "Todos" };
        private static readonly string[] TiposValidos = { "Tarea", "Recordatorio" };

        private static readonly Color ColorTextoOk = Color.DarkGreen;
        private static readonly Color ColorFondoOk = Color.LightGreen;
        private static readonly Color ColorTextoRun = Color.DarkOrange;
        private static readonly Color ColorFondoRun = Color.Moccasin;

        #endregion

        private readonly ProdTasksRecurrentRepository _repo;
        private readonly NPGSQLService _npg;
        private readonly User _user;
        private readonly DateTime _day;
        private readonly List<string> _empleadosActivos;

        public ProdTasksRecurrentSpecific(DateTime d, User u)
        {
            if (u == null) throw new ArgumentNullException(nameof(u));

            InitializeComponent();

            _user = u;
            _day = d.Date;

            _npg = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            _repo = new ProdTasksRecurrentRepository();

            _empleadosActivos = _repo.GetActiveEmployees() ?? new List<string>();

            if (!_empleadosActivos.Contains("Cap"))
                _empleadosActivos.Insert(0, "Cap");

            RefreshData();
        }

        #region Inicialización

        private void SetupUserControl()
        {

            radGridView1.Dock = DockStyle.Fill;
            radGridView1.TableElement.RowHeight = 40;
            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            radGridView1.EnableGrouping = false;
            radGridView1.EnablePaging = false;
            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowDeleteRow = _user.nivel >= 6;
            radGridView1.AllowColumnReorder = false;

            radGridView1.CellBeginEdit -= radGridView1_CellBeginEdit;
            radGridView1.EditorRequired -= radGridView1_EditorRequired;
            radGridView1.CellValueChanged -= radGridView1_CellValueChanged;
            radGridView1.UserDeletingRow -= radGridView1_UserDeletingRow;

            radGridView1.CellBeginEdit += radGridView1_CellBeginEdit;
            radGridView1.EditorRequired += radGridView1_EditorRequired;
            radGridView1.CellValueChanged += radGridView1_CellValueChanged;
            radGridView1.UserDeletingRow += radGridView1_UserDeletingRow;

            if (_user.nivel == 6)
            {
                radGridView1.CellDoubleClick += RadGridView1_CellDoubleClick_Empty;
            }
            else
            {
                radGridView1.CellDoubleClick += RadGridView1_CellDoubleClick_OpenDetail;
            }

            if (_user.nivel == 6)
            {
                radGridView1.ContextMenuOpening += RadGridView1_ContextMenuOpening;
                radGridView1.MouseDown += RadGridView1_MouseDown_SelectRowOnRightClick;
            }
        }

        #endregion

        #region Carga de datos

        private void LoadData()
        {
            SetupUserControl();

            var tasksList = _repo.GetAllTareas() ?? new List<Tasks>();

            var filtered = tasksList
                .Where(t => t.Dia.Date == _day)
                .OrderBy(t => t.Prioridad)
                .ToList();

            radGridView1.DataSource = null;
            radGridView1.DataSource = filtered;

            ConfigureColumns();
            CenterGridColumnHeadersAndValues(radGridView1);

            foreach (var row in radGridView1.Rows)
            {
                FormatRow(row as GridViewDataRowInfo);
            }
        }

        private void ConfigureColumns()
        {
            if (radGridView1.Columns.Contains("Id"))
                radGridView1.Columns["Id"].IsVisible = false;

            if (radGridView1.Columns.Contains("Tasca_Id"))
                radGridView1.Columns["Tasca_Id"].IsVisible = false;

            if (radGridView1.Columns.Contains("Employee_Id"))
                radGridView1.Columns["Employee_Id"].IsVisible = false;

            if (radGridView1.Columns.Contains("Dia"))
                radGridView1.Columns["Dia"].FormatString = "{0:dd-MM-yyyy}";

            var headers = new Dictionary<string, string>
                {
                    { "Dia", "Data" },
                    { "Tipo", "Tipus" },
                    { "Titulo", "Títol" },
                    { "Descripcion", "Descripció" },
                    { "Comentario", "Comentari" },
                    { "PersonaAsignada", "Empleat Assignat" },
                    { "Prioridad", "Prioritat" },
                    { "Completada", "Completada" },
                    { "Turno", "Torn" },
                    { "Estado", "Estat" }
                };

            foreach (var kv in headers)
            {
                if (radGridView1.Columns.Contains(kv.Key))
                    radGridView1.Columns[kv.Key].HeaderText = kv.Value;
            }

            bool esSupervisor = _user.nivel == 6;

            var colTitulo = radGridView1.Columns["Titulo"];
            var colDescripcion = radGridView1.Columns["Descripcion"];

            if (colTitulo != null)
            {
                colTitulo.Width = 350;
                colTitulo.TextAlignment = ContentAlignment.MiddleLeft;
                colTitulo.WrapText = true;
            }

            if (colDescripcion != null)
            {
                colDescripcion.Width = 350;
                colDescripcion.TextAlignment = ContentAlignment.MiddleLeft;
                colDescripcion.WrapText = true;
            }

            foreach (GridViewColumn col in radGridView1.Columns)
            {
                if (col == colTitulo || col == colDescripcion)
                    continue;

                col.MinWidth = 60;
                col.MaxWidth = 120;
            }

            foreach (GridViewColumn c in radGridView1.Columns)
                c.ReadOnly = !esSupervisor;

            string[] alwaysReadOnly = { "Completada", "Id", "Tasca_Id", "Employee_Id" };
            foreach (var colName in alwaysReadOnly)
            {
                if (radGridView1.Columns.Contains(colName))
                    radGridView1.Columns[colName].ReadOnly = true;
            }

            if (!esSupervisor && radGridView1.Columns.Contains("Dia"))
                radGridView1.Columns["Dia"].IsVisible = false;
        }


        private static void CenterGridColumnHeadersAndValues(RadGridView gridView)
        {
            foreach (GridViewColumn column in gridView.Columns)
            {
                column.HeaderTextAlignment = ContentAlignment.MiddleCenter;

                if (column is GridViewDataColumn dataColumn)
                    dataColumn.TextAlignment = ContentAlignment.MiddleCenter;
            }
        }

        private static void FormatRow(GridViewDataRowInfo dataRow)
        {
            if (dataRow == null) return;

            foreach (GridViewCellInfo c in dataRow.Cells)
                c.Style.Reset();

            bool isCompleted = SafeBool(dataRow, "Completada");
            string estado = SafeText(dataRow, "Estado");

            if (isCompleted ||
                EqualsIgnoreCase(estado, EstadoFinalizada) ||
                EqualsIgnoreCase(estado, EstadoTerminada))
            {
                foreach (GridViewCellInfo cell in dataRow.Cells)
                {
                    cell.Style.ForeColor = ColorTextoOk;
                    cell.Style.BackColor = ColorFondoOk;
                }
                return;
            }

            if (EqualsIgnoreCase(estado, EstadoEmpezada) ||
                EqualsIgnoreCase(estado, EstadoEnProgreso))
            {
                foreach (GridViewCellInfo cell in dataRow.Cells)
                {
                    cell.Style.ForeColor = ColorTextoRun;
                    cell.Style.BackColor = ColorFondoRun;
                }
                return;
            }
        }

        private static bool SafeBool(GridViewDataRowInfo row, string colName)
        {
            try
            {
                var v = row.Cells[colName]?.Value;
                return v != null && Convert.ToBoolean(v);
            }
            catch { return false; }
        }

        private static string SafeText(GridViewDataRowInfo row, string colName)
        {
            var v = row.Cells[colName]?.Value;
            return v == null ? string.Empty : v.ToString();
        }

        private static bool EqualsIgnoreCase(string a, string b)
        {
            return string.Equals(a ?? string.Empty, b ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Interacción (doble clic / contexto / selección)

        private void RadGridView1_CellDoubleClick_OpenDetail(object sender, GridViewCellEventArgs e)
        {
            if (e.Column != null && e.Column.Name == "Comentario")
                return;

            if (e.Row != null && e.Row.DataBoundItem is Tasks task)
            {
                var formTask = new ViewTask(task);
                formTask.FormClosed += FormTask_FormClosed;
                formTask.ShowDialog();
            }
        }

        private void RadGridView1_CellDoubleClick_Empty(object sender, GridViewCellEventArgs e)
        {
            // Intencionadamente vacío para admin (abre con botón derecho)
        }

        private void RadGridView1_MouseDown_SelectRowOnRightClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var cell = radGridView1.ElementTree.GetElementAtPoint(e.Location) as GridDataCellElement;
            if (cell != null)
            {
                radGridView1.CurrentRow = cell.RowInfo;
                radGridView1.CurrentColumn = cell.ColumnInfo;
            }
            else
            {
                radGridView1.CurrentRow = null;
            }
        }

        private void RadGridView1_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            if (_user.nivel != 6) return;

            var dataRow = radGridView1.CurrentRow as GridViewDataRowInfo;
            if (dataRow == null) return;

            var existing = e.ContextMenu.Items.FirstOrDefault(i => i.Name == "openViewTask");
            if (existing != null) return;

            var openItem = new RadMenuItem("Obrir detall de la tasca") { Name = "openViewTask" };
            openItem.Click += (s, args) =>
            {
                try
                {
                    int id = Convert.ToInt32(dataRow.Cells["Id"].Value);
                    var task = _repo.GetTareaById(id);
                    if (task == null)
                    {
                        RadMessageBox.Show("No s'ha pogut carregar la tasca.", TituloError,
                            MessageBoxButtons.OK, RadMessageIcon.Error);
                        return;
                    }

                    var form = new ViewTask(task);
                    form.FormClosed += FormTask_FormClosed;
                    form.ShowDialog();
                }
                catch (Exception ex)
                {
                    RadMessageBox.Show($"Error en obrir la tasca: {ex.Message}", TituloError,
                        MessageBoxButtons.OK, RadMessageIcon.Error);
                }
            };

            e.ContextMenu.Items.Insert(0, openItem);
            e.ContextMenu.Items.Insert(1, new RadMenuSeparatorItem());
        }

        private void FormTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                radGridView1.EndEdit();
                radGridView1.CancelEdit();
                RefreshData();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"No s'ha pogut actualitzar la vista: {ex.Message}", TituloError,
                    MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        #endregion

        #region Edición y cambios en celdas

        private void radGridView1_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            if (_user.nivel != 6)
            {
                e.Cancel = true;
                return;
            }

            if (e.Column != null && e.Column.Name == "Completada")
            {
                e.Cancel = true;
                RadMessageBox.Show(
                    "Les tasques només es poden completar des del formulari.",
                    "Acció prohibida",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Info
                );
            }
        }

        private void radGridView1_EditorRequired(object sender, EditorRequiredEventArgs e)
        {
            var checkEditor = e.Editor as RadCheckBoxEditor;
            if (checkEditor == null) return;

            checkEditor.ValueChanged -= CheckEditor_ValueChanged;
            checkEditor.ValueChanged += CheckEditor_ValueChanged;
        }

        private void CheckEditor_ValueChanged(object sender, EventArgs e)
        {
            radGridView1.EndEdit();
        }

        private void radGridView1_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            try
            {
                if (_user.nivel != 6) return;

                var editedRow = e.Row;
                if (editedRow == null || e.Column == null)
                    return;

                if (e.Column.Name == "Completada")
                {
                    int id = Convert.ToInt32(editedRow.Cells["Id"].Value);
                    var real = _repo.GetTareaById(id);
                    if (real != null)
                    {
                        editedRow.Cells["Completada"].Value = real.Completada;
                        editedRow.InvalidateRow();
                    }

                    RadMessageBox.Show(
                        "Per completar la tasca, obre el formulari fent doble clic a la fila.",
                        "Acció prohibida",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Info
                    );
                    return;
                }

                int taskId = Convert.ToInt32(editedRow.Cells["Id"].Value);
                var updatedTask = CreateTaskFromRow(editedRow, taskId);

                updatedTask.PersonaAsignada = BestMatch(updatedTask.PersonaAsignada, _empleadosActivos, allowNullFallback: true);
                updatedTask.Tipo = BestMatch(updatedTask.Tipo, TiposValidos);
                updatedTask.Turno = BestMatch(updatedTask.Turno, TurnosValidos);

                if (!ValidarTarea(updatedTask))
                {
                    RadMessageBox.Show(
                        "La tasca conté dades invàlides, revisa-la.",
                        "Error de validació",
                        MessageBoxButtons.OK,
                        RadMessageIcon.Exclamation
                    );

                    LoadData();
                    return;
                }

                _repo.UpdateTask(updatedTask);
                LoadData();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error en actualitzar la tasca: {ex.Message}", TituloError,
                    MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private static Tasks CreateTaskFromRow(GridViewRowInfo row, int taskId)
        {
            return new Tasks
            {
                Id = taskId,
                Dia = Convert.ToDateTime(row.Cells["Dia"].Value),
                Tipo = row.Cells["Tipo"].Value?.ToString(),
                Titulo = row.Cells["Titulo"].Value?.ToString(),
                Descripcion = row.Cells["Descripcion"].Value?.ToString(),
                Comentario = row.Cells["Comentario"].Value?.ToString(),
                PersonaAsignada = row.Cells["PersonaAsignada"].Value?.ToString(),
                Prioridad = Convert.ToInt32(row.Cells["Prioridad"].Value),
                Completada = Convert.ToBoolean(row.Cells["Completada"].Value),
                Turno = row.Cells["Turno"].Value?.ToString(),
                Estado = row.Cells["Estado"].Value?.ToString()
            };
        }

        #endregion

        #region Borrado de filas

        private void radGridView1_UserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {
            if (_user.nivel < 6)
            {
                RadMessageBox.Show("No tens permisos per realitzar aquesta acció", "Avís", MessageBoxButtons.OK, RadMessageIcon.Error);
                e.Cancel = true;
                return;
            }

            var dialogResult = RadMessageBox.Show(
                "Estàs segur que vols eliminar...?",
                "Confirmar eliminació",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Exclamation
            );

            if (dialogResult != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            var selectedRow = radGridView1.CurrentRow as GridViewDataRowInfo;
            if (selectedRow != null)
            {
                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                _repo.DeleteTask(id);
            }
        }

        #endregion

        #region API pública

        public void RefreshData()
        {
            LoadData();
        }

        public Tasks GetSelectedTask()
        {
            var row = radGridView1.CurrentRow as GridViewDataRowInfo;
            return row?.DataBoundItem as Tasks;
        }

        #endregion

        #region Utilidades / Validación Jaro-Winkler

        private static string Normalize(string s)
        {
            return (s ?? string.Empty).Trim();
        }

        private string BestMatch(string input, IEnumerable<string> candidates, bool allowNullFallback = false)
        {
            string source = Normalize(input);
            if (string.IsNullOrEmpty(source))
                return allowNullFallback ? (candidates != null && candidates.Contains("Cap") ? "Cap" : null) : null;

            if (candidates == null) return source;

            string best = null;
            double bestScore = double.NegativeInfinity;

            foreach (var c in candidates)
            {
                double score = JaroWinklerDistance(source, Normalize(c));
                if (score > bestScore)
                {
                    bestScore = score;
                    best = c;
                }
            }

            return best ?? source;
        }

        private bool ValidarTarea(Tasks task)
        {
            if (task == null) return false;

            if (!TiposValidos.Contains(task.Tipo ?? string.Empty))
                return false;

            if (string.IsNullOrWhiteSpace(task.Titulo))
                return false;

            if (task.Dia == default(DateTime))
                return false;

            if (!string.IsNullOrWhiteSpace(task.PersonaAsignada))
            {
                if (_empleadosActivos != null && _empleadosActivos.Count > 0 &&
                    !_empleadosActivos.Contains(task.PersonaAsignada))
                    return false;
            }

            return true;
        }

        private double JaroWinklerDistance(string s, string t)
        {
            s = s ?? string.Empty;
            t = t ?? string.Empty;

            if (s == t)
                return 1.0;

            int sLen = s.Length;
            int tLen = t.Length;

            if (sLen == 0 || tLen == 0)
                return 0.0;

            int matchDistance = Math.Max(sLen, tLen) / 2 - 1;

            var sMatches = new bool[sLen];
            var tMatches = new bool[tLen];

            int matches = 0;
            for (int i = 0; i < sLen; i++)
            {
                int start = Math.Max(0, i - matchDistance);
                int end = Math.Min(i + matchDistance + 1, tLen);

                for (int j = start; j < end; j++)
                {
                    if (tMatches[j]) continue;
                    if (s[i] != t[j]) continue;

                    sMatches[i] = true;
                    tMatches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0)
                return 0.0;

            double transpositions = 0;
            int k = 0;
            for (int i = 0; i < sLen; i++)
            {
                if (!sMatches[i]) continue;
                while (!tMatches[k]) k++;
                if (s[i] != t[k]) transpositions++;
                k++;
            }

            transpositions /= 2.0;

            double jaro = ((double)matches / sLen + (double)matches / tLen +
                          ((double)matches - transpositions) / matches) / 3.0;

            int prefix = 0;
            for (int i = 0; i < Math.Min(4, Math.Min(sLen, tLen)); i++)
            {
                if (s[i] == t[i]) prefix++;
                else break;
            }

            double jaroWinkler = jaro + prefix * 0.1 * (1 - jaro);
            return jaroWinkler;
        }

        #endregion
    }
}
