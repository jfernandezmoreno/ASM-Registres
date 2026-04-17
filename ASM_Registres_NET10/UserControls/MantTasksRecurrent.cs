using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using ASM_Registres.Forms;

namespace ASM_Registres.UserControls
{
    public partial class MantTasksRecurrent : UserControl
    {
        ManTasksRecurrentRepository connection;

        User user;
        List<string> empleadosActivos;

        public MantTasksRecurrent(User user)
        {
            InitializeComponent();

            connection = new ManTasksRecurrentRepository();

            this.user = user;

            //INIT DOCKS
            infoPanel.Dock = DockStyle.Top;
            buttonsPanel.Dock = DockStyle.Bottom;
            tasksGrid.Dock = DockStyle.Fill;

            /*buttonsPanel.Controls.Add(radPanel1);
            buttonsPanel.Controls.Add(radPanel2);*/

            radPanel1.Controls.Add(radButton1);
            radPanel2.Controls.Add(radButton2);

            radButton1.Dock = DockStyle.Fill;
            radButton2.Dock = DockStyle.Fill;

            InitTasksGrid();

            //INIT EVENTS
            tasksGrid.CellValueChanged += tasksGrid_CellValueChanged;
            tasksGrid.UserDeletingRow += recordatoriosGrid_UserDeletedRow;
            tasksGrid.EditorRequired += TasksGrid_EditorRequired;

            if (user.nivel != 6)
            {
                tasksGrid.CellDoubleClick += TasksGrid_CellDoubleClick1;
            }

            //INIT PERSONAL
            personaAsignadaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            empleadosActivos = connection.GetActiveEmployees();
            empleadosActivos.Insert(0, "Ninguno");
            personaAsignadaCombo.DataSource = empleadosActivos;
            personaAsignadaCombo.SelectedIndex = 0;

            //CONFIGURE COMBO
            tipoComboPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            tipoComboPicker.SelectedIndex = 0;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndex = 0;

            tasksGrid.TableElement.RowHeight = 40;


            //INIT DATETIME PICKER
            fechaPicker.Value = DateTime.Now;

            CheckUser();

            radButton1.MouseClick += radButton1_Click;
            radButton2.MouseClick += radButton2_Click;
        }
        private void TasksGrid_CellDoubleClick1(object sender, GridViewCellEventArgs e)
        {
            if (e.Column != null && e.Column.Name == "Comentario")
                return;

            if (e.Row != null && e.Row.DataBoundItem is Tasks task)
            {
                ViewTask formTask = new ViewTask(task);
                formTask.FormClosed += FormTask_FormClosed;
                formTask.ShowDialog();
            }
        }

        private void FormTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            tasksGrid.CancelEdit();
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

        private void CheckUser()
        {
            if (user.nivel < 6)
            {
                infoPanel.Enabled = true;
                buttonsPanel.Enabled = false;

                tipoComboPicker.Enabled = false;
                tituloTextBox.Enabled = false;
                richTextBox1.Enabled = false;
                richTextBox2.Enabled = false;
                personaAsignadaCombo.Enabled = false;
                comboBox1.Enabled = false;

                // Deshabilitar todas las columnas excepto "Comentario"
                foreach (GridViewColumn column in tasksGrid.Columns)
                {
                    if (column.Name != "Comentario" && column.Name != "Completada")
                    {
                        column.ReadOnly = true;
                    }
                    else
                    {
                        column.ReadOnly = false;
                    }
                }
                ActualizarGrid();
                tasksGrid.CellDoubleClick += TasksGrid_CellDoubleClick;
            }
        }

        private void TasksGrid_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            // ABRIR FORMULARIO DE VISTA DE LA TAREA
        }

