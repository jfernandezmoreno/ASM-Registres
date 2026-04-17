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
    public partial class HAET : Telerik.WinControls.UI.RadForm
    {
        RegistroHAETRepository RegistroHAETRepository;
        NPGSQLService NPGSQLService;
        User User;
        public HAET(User user)
        {
            InitializeComponent();
            SetupWindow();

            NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
            RegistroHAETRepository = new RegistroHAETRepository(NPGSQLService);
            User = user;
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            acetonaLOT.Enabled = false;
            hexaLOT.Enabled = false;
            etanolLOT.Enabled = false;
            tolueLOT.Enabled = false;

            dateTimePicker.Value = DateTime.Now;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            bool continuar = CheckEstadoForm();

            if (continuar)
            {
                RegistreHAET registreHAET = new RegistreHAET();

                registreHAET.Date = dateTimePicker.Value;
                registreHAET.LaborantId = User.Id;

                registreHAET.HexaTick = hexaTICK.Checked ? true : false;
                registreHAET.AcetonaTick = acetonaTICK.Checked ? true : false;
                registreHAET.EtanolTick = etanolTICK.Checked ? true : false;
                registreHAET.TouleTick = tolueTICK.Checked ? true : false;

                registreHAET.HexaBatch = hexaTICK.Checked ? hexaLOT.Text : string.Empty;
                registreHAET.AcetonaBatch = acetonaTICK.Checked ? acetonaLOT.Text : string.Empty;
                registreHAET.EtanolBatch = etanolTICK.Checked ? etanolLOT.Text : string.Empty;
                registreHAET.TolueBatch = tolueTICK.Checked ? tolueLOT.Text : string.Empty;

                RegistroHAETRepository.Insert(registreHAET);

                RadMessageBox.Show("Registro guardado correctamente", "Información", MessageBoxButtons.OK, RadMessageIcon.Info);

                Close();

            }
            else 
            {
                return;   
            }
        }

        private bool CheckEstadoForm()
        {
            if (dateTimePicker.Value == DateTime.MinValue || dateTimePicker.Value == null || dateTimePicker.Value == DateTime.MaxValue) { RadMessageBox.Show("Debes escoger una fecha correcta", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (hexaTICK.Checked && string.IsNullOrEmpty(hexaLOT.Text)) { RadMessageBox.Show("El lote de HEXA debe ser válido", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (acetonaTICK.Checked && string.IsNullOrEmpty(acetonaLOT.Text)) { RadMessageBox.Show("El lote de Acetona debe ser válido", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (etanolTICK.Checked && string.IsNullOrEmpty(etanolLOT.Text)) { RadMessageBox.Show("El lote de Etanol debe ser válido", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
            if (tolueTICK.Checked && string.IsNullOrEmpty(tolueLOT.Text)) { RadMessageBox.Show("El lote de Toluè debe ser válido", "Adverténcia", MessageBoxButtons.OK, RadMessageIcon.Error); return false; }
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

        private void etanolTICK_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (etanolTICK.Checked)
            {
                etanolLOT.Enabled = true;
            }
            else
            {
                etanolLOT.Text = string.Empty;
                etanolLOT.Enabled = false;
            }
        }

        private void tolueTICK_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (tolueTICK.Checked)
            {
                tolueLOT.Enabled = true;
            }
            else
            {
                tolueLOT.Text = string.Empty;
                tolueLOT.Enabled = false;
            }
        }
    }
}
