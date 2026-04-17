using ASM_Registres.Forms;
using ASM_Registres.UserControls;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

using Res = ASM_Registres_NET10.Properties.Resources;

namespace ASM_Registres.Forms
{
    public partial class MainForm : RadRibbonForm
    {
        private RadLabelElement exclamationIcon;
        private RadLabelElement incidenciasIcon;

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        private User user;

        private RegistroComunRepository registreComuRepository;
        private RegistroBasculaRepository registroBasculasRepository;
        private IncidenciasRepository incidenciasRepository;

        RibbonTab ribbonTab;

        public MainForm(User user)
        {
            InitializeComponent();
            InitializeVariables(user);
            InitializeFormProperties();
            InitializeIcons();
            InitializeWaitingBar();
            SetupPageViewer();
            InitializeStatusLabel();
            InitializeMenu();
            SetLoginStatus(true);
        }

        private void InitializeVariables(User user)
        {
            this.user = user;
            registreComuRepository = new RegistroComunRepository(NPGSQLService);
            registroBasculasRepository = new RegistroBasculaRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);

            var alert = new RadDesktopAlert
            {
                CaptionText = "Benvingut de nou, " + user.Name
            };
            alert.Show();
        }

        private void InitializeFormProperties()
        {
            Text = "Registres";

            // Si "Logo_a_" es un Icon en Resources, lo usa directamente; si es Bitmap, lo convierte.
            Icon = EnsureIconFromResource(Res.Logo_a_);

            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            AllowAero = false;
        }

        private void InitializeIcons()
        {
            exclamationIcon = CreateIconElement("!", 26, new Padding(47, -30, 0, 0));
            int numeroIncidencias = incidenciasRepository.GetNumeroIncidenciasNoResueltas();
            incidenciasIcon = CreateIconElement(numeroIncidencias.ToString(), 20, new Padding(40, -40, 0, 0));
        }

        private RadLabelElement CreateIconElement(string text, int fontSize, Padding margin)
        {
            return new RadLabelElement
            {
                Text = text,
                ForeColor = Color.Red,
                Font = new Font("Arial", fontSize, FontStyle.Bold),
                Alignment = ContentAlignment.TopRight,
                AutoSize = true,
                ZIndex = 1000,
                Margin = margin
            };
        }

        private void InitializeWaitingBar()
        {
            radWaitingBar1.AssociatedControl = this;
            radWaitingBar1.WaitingIndicatorSize = new Size(100, 14);
            radWaitingBar1.WaitingSpeed = 50;
            radWaitingBar1.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;
            radWaitingBar1.StartWaiting();
        }

        private void SetupPageViewer()
        {
            showPanel.Dock = DockStyle.Fill;
            showPanel.Controls.Add(pageViewer);
            pageViewer.Dock = DockStyle.Fill;
            pageViewer.Controls.Clear();
        }

        private void InitializeStatusLabel()
        {
            statusLabel.ForeColor = Color.DarkBlue;
            radLabelElement1.Text = "Benvingut/Benvinguda: " + user.Name;
        }

