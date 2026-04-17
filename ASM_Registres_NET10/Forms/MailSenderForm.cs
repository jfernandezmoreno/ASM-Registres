using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace ASM_Registres.Forms
{
    public partial class MailSenderForm : Telerik.WinControls.UI.RadForm
    {
        //MailSenderService mailSenderService;
        string path;
        public MailSenderForm(string path)
        {
            InitializeComponent();

            SetupGrid();

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;

            //mailSenderService = new MailSenderService();
                
            radTextBoxControl1.Text = "joan@additius.com";

            this.path = path;
        }

        private void SetupGrid()
        {
            radGridView1.Columns.Clear();
            radGridView1.Rows.Clear();

            radGridView1.EnableGrouping = false;
            radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

            GridViewTextBoxColumn correoColumn = new GridViewTextBoxColumn
            {
                Name = "Correo",
                HeaderText = "Correos",
                FieldName = "Correo",
                Width = 250
            };

            radGridView1.Columns.Add(correoColumn);

            string[] correos = new string[]
            {
            "juancarlos@additius.com",
            "gerard.blanes@additius.com",
            "montse.berenguer@additius.com",
            "barbara@additius.com",
            "emilio@additius.eu",
            "lluc@additius.com",
            "planta@additius.eu",
            "correolaboratorio@additius.eu"
            };

            foreach (string correo in correos)
            {
                radGridView1.Rows.Add(correo);
            }
        }

        private async void radButton1_Click(object sender, EventArgs e)
        {

            List<string> cc = new List<string>();

            foreach (GridViewRowInfo row in radGridView1.Rows)
            {
                if (row.Cells["Correo"].Value != null)
                {
                    cc.Add(row.Cells["Correo"].Value.ToString());
                }
            }

            EnviarMail(radTextBoxControl1.Text, "En éste mail se adjunta el PDF en el que se pueden ver las tareas y recordatorios del día", path, cc);

            //await mailSenderService.SendEmailAsync(radTextBoxControl1.Text, "Tareas PDF " + DateTime.Now.ToString(), "En éste mail se adjunta el PDF en el que se pueden ver las tareas y recordatorios del día", path, cc);
            RadMessageBox.Show("Mail enviado correctamente", "Éxito", MessageBoxButtons.OK, RadMessageIcon.Info);
            Close();
        }

        public static void EnviarMail(string to, string cuerpo, string path, List<string> cc)
        {
            string claveCorreo = "ktkr xyyo qvgk geuz";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var mail = new MailMessage
                {
                    From = new MailAddress("additius@gmail.com", "Additius"),
                    Subject = "Tareas PDF " + DateTime.Now.ToString(),
                    Body = cuerpo,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                foreach (var email in cc)
                {
                    mail.CC.Add(email);
                }

                mail.Attachments.Add(new Attachment(path));

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
    }
}
