using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class TascaNoFetaBascula : Telerik.WinControls.UI.RadForm
    {

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RegistroBasculaRepository registroBasculaRepository;
        RegistroBascula registro;

        public TascaNoFetaBascula(RegistroBascula registroBascula)
        {
            InitializeComponent();
            SetupWindow();
            SetupConnections();
            SetupVariables(registroBascula);
        }

        private void SetupVariables(RegistroBascula actualRegistroBascula)
        {
            registro = actualRegistroBascula;
        }

        private void SetupConnections()
        {
            registroBasculaRepository = new RegistroBasculaRepository(NPGSQLService);
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
            if (!string.IsNullOrEmpty(radTextBoxControl1.Text))
            {
                registro.Observacions = radTextBoxControl1.Text;
                registroBasculaRepository.UpdateRegistroBascula(registro.Id, "No Completada", registro.Observacions);

                Close();
            }
            else 
            {
                RadMessageBox.Show("Has de donar una explicació obligatóriament", "Advertecnia", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
        }
    }
}
