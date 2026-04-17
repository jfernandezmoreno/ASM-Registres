using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.Forms
{
    public partial class RegistroSiloForm : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        User user;
        Tasques tasca;

        RegistroSilosRepository registroSilosRepository;

        public RegistroSiloForm(Tasques tasca, User user)
        {
            InitializeComponent();
            SetupWindow();
            SetupConnections();
            SetupVariables(tasca, user);
        }

        private void SetupVariables(Tasques actualTasca, User actualUser)
        {
            user = actualUser;
            tasca = actualTasca;

            radLabel1.Text = tasca.Titol;
        }

        private void SetupConnections()
        {
            registroSilosRepository = new RegistroSilosRepository(NPGSQLService);
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            float result;

            if (!TryGetFloatFromRadTextBoxControl(radTextBoxControl1, out result))
            {
                RadMessageBox.Show("No puedes introducir un registro si la Cantidad no es un número", "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            RegistroSilos registroSilos = new RegistroSilos();
            registroSilos.Fecha = DateTime.Now;
            registroSilos.Precinto = result;
            registroSilos.PrecintadoPor = user.Name;
            registroSilos.NombreTarea = tasca.Titol;
            registroSilos.NombreGrupo = tasca.Zona;

            registroSilosRepository.InsertRegistroSilos(registroSilos);

            Close();
        }

        private bool TryGetFloatFromRadTextBoxControl(RadTextBoxControl radTextBoxControl1, out float result)
        {
            return float.TryParse(radTextBoxControl1.Text, out result);
        }
    }
}
