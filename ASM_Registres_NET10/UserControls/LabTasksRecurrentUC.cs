using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Tasks;
using ASM_Registres.Forms;

namespace ASM_Registres.UserControls
{
    public partial class LabTasksRecurrentUC : UserControl
    {
        private readonly LabTasksRecurrentRepository _repository;
        private readonly User _currentUser;
        private List<string> _activeEmployees;

        private static readonly string[] ValidTypes = { "Tarea", "Recordatorio" };
        private static readonly string[] ValidShifts = { "Noche", "Mañana", "Tarde", "Todos" };

        public LabTasksRecurrentUC(User user)
        {
            InitializeComponent();

            _repository = new LabTasksRecurrentRepository();
            _currentUser = user;
            _activeEmployees = LoadActiveEmployees();

            ConfigureLayout();
            ConfigureControls();
            AttachEventHandlers();

            RefreshGrid();

            CheckUserAndLockEditingIfNeeded();
        }

        private List<string> LoadActiveEmployees()
        {
            var list = _repository.GetActiveEmployees() ?? new List<string>();

            list.Insert(0, "Ninguno");
            return list;
        }

        private void ConfigureLayout()
        {
            infoPanel.Dock = DockStyle.Top;
            buttonsPanel.Dock = DockStyle.Bottom;
            tasksGrid.Dock = DockStyle.Fill;

            radPanel1.Controls.Add(radButton1);
            radPanel2.Controls.Add(radButton2);
            radButton1.Dock = DockStyle.Fill;
            radButton2.Dock = DockStyle.Fill;

            tasksGrid.TableElement.RowHeight = 40;
        }

        private void ConfigureControls()
        {
            fechaPicker.Value = DateTime.Now;
            fechaPicker.Format = DateTimePickerFormat.Short;

            personaAsignadaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            personaAsignadaCombo.DataSource = _activeEmployees;
            personaAsignadaCombo.SelectedIndex = 0;

            tipoComboPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            tipoComboPicker.SelectedIndex = 0;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;

            tasksGrid.AllowAddNewRow = false;
            tasksGrid.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            tasksGrid.EnablePaging = true;
            tasksGrid.PageSize = 30;
            tasksGrid.EnableGrouping = false;
        }

        private void AttachEventHandlers()
        {
            fechaPicker.ValueChanged += FechaPicker_ValueChanged;

            radButton1.Click += BtnAdd_Click;     
            radButton2.Click += BtnCopy_Click;    
            radButton3.Click += BtnRefresh_Click; 

            tasksGrid.CellValueChanged += TasksGrid_CellValueChanged;
            tasksGrid.UserDeletingRow += TasksGrid_UserDeletingRow;
            tasksGrid.EditorRequired += TasksGrid_EditorRequired;

            if (_currentUser.nivel != 6)
            {
                tasksGrid.CellDoubleClick += TasksGrid_CellDoubleClick;
            }
        }

        private void RefreshGrid()
        {
            if (_currentUser.nivel == 6)
            {
                LoadAllTasks();
            }
            else
            {
                LoadFilteredTasks();
            }
        }

        private void LoadAllTasks()
        {
            var tasks = _repository.GetAllTasksByDateTime()
                .OrderByDescending(t => t.Dia)
                .ThenBy(t => t.Prioridad)
                .ToList();

            BindTasks(tasks);
        }

        private void LoadFilteredTasks()
        {
            var selectedDate = fechaPicker.Value.Date;
            var previousDate = selectedDate.AddDays(-1);

            var tasks = _repository.GetAllTareas()
                .Where(t => t.Dia.Date == selectedDate || t.Dia.Date == previousDate)
                .OrderByDescending(t => t.Dia)
                .ThenBy(t => t.Prioridad)
                .ToList();

            BindTasks(tasks);
        }

        private void BindTasks(List<Tasks> tasks)
        {
            tasksGrid.DataSource = null;
            tasksGrid.DataSource = tasks;

            ConfigureColumnsIfPresent();
            CenterGridHeadersAndCells(tasksGrid);
            ApplyRowStyles();
        }

        private void ConfigureColumnsIfPresent()
        {
            var colId = tasksGrid.Columns["Id"];
            if (colId != null)
                colId.IsVisible = false;

            var colDia = tasksGrid.Columns["Dia"];
            if (colDia != null)
            {
                if (_currentUser.nivel != 6)
                    colDia.IsVisible = false;

                colDia.FormatString = "{0:dd-MM-yyyy}";
            }

            RenameGridColumns();
        }

