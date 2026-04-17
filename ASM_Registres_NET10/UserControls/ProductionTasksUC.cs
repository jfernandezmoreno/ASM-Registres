using ASM_Registres_NET10.Modules;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls.UI;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Services;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.Modules.Registros;
using Telerik.WinControls;

namespace ASM_Registres.UserControls
{
    public partial class ProductionTasksUC : UserControl
    {
        NPGSQLService NPGSQLService { get; set; }

        RegistroLimpiezaProduccionRepository registroLimpiezaProduccionRepository;

        private SplitPanel panel1;
        private SplitPanel panel2;

        User user;

        public ProductionTasksUC(User user)
        {
            InitializeComponent();

            NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            registroLimpiezaProduccionRepository = new RegistroLimpiezaProduccionRepository(NPGSQLService);

            this.user = user;

            InitializeUCComponents();
            InitializeRadTreeView();

            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Parent == null)
            {
                return;
            }
            else if (e.Node.Parent.Text == "Registre de neteja d’equips")
            {
                RegistroLimpiezaProduccion registre = new RegistroLimpiezaProduccion();

                try
                {
                    string text = e.Node.Text;

                    if (text.StartsWith("Registre:"))
                    {
                        string[] parts = text.Split('-');
                        if (parts.Length >= 1)
                        {
                            string idPart = parts[0].Replace("Registre:", "").Trim();

                            if (int.TryParse(idPart, out int id))
                            {
                                registre = registroLimpiezaProduccionRepository.GetRegistroById(id);
                                ControlLimpiezaProduccion prod = new ControlLimpiezaProduccion(registre, user);
                                radPanel1.Controls.Clear();
                                radPanel1.Controls.Add(prod);
                                prod.Dock = DockStyle.Fill;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RadMessageBox.Show($"Error en obtenir el registre: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                }
            }
        }

        private void InitializeRadTreeView()
        {
            var registres = registroLimpiezaProduccionRepository
                .GetAllRegistros()
                .OrderByDescending(r => r.Fecha)
                .ToList();

            RadTreeNode rootNode = new RadTreeNode("Registre de neteja d’equips")
            {
                Expanded = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            foreach (var registre in registres)
            {
                string dataFormatejada = registre.Fecha.ToString("dd/MM/yyyy HH:mm");
                string textNode = $"Registre: {registre.Id} - {registre.Tipo} - {registre.Operario} - {dataFormatejada}";

                RadTreeNode childNode = new RadTreeNode(textNode)
                {
                    Tag = registre,
                    ForeColor = registre.Finalizada ? Color.Green : Color.Blue
                };

                rootNode.Nodes.Add(childNode);
            }

            radTreeView1.Nodes.Clear();
            radTreeView1.Nodes.Add(rootNode);
        }

        public void RecargarArbol()
        {
            InitializeRadTreeView();
        }

        private void InitializeUCComponents()
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
                Controls = { radPanel1 }
            };

            radTreeView1.Dock = DockStyle.Fill;
            radPanel1.Dock = DockStyle.Fill;

            verticalSplitContainer.SplitPanels.Add(panel1);
            verticalSplitContainer.SplitPanels.Add(panel2);

            Controls.Add(verticalSplitContainer);
        }

        private void radButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
