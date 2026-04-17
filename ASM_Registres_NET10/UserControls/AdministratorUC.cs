using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace ASM_Registres.UserControls
{
    public partial class AdministratorUC : UserControl
    {
        readonly NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RadWaitingBar waitingBar1;
        RegistroComunRepository registreComuRepository;
        RegistroBasculaRepository registroBasculaRepository;

        RadLabelElement exclamationIcon;

        List<RegistroBascula> basculesPendents;
        List<RegistroBascula> basculesIncompletes;

        List<RegistroComun> comunsPendents;
        List<RegistroComun> comunsIncomplets;

        private bool Capado = false; 
        User UsuariActual;

        List<int> gruposNoCapados = new List<int> { 1, 2, 3,4, 5, 6, 9, 10 };

        private static readonly Dictionary<string, string> RANGO_POR_BASCULA =
            new Dictionary<string, string>(StringComparer.InvariantCulture)
            {
                { "Bàscula oleo WT-105", "998 - 1002" },
                { "Bàscula líquids WT-601", "998 - 1002" },
                { "WT-409 (Ref. 25KG)", "24.8 - 25.2" },
                { "WT-602 (Ref. 25KG)", "24.8 - 25.2" },
            };

        public AdministratorUC(User u, RadWaitingBar radWaitingBar, RadLabelElement exclamationIcon)
        {
            InitializeComponent();
            SetupConnections();
            SetupVariables(u, radWaitingBar, exclamationIcon);
            InitContainers();
            LoadPlans();
            CheckCapado();


            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
            radGridView1.CellDoubleClick += Grid1DoubleClick;
            radGridView2.CellDoubleClick += Grid2DoubleClick;
        }

        private void CheckCapado()
        {
            if (UsuariActual.Id == 23 || UsuariActual.Id == 4)
            {
                Capado = true;
            }
        }

        private void SetupVariables(User u, RadWaitingBar actualRadWaitingBar, RadLabelElement exclamationIcon)
        {
            waitingBar1 = actualRadWaitingBar;
            this.exclamationIcon = exclamationIcon;

            UsuariActual = u;

            exclamationIcon.Enabled = true;

            if (!basculesPendents.Any() && !basculesIncompletes.Any() && !comunsPendents.Any() && !comunsIncomplets.Any())
            {
                exclamationIcon.Enabled = false;
            }
        }

        private void SetupConnections()
        {
            registreComuRepository = new RegistroComunRepository(NPGSQLService);
            registroBasculaRepository = new RegistroBasculaRepository(NPGSQLService);

            basculesPendents = registroBasculaRepository.GetRegistrosBasculaPendent();
            basculesIncompletes = registroBasculaRepository.GetRegistrosBasculaIncomplet();

            comunsPendents = registreComuRepository.GetRegistrosPendent();
            comunsIncomplets = registreComuRepository.GetRegistrosIncomplet();
        }

        private void Grid1DoubleClick(object sender, EventArgs e)
        {
            if (radGridView1.SelectedRows.Count == 0)
                return;

            if (!ConfirmarCierre())
                return;

            string tipoRegistro = ProcesarRegistrosSeleccionados();

            ActualizarGrid(tipoRegistro);
            ConfigurarUI();
        }

        private bool ConfirmarCierre()
        {
            DialogResult dialogResult = RadMessageBox.Show(
                "Vols confirmar el registre de les tasques i marcar-les com a completades?",
                "Confirmar el tancament",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Info);

            return dialogResult == DialogResult.Yes;
        }

        private string ProcesarRegistrosSeleccionados()
        {
            bool hayBasculasActualizadas = false;
            bool hayComunesActualizados = false;

            bool seACapadoAlgunRegistro = false;

            foreach (var row in radGridView1.SelectedRows)
            {
                if (!(row is GridViewDataRowInfo selectedRow))
                    continue;

                int id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                string nomGrup = selectedRow.Cells["NomGrup"].Value.ToString();
                string estado = "Completada";

                if (Capado)
                {
                    if (gruposNoCapados.Contains(Convert.ToInt32(selectedRow.Cells["IdGrupTasques"].Value)))
                    {
                        CompletaRegistro(ref hayBasculasActualizadas, ref hayComunesActualizados, id, nomGrup, estado);
                    }
                    else
                    {
                        seACapadoAlgunRegistro = true;
                        continue;
                    }
                }
                else
                {
                    CompletaRegistro(ref hayBasculasActualizadas, ref hayComunesActualizados, id, nomGrup, estado);
                }
            }

            if (seACapadoAlgunRegistro) 
            {
                RadMessageBox.Show("Se ha capado la confirmación de algun registro, ya que no perteneces al departamento responsable de se confirmación :)", "Casi", MessageBoxButtons.OK, RadMessageIcon.Error);
            }

            if (hayBasculasActualizadas)
            {
                ActualizarRegistrosBascula();
                return "Bascula";
            }

            if (hayComunesActualizados)
            {
                ActualizarRegistrosComunes();
                return "Comun";
            }

            return "Non";
        }

        private void CompletaRegistro(ref bool hayBasculasActualizadas, ref bool hayComunesActualizados, int id, string nomGrup, string estado)
        {
            if (EsBascula(nomGrup))
            {
                registroBasculaRepository.UpdateRegistroBascula(id, estado);
                hayBasculasActualizadas = true;
            }
            else
            {
                registreComuRepository.UpdateRegistroComun(id, estado);
                hayComunesActualizados = true;
            }
        }

        private void ActualizarRegistrosBascula()
        {
            basculesPendents = registroBasculaRepository.GetRegistrosBasculaPendent();
            basculesIncompletes = registroBasculaRepository.GetRegistrosBasculaIncomplet();
        }

        private void ActualizarRegistrosComunes()
        {
            comunsPendents = registreComuRepository.GetRegistrosPendent();
            comunsIncomplets = registreComuRepository.GetRegistrosIncomplet();
        }

        private void ActualizarGrid(string tipo)
        {
            if (tipo == "Bascula")
            {
                radGridView1.DataSource = basculesPendents;
                radGridView2.DataSource = basculesIncompletes;

                AddOrUpdateRangoColumn(radGridView1, basculesPendents);
                AddOrUpdateRangoColumn(radGridView2, basculesIncompletes);
            }
            else if (tipo == "Comun")
            {
                radGridView1.DataSource = comunsPendents;
                radGridView2.DataSource = comunsIncomplets;

                AddOrUpdateRangoColumn(radGridView1, comunsPendents);
                AddOrUpdateRangoColumn(radGridView2, comunsIncomplets);
            }
            else
            {
                radGridView1.DataSource = null;
                radGridView2.DataSource = null;

                AddOrUpdateRangoColumn(radGridView1, null);
                AddOrUpdateRangoColumn(radGridView2, null);
            }

            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            radGridView2.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
        }

        private void ConfigurarUI()
        {
            SetIdsVisibleFalse();
            exclamationIcon.Enabled = basculesPendents.Any() || basculesIncompletes.Any() ||
                                      comunsPendents.Any() || comunsIncomplets.Any();
        }

        private void Grid2DoubleClick(object sender, EventArgs e)
        {
            var currentRow = radGridView2.CurrentRow as GridViewDataRowInfo;
            if (currentRow == null)
                return;

            int id = Convert.ToInt32(currentRow.Cells["Id"].Value);
            string nomGrup = currentRow.Cells["NomGrup"].Value.ToString();

            if (EsBascula(nomGrup))
            {
                MostrarDialogoBascula(id);
            }
            else
            {
                MostrarDialogoComuna(id);
            }
        }

        private bool EsBascula(string nomGrup)
        {
            var gruposBascula = new HashSet<string>
            {
                "Bàscula oleo WT-105", // 998 - 1002
                "Bácula potasa WT-003", // 998 - 1002
                "Bàscula líquids WT-601", // 998 - 1002
                "WT-409 (Ref. 25KG)", // 24.8 - 25.2
                "WT-602 (Ref. 25KG)", // 24.8 - 25.2
                "AOX Depósitos Óleo", // 998 - 1002
                "AOX Reactores" // 998 - 1002
            };

            return gruposBascula.Contains(nomGrup);
        }

        private void MostrarDialogoBascula(int id)
        {
            RegistroBascula registroBascula = registroBasculaRepository.GetRegistro(id);
            TascaNoFetaBascula tascaNoFetaBascula = new TascaNoFetaBascula(registroBascula);
            tascaNoFetaBascula.FormClosing += newFormClosedBascules;
            tascaNoFetaBascula.ShowDialog();
        }

        private void MostrarDialogoComuna(int id)
        {
            RegistroComun registroComun = registreComuRepository.GetRegistroComun(id);
            TascaNoFetaComuna tascaNoFetaComuna = new TascaNoFetaComuna(registroComun);
            tascaNoFetaComuna.FormClosing += newFormClosedComunes;
            tascaNoFetaComuna.ShowDialog();
        }

        private void newFormClosedBascules(object o, FormClosingEventArgs a)
        {
            basculesPendents = registroBasculaRepository.GetRegistrosBasculaPendent();
            basculesIncompletes = registroBasculaRepository.GetRegistrosBasculaIncomplet();

            radGridView1.DataSource = basculesPendents;
            radGridView2.DataSource = basculesIncompletes;

            AddOrUpdateRangoColumn(radGridView1, basculesPendents);
            AddOrUpdateRangoColumn(radGridView2, basculesIncompletes);

            if (!basculesPendents.Any() && !basculesIncompletes.Any() && !comunsPendents.Any() && !comunsIncomplets.Any())
            {
                exclamationIcon.Enabled = false;
            }
        }

        private void newFormClosedComunes(object o, FormClosingEventArgs a)
        {
            comunsPendents = registreComuRepository.GetRegistrosPendent();
            comunsIncomplets = registreComuRepository.GetRegistrosIncomplet();

            radGridView1.DataSource = comunsPendents;
            radGridView2.DataSource = comunsIncomplets;

            AddOrUpdateRangoColumn(radGridView1, comunsPendents);
            AddOrUpdateRangoColumn(radGridView2, comunsIncomplets);

            if (!basculesPendents.Any() && !basculesIncompletes.Any() && !comunsPendents.Any() && !comunsIncomplets.Any())
            {
                exclamationIcon.Enabled = false;
            }
        }

        private void SetIdsVisibleFalse()
        {
            if (radGridView1.Columns.Contains("Id")) radGridView1.Columns["Id"].IsVisible = false;
            if (radGridView1.Columns.Contains("IdTasca")) radGridView1.Columns["IdTasca"].IsVisible = false;
            if (radGridView1.Columns.Contains("IdGrupTasques")) radGridView1.Columns["IdGrupTasques"].IsVisible = false;

            if (radGridView2.Columns.Contains("Id")) radGridView2.Columns["Id"].IsVisible = false;
            if (radGridView2.Columns.Contains("IdTasca")) radGridView2.Columns["IdTasca"].IsVisible = false;
            if (radGridView2.Columns.Contains("IdGrupTasques")) radGridView2.Columns["IdGrupTasques"].IsVisible = false;
        }

        private void RenameGridColumns()
        {
            var columnHeaders = new Dictionary<string, string>
            {
                { "nomtasca", "Tasca" },
                { "nomgrup", "Grup de tasques" },
                { "FetaPer", "Feta per" }
            };

            foreach (var column in columnHeaders)
            {
                if (radGridView1.Columns.Contains(column.Key))
                    radGridView1.Columns[column.Key].HeaderText = column.Value;

                if (radGridView2.Columns.Contains(column.Key))
                    radGridView2.Columns[column.Key].HeaderText = column.Value;
            }
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            ResetGridViews();

            if (e.Node.Parent != null)
            {
                string parentText = e.Node.Parent.Text;
                string nodeText = e.Node.Text;

                if (EsNodoDeBasculas(parentText, nodeText))
                {
                    CargarDatosEnGrid(basculesPendents, basculesIncompletes);
                }
                else if (EsNodoDeTareas(parentText, nodeText))
                {
                    CargarDatosEnGrid(comunsPendents, comunsIncomplets);
                }
            }
        }

        private void ResetGridViews()
        {
            radGridView1.BeginUpdate();
            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            radGridView1.DataSource = null;
            radGridView1.Columns.Clear();   
            radGridView1.EndUpdate();

            radGridView2.BeginUpdate();
            radGridView2.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            radGridView2.DataSource = null;
            radGridView2.Columns.Clear();   
            radGridView2.EndUpdate();
        }


        private bool EsNodoDeBasculas(string parentText, string nodeText)
        {
            return parentText == "Verificació de bàscules" || nodeText == "Verificació de bàscules";
        }

        private bool EsNodoDeTareas(string parentText, string nodeText)
        {
            return parentText == "Verificació de tasques" || nodeText == "Verificació de tasques";
        }

        private void CargarDatosEnGrid(object dataSource1, object dataSource2)
        {
            radGridView1.BeginUpdate();
            radGridView2.BeginUpdate();
            try
            {
                radGridView1.DataSource = dataSource1;
                radGridView2.DataSource = dataSource2;

                RenameGridColumns();
                SetIdsVisibleFalse();

                AddOrUpdateRangoColumn(radGridView1, dataSource1);
                AddOrUpdateRangoColumn(radGridView2, dataSource2);

                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                radGridView2.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

                FormatearColumnasDeFecha();
            }
            finally
            {
                radGridView1.EndUpdate();
                radGridView2.EndUpdate();
            }
        }


        private void FormatearColumnasDeFecha()
        {
            if (radGridView1.Columns["Data"] != null)
                radGridView1.Columns["Data"].FormatString = "{0:dd-MM-yyyy}";

            if (radGridView2.Columns["Data"] != null)
                radGridView2.Columns["Data"].FormatString = "{0:dd-MM-yyyy}";
        }

        private void InitContainers()
        {
            RadSplitContainer verticalSplitContainer = new RadSplitContainer();
            verticalSplitContainer.Dock = DockStyle.Fill;

            SplitPanel panel1 = new SplitPanel();
            SplitPanel panel2 = new SplitPanel();
            SplitPanel panel3 = new SplitPanel();

            panel1.Controls.Add(radTreeView1);
            panel2.Controls.Add(radGridView1);
            panel3.Controls.Add(radGridView2);

            radTreeView1.Dock = DockStyle.Fill;
            radGridView1.Dock = DockStyle.Fill;
            radGridView2.Dock = DockStyle.Fill;

            panel1.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel1.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.15f, 0.15f);

            panel2.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel2.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.85f, 0.85f);

            panel3.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel3.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.85f, 0.85f);

            RadSplitContainer horizontalSplitContainer = new RadSplitContainer();
            horizontalSplitContainer.Orientation = Orientation.Horizontal;
            horizontalSplitContainer.SplitPanels.Add(panel2);
            horizontalSplitContainer.SplitPanels.Add(panel3);

            verticalSplitContainer.SplitPanels.Add(panel1);
            verticalSplitContainer.SplitPanels.Add(horizontalSplitContainer);

            Controls.Add(verticalSplitContainer);

            radGridView1.AllowAddNewRow = false;
            radGridView1.AllowEditRow = false;
            radGridView1.AllowDeleteRow = false;
            radGridView1.AllowColumnReorder = false;
            radGridView1.AllowRowReorder = false;
            radGridView1.EnableFiltering = true;
            radGridView1.EnableGrouping = false;
            radGridView1.MultiSelect = true;
            radGridView1.TitleText = "Verificar registres";

            radGridView2.AllowAddNewRow = false;
            radGridView2.AllowEditRow = false;
            radGridView2.AllowDeleteRow = false;
            radGridView2.AllowColumnReorder = false;
            radGridView2.AllowRowReorder = false;
            radGridView2.EnableFiltering = true;
            radGridView2.EnableGrouping = false;
            radGridView2.TitleText = "Registres incomplets";
        }

        private void LoadPlans()
        {
            radTreeView1.Nodes.Clear();

            RadTreeNode verificacioBasculesNode = new RadTreeNode("Verificació de bàscules");
            RadTreeNode verificacioTasquesNode = new RadTreeNode("Verificació de tasques");

            RadTreeNode verificacioBasculesChild = new RadTreeNode("Verificació");
            RadTreeNode verificacioTasquesChild = new RadTreeNode("Verificació");

            verificacioBasculesNode.Nodes.Add(verificacioBasculesChild);
            verificacioTasquesNode.Nodes.Add(verificacioTasquesChild);

            radTreeView1.Nodes.Add(verificacioBasculesNode);
            radTreeView1.Nodes.Add(verificacioTasquesNode);

            waitingBar1.StopWaiting();
        }

        private string GetRango(string nomGrup)
        {
            if (string.IsNullOrWhiteSpace(nomGrup)) return string.Empty;
            return RANGO_POR_BASCULA.TryGetValue(nomGrup, out var rango) ? rango : string.Empty;
        }

        private void AddOrUpdateRangoColumn(RadGridView grid, object dataSource)
        {
            bool esBasculas = dataSource is IEnumerable<RegistroBascula>;

            grid.BeginUpdate();
            try
            {
                var col = grid.Columns["Rango"];
                if (esBasculas)
                {
                    if (col == null)
                    {
                        var rangoCol = new GridViewTextBoxColumn("Rango")
                        {
                            HeaderText = "Rango",
                            ReadOnly = true
                        };

                        string nomGrupKey = grid.Columns.Contains("NomGrup") ? "NomGrup" :
                                            (grid.Columns.Contains("nomgrup") ? "nomgrup" : null);

                        if (nomGrupKey != null)
                        {
                            int insertIndex = grid.Columns[nomGrupKey].Index + 1;
                            grid.Columns.Insert(insertIndex, rangoCol);
                        }
                        else
                        {
                            grid.Columns.Add(rangoCol);
                        }
                    }
                    else
                    {
                        col.IsVisible = true;
                    }

                    foreach (var row in grid.Rows.OfType<GridViewDataRowInfo>())
                    {
                        string nomGrup = Convert.ToString(row.Cells["NomGrup"]?.Value
                                          ?? row.Cells["nomgrup"]?.Value);
                        row.Cells["Rango"].Value = GetRango(nomGrup);
                    }
                }
                else
                {
                    if (col != null) col.IsVisible = false;
                }
            }
            finally
            {
                grid.EndUpdate();
            }
        }
    }
}
