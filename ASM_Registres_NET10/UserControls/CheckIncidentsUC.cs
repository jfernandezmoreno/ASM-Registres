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
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace ASM_Registres.UserControls
{
    public partial class CheckIncidentsUC : UserControl
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        private List<Tasques> tasques;
        private TareasRepository tareasRepository;
        private PlanRepository planRepository;
        private GrupoTareasRepository grupoTareasRepository;
        private IncidenciasRepository incidenciasRepository;
        private Incidencia incidencia;
        private RadWaitingBar waitingBar1;
        private RadLabelElement numero;
        private User user;
        private SplitPanel panel1;
        private SplitPanel panel2;

        public CheckIncidentsUC(RadWaitingBar radWaitingBar, User user, RadLabelElement numero)
        {
            InitializeComponent();

            waitingBar1 = radWaitingBar;
            this.numero = numero;
            this.user = user;

            InitializeWaitingBar();
            InitializeConnections();
            InitContainers();
            LoadPlans();
        }

        private void InitializeWaitingBar()
        {
            waitingBar1.AssociatedControl = this;
            waitingBar1.WaitingIndicatorSize = new Size(100, 14);
            waitingBar1.WaitingSpeed = 50;
            waitingBar1.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;
            waitingBar1.StartWaiting();
        }

        private void InitializeConnections()
        {
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
            tareasRepository = new TareasRepository(NPGSQLService);
            planRepository = new PlanRepository(NPGSQLService);
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            tasques = tareasRepository.GetTasks();
        }

        private void InitContainers()
        {
            RadSplitContainer verticalSplitContainer = new RadSplitContainer { Dock = DockStyle.Fill };

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
            radGridView1.EnableFiltering = true;
            radGridView1.EnableGrouping = false;

            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
            radGridView1.CellDoubleClick += GridSelectionChanged;
        }

        private void GridSelectionChanged(object sender, EventArgs e)
        {
            if (radGridView1.CurrentRow is GridViewDataRowInfo selectedRow)
            {
                if (user.nivel == DatabaseCredentials.ADMIN_LVL)
                {
                    incidencia = incidenciasRepository.GetIncidenciaById(Convert.ToInt32(selectedRow.Cells["Id"].Value));
                    GestionarIncidenciesForm form = new GestionarIncidenciesForm(incidencia, user);
                    form.FormClosing += IncidenciaFormClosed;
                    form.ShowDialog();
                }
                else
                {
                    RadMessageBox.Show("Només els administradors poden resoldre incidències", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                }
            }
        }

        private void IncidenciaFormClosed(object sender, FormClosingEventArgs e)
        {
            int grupId = incidencia.IdGrupTasques;
            string grup = grupoTareasRepository.GetGroupById(grupId);
            UpdateRadGridView(grup);

        }

        private void UpdateRadGridView(string grup)
        {

            int grupId = GetGrupId(grup);
            if (grupId != -1)
            {
                List<Incidencia> incidencies = incidenciasRepository.GetIncidenciasByIdGrupTasques(grupId);
                ConfigureRadGridView(incidencies);

                RadTreeNode radTreeNode = FindNodeByTitle(radTreeView1, grup);
                UpdateTreeNodeAppearance(radTreeNode, grupId);

                UpdateNumeroIncidenciasNoResueltas();
            }
        }

        private void ConfigureRadGridView(List<Incidencia> incidencies)
        {
            radGridView1.DataSource = incidencies;

            if (radGridView1.Columns["Data"] != null)
                radGridView1.Columns["Data"].FormatString = "{0:dd-MM-yyyy}";

            RenameGridColumns();
            SetIdsVisibleFalse();

            ApplyDefaultSorting("Data", ListSortDirection.Descending);
            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
        }

        private void ApplyDefaultSorting(string columnName, ListSortDirection direction)
        {
            SortDescriptor sortDescriptor = new SortDescriptor
            {
                PropertyName = columnName,
                Direction = direction
            };

            radGridView1.SortDescriptors.Clear();
            radGridView1.SortDescriptors.Add(sortDescriptor);
        }

        private void UpdateTreeNodeAppearance(RadTreeNode radTreeNode, int grupId)
        {
            if (incidenciasRepository.HasUnresolvedIncidencias(grupId))
            {
                radTreeNode.ForeColor = Color.Red;
            }
            else
            {
                radTreeNode.ForeColor = Color.Black;
            }
        }

        private void UpdateNumeroIncidenciasNoResueltas()
        {
            numero.Text = incidenciasRepository.GetNumeroIncidenciasNoResueltas().ToString();
        }


        public RadTreeNode FindNodeByTitle(RadTreeView treeView, string grup)
        {
            foreach (RadTreeNode node in treeView.Nodes)
            {
                RadTreeNode foundNode = FindNodeRecursive(node, grup);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }

        private RadTreeNode FindNodeRecursive(RadTreeNode currentNode, string grup)
        {
            if (currentNode.Text.Equals(grup, StringComparison.OrdinalIgnoreCase))
            {
                return currentNode;
            }

            foreach (RadTreeNode childNode in currentNode.Nodes)
            {
                RadTreeNode foundNode = FindNodeRecursive(childNode, grup);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }

        private int GetGrupId(string grup)
        {
            switch (grup)
            {
                case "Control de bàscules grans (1000 kg)": return 9;
                case "Control de bàscules petites (25 kg)": return 10;
                case "Registre setmanal Manteniment preventiu Carretilla": return 11;
                case "Registre setmanal Manteniment preventiu Compressor": return 12;
                case "Registre estat instal·lació": return 13;
                case "Registre general de manteniment": return 14;
                case "Tasques Empresa Externa": return 1;
                case "Tasques Empresa Externa Segons Necesitat": return 2;
                case "Tasques Producció": return 3;
                case "Tasques Producció S/N (Depòsit)": return 4;
                case "Tasques Producció S/N (Magatzem/Moll)": return 5;
                case "Tasques Producció Segons Necessitat": return 6;
                case "Tasques Laboratori": return 7;
                case "Tasques Manteniment": return 16;
                case "Control Aigues Laboratori": return 20;
                default: return -1;
            }
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Tag is GrupTasques grup)
            {
                UpdateRadGridView(grup.Nom);
            }
        }

        private void RenameGridColumns()
        {
            var columnHeaders = new Dictionary<string, string>
            {
                { "nomtasca", "Tasca" },
                { "nomgrup", "Grup Tasques" },
                { "descripcioincidencia", "Descripcio" },
                { "resolta", "Resolta?" }
            };

            foreach (var column in columnHeaders)
            {
                if (radGridView1.Columns.Contains(column.Key))
                {
                    radGridView1.Columns[column.Key].HeaderText = column.Value;
                }
            }
        }

        private void SetIdsVisibleFalse()
        {
            var columnsToHide = new[] { "Id", "IdTasca", "IdGrupTasques" };
            foreach (var column in columnsToHide)
            {
                if (radGridView1.Columns.Contains(column))
                {
                    radGridView1.Columns[column].IsVisible = false;
                }
            }
        }

        private void LoadPlans()
        {
            List<Pla> plans = planRepository.GetAllPlans();
            List<GrupTasques> grups = grupoTareasRepository.GetAllGrups();

            radTreeView1.Nodes.Clear();

            var plansConGrupos = plans.Where(plan => grups.Any(grup => grup.PlanId == plan.Id)).ToList();

            foreach (var plan in plansConGrupos) 
            {
                RadTreeNode planNode = new RadTreeNode(plan.Nom) { Tag = plan };

                var gruposDelPlan = grups.Where(g => g.PlanId == plan.Id).OrderBy(g => g.Nom).ToList();

                foreach (var grup in gruposDelPlan)
                {   
                    List<Incidencia> inc = incidenciasRepository.GetIncidenciasByIdGrupTasques(grup.Id);
                    if (inc.Count != 0)
                    {
                        RadTreeNode grupNode = new RadTreeNode(grup.Nom) { Tag = grup };
                        
                        if (incidenciasRepository.HasUnresolvedIncidencias(grup.Id))
                        {
                            grupNode.ForeColor = Color.Red;
                        }
                        else
                        {
                            grupNode.ForeColor = Color.Black;
                        }

                        planNode.Nodes.Add(grupNode);
                    }
                }
                radTreeView1.Nodes.Add(planNode);
            }
            waitingBar1.StopWaiting();
        }

        private void radGridView1_Click(object sender, EventArgs e)
        {

        }

        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {

        }
    }
}