        private void RenameGridColumns()
        {
            var headers = new Dictionary<string, string>
                {
                    { "Dia",             "Data" },
                    { "Tipo",            "Tipus" },
                    { "Titulo",          "Títol" },
                    { "Descripcion",     "Descripció" },
                    { "Comentario",      "Comentari" },
                    { "PersonaAsignada", "Empleat assignat" },
                    { "Prioridad",       "Prioritat" },
                    { "Completada",      "Completada" },
                    { "Turno",           "Torn" }
                };


            foreach (var kvp in headers)
            {
                var col = tasksGrid.Columns[kvp.Key];
                if (col != null)
                    col.HeaderText = kvp.Value;
            }
        }

        private void CenterGridHeadersAndCells(RadGridView grid)
        {
            foreach (GridViewColumn col in grid.Columns)
            {
                col.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                if (col is GridViewDataColumn dataCol)
                    dataCol.TextAlignment = ContentAlignment.MiddleCenter;
            }
        }

        private void ApplyRowStyles()
        {
            foreach (var r in tasksGrid.Rows)
            {
                if (r is GridViewDataRowInfo row)
                {
                    bool completed = Convert.ToBoolean(row.Cells["Completada"].Value);
                    var foreColor = completed ? Color.DarkGreen : Color.Black;
                    var backColor = completed ? Color.LightGreen : Color.White;

                    foreach (GridViewCellInfo cell in row.Cells)
                    {
                        cell.Style.ForeColor = foreColor;
                        cell.Style.BackColor = backColor;
                    }
                }
            }
        }

        private void CheckUserAndLockEditingIfNeeded()
        {
            if (_currentUser.nivel < 6)
            {
                infoPanel.Enabled = true;
                buttonsPanel.Enabled = false;

                tipoComboPicker.Enabled = false;
                tituloTextBox.Enabled = false;
                richTextBox1.Enabled = false;
                richTextBox2.Enabled = false;
                personaAsignadaCombo.Enabled = false;
                comboBox1.Enabled = false;

                foreach (GridViewColumn column in tasksGrid.Columns)
                {
                    if (column.Name != "Comentario" && column.Name != "Completada")
                        column.ReadOnly = true;
                    else
                        column.ReadOnly = false;
                }
            }
        }

        private void TasksGrid_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if (e.Column?.Name == "Comentario")
                return;

            if (e.Row?.DataBoundItem is Tasks task)
            {
                using (var viewForm = new ViewTask(task))
                {
                    viewForm.FormClosed += (s, ev) => { tasksGrid.CancelEdit(); };
                    viewForm.ShowDialog();
                }
            }
        }

        private void TasksGrid_EditorRequired(object sender, EditorRequiredEventArgs e)
        {
            if (e.Editor is RadCheckBoxEditor checkEditor)
            {
                checkEditor.ValueChanged -= CheckEditor_ValueChanged;
                checkEditor.ValueChanged += CheckEditor_ValueChanged;
            }
        }

        private void CheckEditor_ValueChanged(object sender, EventArgs e)
        {
            tasksGrid.EndEdit();
        }

        private void TasksGrid_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            if (e.Row == null || e.Column == null)
                return;

            if (!int.TryParse(e.Row.Cells["Id"]?.Value?.ToString(), out int taskId))
                return;

            var task = MapRowToTask(e.Row);

            task.PersonaAsignada = MatchClosest(task.PersonaAsignada, _activeEmployees?.ToArray());
            task.Tipo = MatchClosest(task.Tipo, ValidTypes);
            task.Turno = MatchClosest(task.Turno, ValidShifts);

            if (!ValidateTask(task))
            {
                RadMessageBox.Show("La tarea contiene datos inválidos. Revisa los valores ingresados.",
                    "Error de validación",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Exclamation);

                RefreshGrid();
                return;
            }

