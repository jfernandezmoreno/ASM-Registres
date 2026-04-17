using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using System.Text;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;

namespace ASM_Registres.UserControls
{
    public partial class EstadisticaUc : UserControl
    {
        private const int GROUP_RESIDUS = 21;
        private const int GROUP_REPROCESSAT = 23;
        private const int GROUP_HAET = 24;
        private const int GROUP_FASE_MOBIL = 25;
        private const int GROUP_CONTROL_AIGUES = 20;
        private const int GROUP_MANTENIMENT_GENERAL = 14;
        private const int GROUP_BASCULES_GRANS = 9;
        private const int GROUP_BASCULES_PETITES = 10;

        PlanRepository planRepository;
        GrupoTareasRepository grupoTareasRepository;

        List<Pla> plans;
        List<GrupTasques> grupTasques;
        List<Tasques> tasques;

        NPGSQLService npgsqlService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RegistroComunRepository registreComuRepository;
        RegistroBasculaRepository registroBasculaRepository;
        IncidenciasRepository incidenciasRepository;
        RegistroTemperaturaCamaraRepository registroTemperaturaCamaraRepository;
        RegistroGeneralRepository registroGeneralRepository;
        ControlAguasRepository controlAguasRepository;
        RegistroMantenimientoRepository registroMantenimientoRepository;
        RegistroLimpiezaRepository registroLimpiezaRepository;
        RegistroResiduoRepository registroResiduoRepository;
        UsuarioRepository usuarioRepository;


        private ProdTasksRecurrentRepository prodTasksRepo;
        private readonly Dictionary<int, string> _empNameCache = new Dictionary<int, string>();

        RegistroHAETRepository registroHaetRepository;
        RegistroFaseMovilRepository registroFaseMovilRepository;
        ReprocessatMostresLaboratoriRepository reprocessatRepository;

        TareasRepository tareasRepository;

        User user;

        public EstadisticaUc(User user)
        {
            InitializeComponent();
            SetupSettings();
            SetRadGridView();

            radGridView1.CellFormatting += RadGridView1_CellFormatting;

            this.user = user;
        }

        private void RadGridView1_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            if (e.CellElement is GridDataCellElement && e.Row is GridViewDataRowInfo row)
            {
                if (e.Column.Name.Equals("LaborantId", StringComparison.InvariantCultureIgnoreCase) ||
                    e.Column.Name.Equals("Laborant", StringComparison.InvariantCultureIgnoreCase))
                {
                    var val = row.Cells[e.Column.Name]?.Value;
                    if (val != null && TryGetEmpId(val, out int empId) && empId > 0)
                    {
                        e.CellElement.Text = ResolveEmpName(empId);
                        e.CellElement.ToolTipText = $"Id: {empId}";
                    }
                    e.CellElement.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
            }
        }


        private bool TryGetEmpId(object value, out int empId)
        {
            if (value is int i) { empId = i; return true; }
            return int.TryParse(value?.ToString(), out empId);
        }

        private string ResolveEmpName(int empId)
        {
            if (_empNameCache.TryGetValue(empId, out var name)) return name;
            try
            {
                var emp = prodTasksRepo.GetEmployeeById(empId);
                name = emp?.NombreUsuario ?? empId.ToString();
            }
            catch
            {
                name = empId.ToString();
            }
            _empNameCache[empId] = name;
            return name;
        }