        private void tasksGrid_CellValueChanged(object sender, GridViewCellEventArgs e)
        {
            try
            {
                var editedRow = e.Row;
                if (editedRow == null || e.Column == null)
                    return;

                int taskId = Convert.ToInt32(editedRow.Cells["Id"].Value);

                Tasks updatedTask = new Tasks
                {
                    Id = taskId,
                    Dia = Convert.ToDateTime(editedRow.Cells["Dia"].Value),
                    Tipo = editedRow.Cells["Tipo"].Value?.ToString(),
                    Titulo = editedRow.Cells["Titulo"].Value?.ToString(),
                    Descripcion = editedRow.Cells["Descripcion"].Value?.ToString(),
                    Comentario = editedRow.Cells["Comentario"].Value?.ToString(),
                    PersonaAsignada = editedRow.Cells["PersonaAsignada"].Value?.ToString(),
                    Prioridad = Convert.ToInt32(editedRow.Cells["Prioridad"].Value),
                    Completada = Convert.ToBoolean(editedRow.Cells["Completada"].Value),
                    Turno = editedRow.Cells["Turno"].Value?.ToString()
                };

                updatedTask.PersonaAsignada = checkAsignada(updatedTask.PersonaAsignada);
                updatedTask.Tipo = checkTipo(updatedTask.Tipo);
                updatedTask.Turno = checkTurno(updatedTask.Turno);


                if (!ValidarTarea(updatedTask))
                {
                    RadMessageBox.Show("La tarea contiene datos inválidos. Revisa los valores ingresados.", "Error de validación", MessageBoxButtons.OK, RadMessageIcon.Exclamation);

                    if (user.nivel == 6)
                    {
                        InitTasksGrid();
                    }
                    else
                    {
                        ActualizarGrid();
                    }

                    return;
                }

                if (user.nivel == 6)
                {
                    connection.UpdateTask(updatedTask);

                    InitTasksGrid();
                }
                else
                {
                    connection.UpdateTask(updatedTask);

                    ActualizarGrid();
                }


                return;
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error al actualizar la tarea: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private string checkTurno(string turno)
        {
            var tiposValidos = new[] { "Noche", "Mañana", "Tarde", "Todos" };

            string bestMatch = null;
            double bestSimilarity = -1;

            foreach (string empleado in tiposValidos)
            {
                double similarity = JaroWinklerDistance(turno, empleado);
                if (similarity > bestSimilarity)
                {
                    bestSimilarity = similarity;
                    bestMatch = empleado;
                }
            }

            return bestMatch;
        }

        private void ActualizarGrid()
        {
            DateTime selectedDate = fechaPicker.Value.Date;
            DateTime previousDate = selectedDate.AddDays(-1);

            List<Tasks> tasksList = connection.GetAllTareas();

            var filteredTasks = tasksList
                .Where(t => t.Dia.Date == selectedDate || t.Dia.Date == previousDate)
                .ToList();

            var sortedTasks = filteredTasks
                .OrderByDescending(t => t.Dia)
                .ThenBy(t => t.Prioridad)
                .ToList();

            tasksGrid.DataSource = sortedTasks;

            foreach (var row in tasksGrid.Rows)
            {
                if (row is GridViewDataRowInfo dataRow)
                {
                    bool isCompleted = Convert.ToBoolean(dataRow.Cells["Completada"].Value);

                    if (isCompleted)
                    {
                        foreach (GridViewCellInfo cell in dataRow.Cells)
                        {
                            cell.Style.ForeColor = Color.DarkGreen;
                            cell.Style.BackColor = Color.LightGreen;
                        }
                    }
                    else
                    {
                        foreach (GridViewCellInfo cell in dataRow.Cells)
                        {
                            cell.Style.Reset();
                        }
                    }
                }
            }
        }

        private void MarcarTareaComoCompletada(int taskId, bool completada)
        {
            try
            {
                connection.UpdateCompletadaEstado(taskId, completada);
                RadMessageBox.Show("¡Tarea marcada como completada!", "Tarea Finalizada", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error al marcar como completada: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private string checkAsignada(string updatedTaskPersonaAsignada)
        {
            if (empleadosActivos == null || empleadosActivos.Count == 0)
                return null;

            string bestMatch = null;
            double bestSimilarity = -1; // La similitud varía entre 0 y 1 (donde 1 es idéntico).

            foreach (string empleado in empleadosActivos)
            {
                double similarity = JaroWinklerDistance(updatedTaskPersonaAsignada, empleado);
                if (similarity > bestSimilarity)
                {
                    bestSimilarity = similarity;
                    bestMatch = empleado;
                }
            }

            return bestMatch;
        }

        private string checkTipo(string tipo)
        {
            var tiposValidos = new[] { "Tarea", "Recordatorio" };

            string bestMatch = null;
            double bestSimilarity = -1; // La similitud varía entre 0 y 1 (donde 1 es idéntico).

            foreach (string empleado in tiposValidos)
            {
                double similarity = JaroWinklerDistance(tipo, empleado);
                if (similarity > bestSimilarity)
                {
                    bestSimilarity = similarity;
                    bestMatch = empleado;
                }
            }

            return bestMatch;
        }

        private double JaroWinklerDistance(string s, string t)
        {
            if (s == t)
                return 1.0;

            int sLen = s.Length;
            int tLen = t.Length;

            if (sLen == 0 || tLen == 0)
                return 0.0;

            int matchDistance = Math.Max(sLen, tLen) / 2 - 1;

            bool[] sMatches = new bool[sLen];
            bool[] tMatches = new bool[tLen];

            int matches = 0;
            for (int i = 0; i < sLen; i++)
            {
                int start = Math.Max(0, i - matchDistance);
                int end = Math.Min(i + matchDistance + 1, tLen);

                for (int j = start; j < end; j++)
                {
                    if (tMatches[j])
                        continue;
                    if (s[i] != t[j])
                        continue;
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
                if (!sMatches[i])
                    continue;
                while (!tMatches[k])
                    k++;
                if (s[i] != t[k])
                    transpositions++;
                k++;
            }

            transpositions /= 2.0;

            double jaro = ((double)matches / sLen + (double)matches / tLen +
                           ((double)matches - transpositions) / matches) / 3.0;

            int prefix = 0;
            for (int i = 0; i < Math.Min(4, Math.Min(sLen, tLen)); i++)
            {
                if (s[i] == t[i])
                    prefix++;
                else
                    break;
            }

            double jaroWinkler = jaro + prefix * 0.1 * (1 - jaro);
            return jaroWinkler;
        }

        private bool ValidarTarea(Tasks task)
        {
            // Validar que el tipo sea válido
            var tiposValidos = new[] { "Tarea", "Recordatorio" };
            if (!tiposValidos.Contains(task.Tipo))
            {
                return false;
            }

            // Validar que el título no esté vacío
            if (string.IsNullOrWhiteSpace(task.Titulo))
            {
                return false;
            }

            // Validar que la fecha no sea inválida
            if (task.Dia == default(DateTime))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(task.PersonaAsignada) && !empleadosActivos.Contains(task.PersonaAsignada))
            {
                return false;
            }

            return true;
        }

        private void recordatoriosGrid_UserDeletedRow(object sender, GridViewRowCancelEventArgs e)
        {

            if (user.nivel < 6)
            {
                RadMessageBox.Show("No tienes permisos para realizar esta acción", "Avíso", MessageBoxButtons.OK, RadMessageIcon.Error);
                e.Cancel = true;
                return;
            }

            DialogResult dialogResult = RadMessageBox.Show("¿Estás seguro de que deseas eliminar esta tarea/recordatorio?", "Confirmar eliminación", MessageBoxButtons.YesNo, RadMessageIcon.Exclamation);

            if (dialogResult == DialogResult.Yes)
            {
                if (tasksGrid.CurrentRow is GridViewDataRowInfo selectedRow)
                {
                    int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                    connection.DeleteTask(id);
                }
            }
            else { e.Cancel = true; }
        }

        private void InitTasksGrid()
        {
            tasksGrid.DataSource = null;
            List<Tasks> tasksList = connection.GetAllTareas();
            var sortedTasks = tasksList
                .OrderByDescending(t => t.Dia)
                .ThenBy(t => t.Prioridad)
                .ToList();
            tasksGrid.DataSource = sortedTasks;
            tasksGrid.AllowAddNewRow = false;
            tasksGrid.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            tasksGrid.EnablePaging = true;
            tasksGrid.PageSize = 30;
            tasksGrid.Columns["Id"].IsVisible = false;

            if (user.nivel != 6)
            {
                tasksGrid.Columns["Dia"].IsVisible = false;

            }

            tasksGrid.Columns["Dia"].FormatString = "{0:dd-MM-yyyy}";
            tasksGrid.EnableGrouping = false;

            RenameGridColumns();
            CenterGridColumnHeadersAndValues(tasksGrid);


            foreach (var row in tasksGrid.Rows)
            {
                if (row is GridViewDataRowInfo dataRow)
                {
                    bool isCompleted = Convert.ToBoolean(dataRow.Cells["Completada"].Value);

                    if (isCompleted)
                    {
                        foreach (GridViewCellInfo cell in dataRow.Cells)
                        {
                            cell.Style.ForeColor = Color.DarkGreen;
                            cell.Style.BackColor = Color.LightGreen;
                        }
                    }
                    else
                    {
                        foreach (GridViewCellInfo cell in dataRow.Cells)
                        {
                            cell.Style.Reset();
                        }
                    }
                }
            }
        }

        private void RenameGridColumns()
        {
            var columnHeaders = new Dictionary<string, string>
            {
                { "Id", "Identificador" },
                { "Description", "Título" },
                { "Date", "Fecha" },
                { "PersonaAsignada", "Empleado Asignado" },
                { "Urgent", "Urgente" },
                { "Completed", "Completado" }
            };

            foreach (var column in columnHeaders)
            {
                if (tasksGrid.Columns.Contains(column.Key))
                {
                    tasksGrid.Columns[column.Key].HeaderText = column.Value;
                }
            }
        }

        private void CenterGridColumnHeadersAndValues(RadGridView gridView)
        {
            foreach (GridViewColumn column in gridView.Columns)
            {
                column.HeaderTextAlignment = ContentAlignment.MiddleCenter;

                if (column is GridViewDataColumn dataColumn)
                {
                    dataColumn.TextAlignment = ContentAlignment.MiddleCenter;
                }
            }
        }

        private void newFormClosed(object o, FormClosingEventArgs a)
        {
            InitTasksGrid();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tituloTextBox.Text))
            {
                RadMessageBox.Show("El campo título no puede estar vacío.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (fechaPicker.Value == null || fechaPicker.Value == DateTime.MinValue)
            {
                RadMessageBox.Show("Debes seleccionar una fecha válida.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (tipoComboPicker.SelectedIndex == -1)
            {
                RadMessageBox.Show("Debes seleccionar un tipo.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            int prioridad;
            if (!int.TryParse(richTextBox2.Text, out prioridad))
            {
                RadMessageBox.Show("El campo debe ser un número entero válido.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            string empAssigned = null;

            if (personaAsignadaCombo.SelectedIndex != -1)
            {
                empAssigned = personaAsignadaCombo.Text;
            }

            var newTasw = new Tasks(
                fechaPicker.Value,
                comboBox1.Text,
                tipoComboPicker.Text,
                tituloTextBox.Text,
                prioridad,
                richTextBox1.Text,
                null,
                empAssigned,
                completada: false,
                estado: null
            );

            connection.AddTask(newTasw);

            connection.AddTask(newTasw);
            InitTasksGrid();

            tituloTextBox.Text = string.Empty;
            richTextBox1.Text = string.Empty;
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            if (tasksGrid.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                Tasks actualTask = connection.GetTareaById(Convert.ToInt32(selectedRow.Cells["Id"].Value));
                CopyTaskForm copyTaskForm = new CopyTaskForm(actualTask, 3);
                copyTaskForm.FormClosing += newFormClosed;
                copyTaskForm.ShowDialog();
            }
        }

        private void fechaPicker_ValueChanged(object sender, EventArgs e)
        {
            if (user.nivel != 6)
            {
                ActualizarGrid();
            }
        }

        private void radButton3_Click_1(object sender, EventArgs e)
        {

            if (user.nivel == 6)
            {
                InitTasksGrid();
            }
            else
            {
                ActualizarGrid();
            }
        }
    }
}
