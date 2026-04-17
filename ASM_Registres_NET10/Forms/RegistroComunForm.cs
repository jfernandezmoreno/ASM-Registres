using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.Forms
{
    public partial class RegistroComunForm : Telerik.WinControls.UI.RadForm
    {
        Tasques tasca;

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        IncidenciasRepository incidenciasRepository;
        GrupoTareasRepository grupoTareasRepository;
        TareasRepository tareasRepository;
        RegistroComunRepository registreComuRepository;

        RadLabelElement numero;
        User user;

        public RegistroComunForm(Tasques tasca, User user, RadLabelElement numero)
        {
            InitializeComponent();
            SetupWindow();
            SetupConnections();
            SetupVariables(tasca, user, numero);
            ConfigureEstadoCombo();
        }

        private void SetupVariables(Tasques actualTasca, User actualUser, RadLabelElement actualNumero)
        {
            tasca = actualTasca;
            numero = actualNumero;
            user = actualUser;

            dateTimePicker.Value = DateTime.Today;
            tituloLabel.Text = tasca.Titol;
        }

        private void SetupConnections()
        {
            grupoTareasRepository = new GrupoTareasRepository(NPGSQLService);
            tareasRepository = new TareasRepository(NPGSQLService);
            registreComuRepository = new RegistroComunRepository(NPGSQLService);
            incidenciasRepository = new IncidenciasRepository(NPGSQLService);
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void ConfigureEstadoCombo()
        {
            string[] estados = new string[] { "Completada", "Incidencia" };
            estadoCombo.DataSource = estados;
            estadoCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private async void radButton1_Click(object sender, EventArgs e)
        {
            if (dateTimePicker.Value.Date != DateTime.Today)
            {
                RadMessageBox.Show("No pots crear un registre per a una altra data que no sigui avui.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (estadoCombo.SelectedItem.ToString() == "Incidencia")
            {
                await CreateIncidencia();
                Close();
                
            }
            else
            {
                CreateRegistre();
                Close();
            }
        }

        private void CreateRegistre()
        {
            RegistroComun registroComun = new RegistroComun();
            registroComun.IdTasca = tasca.Id;
            registroComun.IdGrupTasques = tasca.IdGrup;
            registroComun.NomTasca = tasca.Titol;
            registroComun.NomGrup = tasca.Zona;
            registroComun.Data = dateTimePicker.Value.Date;
            registroComun.Estat = "Pendent";
            registroComun.FetaPer = user.Name;
            registroComun.Observacions = textBoxObsv.Text;

            if (string.IsNullOrEmpty(textBoxObsv.Text))
            {
                textBoxObsv.Text = "";
            }

            registreComuRepository.InsertRegistroComun(registroComun);

            tareasRepository.UpdateTasquesDarrera(tasca.Id, dateTimePicker.Value.Date);
        }

        private async Task CreateIncidencia()
        {
            DialogResult dialogResult = RadMessageBox.Show("Vols crear una incidència de la tasca?", "Confirma la creació de la incidència", MessageBoxButtons.YesNo, RadMessageIcon.Info);

            if (dialogResult == DialogResult.Yes)
            {

                Incidencia incidencia = new Incidencia();
                incidencia.IdTasca = tasca.Id;
                incidencia.IdGrupTasques = tasca.IdGrup;
                incidencia.NomTasca = tasca.Titol;
                incidencia.NomGrup = tasca.Zona;
                incidencia.Data = dateTimePicker.Value.Date;
                incidencia.DescripcioIncidencia = textBoxObsv.Text;
                incidencia.Resolta = false;
                incidencia.Solucio = "";

                await incidenciasRepository.AddIncidencia(incidencia);

                numero.Text = incidenciasRepository.GetNumeroIncidenciasNoResueltas().ToString();
            }
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
