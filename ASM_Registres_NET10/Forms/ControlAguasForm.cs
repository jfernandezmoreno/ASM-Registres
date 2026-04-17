using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class ControlAguasForm : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        ControlAguasRepository connection;
        IncidenciasRepository incidenciasRepository;

        User user;

        float controlAguasValue, controlLibreValue, nitritosValue, phValue;

        public ControlAguasForm(User user)
        {
            InitializeComponent();
            SetupWindow();
            SetupVariables(user);
            SetupConnections();
        }

        private void SetupConnections()
        {
            connection = new ControlAguasRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
        }

        private void SetupVariables(User user)
        {
            this.user = user;
            olorCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            turbiCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private string ConvertirNumeroConComa(string valor)
        {
            return valor.Replace('.', ',');
        }
        private async void radButton1_Click(object sender, EventArgs e)
        {
            string controlAguas = ConvertirNumeroConComa(controlAguasTextBox.Text);
            string controlLlibre = ConvertirNumeroConComa(controlLibreTextBox.Text);
            string nitritos = ConvertirNumeroConComa(nitritosTextBox.Text);
            string ph = ConvertirNumeroConComa(phTextBox.Text);

            /* ¡Vamos bien! Solo queda un pequeño detalle, cuado intentamos hacer el float.TryParse más tarde (en los 4 IF's), debemos mirar el valor que sale de ConvertirNumeroConComa, no el del .Text, a la que acabes con eso, ya podremos actualizar el programa*/

            /* ESTE SE TIENE QUE COMPROBAR */

            if (!float.TryParse(controlAguas, out controlAguasValue) || controlAguasValue < 0.1f || controlAguasValue > 2.0f)
            {
                var result = RadMessageBox.Show(
                    "El valor de clor combinat ha de ser entre 0,1 i 2,0.\nN'ets conscient? Vols crear una incidència al respecte?",
                    "Error de validació",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);
                if (result == DialogResult.Yes)
                {
                    await CrearIncidencia("Clor combinat", "El valor introduït és fora de rang (0,1–2,0): " + controlAguasValue);
                }

            }

            if (!float.TryParse(controlLlibre, out controlLibreValue) || controlLibreValue < 0.1f || controlLibreValue > 1.0f)
            {
                var result = RadMessageBox.Show(
                    "El valor de clor lliure ha de ser entre 0,1 i 1,0.\nN'ets conscient? Vols crear una incidència al respecte?",
                    "Error de validació",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);
                if (result == DialogResult.Yes)
                {
                    await CrearIncidencia("Clor lliure", "El valor introduït és fora de rang (0,1–1,0): " + controlLibreValue);
                }

            }

            if (!float.TryParse(nitritos, out nitritosValue) || nitritosValue >= 0.5f)
            {
                var result = RadMessageBox.Show(
                    "El valor de nitrits ha de ser inferior a 0,5.\nN'ets conscient? Vols crear una incidència al respecte?",
                    "Error de validació",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Question);
                if (result == DialogResult.Yes)
                {
                    await CrearIncidencia("Nitrits", "El valor introduït és ≥ 0,5");
                }

            }

            if (!float.TryParse(ph, out phValue) || phValue < 6.5f || phValue > 9.5f)
            {
                var result = RadMessageBox.Show(
                    "El pH ha d'estar entre 6,5 i 9,5.\nN'ets conscient? Vols crear una incidència al respecte?",
                    "Error de validació",
                    MessageBoxButtons.YesNo,
                    RadMessageIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    await CrearIncidencia("pH", "El valor introduït és fora del rang permès (6,5–9,5)");
                }

            }

            if (string.IsNullOrWhiteSpace(durezaTextBox.Text))
            {
                RadMessageBox.Show("El camp «Duresa» no pot estar buit", "Error de validació", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(olorCombo.Text))
            {
                RadMessageBox.Show("El camp «Olor» no pot estar buit", "Error de validació", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(turbiCombo.Text))
            {
                RadMessageBox.Show("El camp «Turbidesa» no pot estar buit", "Error de validació", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            if (string.IsNullOrWhiteSpace(dqsTextbox.Text))
            {
                RadMessageBox.Show("El camp «DQS» no pot estar buit", "Error de validació", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                return;
            }

            ControlAguas control = CreateControlAguas();
            
            try
            {
                connection.AddControlAguas(control);
            }
            catch (Exception ex) 
            {
                RadMessageBox.Show("Error en inserir el registre d’aigües: " + ex.Message + " Contacta amb l’administrador.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
            }

            RadMessageBox.Show("Totes les dades són vàlides.", "Validació correcta", MessageBoxButtons.OK, RadMessageIcon.Info);

            Close();
        }

        private ControlAguas CreateControlAguas()
        {
            var control = new ControlAguas();

            control.Fecha = DateTime.Now;
            control.ControlAguasValue = controlAguasValue;
            control.CloroLibre = controlLibreValue;
            control.Nitritos = nitritosValue;
            control.Ph = phValue;
            control.Dureza = durezaTextBox.Text;
            control.Olor = olorCombo.Text;
            control.Turbidez = turbiCombo.Text;
            control.Operario = user.Name;
            control.DQS = dqsTextbox.Text;
            control.Observaciones = comentariosTextbox.Text;

            return control; 
        }

        private async Task CrearIncidencia(string campo, string descripcion)
        {
            Incidencia incidencia = new Incidencia();
            incidencia.IdTasca = 1;
            incidencia.IdGrupTasques = 20;
            incidencia.NomTasca = "Control d’aigües (laboratori)";
            incidencia.NomGrup = campo;
            incidencia.Data = DateTime.Now;
            incidencia.DescripcioIncidencia = descripcion;
            incidencia.Resolta = false;
            incidencia.Solucio = "";

            await incidenciasRepository.AddIncidencia(incidencia);

            RadMessageBox.Show($"Incidència creada per al camp: {campo}\nDescripció: {descripcion}", "Incidència creada", MessageBoxButtons.OK, RadMessageIcon.Info);
            Close();
        }
    }
}
