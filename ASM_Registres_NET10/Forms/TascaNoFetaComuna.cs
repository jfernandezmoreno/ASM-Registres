using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class TascaNoFetaComuna : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RegistroComun registro;
        RegistroComunRepository registreComuRepository;
        public TascaNoFetaComuna(RegistroComun registroComun)
        {
            InitializeComponent();
            SetupWindow();
            SetupConnections();
            SetupVariables(registroComun);
        }

        private void SetupVariables(RegistroComun actualRegistroComun)
        {
            registro = actualRegistroComun;
        }

        private void SetupConnections()
        {
            registreComuRepository = new RegistroComunRepository(NPGSQLService);
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
                registreComuRepository.UpdateRegistroComun(registro.Id, "No Completada", registro.Observacions);

                Close();
            }
            else
            {
                RadMessageBox.Show("Has de donar una explicació obligatóriament", "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Info);
            }
        }
    }
}
