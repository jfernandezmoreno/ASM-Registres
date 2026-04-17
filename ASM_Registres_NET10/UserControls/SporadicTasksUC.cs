using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace ASM_Registres.UserControls
{
    public partial class SporadicTasksUC : UserControl
    {

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        List<Tasques> tasques;
        Tasques tascaActualSeleccionada;
        List<DateTime> festivos;

        TareasRepository tareasRepository;
        PlanRepository planRepository;
        GrupoTareasRepository grupoTareasRepository;
        FestivosRepository festivosRepository;
        IncidenciasRepository incidenciasRepository;

        User user;
        RadLabelElement numero;
        RadWaitingBar waitingBar1;

        SplitPanel panel1 = new SplitPanel();
        SplitPanel panel2 = new SplitPanel();

        public SporadicTasksUC(RadWaitingBar waitingBar, User user, RadLabelElement numero)
        {
            InitializeComponent();
            ConfigWaitingBar(waitingBar);
            SetupConnections();
            SetupVariables(user, numero);

            tascaActualSeleccionada = new Tasques();

            InitContainers();
            LoadPlans();

            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
            radGridView1.TableElement.BorderColor = Color.Blue;
            radGridView1.CellDoubleClick += grid_selectionChanged;
        }

        private void SetupVariables(User actualUser, RadLabelElement actualNumero)
        {
            user = actualUser;
            numero = actualNumero;
        }

        private void SetupConnections()
        {
            tareasRepository = new TareasRepository(NPGSQLService);
            planRepository = new PlanRepository(NPGSQLService);
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            festivosRepository = new FestivosRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);

            tasques = new List<Tasques>();
            tasques = tareasRepository.GetTasks();
            festivos = festivosRepository.GetFestivos();
        }

        private void ConfigWaitingBar(RadWaitingBar actualWaitingBar)
        {
            waitingBar1 = actualWaitingBar;
            waitingBar1.AssociatedControl = this;
            waitingBar1.WaitingIndicatorSize = new System.Drawing.Size(100, 14);
            waitingBar1.WaitingSpeed = 50;
            waitingBar1.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;
            waitingBar1.StartWaiting();
        }

        private async void LoadPlans()
        {
            await Task.Delay(3000);

            List<Pla> plans = await Task.Run(() => planRepository.GetAllPlans());
            plans.RemoveAll(plan => plan.Id == 2);

            List<GrupTasques> grups = await Task.Run(() => grupoTareasRepository.GetAllGrups());
            grups.RemoveAll(grup => grup.Id == 15);
            grups.RemoveAll(grup => grup.Id == 1);
            grups.RemoveAll(grup => grup.Id == 2);

            grups = grups.Where(grup => !grup.Nom.Equals("Tareas Mantenimiento", StringComparison.OrdinalIgnoreCase)).ToList();

            radTreeView1.Nodes.Clear();

            var plansConGrupos = plans
                .Where(plan => grups.Any(grup => grup.PlanId == plan.Id))
                .ToList();

            foreach (var plan in plansConGrupos)
            {
                RadTreeNode planNode = new RadTreeNode(plan.Nom);
                planNode.Tag = plan;

                var gruposDelPlan = grups
                    .Where(g => g.PlanId == plan.Id)
                    .OrderBy(g => g.Nom)
                    .ToList();

                foreach (var grup in gruposDelPlan)
                {
                    RadTreeNode grupNode = new RadTreeNode(grup.Nom);
                    grupNode.Tag = grup;
                    planNode.Nodes.Add(grupNode);
                }

                radTreeView1.Nodes.Add(planNode);
            }
            waitingBar1.StopWaiting();
        }

        private void grid_selectionChanged(object sender, EventArgs e)
        {
            if (radGridView1.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                tascaActualSeleccionada = tareasRepository.GetTaskById(Convert.ToInt32(selectedRow.Cells["Id"].Value));

                if (tascaActualSeleccionada.IdGrup == 9 || tascaActualSeleccionada.IdGrup == 10)
                {
                    RegistroBasculaForm registroBasculaForm = new RegistroBasculaForm(tascaActualSeleccionada, user, numero);
                    registroBasculaForm.FormClosed += RegistroBasculaForm_FormClosed;
                    registroBasculaForm.ShowDialog();
                }
                else if (tascaActualSeleccionada.IdGrup == 18)
                {
                    RegistroSiloForm registro = new RegistroSiloForm(tascaActualSeleccionada, user);
                    registro.FormClosed += RegistroBasculaForm_FormClosed;
                    registro.ShowDialog();
                }
                else
                {
                    RegistroComunForm registroComunForm = new RegistroComunForm(tascaActualSeleccionada, user, numero);
                    registroComunForm.FormClosed += RegistroBasculaForm_FormClosed;
                    registroComunForm.ShowDialog();
                }
            }
        }

        private void RegistroBasculaForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            int numero2 = incidenciasRepository.GetNumeroIncidenciasNoResueltas();
            numero.Text = numero2.ToString();
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Tag is GrupTasques grup)
            {
                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
                radGridView1.DataSource = null;
                radGridView1.ClearSelection();

                var grupFormActions = new Dictionary<string, Action>
                {
                    { "Registre Temperatures Camares Tèrmiques",            () => new RegistreTemperaturesForm(user).ShowDialog() },
                    { "Registre general de manteniment",                    () => new RegistreGeneralForm(user).ShowDialog() },
                    { "Us Productes de Nateja",                             () => new RegistroLimpiezaForm().ShowDialog() },
                    { "Control Aigues Laboratori",                          () => new ControlAguasForm(user).ShowDialog() },
                    { "Control Residus",                                    () => new ControlResidusForm(user).ShowDialog() },
                    { "Gestió Material",                                    () => new GestionMaterialLaboratoriForm(user).ShowDialog() },
                    { "Registro Fase Movil",                                () => new FASE_MOVIL(user).ShowDialog() },
                    { "Registro HAET",                                      () => new HAET(user).ShowDialog() },
                    { "Reprocessat Mostres Laboratori",                     () => new AgregarReprocesatMostresLaboratoriForm(user).ShowDialog()}
                };

                var grupTareasMapping = new Dictionary<string, Func<List<Tasques>>>
                {
                    { "Tasques Producció S/N (Depòsit)",                    () => GetTareasEsporadicasParaHoy(tasques, 4) },
                    { "Tasques Producció S/N (Magatzem/Moll)",              () => GetTareasEsporadicasParaHoy(tasques, 5) },
                    { "Tasques Producció Segons Necessitat",                () => GetTareasEsporadicasParaHoy(tasques, 6) },
                    { "Tasques Laboratori",                                 () => GetTareasParaHoy(tasques, 7) },
                    { "Tasques Producció",                                  () => GetTareasParaHoy(tasques, 3) },
                    { "Control de bàscules petites (25 kg)",                () => GetTareasParaHoy(tasques, 10) },
                    { "Control de bàscules grans (1000 kg)",                () => GetTareasBaculesGrans(tasques, 9) },
                    { "Registro de estado de instalación",                  () => GetTareasParaHoy(tasques, 13) },
                    { "Registre setmanal Manteniment preventiu Carretilla", () => GetTareasParaHoy(tasques, 11) },
                    { "Registre setmanal Manteniment preventiu Compressor", () => GetTareasParaHoy(tasques, 12) },
                    { "Registros de precintado para los silos",             () => GetTareasEsporadicasParaHoy(tasques, 18) },
                    { "Registre estat instal·lació",                        () => GetTareasEsporadicasParaHoy(tasques, 13) },
                    { "Tasques Manteniment",                                () => GetTareasParaHoy(tasques, 16) },
                    { "Sala Polivalent",                                    () => GetTareasEsporadicasParaHoy(tasques, 26)}
                };


                if (grupFormActions.ContainsKey(grup.Nom))
                {
                    grupFormActions[grup.Nom].Invoke();
                    return;
                }

                if (grupTareasMapping.ContainsKey(grup.Nom))
                {
                    panel2.Controls.Clear();
                    panel2.Controls.Add(radGridView1);
                    radGridView1.Dock = DockStyle.Fill;

                    List<Tasques> tasquesHoy = grupTareasMapping[grup.Nom].Invoke();
                    ConfigurarGridView(tasquesHoy);
                }

                RenameGridColumns();
                SetIdsVisibleFalse();
                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void ConfigurarGridView(List<Tasques> tasquesHoy)
        {
            radGridView1.ClearSelection();
            radGridView1.DataSource = tasquesHoy;
            radGridView1.GroupDescriptors.Clear();

            GroupDescriptor descriptor = new GroupDescriptor();
            descriptor.GroupNames.Add("Zona", ListSortDirection.Ascending);
            radGridView1.GroupDescriptors.Add(descriptor);
            radGridView1.MasterTemplate.ExpandAllGroups();
        }

        public List<Tasques> GetTareasParaHoy(List<Tasques> todasLasTareas, int idGrup)
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                return new List<Tasques>(); 
            }

            if (festivos.Contains(DateTime.Today))
            {
                return new List<Tasques>();
            }

            return todasLasTareas
                .Where(t => t.IdGrup == idGrup)
                .Where(t =>
                    (t.Periodicitat == "DIARIA") ||
                    (t.Periodicitat == "SEMANAL")
                ).ToList();
        }

        public List<Tasques> GetTareasBaculesGrans(List<Tasques> todasLasTareas, int idGrup)
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                return new List<Tasques>();
            }

            if (festivos.Contains(DateTime.Today))
            {
                return new List<Tasques>();
            }

            return todasLasTareas.Where(t => t.IdGrup == idGrup).ToList();
        }

        private void SetIdsVisibleFalse()
        {
            radGridView1.Columns["Id"].IsVisible = false;
            radGridView1.Columns["IdGrup"].IsVisible = false;
            radGridView1.Columns["IdIntern"].IsVisible = false;
            radGridView1.Columns["Periodicitat"].IsVisible = false;
            radGridView1.Columns["Darrera"].IsVisible = false;
        }

        private void RenameGridColumns()
        {
            var columnHeaders = new Dictionary<string, string>
            {
                ["ValorMin"] = "Valor Mínimo",
                ["ValorMax"] = "Valor Máximo",
                ["CriterisAcceptacio"] = "Criterios de Aceptación",
                ["CaracteristiquesControl"] = "Características de Control",
                ["TipusEquip"] = "Tipo de Equipo",
                ["NomEquip"] = "Nombre del Equipo",
                ["Frequencia"] = "Frecuencia",
                ["Ambit"] = "Ámbito",
                ["Metode"] = "Método",
                ["Item"] = "Ítem",
                ["Alies"] = "Alias",
                ["Operacio"] = "Operación"
            };

            foreach (var column in columnHeaders)
            {
                if (radGridView1.Columns.Contains(column.Key))
                {
                    radGridView1.Columns[column.Key].HeaderText = column.Value;
                }
            }
        }
        private async void InitContainers()
        {
            RadSplitContainer verticalSplitContainer = new RadSplitContainer();
            verticalSplitContainer.Dock = DockStyle.Fill;

            panel1.Controls.Add(radTreeView1);
            panel2.Controls.Add(radGridView1);

            radTreeView1.Dock = await Task.Run(() => DockStyle.Fill);
            radGridView1.Dock = await Task.Run(() => DockStyle.Fill);

            panel1.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel1.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.15f, 0.15f);

            panel2.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel2.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.85f, 0.85f);

            verticalSplitContainer.SplitPanels.Add(panel1);
            verticalSplitContainer.SplitPanels.Add(panel2);

            Controls.Add(verticalSplitContainer);

            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableGrouping = true;
        }

        public List<Tasques> GetTareasEsporadicasParaHoy(List<Tasques> todasLasTareas, int idGrup)
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                return new List<Tasques>();
            }

            if (festivos.Contains(DateTime.Today))
            {
                return new List<Tasques>();
            }

            return todasLasTareas
                .Where(t => t.IdGrup == idGrup)
                .Where(t =>
                    t.Periodicitat == "DESPRES US"
                ).ToList();
        }
    }
}
