using ASM_Registres_NET10.Constants;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace ASM_Registres.UserControls
{
    public partial class tareasHoy : UserControl
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        private List<Tasques> tasques;
        private TareasRepository tareasRepository;
        private PlanRepository planRepository;
        private GrupoTareasRepository grupoTareasRepository;
        private FestivosRepository festivosRepository;
        private IncidenciasRepository incidenciasRepository;
        private List<DateTime> festivos;
        private Tasques tascaActualSeleccionada;
        private RadLabelElement numero;
        private RadWaitingBar waitingBar1;
        private User user;
        private SplitPanel panel1;
        private SplitPanel panel2;

        public tareasHoy(RadWaitingBar waitingBar, User user, RadLabelElement numero)
        {
            InitializeComponent();

            waitingBar1 = waitingBar;
            this.numero = numero;
            this.user = user;

            InitializeConnections();
            InitializeWaitingBar();
            LoadData();
            SetupEvents();
            InitContainers();
            LoadPlans();
        }

        private void InitializeConnections()
        {
            tareasRepository = new TareasRepository(NPGSQLService);
            planRepository = new PlanRepository(NPGSQLService);
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            festivosRepository = new FestivosRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
            tasques = tareasRepository.GetTasks();
            festivos = festivosRepository.GetFestivos(); 
        }

        private void InitializeWaitingBar()
        {
            waitingBar1.AssociatedControl = this;
            waitingBar1.WaitingIndicatorSize = new Size(100, 14);
            waitingBar1.WaitingSpeed = 50;
            waitingBar1.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;
            waitingBar1.StartWaiting();
        }

        private void LoadData()
        {
            tasques = tareasRepository.GetTasks();
            tascaActualSeleccionada = new Tasques();
        }

        private void SetupEvents()
        {
            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
            radGridView1.TableElement.BorderColor = Color.Blue;
            radGridView1.CellDoubleClick += GridSelectionChanged;
        }

        private void InitContainers()
        {
            var verticalSplitContainer = new RadSplitContainer { Dock = DockStyle.Fill };

            panel1 = new SplitPanel
            {
                SizeInfo = { SizeMode = SplitPanelSizeMode.Relative, RelativeRatio = new SizeF(0.15f, 0.15f) },
                Controls = { radTreeView1 }
            };

            panel2 = new SplitPanel
            {
                SizeInfo = { SizeMode = SplitPanelSizeMode.Relative, RelativeRatio = new SizeF(0.85f, 0.85f) },
                Controls = { radGridView1 }
            };

            radTreeView1.Dock = DockStyle.Fill;
            radGridView1.Dock = DockStyle.Fill;

            verticalSplitContainer.SplitPanels.Add(panel1);
            verticalSplitContainer.SplitPanels.Add(panel2);
            Controls.Add(verticalSplitContainer);

            ConfigureRadGridView();
        }

        private void ConfigureRadGridView()
        {
            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableGrouping = true;
        }

        private async void LoadPlans()
        {
            await Task.Delay(3000);
            var plans = await Task.Run(() => planRepository.GetAllPlans());
            var grups = await Task.Run(() => grupoTareasRepository.GetSelectedGrupsHoy());

            radTreeView1.Nodes.Clear();

            var gruposProduccio = new[] { 3, 9, 10 };
            var gruposLaboratori = new[] { 7 };
            var gruposManteniment = new[] { 3, 9, 10, 11, 12, 13, 16 };

            foreach (var plan in plans.Where(plan => grups.Any(grup => grup.PlanId == plan.Id)))
            {
                var planNode = new RadTreeNode(plan.Nom) { Tag = plan };

                foreach (var grup in grups.Where(g => g.PlanId == plan.Id).OrderBy(g => g.Nom))
                {
                    if ((user.Departament == "Producción" && gruposProduccio.Contains(grup.Id)) ||
                    (user.Departament == "Mantenimiento" && gruposManteniment.Contains(grup.Id)) ||
                    (user.Departament != "Producción" && user.Departament != "Laboratori" && user.Departament != "Mantenimiento"))
                    {
                        if (GetTareasParaHoy(tasques, grup.Id).Count != 0)
                        {
                            var grupNode = new RadTreeNode(grup.Nom) { Tag = grup, Name = grup.Nom };
                            planNode.Nodes.Add(grupNode);
                        }
                    }
                }
                radTreeView1.Nodes.Add(planNode);
            }
            waitingBar1.StopWaiting();
        }

        private void GridSelectionChanged(object sender, EventArgs e)
        {
            if (radGridView1.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                tascaActualSeleccionada = tareasRepository.GetTaskById(Convert.ToInt32(selectedRow.Cells["Id"].Value));

                Form registroForm;
                if (tascaActualSeleccionada.IdGrup == 9 || tascaActualSeleccionada.IdGrup == 10)
                {
                    registroForm = new RegistroBasculaForm(tascaActualSeleccionada, user, numero);
                }
                else
                {
                    registroForm = new RegistroComunForm(tascaActualSeleccionada, user, numero);
                }

                registroForm.FormClosing += NewFormClosed;
                registroForm.ShowDialog();
            }
        }

        private async void NewFormClosed(object sender, FormClosingEventArgs e)
        {
            var grupTasques = await Task.Run(() => grupoTareasRepository.ObtenerNombreGrupoPorId(tascaActualSeleccionada.IdGrup));
            tasques = tareasRepository.GetTasks();

            UpdateRadGridView(grupTasques);
            RenameGridColumns();
            SetIdsVisibleFalse();

            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            numero.Text = incidenciasRepository.GetNumeroIncidenciasNoResueltas().ToString();
        }

        private void UpdateRadGridView(string grupTasques)
        {
            panel2.Controls.Clear();
            panel2.Controls.Add(radGridView1);
            radGridView1.Dock = DockStyle.Fill;
            radGridView1.ClearSelection();

            int grupId;
            switch (grupTasques)
            {
                case "Tasques Empresa Externa":
                    grupId = 1;
                    break;
                case "Tasques Laboratori":
                    grupId = 7;
                    break;
                case "Tasques Producció":
                    grupId = 3;
                    break;
                case "Control de bàscules petites (25 kg)":
                    grupId = 10;
                    break;
                case "Control de bàscules grans (1000 kg)":
                    grupId = 9;
                    break;
                case "Tasques Manteniment":
                    grupId = 16;
                    break;
                case "Registre estat instal·lació":
                    grupId = 13;
                    break;
                case "Registre setmanal Manteniment preventiu Carretilla":
                    grupId = 11;
                    break;
                case "Registre setmanal Manteniment preventiu Compressor":
                    grupId = 12;
                    break;
                default:
                    grupId = -1;
                    break;
            }

            if (grupId != -1)
            {
                var tasquesHoy = GetTareasParaHoy(tasques, grupId);
                radGridView1.DataSource = tasquesHoy;
                radGridView1.GroupDescriptors.Clear();
                GroupDescriptor descriptor = new GroupDescriptor();
                descriptor.GroupNames.Add("Zona", ListSortDirection.Ascending);
                radGridView1.GroupDescriptors.Add(descriptor);
                radGridView1.MasterTemplate.ExpandAllGroups();
            }
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Tag is GrupTasques grup)
            {
                UpdateRadGridView(grup.Nom);
                RenameGridColumns();
                SetIdsVisibleFalse();
                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void SetIdsVisibleFalse()
        {
            var columnsToHide = new[] { "Id", "IdGrup", "IdIntern", "Darrera" };
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
                    { "ValorMin", "Valor mínim" },
                    { "ValorMax", "Valor màxim" },
                    { "CriterisAcceptacio", "Criteris d'acceptació" },
                    { "CaracteristiquesControl", "Característiques de control" },
                    { "TipusEquip", "Tipus d'equip" },
                    { "NomEquip", "Nom de l'equip" },
                    { "Frequencia", "Freqüència" },
                    { "Ambit", "Àmbit" },
                    { "Metode", "Mètode" },
                    { "Item", "Ítem" },
                    { "Alies", "Àlies" },
                    { "Operacio", "Operació" }
                };


            foreach (var column in columnHeaders)
            {
                if (radGridView1.Columns.Contains(column.Key))
                {
                    radGridView1.Columns[column.Key].HeaderText = column.Value;
                }
            }
        }

        public List<Tasques> GetTareasParaHoy(List<Tasques> todasLasTareas, int idGrup)
        {
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday || festivos.Contains(DateTime.Today))
            {
                return new List<Tasques>();
            }

            return todasLasTareas
                .Where(t => t.IdGrup == idGrup)
                .Where(t =>
                    (t.Periodicitat == "DIARIA" && t.Darrera != DateTime.Today && !EsFestivoOFinDeSemana(DateTime.Today)) ||
                    (t.Periodicitat == "SEMANAL" && TareaSemanalAplicaHoy(t))
                ).ToList();
        }

        private bool EsFestivoOFinDeSemana(DateTime date)
        {
            return festivos.Contains(date) || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool TareaSemanalAplicaHoy(Tasques tarea)
        {
            DateTime startOfWeek = GetPrimerDiaNoFestivoDeLaSemana();
            return (tarea.Darrera < startOfWeek);
        }

        private DateTime GetPrimerDiaNoFestivoDeLaSemana()
        {
            DateTime today = DateTime.Today;
            int daysToSubtract = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
            DateTime primerDiaSemana = today.AddDays(-daysToSubtract);

            while (EsFestivoOFinDeSemana(primerDiaSemana))
            {
                primerDiaSemana = primerDiaSemana.AddDays(1);
            }

            return primerDiaSemana;
        }

        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {

        }
    }
}
