using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using System;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Windows.Forms;
using Telerik.WinControls;
using ASM_Registres_NET10.Services;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;

namespace ASM_Registres.Forms
{
    public partial class ControlResidusForm : Telerik.WinControls.UI.RadForm
    {

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        User user;
        RegistroResiduoRepository registroResiduoRepository;

        public ControlResidusForm(User user)
        {
            InitializeComponent();
            SetupWindow();
            InitializeVariables(user);
            InitializeConnections();
        }

        private void InitializeConnections()
        {
            registroResiduoRepository = new RegistroResiduoRepository(NPGSQLService);
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;
        }

        private void InitializeVariables(User user)
        {
            this.user = user;
            residuCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool isPlenaCheck = plenaCheckbox.Checked;

            if (isPlenaCheck)
            {
                ensureCorrectData();

                saveData();
                sendMail();
                
                Close(); return;
            }

            if (checkParams())
            {
                saveData();
                Close();
            }

            else
            {
                RadMessageBox.Show("ERROR! Alguns dels parámetres que has introduit són errónis!", "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private bool checkParams()
        {
            return checkCorrectResidu() && checkCorrectQuantity();
        }

        private bool checkCorrectQuantity()
        {

            if (string.IsNullOrWhiteSpace(quantitatTextBox.Text))
            {
                return false;
            }

            bool isIntegerFirstParam = int.TryParse(quantitatTextBox.Text, out int firstParam);

            if (string.IsNullOrWhiteSpace(qEnLitresTextBox.Text))
            {
                return false;
            }

            bool isIntegerSecondParam = int.TryParse(qEnLitresTextBox.Text, out int secondParam);

            return isIntegerFirstParam && isIntegerSecondParam;
        }

        private bool checkCorrectResidu()
        {
            return residuCombobox.Text == "Vidre" || residuCombobox.Text == "Dissolvent" || residuCombobox.Text == "Envassos";
        }

        private async void saveData()
        {
            RegistreResidusLaboratori registreResidusLaboratori = new RegistreResidusLaboratori();

            registreResidusLaboratori.Data = DateTime.Now;
            registreResidusLaboratori.Laborant = user.Name;
            registreResidusLaboratori.Residu = residuCombobox.Text;
            registreResidusLaboratori.Quantitat = quantitatTextBox.Text;
            registreResidusLaboratori.QuantitatLitres = qEnLitresTextBox.Text;

            try
            {
                await registroResiduoRepository.AddRegistreAsync(registreResidusLaboratori);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("S'ha produit la següent excepció' " + ex.Message, "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void ensureCorrectData()
        {
            quantitatTextBox.Text = "Bidó Ple";
            qEnLitresTextBox.Text = string.Empty;
        }

        public async static void sendMail()
        {
            try
            {
                string to = "montse.berenguer@additius.com";
                string cuerpo = "Hola Montse, s'acaba d'omplir el bidó";

                EnviarMail(to, cuerpo);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("No s'ha pogut enviar el correu. Error: " + ex.Message, "Advertencia", MessageBoxButtons.OK, RadMessageIcon.Error);
            }

        }

        public static void EnviarMail(string to, string cuerpo)
        {
            string claveCorreo = "ktkr xyyo qvgk geuz";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var mail = new MailMessage
                {
                    From = new MailAddress("additius@gmail.com", "Additius"),
                    Subject = "Bidó Ple",
                    Body = cuerpo,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(
                        "additius@gmail.com",
                        claveCorreo
                    );
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mail);
                }
            }
            catch (Exception e)
            {
                RadMessageBox.Show($"Error al enviar el correo:\n{e}");
            }
        }

        private void plenaCheckbox_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            checkIfChekedAndAct();
        }

        private void quantitatTextBox_TextChanged(object sender, EventArgs e)
        {
            checkIfChekedAndAct();
        }

        private void checkIfChekedAndAct()
        {
            if (plenaCheckbox.Checked)
            {
                quantitatTextBox.Text = "Bidó Ple";
                qEnLitresTextBox.Text = string.Empty;
            }
        }

        private void ControlResidusForm_Load(object sender, EventArgs e)
        {

        }
    }
}
