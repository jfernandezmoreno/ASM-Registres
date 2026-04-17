using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;

namespace ASM_Registres.UserControls
{
    public partial class CheckPlansUC : UserControl
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        SplitPanel panel1 = new SplitPanel();
        SplitPanel panel2 = new SplitPanel();

        GrupoTareasRepository grupoTareasRepository;
        PlanRepository planRepository;
        BasculaGrandeRepository basculaGrandeRepository;
        BasculaPequenaRepository basculaPequenaRepository;
        CarretillasRepository carretillasRepository;
        CompresorRepository compresorRepository;
        EstadoInstalacionRepository estadoInstalacionRepository;
        PNDRepository PNDRepository;
        RegistroGeneralMantenimientoRespository registroGeneralMantenimientoRespository;
        VerificacionRegistrosRepository verificacionRegistrosRepository;
        TareasMantenimientoRepository tareasMantenimientoRepository;

        public CheckPlansUC()
        {
            InitializeComponent();
            SetupConnections();
            InitContainers();
            LoadPlans();

            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
            radPdfViewer1.Visible = false;
        }

        private void SetupConnections()
        {
            planRepository = new PlanRepository(NPGSQLService);
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            basculaGrandeRepository = new BasculaGrandeRepository(NPGSQLService);
            basculaPequenaRepository = new BasculaPequenaRepository(NPGSQLService);
            carretillasRepository = new CarretillasRepository(NPGSQLService);
            compresorRepository = new CompresorRepository(NPGSQLService);
            estadoInstalacionRepository = new EstadoInstalacionRepository(NPGSQLService);
            PNDRepository = new PNDRepository(NPGSQLService);
            registroGeneralMantenimientoRespository = new RegistroGeneralMantenimientoRespository(NPGSQLService);
            verificacionRegistrosRepository = new VerificacionRegistrosRepository(NPGSQLService);
            tareasMantenimientoRepository = new TareasMantenimientoRepository(NPGSQLService);
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Tag is GrupTasques grup)
            {
                panel2.Controls.Clear();

                switch (grup.Nom)
                {
                    case "Control de bàscules grans (1000 kg)":
                        CargarDatosEnGrilla(basculaGrandeRepository.SelectCBascGranKoh(), "Equip");
                        break;

                    case "Control de bàscules petites (25 kg)":
                        CargarDatosEnGrilla(basculaPequenaRepository.SelectCBascPetites(), "NomEquip");
                        break;

                    case "Documentació Pla de Manteniment":
                        CargarPdf(@"C:\Users\lluc\Desktop\plaManteniment.pdf");
                        break;

                    case "Tasques Empresa Externa":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(1), "Zona");
                        break;

                    case "Tasques Empresa Externa Segons Necesitat":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(2), "Zona");
                        break;

