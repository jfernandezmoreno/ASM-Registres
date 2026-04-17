using ASM_Registres_NET10.Modules;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using ASM_Registres_NET10.Modules.Registros;
using Telerik.WinControls.UI;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Services;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using System.Threading.Tasks;

namespace ASM_Registres.Forms
{
    public partial class RegistroBasculaForm : Telerik.WinControls.UI.RadForm
    {
        Tasques tasca;

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        GrupoTareasRepository grupoTareasRepository;
        TareasRepository tareasRepository;
        RegistroBasculaRepository registroBasculaRepository;
        IncidenciasRepository incidenciasRepository;

        RadLabelElement numero;
        User user;

        public RegistroBasculaForm(Tasques tasca, User user, RadLabelElement numero)
        {
            InitializeComponent();
            SetupConnections();
            SetupWindow();
            SetupVariables(tasca, user, numero);
        }

        private void SetupVariables(Tasques actualTasca, User actualUser, RadLabelElement actualNumero)
        {
            tasca = actualTasca;
            user = actualUser;
            numero = actualNumero;
            
            dateTimePicker.Value = DateTime.Today;
            tituloLabel.Text = tasca.Titol;
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void SetupConnections()
        {
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            tareasRepository = new TareasRepository(NPGSQLService);
            registroBasculaRepository = new RegistroBasculaRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
        }

        private async void radButton1_Click(object sender, EventArgs e)
        {
            if (dateTimePicker.Value.Date != DateTime.Today)
            {
                RadMessageBox.Show("No pots crear un registre per a una altra data que no sigui avui.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            float result;
            bool isFloat = float.TryParse(valorTextBox.Text, out result);

            if (!isFloat)
            {
                RadMessageBox.Show("El valor ha de ser un nombre amb o sense decimals.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (tasca.IdGrup == 9)
            {
                await RegistrarBasculaGran(result);
            }
            else
            {
                await RegistrarBasculaPetita(result);
            }
        }

        private async Task RegistrarBasculaPetita(float result)
        {
            if (result >= 24.8f && result <= 25.2f)
            {
                RegistroBascula registro = CreateRegistroBascula(result);

                registroBasculaRepository.AddRegistreBascula(registro);

                tareasRepository.UpdateTasquesDarrera(tasca.Id, dateTimePicker.Value.Date);

                Close();
            }
            else
            {
                DialogResult dialogResult = RadMessageBox.Show("El valor introduït és fora del rang de valors. Vols crear una incidència?", "Confirma la creació de la incidència", MessageBoxButtons.YesNo, RadMessageIcon.Info);

                if (dialogResult == DialogResult.Yes)
                {
                    await CreateIncidenciaBascula(result);
                    
                    Close();
                }
            }
        }

        private async Task CreateIncidenciaBascula(float result)
        {
            Incidencia incidencia = new Incidencia();
            incidencia.IdTasca = tasca.Id;
            incidencia.IdGrupTasques = tasca.IdGrup;
            incidencia.NomTasca = tasca.Titol;
            incidencia.NomGrup = tasca.Zona;
            incidencia.DescripcioIncidencia = "El valor de la bàscula és: " + result.ToString();
            incidencia.Resolta = false;
            incidencia.Data = this.dateTimePicker.Value.Date;
            incidencia.Solucio = "";

            await incidenciasRepository.AddIncidencia(incidencia);

            numero.Text = incidenciasRepository.GetNumeroIncidenciasNoResueltas().ToString();
        }

        private RegistroBascula CreateRegistroBascula(float result)
        {
            RegistroBascula registroBascula = new RegistroBascula();

            registroBascula.IdTasca = tasca.Id;
            registroBascula.IdGrupTasques = tasca.IdGrup;
            registroBascula.NomTasca = tasca.Titol;
            registroBascula.NomGrup = tasca.Zona;
            registroBascula.Data = dateTimePicker.Value.Date;
            registroBascula.Valor = result;
            registroBascula.FetaPer = user.Name;
            registroBascula.Observacions = textBoxObsv.Text;
            registroBascula.Estat = "Pendent";

            if (string.IsNullOrEmpty(textBoxObsv.Text))
            {
                textBoxObsv.Text = "";
            }

            return registroBascula;
        }

        private async Task RegistrarBasculaGran(float result)
        {
            if (result >= 998.0f && result <= 1002f)
            {
                RegistroBascula registro = CreateRegistroBascula(result);

                registroBasculaRepository.AddRegistreBascula(registro);

                tareasRepository.UpdateTasquesDarrera(tasca.Id, this.dateTimePicker.Value.Date);

                Close();
            }
            else
            {
                DialogResult dialogResult = RadMessageBox.Show("El valor introduït és fora del rang de valors. Vols crear una incidència?", "Confirma la creació de la incidència", MessageBoxButtons.YesNo, RadMessageIcon.Info);

                if (dialogResult == DialogResult.Yes)
                {
                    await CreateIncidenciaBascula(result);

                    Close();
                }
            }
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RegistroBasculaForm_Load(object sender, EventArgs e)
        {

        }
    }
}