            try
            {
                _repository.UpdateTask(task);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error al actualizar la tarea: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Error);
            }
        }

        private Tasks MapRowToTask(GridViewRowInfo row)
        {
            return new Tasks
            {
                Id = Convert.ToInt32(row.Cells["Id"].Value),
                Dia = Convert.ToDateTime(row.Cells["Dia"].Value),
                Tipo = row.Cells["Tipo"].Value?.ToString(),
                Titulo = row.Cells["Titulo"].Value?.ToString(),
                Descripcion = row.Cells["Descripcion"].Value?.ToString(),
                Comentario = row.Cells["Comentario"].Value?.ToString(),
                PersonaAsignada = row.Cells["PersonaAsignada"].Value?.ToString(),
                Prioridad = Convert.ToInt32(row.Cells["Prioridad"].Value),
                Completada = Convert.ToBoolean(row.Cells["Completada"].Value),
                Turno = row.Cells["Turno"].Value?.ToString()
            };
        }

        private void TasksGrid_UserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {
            if (_currentUser.nivel < 6)
            {
                RadMessageBox.Show("No tienes permisos para eliminar.", "Aviso",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                e.Cancel = true;
                return;
            }

            var result = RadMessageBox.Show("¿Confirmar eliminación?", "Eliminar tarea",
                MessageBoxButtons.YesNo, RadMessageIcon.Exclamation);

            if (result == DialogResult.Yes && tasksGrid.CurrentRow is GridViewDataRowInfo row)
            {
                int id = Convert.ToInt32(row.Cells["Id"].Value);
                _repository.DeleteTask(id);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tituloTextBox.Text))
            {
                RadMessageBox.Show("El campo título no puede estar vacío.", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (fechaPicker.Value == DateTime.MinValue)
            {
                RadMessageBox.Show("Debes seleccionar una fecha válida.", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (tipoComboPicker.SelectedIndex == -1)
            {
                RadMessageBox.Show("Debes seleccionar un tipo.", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (!int.TryParse(richTextBox2.Text, out int prioridad))
            {
                RadMessageBox.Show("Prioridad debe ser un número entero.", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            string persona = personaAsignadaCombo.SelectedIndex > 0 ? personaAsignadaCombo.Text : null;

            var newTask = new Tasks(
                fechaPicker.Value,
                comboBox1.Text,
                tipoComboPicker.Text,
                tituloTextBox.Text.Trim(),
                prioridad,
                richTextBox1.Text.Trim(),
                null,
                persona,
                estado: null
            );

            if (!ValidateTask(newTask))
            {
                RadMessageBox.Show("Datos de la nueva tarea inválidos.", "Error",
                    MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            _repository.AddTask(newTask);

            RefreshGrid();
            ClearInputFields();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (tasksGrid.CurrentRow is GridViewDataRowInfo row)
            {
                int id = Convert.ToInt32(row.Cells["Id"].Value);
                var taskToCopy = _repository.GetTareaById(id);

                using (var copyForm = new CopyTaskForm(taskToCopy, 2))
                {
                    copyForm.FormClosing += (s, ev) => RefreshGrid();
                    copyForm.ShowDialog();
                }
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void FechaPicker_ValueChanged(object sender, EventArgs e)
        {
            if (_currentUser.nivel != 6)
                LoadFilteredTasks();
        }

        private void ClearInputFields()
        {
            tituloTextBox.Clear();
            richTextBox1.Clear();
            richTextBox2.Clear();
            personaAsignadaCombo.SelectedIndex = 0;
            tipoComboPicker.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }

        private string MatchClosest(string input, string[] candidates)
        {
            if (candidates == null || candidates.Length == 0)
                return null;

            if (string.IsNullOrWhiteSpace(input))
                return null;

            return candidates
                .OrderByDescending(c => JaroWinklerDistance(input, c))
                .FirstOrDefault();
        }

        private bool ValidateTask(Tasks task)
        {
            if (!ValidTypes.Contains(task.Tipo))
                return false;

            if (string.IsNullOrWhiteSpace(task.Titulo))
                return false;

            if (task.Dia == default)
                return false;

            if (!string.IsNullOrWhiteSpace(task.PersonaAsignada) &&
                (_activeEmployees == null || !_activeEmployees.Contains(task.PersonaAsignada)))
                return false;

            return true;
        }

        private double JaroWinklerDistance(string s, string t)
        {
            if (s == t) return 1.0;
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t)) return 0.0;

            int sLen = s.Length, tLen = t.Length;
            int matchDist = Math.Max(sLen, tLen) / 2 - 1;

            bool[] sMatch = new bool[sLen], tMatch = new bool[tLen];
            int matches = 0;

            for (int i = 0; i < sLen; i++)
            {
                int start = Math.Max(0, i - matchDist);
                int end = Math.Min(i + matchDist + 1, tLen);
                for (int j = start; j < end; j++)
                {
                    if (!tMatch[j] && s[i] == t[j])
                    {
                        sMatch[i] = tMatch[j] = true;
                        matches++;
                        break;
                    }
                }
            }

            if (matches == 0) return 0.0;

            double transpositions = 0;
            for (int i = 0, j = 0; i < sLen && j < tLen; i++)
            {
                if (sMatch[i])
                {
                    while (!tMatch[j]) j++;
                    if (s[i] != t[j]) transpositions++;
                    j++;
                }
            }

            transpositions /= 2.0;

            double jaro = ((matches / (double)sLen) +
                           (matches / (double)tLen) +
                           ((matches - transpositions) / matches)) / 3.0;

            int prefix = 0;
            int maxPrefix = Math.Min(4, Math.Min(sLen, tLen));
            for (int i = 0; i < maxPrefix; i++)
            {
                if (s[i] == t[i]) prefix++;
                else break;
            }

            return jaro + prefix * 0.1 * (1 - jaro);
        }
    }
}
