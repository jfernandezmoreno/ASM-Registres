using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Globalization;


namespace ASM_Registres.Forms
{
    public partial class GestionarIncidenciesForm : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        Incidencia incidencia;
        IncidenciasRepository incidenciasRepository;
        RegistroComunRepository registroComunRepository;
        RegistroBasculaRepository registroBasculaRepository;
        TareasRepository tareasRepository;
        User user;

        public GestionarIncidenciesForm(Incidencia incidencia, User u)
        {
            InitializeComponent();
            SetupWindow();
            CompletarCampos(incidencia);

            user = u;
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
            registroComunRepository = new RegistroComunRepository(NPGSQLService);
            registroBasculaRepository = new RegistroBasculaRepository(NPGSQLService);
            tareasRepository = new TareasRepository(NPGSQLService);
        }

        private void CompletarCampos(Incidencia nuestraIncidencia)
        {
            incidencia = nuestraIncidencia;

            if (incidencia != null)
            {
                dateIncidencia.Value = incidencia.Data;
                tascaTextBox.Text = incidencia.NomTasca;
                gruptextBox.Text = incidencia.NomGrup;
                descInTextBox.Text = incidencia.DescripcioIncidencia;

                if (incidencia.esResolta()){ complCheck.Checked = true; solucioTextBox.IsReadOnly = false; }

                else { complCheck.Checked = false; solucioTextBox.IsReadOnly = true; }


                solucioTextBox.Text = incidencia.Solucio;
                dateIncidencia.ReadOnly = true;
                tascaTextBox.IsReadOnly = true;
                gruptextBox.IsReadOnly = true;
                descInTextBox.IsReadOnly = true;
            }
        }

        private void complCheck_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (complCheck.Checked)
            {
                solucioTextBox.IsReadOnly = false;
            }
            else 
            { 
                solucioTextBox.Text = ""; 
                solucioTextBox.IsReadOnly = true; 
            }
         }

        private void GestionarIncidenciesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = RadMessageBox.Show(
                "Advertència: els canvis no desats es perdran. Vols continuar?",
                "Confirma el tancament",
                MessageBoxButtons.YesNo,
                RadMessageIcon.Exclamation);

            if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }
            
        }
        private void radButton1_Click(object sender, EventArgs e)
        {
            if (complCheck.Checked && string.IsNullOrWhiteSpace(solucioTextBox.Text))
            {
                RadMessageBox.Show(
                    "La solució no pot estar buida. Si us plau, introdueix una solució abans de continuar.",
                    "Error",
                    MessageBoxButtons.OK,
                    RadMessageIcon.Exclamation);
                return;
            }

            bool estavaResoltaAbans = incidencia.Resolta;

            incidencia.Resolta = complCheck.Checked;
            incidencia.Solucio = solucioTextBox.Text;

            incidenciasRepository.UpdateIncidencia(incidencia);

            if (incidencia.Resolta && !estavaResoltaAbans)
            {
                CrearRegistrePerIncidencia();
            }

            RadMessageBox.Show(
                "Incidència actualitzada correctament.",
                "Correcte",
                MessageBoxButtons.OK,
                RadMessageIcon.Info);
        }

        private void CrearRegistrePerIncidencia()
        {
            if (incidencia == null) return;

            DateTime avui = DateTime.Today;

            string observacions =
                $"Tasca realitzada amb una incidència (ID incidència: {incidencia.Id}), però resolta.";

            if (incidencia.IdGrupTasques == 9 || incidencia.IdGrupTasques == 10)
            {
                double valorBascula = ObtenerValorBasculaDesdeIncidencia(incidencia);

                var registreBascula = new RegistroBascula
                {
                    IdTasca = incidencia.IdTasca,
                    IdGrupTasques = incidencia.IdGrupTasques,
                    NomTasca = incidencia.NomTasca,
                    NomGrup = incidencia.NomGrup,
                    Data = avui,
                    Valor = valorBascula,
                    Estat = "Pendent",
                    Observacions = observacions,
                    FetaPer = user.Name
                };

                registroBasculaRepository.AddRegistreBascula(registreBascula);
            }

            else if (incidencia.IdGrupTasques != 20 && incidencia.IdGrupTasques != 9 && incidencia.IdGrupTasques != 10)
            {
                var registreComun = new RegistroComun
                {
                    IdTasca = incidencia.IdTasca,
                    IdGrupTasques = incidencia.IdGrupTasques,
                    NomTasca = incidencia.NomTasca,
                    NomGrup = incidencia.NomGrup,
                    Data = avui,
                    Estat = "Pendent",
                    Observacions = observacions,
                    FetaPer = user.Name
                };

                registroComunRepository.InsertRegistroComun(registreComun);
            }

            tareasRepository.UpdateTasquesDarrera(incidencia.IdTasca, avui);
        }

        private double ObtenerValorBasculaDesdeIncidencia(Incidencia incidencia)
        {
            if (incidencia == null || string.IsNullOrWhiteSpace(incidencia.DescripcioIncidencia))
                return 0d;

            string text = incidencia.DescripcioIncidencia;

            string marcador1 = "El valor de la bàscula és:";
            string marcador2 = "El valor de la bàscula es:";

            int idx = text.IndexOf(marcador1, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                idx = text.IndexOf(marcador2, StringComparison.OrdinalIgnoreCase);

            if (idx < 0)
                return 0d;

            idx = text.IndexOf(':', idx);
            if (idx < 0 || idx + 1 >= text.Length)
                return 0d;

            string resto = text.Substring(idx + 1).Trim();

            int newlineIdx = resto.IndexOfAny(new[] { '\r', '\n' });
            if (newlineIdx >= 0)
                resto = resto.Substring(0, newlineIdx).Trim();

            if (double.TryParse(resto, NumberStyles.Any, CultureInfo.GetCultureInfo("es-ES"), out double valor))
                return valor;

            resto = resto.Replace(',', '.');
            if (double.TryParse(resto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                return valor;

            return 0d;
        }

        private void GestionarIncidenciesForm_Load(object sender, EventArgs e)
        {

        }
    }
}
