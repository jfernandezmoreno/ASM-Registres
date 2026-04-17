using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class RegistreTemperaturesForm : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RegistroTemperaturaCamaraRepository registroTemperaturaCamaraRepository;
        User user;

        public RegistreTemperaturesForm(User user)
        {
            InitializeComponent();
            SetupWindow(user);
            SetupConnections();
        }

        private void SetupConnections()
        {
            this.registroTemperaturaCamaraRepository = new RegistroTemperaturaCamaraRepository(NPGSQLService);
        }

        private void SetupWindow(User actualUser)
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            user = actualUser;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            float temp;

            if (!float.TryParse(radTextBoxControl1.Text, out temp))
            {
                RadMessageBox.Show("La temperatura debe ser un número!", "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            RegistroTemperaturaCamara registro = new RegistroTemperaturaCamara();

            registro.Camara = camaraCombo.Text;
            registro.DiaHora = DateTime.Now;
            registro.Operario = user.Name;
            registro.TemperaturaCamara = temp;

            if (radCheckBox1.Checked)
            {
                registro.Paro = true;
            }
            else 
            { 
                registro.Paro = false; 
            }

            if (radCheckBox2.Checked)
            {
                registro.Marcha = true;
            }
            else 
            { 
                registro.Marcha = false; 
            }

            if (radCheckBox3.Checked)
            {
                registro.CambioTemperatura = true;
            }
            else 
            { 
                registro.CambioTemperatura = false; 
            }

            registroTemperaturaCamaraRepository.AddRegistroTermico(registro);

            Close();
        }

        private void RegistreTemperaturesForm_Load(object sender, EventArgs e)
        {

        }
    }
}
