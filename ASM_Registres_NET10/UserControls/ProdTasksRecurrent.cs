using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Tasks;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ASM_Registres.UserControls
{
    public partial class ProdTasksRecurrent : UserControl
    {
        #region Constantes

        private const string EstadoFinalizada = "Finalizada";
        private const string EstadoTerminada = "Terminada";
        private const string EstadoEnProgreso = "En progreso";
        private const string EstadoEmpezada = "Empezada";

        private const string TituloInfo = "Información";
        private const string TituloError = "Error";
        private const string TituloAviso = "Aviso";

        private static readonly string[] TurnosValidos = { "Todos", "Mañana", "Tarde", "Noche" };
        private static readonly string[] TiposValidos = { "Tarea", "Recordatorio" };

        private static readonly Color ColorTextoOk = Color.DarkGreen;
        private static readonly Color ColorFondoOk = Color.LightGreen;
        private static readonly Color ColorTextoRun = Color.DarkOrange;
        private static readonly Color ColorFondoRun = Color.Moccasin;

        #endregion

        #region Campos

        private readonly ProdTasksRecurrentRepository _repo;
        private readonly TareasRepository _repoTareas;
        private readonly NPGSQLService _npg;

        private readonly User _user;
        private List<string> _empleadosActivos;

        private readonly Dictionary<string, RadPageViewPage> _pagesByDate = new Dictionary<string, RadPageViewPage>();

        #endregion

        #region Constructor

        public ProdTasksRecurrent(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            InitializeComponent();

            _user = user;
            _npg = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            _repo = new ProdTasksRecurrentRepository();
            _repoTareas = new TareasRepository(_npg);

            ConfigureDock();
            SetupEvents();

            ConfigureComboboxes();
            FillRegistrosCombo();
            CheckUserAndApplyRules();

            var allTasks = _repo.GetAllTareas() ?? new List<Tasks>();

            DateTime initialDate;

            if (allTasks.Any())
                initialDate = allTasks.Max(t => t.Dia.Date);
            else
                initialDate = DateTime.Today;

            OpenDayPage(initialDate);
        }

        #endregion

        #region Inicialización de UI

        private void ConfigureDock()
        {
            infoPanel.Dock = DockStyle.Top;
            buttonsPanel.Dock = DockStyle.Bottom;

            radPageView1.Dock = DockStyle.Fill;

            radPanel1.Controls.Add(radButton1);
            radPanel2.Controls.Add(radButton2);

            radButton1.Dock = DockStyle.Fill;
            radButton2.Dock = DockStyle.Fill;
        }

        private void OpenDayPage(DateTime date)
        {
            date = date.Date;
            string key = date.ToString("yyyyMMdd");

            RadPageViewPage page;

            if (!_pagesByDate.TryGetValue(key, out page)
                || page == null
                || !radPageView1.Pages.Contains(page))
            {
                page = new RadPageViewPage
                {
                    Name = $"page_{key}",
                    Text = date.ToString("dd/MM/yyyy"),
                    Tag = date
                };

                var dayControl = new ProdTasksRecurrentSpecific(date, _user)
                {
                    Dock = DockStyle.Fill
                };

                page.Controls.Add(dayControl);
                radPageView1.Pages.Add(page);
                _pagesByDate[key] = page;
            }
            else
            {
                var dayControl = page.Controls.OfType<ProdTasksRecurrentSpecific>().FirstOrDefault();
                //dayControl?.RefreshData();
            }

            radPageView1.SelectedPage = page;
        }

        private ProdTasksRecurrentSpecific GetCurrentDayControl()
        {
            var page = radPageView1.SelectedPage;
            return page?.Controls.OfType<ProdTasksRecurrentSpecific>().FirstOrDefault();
        }

        private void ConfigureComboboxes()
        {
            personaAsignadaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _empleadosActivos = _repo.GetActiveEmployees() ?? new List<string>();
            if (!_empleadosActivos.Contains("Cap"))
                _empleadosActivos.Insert(0, "Cap");
            personaAsignadaCombo.DataSource = _empleadosActivos;
            personaAsignadaCombo.SelectedIndex = 0;

            tipoComboPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            tipoComboPicker.DataSource = TiposValidos.ToList();
            tipoComboPicker.SelectedIndex = 0;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DataSource = TurnosValidos.ToList();
            comboBox1.SelectedIndex = 0;

            fechaPicker.Value = DateTime.Now;
        }

        private void FillRegistrosCombo()
        {
            var tasksList = _repoTareas.GetTasks();

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Texto", typeof(string));

            var emptyRow = table.NewRow();
            emptyRow["Id"] = DBNull.Value;
            emptyRow["Texto"] = "";
            table.Rows.Add(emptyRow);

            foreach (var t in tasksList)
            {
                var row = table.NewRow();
                row["Id"] = t.Id;
                row["Texto"] = $"{t.Titol} - {t.Zona}";
                table.Rows.Add(row);
            }

            comboBox2.DisplayMember = "Texto";
            comboBox2.ValueMember = "Id";
            comboBox2.DataSource = table;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 0;
        }

        private void SetupEvents()
        {
            radButton1.Click += radButton1_Click;
            radButton2.Click += radButton2_Click;
            radButton4.Click += radButton4_Click;
        }

        #endregion

        #region Reglas según nivel de usuario

        private void CheckUserAndApplyRules()
        {
            bool esSupervisor = _user.nivel == 6;

            infoPanel.Enabled = true;
            buttonsPanel.Enabled = esSupervisor;

            tipoComboPicker.Enabled = esSupervisor;
            tituloTextBox.Enabled = esSupervisor;
            richTextBox1.Enabled = esSupervisor;
            richTextBox2.Enabled = esSupervisor;
            personaAsignadaCombo.Enabled = esSupervisor;
            comboBox1.Enabled = esSupervisor;
            fechaPicker.Enabled = esSupervisor;
            comboBox2.Enabled = esSupervisor;
        }

        #endregion

        #region Botones (crear / copiar tarea)

        private void radButton1_Click(object sender, EventArgs e)
        {
            int prioridad;
            if (!TryBuildPrioridad(out prioridad))
                return;

            if (string.IsNullOrWhiteSpace(tituloTextBox.Text))
            {
                RadMessageBox.Show("El camp 'títol' no pot estar vuit.", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (fechaPicker.Value == null || fechaPicker.Value == DateTime.MinValue)
            {
                RadMessageBox.Show("Has de seleccionar una data vàlida.", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (tipoComboPicker.SelectedIndex == -1)
            {
                RadMessageBox.Show("És obligatori seleccionar un tipus.", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            string empAssigned = personaAsignadaCombo.SelectedIndex != -1 ? personaAsignadaCombo.Text : null;

            int? tascaId = null;
            if (comboBox2.SelectedValue != null && comboBox2.SelectedValue != DBNull.Value)
                tascaId = Convert.ToInt32(comboBox2.SelectedValue);

            var newTask = new Tasks(
                fechaPicker.Value,
                comboBox1.Text,
                tipoComboPicker.Text,
                tituloTextBox.Text,
                prioridad,
                richTextBox1.Text,
                null,
                empAssigned,
                completada: false,
                estado: null,
                tasca_id: tascaId,
                employee_id: null
            );

            _repo.AddTask(newTask);

            tituloTextBox.Text = string.Empty;
            richTextBox1.Text = string.Empty;
            comboBox2.SelectedIndex = 0;

            DateTime date;
            date = fechaPicker.Value.Date;
            string key = date.ToString("yyyyMMdd");

            RadPageViewPage page;

            if (!_pagesByDate.TryGetValue(key, out page)
                || page == null
                || !radPageView1.Pages.Contains(page))
            {
                page = new RadPageViewPage
                {
                    Name = $"page_{key}",
                    Text = date.ToString("dd/MM/yyyy"),
                    Tag = date
                };

                var dayControl = new ProdTasksRecurrentSpecific(date, _user)
                {
                    Dock = DockStyle.Fill
                };

                page.Controls.Add(dayControl);
                radPageView1.Pages.Add(page);
                _pagesByDate[key] = page;
            }
            else
            {
                var dayControl = page.Controls.OfType<ProdTasksRecurrentSpecific>().FirstOrDefault();
                //dayControl?.RefreshData();
            }

            radPageView1.SelectedPage = page;
        }

        private bool TryBuildPrioridad(out int prioridad)
        {
            prioridad = 0;
            if (!int.TryParse(richTextBox2.Text, out prioridad))
            {
                RadMessageBox.Show("El camp 'prioritat' ha de ser un nombre enter vàlid.", TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
                return false;
            }
            return true;
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            var dayControl = GetCurrentDayControl();
            if (dayControl == null)
            {
                RadMessageBox.Show("No hi ha cap dia obert.", "Avís",
                    MessageBoxButtons.OK, RadMessageIcon.Info);
                return;
            }

            var actualTask = dayControl.GetSelectedTask();
            if (actualTask == null)
            {
                RadMessageBox.Show("Has de seleccionar una tasca.", "Avís",
                    MessageBoxButtons.OK, RadMessageIcon.Info);
                return;
            }

            var copyTaskForm = new CopyTaskForm(actualTask, 1);
            copyTaskForm.FormClosing += (o, a) =>
            {
                dayControl.RefreshData();
            };
            copyTaskForm.ShowDialog();
        }

        private void radButton4_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = radDateTimePicker1.Value.Date;
            OpenDayPage(selectedDate);
        }

        #endregion

        private void radButton5_Click(object sender, EventArgs e)
        {
            var dialogResult = RadMessageBox.Show(
                "Estàs segur que vols realitzar aquesta acció?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Exclamation
            );

            if (dialogResult != DialogResult.Yes)
                return;

            try
            {
                var allTasks = _repo.GetAllTareas() ?? new List<Tasks>();
                if (!allTasks.Any())
                {
                    RadMessageBox.Show("No hi ha tasques per copiar.", TituloAviso,
                        MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                DateTime lastDate = allTasks.Max(t => t.Dia.Date);

                if (lastDate.Date == DateTime.Today.Date)
                {
                    RadMessageBox.Show("L'últim dia amb tasques ja és avui. No s'han copiat tasques.",
                        TituloAviso, MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                var sourceTasks = allTasks
                    .Where(t => t.Dia.Date == lastDate)
                    .OrderBy(t => t.Prioridad)
                    .ToList();

                if (!sourceTasks.Any())
                {
                    RadMessageBox.Show("No s'han trobat tasques per copiar.", TituloAviso,
                        MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                int created = 0;
                foreach (var t in sourceTasks)
                {
                    var newTask = new Tasks(
                        DateTime.Today,
                        t.Turno,
                        t.Tipo,
                        t.Titulo,
                        t.Prioridad,
                        t.Descripcion,
                        t.Comentario,
                        "Cap",
                        completada: false,
                        estado: null,
                        tasca_id: t.Tasca_Id,
                        employee_id: t.Employee_Id
                    );

                    _repo.AddTask(newTask);
                    created++;
                }

                RadMessageBox.Show(
                    $"S'han copiat {created} tasques del dia {lastDate:dd/MM/yyyy} a avui.",
                    TituloInfo,
                    MessageBoxButtons.OK,
                    RadMessageIcon.Info
                );

                OpenDayPage(DateTime.Today);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error en copiar les tasques: {ex.Message}",
                    TituloError, MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            var currentPage = radPageView1.SelectedPage;
            if (currentPage == null)
            {
                RadMessageBox.Show("No hi ha cap pàgina seleccionada.", TituloAviso,
                    MessageBoxButtons.OK, RadMessageIcon.Info);
                return;
            }

            if (!(currentPage.Tag is DateTime date))
            {
                var ctrl = currentPage.Controls.OfType<ProdTasksRecurrentSpecific>().FirstOrDefault();
                ctrl?.RefreshData();
                return;
            }

            string key = date.ToString("yyyyMMdd");
            if (_pagesByDate.ContainsKey(key))
                _pagesByDate.Remove(key);

            radPageView1.Pages.Remove(currentPage);

            OpenDayPage(date);
        }
    }
}