        private void SetupSettings()
        {
            plaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            grupCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            tascaCombo.DropDownStyle = ComboBoxStyle.DropDownList;

            totalTextbox.ReadOnly = true;
            completosTextbox.ReadOnly = true;
            incompletosTextbox.ReadOnly = true;
            incidenciasTextbox.ReadOnly = true;
            radTextBox2.ReadOnly = true;

            planRepository = new PlanRepository(npgsqlService);
            grupoTareasRepository = new GrupoTareasRepository(npgsqlService);
            registreComuRepository = new RegistroComunRepository(npgsqlService);
            registroBasculaRepository = new RegistroBasculaRepository(npgsqlService);
            incidenciasRepository = new IncidenciasRepository(npgsqlService);
            registroTemperaturaCamaraRepository = new RegistroTemperaturaCamaraRepository(npgsqlService);
            registroGeneralRepository = new RegistroGeneralRepository(npgsqlService);
            controlAguasRepository = new ControlAguasRepository(npgsqlService);
            registroMantenimientoRepository = new RegistroMantenimientoRepository(npgsqlService);
            registroLimpiezaRepository = new RegistroLimpiezaRepository(npgsqlService);
            registroResiduoRepository = new RegistroResiduoRepository(npgsqlService);
            prodTasksRepo = new ProdTasksRecurrentRepository();
            usuarioRepository = new UsuarioRepository(npgsqlService);

            registroHaetRepository = new RegistroHAETRepository(npgsqlService);
            registroFaseMovilRepository = new RegistroFaseMovilRepository(npgsqlService);
            reprocessatRepository = new ReprocessatMostresLaboratoriRepository(npgsqlService);

            tareasRepository = new TareasRepository(npgsqlService);

            plans = planRepository.GetAllPlans();
            grupTasques = grupoTareasRepository.GetAllGrups();
            tasques = tareasRepository.GetTasks();

            plaCombo.DataSource = planRepository.GetAllPlanNames();

            if (plaCombo.SelectedIndex == 0)
            {
                List<string> grupsIncomperts = grupoTareasRepository.GetGroupsByPlaId(3);
                List<string> grupsComplerts = getGroupsComplerts(grupsIncomperts);
                grupCombo.DataSource = grupsComplerts;
            }
            else
            {
                List<string> grupsIncomperts = grupoTareasRepository.GetGroupsByPlaId(1);
                List<string> grupsComplerts = getGroupsComplerts(grupsIncomperts);
                grupCombo.DataSource = grupsComplerts;
            }

            string selectedGroupName = grupCombo.Text;
            GrupTasques selectedGroup = grupTasques.FirstOrDefault(g => g.Nom == selectedGroupName);

            List<Tasques> tasquesActuals = tareasRepository.GetTasksByGroupId(1);
            
            List<string> nomsActuals = new List<string>
            {
                "Totes"
            };

            for (int i = 0; i < tasquesActuals.Count; i++)
                nomsActuals.Add(tasquesActuals[i].Titol + " - " + tasquesActuals[i].Zona);

            tascaCombo.DataSource = nomsActuals;

            radPanel1.Dock = DockStyle.Top;
            radPanel3.Dock = DockStyle.Fill;
            radPanel3.Controls.Add(radGridView1);
            radGridView1.Dock = DockStyle.Fill;
            radPanel2.Dock = DockStyle.Bottom;

            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

            radDateTimePicker2.Value = DateTime.Now;

            incidenciasButton.Anchor = AnchorStyles.Right;
            radButton2.Anchor = AnchorStyles.Right;
        }

        private List<string> getGroupsComplerts(List<string> grupsIncomperts)
        {
            List<string> grupsComplerts = new List<string>();

            foreach (string grup in grupsIncomperts)
            {
                GrupTasques selectedGroup = grupTasques.FirstOrDefault(g => g.Nom == grup);
                if (selectedGroup != null)
                {
                    List<Tasques> tasquesActuals = tareasRepository.GetTasksByGroupId(selectedGroup.Id);

                    if (tasquesActuals.Count > 0
                        || selectedGroup.Id == GROUP_CONTROL_AIGUES
                        || selectedGroup.Id == GROUP_RESIDUS
                        || selectedGroup.Id == GROUP_REPROCESSAT
                        || selectedGroup.Id == GROUP_HAET
                        || selectedGroup.Id == GROUP_FASE_MOBIL)
                    {
                        grupsComplerts.Add(selectedGroup.Nom);
                    }
                }
            }
            return grupsComplerts;
        }

        private void plaCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (plaCombo.SelectedIndex == 0)
            {
                List<string> grupsIncomperts = grupoTareasRepository.GetGroupsByPlaId(3);
                List<string> grupsComplerts = getGroupsComplerts(grupsIncomperts);
                grupCombo.DataSource = grupsComplerts;
            }
            else
            {
                List<string> grupsIncomperts = grupoTareasRepository.GetGroupsByPlaId(1);
                List<string> grupsComplerts = getGroupsComplerts(grupsIncomperts);
                grupCombo.DataSource = grupsComplerts;
            }
            tascaCombo.SelectedIndex = -1;
        }

        private void grupCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGroupName = grupCombo.Text;
            GrupTasques selectedGroup = grupTasques.FirstOrDefault(g => g.Nom == selectedGroupName);

            List<Tasques> tasquesActuals = tareasRepository.GetTasksByGroupId(selectedGroup.Id);
            List<string> nomsActuals = new List<string>();

            nomsActuals.Add("Totes");
            for (int i = 0; i < tasquesActuals.Count; i++)
                nomsActuals.Add(tasquesActuals[i].Titol + " - " + tasquesActuals[i].Zona);

            tascaCombo.DataSource = nomsActuals;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            var ctxBase = new Dictionary<string, object>
            {
                { "plaCombo", plaCombo.Text },
                { "grupCombo", grupCombo.Text },
                { "tascaCombo", tascaCombo.Text }
            };

