using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class AgregarReprocesatMostresLaboratoriForm : Telerik.WinControls.UI.RadForm
    {
        private ReprocessatMostresLaboratoriRepository repository;
        NPGSQLService npgSQLService;
        private User user;
        //MailSenderService mailSenderService;

        public AgregarReprocesatMostresLaboratoriForm(User user)
        {
            InitializeComponent();
            InitWindow();

            npgSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            repository = new ReprocessatMostresLaboratoriRepository(npgSQLService);
            
            this.user = user;

            //mailSenderService = new MailSenderService();
        }

        private void InitWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lotTextBox.Text))
            {
                RadMessageBox.Show("Debes agregar un número de lote para guardar el registro", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(pesTextBox.Text))
            {
                RadMessageBox.Show("Debes agregar un peso para guardar el registro", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(producteTextBox.Text))
            {
                RadMessageBox.Show("Debes agregar un producto para guardar el registro", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            double peso = 0;

            if (double.TryParse(pesTextBox.Text, out peso))
            {
                RegistreReprocesatMostresLaboratori registre = new RegistreReprocesatMostresLaboratori();

                registre.Data = DateTime.Now;
                registre.Laborant = user.Name;
                registre.Lot = lotTextBox.Text;
                registre.Pes = peso;
                registre.Producte = producteTextBox.Text;

                repository.AddReprocesatMostraLaboratori(registre);

                EnviarMail("montse.berenguer@additius.com",
                    "Nou Reprocessat per la mostra del producte " + registre.Producte +
                    " S'ha creat un nou reprocesat pel producte " + registre.Producte + " realitzada pel laborant: " +
                    registre.Laborant + " amb un número de lot: " +
                    registre.Lot + " i amb un pes de: " + registre.Pes.ToString(), new List<string>{ "juancarlos@additius.com" });

                Close();
            }
            else
            {
                RadMessageBox.Show("El valor del peso debe tener ser un número", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
        }

        public static void EnviarMail(string to, string cuerpo, List<string> cc)
        {
            string claveCorreo = "ktkr xyyo qvgk geuz";

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var mail = new MailMessage
                {
                    From = new MailAddress("additius@gmail.com", "Additius"),
                    Subject = "Nou Reprocessat",
                    Body = cuerpo,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                foreach (var email in cc)
                {
                    mail.CC.Add(email);
                }

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
                RadMessageBox.Show($"Error al enviar el correo:\n{e}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }
    }
}