                    case "Tasques Producció":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(3), "Zona");
                        break;

                    case "Tasques Producció S/N (Depòsit)":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(4), "Zona");
                        break;

                    case "Tasques Producció S/N (Magatzem/Moll)":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(5), "Zona");
                        break;

                    case "Tasques Producció Segons Necessitat":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(6), "Zona");
                        break;

                    case "Tasques Laboratori":
                        CargarDatosEnGrillaAgrupada(PNDRepository.SelectPndTasques(7), "Zona");
                        break;

                    case "Verificació de Registres":
                        CargarDatosEnGrilla(verificacionRegistrosRepository.GetVRegistresByGrup(), null);
                        break;

                    case "Tasques Manteniment":
                        CargarDatosEnGrilla(tareasMantenimientoRepository.GetAllTManteniment(), null, ocultarColumnaId: true);
                        break;

                    default:
                        if (grup.Nom.Contains("Carretilla"))
                            CargarDatosEnGrilla(carretillasRepository.SelectRsCarretilles(), "Equip");
                        else if (grup.Nom.Contains("Compressor"))
                            CargarDatosEnGrilla(compresorRepository.SelectRsCompresor(), "Equip");
                        else if (grup.Nom.Contains("estat insta"))
                            CargarDatosEnGrilla(estadoInstalacionRepository.SelectREstatInstalacio(), null);
                        else if (grup.Nom.Contains("Registre general"))
                            CargarDatosEnGrilla(registroGeneralMantenimientoRespository.GetRGeneralMantenimentByGrup(), null);
                        break;
                }

                RenameGridColumns();
                SetIdsVisibleFalse();
                radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void CargarDatosEnGrilla<T>(List<T> datos, string columnaAgrupar, bool ocultarColumnaId = false)
        {
            radGridView1.DataSource = datos;
            ConfigurarRadGridView(columnaAgrupar, ocultarColumnaId);
            MostrarRadGridView();
        }

        private void CargarDatosEnGrillaAgrupada<T>(List<T> datos, string columnaAgrupar)
        {
            CargarDatosEnGrilla(datos, columnaAgrupar);
        }

        private void ConfigurarRadGridView(string columnaAgrupar, bool ocultarColumnaId)
        {
            radGridView1.GroupDescriptors.Clear();
            if (!string.IsNullOrEmpty(columnaAgrupar))
            {
                GroupDescriptor descriptor = new GroupDescriptor();
                descriptor.GroupNames.Add(columnaAgrupar, ListSortDirection.Ascending);
                radGridView1.GroupDescriptors.Add(descriptor);
                radGridView1.MasterTemplate.ExpandAllGroups();
            }

            if (ocultarColumnaId && radGridView1.Columns["Id"] != null)
                radGridView1.Columns["Id"].IsVisible = false;
        }

        private void MostrarRadGridView()
        {
            panel2.Controls.Add(radGridView1);
            radGridView1.Dock = DockStyle.Fill;
        }


        private void CargarPdf(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    radPdfViewer1.LoadDocument(filePath);
                }
                else
                {
                    RadMessageBox.Show("El archivo PDF no existe en la ruta especificada.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Ocurrió un error al cargar el PDF: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }


        private void SetIdsVisibleFalse()
        {
            string[] columnasOcultar = { "Id", "GrupTasquesId" };

            foreach (string columna in columnasOcultar)
            {
                if (radGridView1.Columns[columna] != null)
                {
                    radGridView1.Columns[columna].IsVisible = false;
                }
            }
        }


        private void RenameGridColumns()
        {
            var columnHeaders = new Dictionary<string, string>
            {
                { "ValorMin", "Valor Mínim" },
                { "ValorMax", "Valor Màxim" },
                { "CriterisAcceptacio", "Criteris d'Acceptació" },
                { "CaracteristiquesControl", "Caracteristiques Control" },
                { "TipusEquip", "Tipus d'Equip" },
                { "NomEquip", "Nom Equip" },
                { "Frequencia", "Freqüencia" },
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

        private void InitContainers()
        {
            RadSplitContainer verticalSplitContainer = new RadSplitContainer();
            verticalSplitContainer.Dock = DockStyle.Fill;

            panel1.Controls.Add(radTreeView1);
            panel2.Controls.Add(radGridView1);

            radTreeView1.Dock = DockStyle.Fill;
            radGridView1.Dock = DockStyle.Fill;

            panel1.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel1.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.15f, 0.15f); // Panel 1 ocupa el 20% del ancho y altura

            panel2.SizeInfo.SizeMode = SplitPanelSizeMode.Relative;
            panel2.SizeInfo.RelativeRatio = new System.Drawing.SizeF(0.85f, 0.85f); // Panel 2 ocupa el 80% del ancho y altura

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

        private void LoadPlans()
        {
            List<Pla> plans = planRepository.GetAllPlans();
            List<GrupTasques> grups = grupoTareasRepository.GetAllGrups();
            grups.RemoveAll(grup => grup.Id == 18);
            grups.RemoveAll(grup => grup.Id == 17);
            grups.RemoveAll(grup => grup.Id == 19);
            grups.RemoveAll(grup => grup.Id == 20);
            radTreeView1.Nodes.Clear();

            foreach (var plan in plans)
            {
                RadTreeNode planNode = new RadTreeNode(plan.Nom);
                planNode.Tag = plan;

                foreach (var grup in grups.Where(g => g.PlanId == plan.Id))
                {
                    RadTreeNode grupNode = new RadTreeNode(grup.Nom);
                    grupNode.Tag = grup;
                    planNode.Nodes.Add(grupNode);
                }

                radTreeView1.Nodes.Add(planNode);
            }
        }

        private void radPdfViewer1_Click(object sender, EventArgs e)
        {

        }

        private void radTreeView1_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {

        }
    }
}