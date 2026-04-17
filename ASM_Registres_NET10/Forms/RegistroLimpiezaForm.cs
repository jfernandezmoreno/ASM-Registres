using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.Forms
{
    public partial class RegistroLimpiezaForm : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RegistroLimpiezaRepository registroLimpiezaRepository;

        public RegistroLimpiezaForm()
        {
            InitializeComponent();
            SetupWindow();
            SetupConnections();
        }

        private void SetupConnections()
        {
            registroLimpiezaRepository = new RegistroLimpiezaRepository(NPGSQLService);
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private bool TryGetFloatFromRadTextBoxControl(RadTextBoxControl radTextBoxControl2, out float result)
        {
            return float.TryParse(radTextBoxControl2.Text, out result);
        }

        private void radButton1_Click(object sender, EventArgs e)
        {

            float result;

            if (string.IsNullOrEmpty(radTextBoxControl1.Text))
            {
                RadMessageBox.Show("No pots introdïr un registre sense un Producte", "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            

            else if (!TryGetFloatFromRadTextBoxControl(radTextBoxControl2, out result)){
                RadMessageBox.Show("No pots introdïr un registre si la Quantitat no es un número", "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            RegistroLimpieza registroLimpieza = new RegistroLimpieza();
            registroLimpieza.Fecha = DateTime.Now;
            registroLimpieza.Cantidad = result;
            registroLimpieza.Producto = radTextBoxControl1.Text;

            registroLimpiezaRepository.InsertRegistroLimpieza(registroLimpieza);
            Close();
        }
    }
}
