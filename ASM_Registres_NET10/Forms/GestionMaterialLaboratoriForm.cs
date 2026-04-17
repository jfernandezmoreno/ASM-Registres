using ASM_Registres_NET10.Modules;
using System;
using System.Drawing;
using System.util;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class GestionMaterialLaboratoriForm : Telerik.WinControls.UI.RadForm
    {

        //MailSenderService mailSenderService;
        User user;

        public GestionMaterialLaboratoriForm(User user)
        {
            InitializeComponent();
            SetupWindow();
            SetupVariables(user);
        }

        private void SetupVariables(User user)
        {
            //mailSenderService = new MailSenderService();
            this.user = user;
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;
        }

        private async void enviarButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                //await mailSenderService.SendEmailAsync("montse.berenguer@additius.com", "Petició material de laboratori de "+user.Name, textBox.Text, null);
                Close();
            }
            else
            {
                RadMessageBox.Show("No pots fer una petició buida!", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void cancelarButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