        private void InitializeMenu()
        {
            radRibbonBar.RootElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            radRibbonBar.Dock = DockStyle.Top;

            // Imagen del botón inicio
            radRibbonBar.StartButtonImage = ResizeImage(Res.fotito, 73, 29);

            RibbonTab registrosTab = CreateRibbonTab("Registres");
            radRibbonBar.CommandTabs.Add(registrosTab);

            RibbonTab tareasTab = CreateRibbonTab("Tasques");
            radRibbonBar.CommandTabs.Add(tareasTab);

            ribbonTab = registrosTab;

            RadRibbonBarGroup groupTasques = CreateRibbonGroup("Registres");
            RadRibbonBarGroup groupPlans = CreateRibbonGroup("Plans");
            RadRibbonBarGroup groupIncidencies = CreateRibbonGroup("Incidències");
            RadRibbonBarGroup groupConfigure = CreateRibbonGroup("Inici/Sortida Sessió");
            RadRibbonBarGroup groupRefresh = CreateRibbonGroup("Dades");
            RadRibbonBarGroup groupUser = CreateRibbonGroup("Usuari");

            registrosTab.Items.Add(groupTasques);
            registrosTab.Items.Add(groupPlans);
            registrosTab.Items.Add(groupIncidencies);
            registrosTab.Items.Add(groupConfigure);
            registrosTab.Items.Add(groupRefresh);

            AddButtonsToGroup(groupTasques, new (string, Image, EventHandler)[]
            {
                ("Avui", Res.DP_305__16__Photoroom, tareasHoyButtonClick),
                ("Esporàdiques", Res.d_Photoroom, tareasEsporadicasButtonClick),
                ("Laboratori", Res.labIcon, tareasLaboratorioButtonClick),
                ("Manteniment", Res.DP_305__21__Photoroom, tareasMantenimientoButtonClick),
                ("Producció", Res.prodprod, tareasProd)
            });

            AddButtonsToGroup(groupPlans, new (string, Image, EventHandler)[]
            {
                ("Consultar Plans", Res.jtareitas, consultaPlanesButtonClick)
            });

            AddButtonsToGroup(groupIncidencies, new (string, Image, EventHandler)[]
            {
                ("Incidències", Res.lol, incidenciasButtonClick)
            }, incidenciasIcon);

            CheckAdmin(registrosTab);

            AddButtonsToGroup(groupConfigure, new (string, Image, EventHandler)[]
            {
                ("Log In", Res.DP_305__19__Photoroom, loginButtonClick),
                ("Log Out", Res.DP_305__18__Photoroom, logoutButtonClick)
            });

            AddButtonsToGroup(groupRefresh, new (string, Image, EventHandler)[]
            {
                ("Actualitzar", Res.DP_305__20__Photoroom, refreshButtonClick)
            });

            RadRibbonBarGroup groupTareasNueva = CreateRibbonGroup("Tasques");
            tareasTab.Items.Add(groupTareasNueva);
            AddButtonsToGroup(groupTareasNueva, new (string, Image, EventHandler)[]
            {
                ("Producció", Res.prodprod, tareasProdRecurrentButtonClick),
                ("Manteniment", Res.DP_305__21__Photoroom, tareasMantRecurrentButtonClick),
                ("Laboratori", Res.labIcon, tareasLabRecurrentButtonClick)
            });

            RadRibbonBarGroup groupPdf = CreateRibbonGroup("PDF");
            tareasTab.Items.Add(groupPdf);
            AddButtonsToGroup(groupPdf, new (string, Image, EventHandler)[]
            {
                ("Generar PDF", Res.DP_305__10__Photoroom, pdfButtonClick)
            });

            RadRibbonBarGroup groupTurnos = CreateRibbonGroup("Torns");
            tareasTab.Items.Add(groupTurnos);
            AddButtonsToGroup(groupTurnos, new (string, Image, EventHandler)[]
            {
                ("Torns", Res.DP_305__9__Photoroom, turnosButtonClick)
            });

            radWaitingBar1.StopWaiting();
        }

        private void pdfButtonClick(object sender, EventArgs e)
        {
            ExportTasks export = new ExportTasks();
            export.ShowDialog();
        }