            try
            {
                bool mostrat = false;

                if (!CheckParams())
                {
                    DateTime startDate = radDateTimePicker1.Value;
                    DateTime endDate = radDateTimePicker2.Value;

                    ctxBase["startDate"] = startDate;
                    ctxBase["endDate"] = endDate;

                    if (tascaCombo.Text == "Totes")
                    {
                        string selectedGroupName = grupCombo.Text;
                        GrupTasques selectedGroup = grupTasques.FirstOrDefault(g => g.Nom == selectedGroupName);

                        ctxBase["selectedGroupName"] = selectedGroupName;
                        ctxBase["selectedGroupId"] = selectedGroup?.Id;

                        if (selectedGroup == null)
                        {
                            RadMessageBox.Show("No s'ha trobat cap grup amb el nom seleccionat.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                            return;
                        }

                        if (selectedGroup.Id == GROUP_RESIDUS)
                        {
                            try
                            {
                                var list = registroResiduoRepository.GetByDateRange(startDate, endDate);
                                ConfigureGrid(list);
                                totalTextbox.Text = list.Count.ToString();
                                FillTextBoxes();
                                return;
                            }
                            catch (Exception ex)
                            {
                                ReportError("Càrrega grup RESIDUS (21)", ex, ctxBase);
                                return;
                            }
                        }

                        if (selectedGroup.Id == GROUP_REPROCESSAT)
                        {
                            try
                            {
                                var list = reprocessatRepository.getRegistresReprocessatMostraLaboratorisByDates(startDate, endDate);
                                ConfigureGrid(list);
                                totalTextbox.Text = list.Count.ToString();
                                FillTextBoxes();
                                return;
                            }
                            catch (Exception ex)
                            {
                                ReportError("Càrrega grup REPROCESSAT (23)", ex, ctxBase);
                                return;
                            }
                        }

                        if (selectedGroup.Id == GROUP_HAET)
                        {
                            try
                            {
                                var all = registroHaetRepository.GetAll();

                                var startDay = startDate.Date;
                                var endDay = endDate.Date;

                                var filtered = (all ?? new List<RegistreHAET>())
                                               .Where(r => r.Date.Date >= startDay && r.Date.Date <= endDay)
                                               .ToList();


                                var view = MapHaetWithLaborantName(filtered);
                                ConfigureGrid(view);

                                totalTextbox.Text = (view?.Count ?? 0).ToString();

                                FillTextBoxes();
                                return;
                            }
                            catch (Exception ex)
                            {
                                ReportError("Càrrega grup HAET (24)", ex, ctxBase);
                                return;
                            }
                        }

                        if (selectedGroup.Id == GROUP_FASE_MOBIL)
                        {
                            try
                            {
                                var all = registroFaseMovilRepository.GetAll();
                                var list = (all ?? new List<RegistreFaseMovil>()).Where(r => r.Date >= startDate && r.Date <= endDate).ToList();
                                ConfigureGrid(list);
                                totalTextbox.Text = list.Count.ToString();
                                FillTextBoxes();
                                return;
                            }
                            catch (Exception ex)
                            {
                                ReportError("Càrrega grup FASE MÒBIL (25)", ex, ctxBase);
                                return;
                            }
                        }

                        if (selectedGroup.Id == GROUP_MANTENIMENT_GENERAL)
                        {
                            try
                            {
                                List<RegistroGeneral> registresGenerals = registroGeneralRepository.GetAllRegistrosGenerales();
                                ConfigureGridGeneralManteniment(registresGenerals);
                                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                                totalTextbox.Text = (registresGenerals ?? new List<RegistroGeneral>()).Count.ToString();
                                return;
                            }
                            catch (Exception ex)
                            {
                                ReportError("Càrrega grup MANTENIMENT GENERAL (14)", ex, ctxBase);
                                return;
                            }
                        }

                        else if (selectedGroup.Id == GROUP_CONTROL_AIGUES)
                        {
                            try
                            {
                                var controles = controlAguasRepository.GetControlAguasPorFecha(startDate, endDate);
                                totalTextbox.Text = (controles ?? new List<ControlAguas>()).Count.ToString();
                                radGridView1.DataSource = controles;
                                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                                ConfigureGrid(controles);
                                return;
                            }
                            catch (Exception ex)
                            {
                                ReportError("Càrrega grup CONTROL AIGÜES (20)", ex, ctxBase);
                                return;
                            }
                        }

                        // --- RESTO DE GRUPOS CON TASQUES ---
                        try
                        {
                            var tareasDelGrupo = tareasRepository.GetTasksByGroupId(selectedGroup.Id) ?? new List<Tasques>();

                            // 1) BÁSCULAS -> mapeo a DTO con Rang fijo y listo
                            if (selectedGroup.Id == GROUP_BASCULES_GRANS || selectedGroup.Id == GROUP_BASCULES_PETITES)
                            {
                                var regsTotales = new List<RegistroBascula>();

                                foreach (var tarea in tareasDelGrupo)
                                {
                                    var regs = registroBasculaRepository
                                        .GetRegistrosBasculaPorFechaYIdTasca(startDate, endDate, tarea.Id);
                                    if (regs != null && regs.Count > 0)
                                        regsTotales.AddRange(regs);
                                }

                                var dtos = ToBasculaDto(regsTotales, selectedGroup.Id);
                                ConfigureGrid(dtos);

                                totalTextbox.Text = dtos.Count.ToString();
                                FillTextBoxes();
                                return;
                            }

                            // 2) RESTO DE GRUPOS (no básculas)
                            var registrosPorTarea = new List<object>();
                            foreach (var tarea in tareasDelGrupo)
                            {
                                switch (tarea.IdGrup)
                                {
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                    case 11:
                                    case 12:
                                    case 13:
                                    case 16:
                                        var registros = registreComuRepository
                                            .GetRegistroComunesPorIdTasca(startDate, endDate, tarea.Id);
                                        if (registros != null) registrosPorTarea.AddRange(registros);
                                        break;
                                }
                            }

                            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                            ConfigureGrid(registrosPorTarea);
                            totalTextbox.Text = registrosPorTarea.Count.ToString();
                            FillTextBoxes();
                            return;
                        }
                        catch (Exception ex)
                        {
                            ReportError("Càrrega de grup amb tasques", ex, ctxBase);
                            return;
                        }
                    }
                    else
                    {
                        // --- CASO: tasca concreta ---
                        string[] parts = tascaCombo.Text.Split(new[] { " - " }, StringSplitOptions.None);
                        if (parts.Length != 2)
                            throw new ArgumentException("Format de tasca no vàlid. Ha de ser 'Títol - Zona'.");

                        string titol = parts[0].Trim();
                        string zona = parts[1].Trim();

                        ctxBase["titol"] = titol;
                        ctxBase["zona"] = zona;

                        Tasques tascaActual = tasques.FirstOrDefault(t => t.Titol == titol && t.Zona == zona);

                        if (tascaActual != null)
                            switch (tascaActual.IdGrup)
                            {
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 11:
                                case 12:
                                case 13:
                                case 16:
                                    try
                                    {
                                        List<RegistroComun> registros =
                                            registreComuRepository.GetRegistroComunesPorIdTasca(startDate, endDate, tascaActual.Id);
                                        ConfigureGrid(registros);
                                        totalTextbox.Text = (registros ?? new List<RegistroComun>()).Count.ToString();
                                        FillTextBoxes();
                                        mostrat = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        ReportError("Càrrega tasca (RegistroComun)", ex, ctxBase);
                                        return;
                                    }
                                    break;

                                case 9:
                                case 10:
                                    try
                                    {
                                        var registrosBasculas =
                                            registroBasculaRepository.GetRegistrosBasculaPorFechaYIdTasca(startDate, endDate, tascaActual.Id);

                                        var dtos = ToBasculaDto(registrosBasculas, tascaActual.IdGrup);
                                        ConfigureGrid(dtos);

                                        totalTextbox.Text = dtos.Count.ToString();
                                        FillTextBoxes();
                                        mostrat = true;
                                        radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                                    }
                                    catch (Exception ex)
                                    {
                                        ReportError("Càrrega tasca (RegistroBàscula)", ex, ctxBase);
                                        return;
                                    }
                                    break;
                            }
                        if (mostrat) return;
                    }
                }
                else
                {
                    RadMessageBox.Show("Has de seleccionar correctament tots els paràmetres per continuar", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                }
            }
            catch (Exception ex)
            {
                ReportError("radButton1_Click (global)", ex, ctxBase);
            }
        }

        /*HELPERS*/
        private List<RegistroBasculaDto> ToBasculaDto(IEnumerable<RegistroBascula> items, int groupId)
        {
            string rang = (groupId == GROUP_BASCULES_GRANS) ? "998 - 1002" : "24.8 - 25.2";
            return (items ?? new List<RegistroBascula>())
                   .Select(x => new RegistroBasculaDto
                   {
                       Id = x.Id,
                       IdTasca = x.IdTasca,
                       IdGrupTasques = x.IdGrupTasques,
                       NomTasca = x.NomTasca,
                       NomGrup = x.NomGrup,
                       Data = x.Data,
                       Valor = x.Valor,
                       Observacions = x.Observacions,
                       Estat = x.Estat,
                       FetaPer = x.FetaPer,
                       Rang = rang
                   })
                   .ToList();
        }

        private void ReportError(string stage, Exception ex, Dictionary<string, object> context)
        {
            string details = BuildExceptionReport(stage, ex, context);

            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            string logPath = Path.Combine(logDir, $"ASM_Registres_{DateTime.Now:yyyyMMdd}.log");

            try
            {
                Directory.CreateDirectory(logDir);
                File.AppendAllText(logPath, details + Environment.NewLine + new string('-', 80) + Environment.NewLine);
            }
            catch { /* si falla el log, no bloqueamos */ }

            try
            {
                Clipboard.SetText(details);
            }
            catch { /* puede fallar en sesiones no-interactivas, ignoramos */ }

            RadMessageBox.Show(
                "S'ha produït un error en: " + stage +
                "\n\nHe copiat els detalls al portapapers i s'han guardat al log." +
                "\nSi us plau, enganxa aquí el contingut que tens al portapapers.",
                "Error",
                MessageBoxButtons.OK,
                RadMessageIcon.Error
            );
        }

        private string BuildExceptionReport(string stage, Exception ex, Dictionary<string, object> context)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Stage: {stage}");

            if (context != null)
            {
                sb.AppendLine("Context:");
                foreach (var kv in context)
                    sb.AppendLine($"  - {kv.Key}: {kv.Value ?? "(null)"}");
            }

            int i = 0;
            Exception cur = ex;
            while (cur != null)
            {
                sb.AppendLine($"Exception #{i}: {cur.GetType().FullName}");
                sb.AppendLine($"Message   : {cur.Message}");
                sb.AppendLine("StackTrace:");
                sb.AppendLine(cur.StackTrace ?? "(no stack)");
                cur = cur.InnerException;
                i++;
                if (cur != null) sb.AppendLine("---- Inner Exception ----");
            }

            return sb.ToString();
        }

