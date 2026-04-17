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
    public partial class FASE_MOVIL : Telerik.WinControls.UI.RadForm
    {
        User User;
        NPGSQLService NPGSQLService;
        RegistroFaseMovilRepository RegistroFaseMovilRepository;
        public FASE_MOVIL(User user)
        {
            InitializeComponent();
            SetupWindow();

            User = user;    
            NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            RegistroFaseMovilRepository = new RegistroFaseMovilRepository(NPGSQLService);
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            acetonaLOT.Enabled = false;
            hexaLOT.Enabled = false;
            acetatLOT.Enabled = false;

            dateTimePicker.Value = DateTime.Now;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool continuar = CheckEstadoForm();

            if (continuar)
            {
                RegistreFaseMovil registreFaseMovil = new RegistreFaseMovil();

                registreFaseMovil.Date = dateTimePicker.Value;
                registreFaseMovil.LaborantId = User.Id;

                registreFaseMovil.HexaTick = hexaTICK.Checked ? true : false;
                registreFaseMovil.AcetonaTick = acetonaTICK.Checked ? true : false;
                registreFaseMovil.AcetatEtilTick = acetatTICK.Checked ? true : false;

                registreFaseMovil.HexaBatch = hexaTICK.Checked ? hexaLOT.Text : string.Empty;
                registreFaseMovil.AcetonaBatch = acetonaTICK.Checked ? acetonaLOT.Text : string.Empty;
                registreFaseMovil.AcetatBatch = acetatTICK.Checked ? acetatLOT.Text : string.Empty;

                RegistroFaseMovilRepository.Insert(registreFaseMovil);

                RadMessageBox.Show("Registre guardat correctament", "Informació", MessageBoxButtons.OK, RadMessageIcon.Info);

                Close();

            }
            else
            {
                return;
            }
        }
        private bool CheckEstadoForm()
        {
            if (dateTimePicker.Value == DateTime.MinValue || dateTimePicker.Value == null || dateTimePicker.Value == DateTime.MaxValue) { RadMessageBox.Show("Has d'escollir una data correcte", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (hexaTICK.Checked && string.IsNullOrEmpty(hexaLOT.Text)) { RadMessageBox.Show("El lot HEXA a de ser vàlid", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (acetonaTICK.Checked && string.IsNullOrEmpty(acetonaLOT.Text)) { RadMessageBox.Show("El lot acetona a de ser vàlid", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (acetatTICK.Checked && string.IsNullOrEmpty(acetatLOT.Text)) { RadMessageBox.Show("El lot Acetat a de ser vàlid", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            return true;
        }

        private void hexaTICK_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (hexaTICK.Checked)
            {
                hexaLOT.Enabled = true;
            }
            else
            {
                hexaLOT.Text = string.Empty;
                hexaLOT.Enabled = false;
            }
        }

        private void acetonaTICK_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (acetonaTICK.Checked)
            {
                acetonaLOT.Enabled = true;
            }
            else
            {
                acetonaLOT.Text = string.Empty;
                acetonaLOT.Enabled = false;
            }
        }

        private void acetatTICK_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (acetatTICK.Checked)
            {
                acetatLOT.Enabled = true;
            }
            else
            {
                acetatLOT.Text = string.Empty;
                acetatLOT.Enabled = false;
            }
        }
    }
}
