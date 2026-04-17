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
    public partial class RegistreGeneralForm : Telerik.WinControls.UI.RadForm
    {

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        RegistroGeneralRepository registroGeneralRepository;
        User user;
        public RegistreGeneralForm(User user)
        {
            InitializeComponent();
            SetupWindw(user);
            ConfigureEstadoCombo();
        }

        private void SetupWindw(User actualUser)
        {
            registroGeneralRepository = new RegistroGeneralRepository(NPGSQLService);
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            user = actualUser;
        }

        private void ConfigureEstadoCombo()
        {
            string[] estados = new string[] { "Preventiu", "Correctiu" };
            mtoCombo.DataSource = estados;
            mtoCombo.DropDownStyle = ComboBoxStyle.DropDownList;

            string[] estados2 = new string[] { "ASM", "Externa" };
            empresaCombo.DataSource = estados2;
            empresaCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(accTextBox.Text))
            {
                RadMessageBox.Show("El camp 'Acció' no pot estar buit", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            float duracion;
            if (!float.TryParse(duraciotextBox.Text, out duracion))
            {
                RadMessageBox.Show("La duració a de tenir un format correcte", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(obvTextBox.Text))
            {
                obvTextBox.Text = "";
            }

            RegistroGeneral registroGeneral = new RegistroGeneral();
            registroGeneral.AccRealitzada = accTextBox.Text;
            registroGeneral.Data = radDateTimePicker.Value.Date;
            registroGeneral.TipusMto = mtoCombo.Text;
            registroGeneral.DuracioH = duracion;
            registroGeneral.Observacions = obvTextBox.Text;
            registroGeneral.Empresa = empresaCombo.Text;
            registroGeneral.FetaPer = user.Name;

            if (netejaCheck.Checked)
            {
                registroGeneral.Nateja = true;
            }

            else
            {
                registroGeneral.Nateja = false;
            }

            registroGeneralRepository.AddRegistroGeneral(registroGeneral);
                
            Close();

        }

        private void radButton2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void RegistreGeneralForm_Load(object sender, EventArgs e)
        {

        }

        private void radPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