        private void turnosButtonClick(object sender, EventArgs e)
        {
            if (user.nivel >= 6)
            {
                OpenPage("Torns", () => new ShiftsManager());
            }
            else
            {
                RadMessageBox.Show("No tens permisos per realitzar aquesta acció.", "Accés denegat", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void CheckAdmin(RibbonTab registrosTab)
        {
            if (user.IsAdmin())
            {
                CreateAdminGroup(registrosTab);
            }
        }

        private void CreateAdminGroup(RibbonTab registrosTab)
        {
            RadRibbonBarGroup groupAdmin = CreateRibbonGroup("Eines");
            RadImageButtonElement adminButton = CreateImageButtonElement("Administrador", Res.DP_305__17__Photoroom, adminButtonClick, exclamationIcon);
            RadImageButtonElement estadistiquesButton = CreateImageButtonElement("Estadistica", Res.estadisticaFoto, estadisticaButtonClick);
            exclamationIcon.Enabled = ExistenTareasPendientesOIncompletas();

            registrosTab.Items.Add(groupAdmin);
            groupAdmin.Items.Add(adminButton);
            groupAdmin.Items.Add(estadistiquesButton);
        }

        private RibbonTab CreateRibbonTab(string text)
        {
            return new RibbonTab(text);
        }

        private RadRibbonBarGroup CreateRibbonGroup(string text)
        {
            return new RadRibbonBarGroup
            {
                Text = text,
                TextOrientation = Orientation.Horizontal,
                AutoSize = true
            };
        }

        private void AddButtonsToGroup(RadRibbonBarGroup group, (string text, Image image, EventHandler clickHandler)[] buttons, RadElement additionalElement = null)
        {
            foreach (var (text, image, clickHandler) in buttons)
            {
                RadImageButtonElement button = CreateImageButtonElement(text, image, clickHandler, additionalElement);
                group.Items.Add(button);
            }
        }

        private RadImageButtonElement CreateImageButtonElement(string text, Image image, EventHandler clickHandler, RadElement additionalElement = null)
        {
            RadImageButtonElement button = new RadImageButtonElement
            {
                ShowBorder = true,
                BackColor = Color.White,
                EnableElementShadow = true,
                ShadowColor = Color.White,
                Image = ResizeImage(image, 45, 45),
                Text = text,
                DisplayStyle = Telerik.WinControls.DisplayStyle.ImageAndText,
                TextImageRelation = TextImageRelation.ImageAboveText,
                ImageAlignment = ContentAlignment.MiddleCenter,
                MinSize = new Size(70, 70),
                MaxSize = new Size(70, 70)
            };

            button.Click += clickHandler;

            if (additionalElement != null)
            {
                button.Children.Add(additionalElement);
            }

            return button;
        }

        private void tareasHoyButtonClick(object sender, EventArgs e)
        {
            OpenPage("Registres avui", () => new tareasHoy(radWaitingBar1, user, incidenciasIcon));
        }

        private void tareasProd(object sender, EventArgs e)
        {
            OpenPage("Registres Producció", () => new ProductionTasksUC(user));
        }

        private void tareasEsporadicasButtonClick(object sender, EventArgs e)
        {
            OpenPage("Registres Esporàdics", () => new SporadicTasksUC(radWaitingBar1, user, incidenciasIcon));
        }

        private void tareasLaboratorioButtonClick(object sender, EventArgs e)
        {
            OpenPage("Registres Laboratori", () => new LaboratoryTasksUC(user, incidenciasIcon));
        }

        private void tareasMantenimientoButtonClick(object sender, EventArgs e)
        {
            OpenPage("Registres Manteniment", () => new MaintenanceTasksUC(user));
        }

        private void consultaPlanesButtonClick(object sender, EventArgs e)
        {
            OpenPage("Consultar Plans", () => new CheckPlansUC());
        }

        private void incidenciasButtonClick(object sender, EventArgs e)
        {
            OpenPage("Incidències", () => new CheckIncidentsUC(radWaitingBar1, user, incidenciasIcon));
        }

        private void adminButtonClick(object sender, EventArgs e)
        {
            OpenPage("Administrador", () => new AdministratorUC(user, radWaitingBar1, exclamationIcon));
        }

        private void estadisticaButtonClick(object sender, EventArgs e)
        {
            OpenPage("Estadistica", () => new EstadisticaUc(user));
        }

        private void userButtonClick(object sender, EventArgs e)
        {
            OpenPage("Administrar", () => new CheckPlansUC());
        }

        private void tareasProdRecurrentButtonClick(object sender, EventArgs e)
        {
            OpenPage("Tasques Producció", () => new ProdTasksRecurrent(user));
        }

        private void tareasLabRecurrentButtonClick(object sender, EventArgs e)
        {
            OpenPage("Tasques Laboratori", () => new LabTasksRecurrentUC(user));
        }

        private void tareasMantRecurrentButtonClick(object sender, EventArgs e)
        {
            OpenPage("Tasques Manteniment", () => new MantTasksRecurrent(user));
        }

        private void loginButtonClick(object sender, EventArgs e)
        {
            ReLogIn reLogIn = new ReLogIn();

            if (reLogIn.ShowDialog() == DialogResult.OK)
            {
                user = reLogIn.LoggedInUser;

                CheckAdmin(ribbonTab);
                SetLoginStatus(true);

                radLabelElement1.Text = "Benvingut/Benvinguda: " + user.Name;
            }
        }

        private void logoutButtonClick(object sender, EventArgs e)
        {
            RemoveAdminGroup();
            DisableViewer();
            SetLoginStatus(false);

            RadMessageBox.Show("'ha tancat la sessió.", "Logout", MessageBoxButtons.OK, RadMessageIcon.Error);
        }

        private void DisableViewer()
        {
            user = null;
            pageViewer.Pages.Clear();
            radLabelElement1.Text = "Status: Logged Out";
            showPanel.Enabled = false;
        }

        private void refreshButtonClick(object sender, EventArgs e)
        {
            pageViewer.Pages.Clear();

            if (user.IsAdmin())
            {
                exclamationIcon.Enabled = ExistenTareasPendientesOIncompletas();
            }

            incidenciasIcon.Text = incidenciasRepository.GetNumeroIncidenciasNoResueltas().ToString();
            RadMessageBox.Show("S'han actualitzat les dades de la aplicació", "Actualització", MessageBoxButtons.OK, RadMessageIcon.Info);
        }

        private void SetLoginStatus(bool loggedIn)
        {
            showPanel.Enabled = loggedIn;

            var tasquesTab = radRibbonBar.CommandTabs
                .OfType<RibbonTab>()
                .FirstOrDefault(t => t.Text == "Tasques");

            if (tasquesTab != null)
            {
                tasquesTab.Enabled = loggedIn;
            }

            foreach (RadRibbonBarGroup group in ribbonTab.Items)
            {
                foreach (RadItem item in group.Items)
                {
                    if (item is RadImageButtonElement button && button.Text == "Log In")
                    {
                        button.Enabled = !loggedIn;
                    }
                    else
                    {
                        item.Enabled = loggedIn;
                    }
                }
            }
        }

        private void OpenPage(string pageName, Func<UserControl> createUserControl)
        {
            RadPageViewPage existingPage = pageViewer.Pages.FirstOrDefault(p => p.Name == pageName);

            if (existingPage == null)
            {
                RadPageViewPage radPageViewPage = new RadPageViewPage
                {
                    Name = pageName,
                    Dock = DockStyle.Fill
                };

                UserControl control = createUserControl();
                control.Dock = DockStyle.Fill;
                radPageViewPage.Controls.Add(control);
                pageViewer.Pages.Add(radPageViewPage);
                existingPage = radPageViewPage;
            }
            pageViewer.SelectedPage = existingPage;
        }

        private Image ResizeImage(Image image, int width, int height)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public bool ExistenTareasPendientesOIncompletas()
        {
            return registreComuRepository.GetRegistrosPendent().Any() ||
                   registreComuRepository.GetRegistrosIncomplet().Any() ||
                   registroBasculasRepository.GetRegistrosBasculaPendent().Any() ||
                   registroBasculasRepository.GetRegistrosBasculaIncomplet().Any();
        }

        private void RemoveAdminGroup()
        {
            if (ribbonTab != null)
            {
                RadRibbonBarGroup existingAdminGroup = ribbonTab.Items.OfType<RadRibbonBarGroup>().FirstOrDefault(group => group.Text == "Eines");
                if (existingAdminGroup != null)
                {
                    ribbonTab.Items.Remove(existingAdminGroup);
                }
            }
        }

        private void radRibbonBar_Click(object sender, EventArgs e)
        {
        }

        // ----- Helpers para asegurar Icon válido y evitar memory leaks -----

        // Si el recurso ya es Icon, lo devuelve sin transformación.
        private static Icon EnsureIconFromResource(Icon iconResource) => iconResource;

        // Si el recurso es Bitmap (PNG/JPG), lo convierte a Icon.
        private static Icon EnsureIconFromResource(Bitmap bitmapResource)
        {
            if (bitmapResource == null) throw new ArgumentNullException(nameof(bitmapResource));
            IntPtr hIcon = bitmapResource.GetHicon();
            try
            {
                using (var icon = Icon.FromHandle(hIcon))
                {
                    // Clonamos para poder destruir el handle sin invalidar el Icon
                    return (Icon)icon.Clone();
                }
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
    }
}