        private void FillTextBoxes()
        {
            int completedCount = 0;
            int incompleteCount = 0;

            if (!radGridView1.Columns.Contains("Estat"))
            {
                completedCount = radGridView1.RowCount;
                incompleteCount = 0;
            }
            else
            {
                foreach (var row in radGridView1.Rows)
                {
                    var cell = (row as GridViewDataRowInfo)?.Cells["Estat"];
                    if (cell != null && cell.Value != null)
                    {
                        string estatValue = cell.Value.ToString();

                        if (estatValue.Equals("Completada", StringComparison.OrdinalIgnoreCase) ||
                            estatValue.Equals("Pendent", StringComparison.OrdinalIgnoreCase))
                        {
                            completedCount++;
                        }
                        else if (estatValue.Equals("Incompleta", StringComparison.OrdinalIgnoreCase) ||
                                 estatValue.Equals("No completada", StringComparison.OrdinalIgnoreCase))
                        {
                            incompleteCount++;
                        }
                    }
                }
            }

            completosTextbox.Text = completedCount.ToString();
            incompletosTextbox.Text = incompleteCount.ToString();

            int totalCount = completedCount + incompleteCount;
            double percentage = 0;
            if (totalCount > 0) percentage = (completedCount * 100.0) / totalCount;

            radTextBox2.Text = percentage.ToString("0.##") + "%";

            DateTime startDate = radDateTimePicker1.Value;
            DateTime endDate = radDateTimePicker2.Value;
            
            List<Incidencia> incidencias = new List<Incidencia>();

            string selectedTasca = tascaCombo.Text;

            if (selectedTasca == "Totes")
            {
                string selectedGroupName = grupCombo.Text;
                GrupTasques selectedGroup = grupTasques.FirstOrDefault(g => g.Nom == selectedGroupName);

                if (selectedGroup == null)
                {
                    RadMessageBox.Show("No s'ha trobat la tasca seleccionada.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                if (selectedGroup.Id == GROUP_RESIDUS ||
                    selectedGroup.Id == GROUP_REPROCESSAT ||
                    selectedGroup.Id == GROUP_HAET ||
                    selectedGroup.Id == GROUP_FASE_MOBIL)
                {
                    incidenciasTextbox.Text = "0";
                    return;
                }

                List<Tasques> tareasDelGrupo = tareasRepository.GetTasksByGroupId(selectedGroup.Id);
                foreach (var tasca in tareasDelGrupo)
                {
                    var incidenciasTasca = incidenciasRepository.GetIncidenciasByIdTascaAndDateRange(tasca.Id, startDate, endDate);
                    incidencias.AddRange(incidenciasTasca);
                }
            }

            else
            {
                string[] parts = selectedTasca.Split(new[] { " - " }, StringSplitOptions.None);

                if (parts.Length != 2)
                {
                    RadMessageBox.Show("Format de tasca no vàlid. Ha de ser 'Títol - Zona'.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                string titol = parts[0].Trim();
                string zona = parts[1].Trim();
                Tasques tascaActual = tasques.FirstOrDefault(t => t.Titol == titol && t.Zona == zona);

                if (tascaActual == null)
                {
                    RadMessageBox.Show("No se encontró la tasca seleccionada.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }
                incidencias = incidenciasRepository.GetIncidenciasByIdTascaAndDateRange(tascaActual.Id, startDate, endDate);
            }
            incidenciasTextbox.Text = incidencias.Count.ToString();
        }

        private void ConfigureGrid(object o)
        {
            radGridView1.BeginUpdate();
            var mt = radGridView1.MasterTemplate;
            try
            {
                radGridView1.DataSource = null;

                mt.AutoGenerateColumns = true;

                mt.FilterDescriptors.Clear();
                mt.SortDescriptors.Clear();
                mt.GroupDescriptors.Clear();

                radGridView1.Columns.Clear();
                mt.Columns.Clear();

                radGridView1.DataSource = o;

                SetIdsVisibleFalse();

                ApplySortByDate(radGridView1);

                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                RenameGridColumns();

                foreach (GridViewDataColumn column in radGridView1.Columns)
                    if (column.DataType == typeof(DateTime))
                        column.FormatString = "{0:dd'/'MM'/'yyyy}";

                foreach (GridViewDataColumn col in radGridView1.Columns)
                    if (col.DataType == typeof(string))
                        col.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;

                if (radGridView1.Columns.Contains("Laborant"))
                {
                    var c = radGridView1.Columns["Laborant"];
                    c.HeaderText = "Laborant";
                    c.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
            }
            finally
            {
                radGridView1.EndUpdate();
            }
        }

        private void ConfigureGridGeneralManteniment(object o)
        {
            radGridView1.BeginUpdate();
            try
            {
                radGridView1.DataSource = o;
                SetIdsVisibleFalse();
                radGridView1.MasterTemplate.GroupDescriptors.Clear();
                ApplySortByDate(radGridView1);
                RenameGridColumns();
            }
            finally
            {
                radGridView1.EndUpdate();
            }
        }

        private void ApplySortByDate(RadGridView grid)
        {
            grid.MasterTemplate.SortDescriptors.Clear();

            string dateCol = null;
            if (grid.Columns.Contains("Data")) dateCol = "Data";
            else if (grid.Columns.Contains("Date")) dateCol = "Date";
            else if (grid.Columns.Contains("data")) dateCol = "data";

            if (dateCol != null)
            {
                var sortDescriptor = new SortDescriptor(dateCol, ListSortDirection.Descending);
                grid.MasterTemplate.SortDescriptors.Add(sortDescriptor);
            }
        }

        private void RenameGridColumns()
        {
            var maps = new List<(string from, string to)>
            {
                // General
                ("nomtasca", "Tasca"),
                ("nomgrup", "Grup de tasques"),
                ("FetaPer", "Feta per"),
                ("Observaciones", "Observacions"),
                ("Observacions", "Observacions"),

                // Control Aguas
                ("ControlAguasValue", "Clor combinat (0.1 - 2 mg/L)"),
                ("CloroLibre", "Clor lliure residual (0.1 - 1 mg/L)"),
                ("Nitritos", "Nitrits (v < 0.5 ppm)"),
                ("Ph", "PH (6.5 - 9.5)"),

                // HAET
                ("Date", "Data"),
                ("LaborantId", "Laborant"),
                ("HexaTick", "Hexà"),
                ("AcetonaTick", "Acetona"),
                ("EtanolTick", "Etanol"),
                ("TouleTick", "Toluè"),
                ("HexaBatch", "Lot hexà"),
                ("AcetonaBatch", "Lot acetona"),
                ("EtanolBatch", "Lot etanol"),
                ("TolueBatch", "Lot toluè"),

                // Fase mòbil
                ("AcetatEtilTick", "Acetat etil"),
                ("AcetatBatch", "Lot acetat etil"),

                // Reprocessat (OJO: AQUÍ NO REPETIMOS "Data"; ya lo mapeamos arriba)
                ("Lot", "Lot"),
                ("Pes", "Pes"),
                ("Laborant", "Laborant"),
                ("Producte", "Producte"),

                // Residus
                ("Residu", "Residu"),
                ("Quantitat", "Quantitat"),
                ("QuantitatLitres", "Quantitat (L)")
            };

            foreach (var (from, to) in maps)
            {
                if (radGridView1.Columns.Contains(from))
                {
                    radGridView1.Columns[from].HeaderText = to;
                }
            }
        }

        private bool CheckParams()
        {
            return plaCombo.SelectedIndex == -1 || grupCombo.SelectedIndex == -1 || tascaCombo.SelectedIndex == -1;
        }

        private void SetIdsVisibleFalse()
        {
            var columnsToHide = new[] { "Idtasca", "IdGrupTasques", "Id", "Darrera" };
            foreach (var column in columnsToHide)
            {
                if (radGridView1.Columns.Contains(column))
                    radGridView1.Columns[column].IsVisible = false;
            }
        }

        private void SetRadGridView()
        {
            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableFiltering = true;
            radGridView1.EnableGrouping = true;
            radGridView1.MultiSelect = true;
            radGridView1.MasterTemplate.AutoGenerateColumns = true;
        }

        private void EstadisticaUC_Load(object sender, EventArgs e)
        {
            radDateTimePicker1.Value = new DateTime(2024, 12, 2);
        }

        private void incidenciasButton_Click(object sender, EventArgs e)
        {
            if (tascaCombo.SelectedIndex == -1)
            {
                RadMessageBox.Show("Selecciona una tasca per continuar.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            DateTime startDate = radDateTimePicker1.Value;
            DateTime endDate = radDateTimePicker2.Value;
            List<Incidencia> incidencias = new List<Incidencia>();

            string selectedTasca = tascaCombo.Text;
            if (selectedTasca == "Totes")
            {
                string selectedGroupName = grupCombo.Text;
                GrupTasques selectedGroup = grupTasques.FirstOrDefault(g => g.Nom == selectedGroupName);

                if (selectedGroup == null)
                {
                    RadMessageBox.Show("No s'ha trobat cap grup amb el nom seleccionat.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                if (selectedGroup.Id == GROUP_RESIDUS ||
                    selectedGroup.Id == GROUP_REPROCESSAT ||
                    selectedGroup.Id == GROUP_HAET ||
                    selectedGroup.Id == GROUP_FASE_MOBIL)
                {
                    RadMessageBox.Show("Aquest grup no té incidències associades.", "Informació", MessageBoxButtons.OK, RadMessageIcon.Info);
                    return;
                }

                List<Tasques> tareasDelGrupo = tareasRepository.GetTasksByGroupId(selectedGroup.Id);
                foreach (var tasca in tareasDelGrupo)
                {
                    var incidenciasTasca = incidenciasRepository.GetIncidenciasByIdTascaAndDateRange(tasca.Id, startDate, endDate);
                    incidencias.AddRange(incidenciasTasca);
                }
            }
            else
            {
                string[] parts = selectedTasca.Split(new[] { " - " }, StringSplitOptions.None);

                if (parts.Length != 2)
                {
                    RadMessageBox.Show("Format de tasca no vàlid. Ha de ser 'Títol - Zona'.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                string titol = parts[0].Trim();
                string zona = parts[1].Trim();
                Tasques tascaActual = tasques.FirstOrDefault(t => t.Titol == titol && t.Zona == zona);

                if (tascaActual == null)
                {
                    RadMessageBox.Show("No s'ha trobat la tasca seleccionada.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }
                incidencias = incidenciasRepository.GetIncidenciasByIdTascaAndDateRange(tascaActual.Id, startDate, endDate);
            }
            if (incidencias.Count == 0)
            {
                RadMessageBox.Show("No s'han trobat incidències en el rang de dates especificat.", "Informació", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            else
            {
                Form incidenciasForm = new VerIncidenciasForm(user, incidencias);
                incidenciasForm.FormClosed += IncidenciasForm_FormClosed;
                incidenciasForm.ShowDialog();
            }
        }

        private void IncidenciasForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Fitxers PDF|*.pdf",
                Title = "Desar PDF",
                FileName = "ExportacioDades.pdf"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Document document = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 30f);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                document.Open();

                string tempFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                BaseFont baseFont = BaseFont.CreateFont(tempFontPath, BaseFont.CP1252, BaseFont.EMBEDDED);

                Font titleFont = new Font(baseFont, 18, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font headerFont = new Font(baseFont, 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                Font cellFont = new Font(baseFont, 7, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                PdfPTable headerTable = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };
                headerTable.SetWidths(new float[] { 1f, 1f });

                PdfPCell leftCell = new PdfPCell(new Phrase("Extracció feta per: " + user.Name, cellFont))
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                PdfPCell rightCell = new PdfPCell(new Phrase("Data d'extracció: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), cellFont))
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };

                headerTable.AddCell(leftCell);
                headerTable.AddCell(rightCell);

                document.Add(headerTable);

                document.Add(new Paragraph("\n"));

                Paragraph title = new Paragraph(grupCombo.Text + " - " + tascaCombo.Text, titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                document.Add(title);

                List<GridViewDataColumn> visibleColumns = new List<GridViewDataColumn>();
                foreach (GridViewDataColumn col in radGridView1.Columns)
                    if (col.IsVisible) visibleColumns.Add(col);

                PdfPTable pdfTable = new PdfPTable(visibleColumns.Count)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };

                float[] columnWidths = new float[visibleColumns.Count];
                for (int i = 0; i < visibleColumns.Count; i++) columnWidths[i] = 1f;
                pdfTable.SetWidths(columnWidths);

                foreach (GridViewDataColumn col in visibleColumns)
                {
                    string header = null;

                    if (col.HeaderText == "Cloro Combinado")
                        header = "Clor combinat (0,1 - 2,0)";
                    else if (col.HeaderText == "Cloro Libre Residual")
                        header = "Clor lliure residual (0,1 - 1,0)";
                    else if (col.HeaderText == "Nitritos")
                        header = "Nitrits (< 0,5)";
                    else if (col.HeaderText == "Ph")
                        header = "pH (6,5 - 9,5)";
                    else if (col.HeaderText == "Observacions")
                        header = "Observacions";
                    else
                        header = col.HeaderText;

                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    pdfTable.AddCell(headerCell);
                }

                foreach (var rowObj in radGridView1.Rows)
                {
                    if (rowObj is GridViewDataRowInfo dataRow)
                    {
                        BaseColor rowColor = BaseColor.WHITE;
                        if (radGridView1.Columns.Any(col => col.Name.Equals("Estat", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var estatCell = dataRow.Cells["Estat"];
                            string estatValue = estatCell != null && estatCell.Value != null
                                ? estatCell.Value.ToString().ToLower()
                                : string.Empty;

                            rowColor = (estatValue == "pendent" || estatValue == "completada")
                                ? BaseColor.GREEN
                                : BaseColor.RED;
                        }

                        foreach (GridViewDataColumn col in visibleColumns)
                        {
                            string cellText = dataRow.Cells[col.Name].Value != null
                                ? dataRow.Cells[col.Name].Value.ToString()
                                : string.Empty;

                            if (float.TryParse(cellText, out float floatValue) && col.HeaderText != "Dureza")
                                cellText = floatValue.ToString("0.00");
                            else if (DateTime.TryParse(cellText, out DateTime dateValue) && col.HeaderText != "Dureza")
                                cellText = dateValue.ToString("dd/MM/yyyy");

                            PdfPCell cell = new PdfPCell(new Phrase(cellText, cellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_LEFT,
                                Padding = 5,
                                BackgroundColor = rowColor
                            };
                            pdfTable.AddCell(cell);
                        }
                    }
                }

                document.Add(pdfTable);
                document.Close();
                writer.Close();

                RadMessageBox.Show("PDF generat correctament.", "Èxit", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("Error en generar el PDF: " + ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private sealed class HaetRow
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Laborant { get; set; }
            public bool HexaTick { get; set; }
            public bool AcetonaTick { get; set; }
            public bool EtanolTick { get; set; }
            public bool TouleTick { get; set; }
            public string HexaBatch { get; set; }
            public string AcetonaBatch { get; set; }
            public string EtanolBatch { get; set; }
            public string TolueBatch { get; set; }
        }

        private List<HaetRow> MapHaetWithLaborantName(List<RegistreHAET> items)
        {
            string GetName(int id)
            {
                if (id <= 0) return null;
                if (_empNameCache.TryGetValue(id, out var cached)) return cached;

                string name;
                try
                {
                    var emp = usuarioRepository.GetNameByUserId(id);
                    name = emp;
                }
                catch
                {
                    name = null;
                }

                _empNameCache[id] = name ?? id.ToString();
                return _empNameCache[id];
            }

            var list = new List<HaetRow>();
            foreach (var x in items ?? Enumerable.Empty<RegistreHAET>())
            {
                list.Add(new HaetRow
                {
                    Id = x.Id,
                    Date = x.Date,
                    Laborant = GetName(x.LaborantId),
                    HexaTick = x.HexaTick,
                    AcetonaTick = x.AcetonaTick,
                    EtanolTick = x.EtanolTick,
                    TouleTick = x.TouleTick,
                    HexaBatch = x.HexaBatch,
                    AcetonaBatch = x.AcetonaBatch,
                    EtanolBatch = x.EtanolBatch,
                    TolueBatch = x.TolueBatch
                });
            }
            return list;
        }


        private void radPanel2_Paint(object sender, PaintEventArgs e)
        {
        }
    }
    public class RegistroBasculaDto
    {
        public int Id { get; set; }
        public int IdTasca { get; set; }
        public int IdGrupTasques { get; set; }
        public string NomTasca { get; set; }
        public string NomGrup { get; set; }
        public DateTime Data { get; set; }
        public double Valor { get; set; }
        public string Rang { get; set; }
        public string Observacions { get; set; }
        public string Estat { get; set; }
        public string FetaPer { get; set; }
    }
}
