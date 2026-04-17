using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls.UI;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.Services;
using ASM_Registres_NET10.Constants;

namespace ASM_Registres.UserControls
{
    public partial class LaboratoryTasksUC : UserControl
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        User user;
        RadLabelElement incidenciasIcon;

        TareasRepository tareasRepository;
        GrupoTareasRepository grupoTareasRepository;
        PlanRepository planRepository;
        IncidenciasRepository incidenciasRepository;

        Tasques tascaActualSeleccionada = null;

        SplitPanel leftPanel = new SplitPanel();
        SplitPanel rightPanel = new SplitPanel();

        public LaboratoryTasksUC(User user, RadLabelElement incidenciasIcon)
        {
            InitializeComponent();
            SetupVariables(user, incidenciasIcon);
            SetupConnections();
            SetupWindow();
            SetupTreeView();
            SetupEvents();
        }

        private void SetupWindow()
        {
            RadSplitContainer verticalSplitContainer = new RadSplitContainer();
            verticalSplitContainer.Dock = DockStyle.Fill;

            leftPanel.Controls.Add(radTreeView1);
            rightPanel.Controls.Add(radGridView1);

            radTreeView1.Dock = DockStyle.Fill;
            radGridView1.Dock = DockStyle.Fill;

            leftPanel.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            leftPanel.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.15f, 0.15f);

            rightPanel.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            rightPanel.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.85f, 0.85f);

            verticalSplitContainer.SplitPanels.Add(leftPanel);
            verticalSplitContainer.SplitPanels.Add(rightPanel);

            Controls.Add(verticalSplitContainer);

            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableGrouping = true;

            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
        }

        private void SetupEvents()
        {
            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
            radGridView1.CellDoubleClick += doubleClickInGridView;
        }

        private void doubleClickInGridView(object sender, GridViewCellEventArgs e)
        {
            if (radGridView1.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                tascaActualSeleccionada = tareasRepository.GetTaskById(Convert.ToInt32(selectedRow.Cells["Id"].Value));

                RegistroComunForm registroComunForm = new RegistroComunForm(tascaActualSeleccionada, user, incidenciasIcon);
                registroComunForm.FormClosed += RegistrosGridClosed;
                registroComunForm.ShowDialog();
            }
        }

        private void RegistrosGridClosed(object sender, FormClosedEventArgs e)
        {
            int numeroIncidenciasNoResueltas = incidenciasRepository.GetNumeroIncidenciasNoResueltas();
            incidenciasIcon.Text = numeroIncidenciasNoResueltas.ToString();
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Tag is GrupTasques grup)
            {
                radGridView1.DataSource = null;
                radGridView1.ClearSelection();

                var grupFormActions = new Dictionary<string, Action>
                {
                    { "Control Aigues Laboratori", () => new ControlAguasForm(user).ShowDialog() },
                    { "Control Residus", () => new ControlResidusForm(user).ShowDialog() },
                    { "Gestió Material", () => new GestionMaterialLaboratoriForm(user).ShowDialog() },
                    { "Reprocessat Mostres Laboratori", () => new AgregarReprocesatMostresLaboratoriForm(user).ShowDialog() },
                    { "Registro HAET", () => new HAET(user).ShowDialog() },
                    { "Registro Fase Movil", () => new FASE_MOVIL(user).ShowDialog() }
                };

                var grupTareasMapping = new Dictionary<string, Func<List<Tasques>>>
                {
                { "Tasques Laboratori", () => GetTareasLaboratori() }
                };

                if (grupFormActions.ContainsKey(grup.Nom))
                {
                    grupFormActions[grup.Nom].Invoke();
                    return;
                }

                else if (grupTareasMapping.ContainsKey(grup.Nom))
                {
                    rightPanel.Controls.Clear();
                    rightPanel.Controls.Add(radGridView1);
                    radGridView1.Dock = DockStyle.Fill;

                    List<Tasques> tasquesHoy = grupTareasMapping[grup.Nom].Invoke();
                    ConfigurarGridView(tasquesHoy);
                    SetIdsVisibleFalse();
                }
                else
                {
                    radGridView1.DataSource = null;
                }
            }
        }

        private void SetIdsVisibleFalse()
        {
            radGridView1.Columns["Id"].IsVisible = false;
            radGridView1.Columns["IdGrup"].IsVisible = false;
            radGridView1.Columns["IdIntern"].IsVisible = false;
            radGridView1.Columns["Darrera"].IsVisible = false;
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

        private List<Tasques> GetTareasLaboratori()
        {
            return tareasRepository.GetTasquesLab();
        }

        private void SetupConnections()
        {
            tareasRepository = new TareasRepository(NPGSQLService);
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            planRepository = new PlanRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
        }

        private void SetupTreeView()
        {
            List<Pla> plans = planRepository.GetAllPlans();
            plans.RemoveAll(plan => plan.Id == 2);

            List<GrupTasques> grups = grupoTareasRepository.GetLabGroups();

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
        }

        private void SetupVariables(User user, RadLabelElement incidenciasIcon)
        {
            this.user = user;
            this.incidenciasIcon = incidenciasIcon;
        }

        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {

        }
    }
}