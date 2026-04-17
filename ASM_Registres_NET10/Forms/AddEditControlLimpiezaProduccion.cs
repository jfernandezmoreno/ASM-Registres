using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class AddEditControlLimpiezaProduccion : Telerik.WinControls.UI.RadForm
    {
        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
        ControlRegistroLimpiezaProduccionRepository repo;
        ControlRegistroLimpiezaProduccion control;
        int limpiezaId;
        public AddEditControlLimpiezaProduccion(int limpiezaId, ControlRegistroLimpiezaProduccion control)
        {
            InitializeComponent();

            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            this.control = control;
            this.limpiezaId = limpiezaId;
            repo = new ControlRegistroLimpiezaProduccionRepository(NPGSQLService);

            var acciones = repo.GetAllAccionesLimpiezaProduccion();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(acciones.ToArray());

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;


            CargarDatosEnFormulario();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            var nuevoControl = new ControlRegistroLimpiezaProduccion
            {
                IdRegistroLimpiezaProduccion = limpiezaId,
                Fecha = radDateTimePicker1.Value,
                HoraInicio = horaInicioTextbox.Text,
                HoraFinal = horaFinalTextBox.Text,
                Accion = comboBox1.SelectedItem?.ToString() ?? "",
                Iniciales = inicialesTextbox.Text,
                LoteSilice = loteSiliceTextBox.Text,
                LoteCarbonato = loteCarbonatoTextbox.Text,
                Lote = lotetextbox.Text,
                Integridad = comboBox2.SelectedItem?.ToString() ?? ""
                
            };

            int kgRecogidos = 0;
            int kgSilice = 0;
            int kgCarbonato = 0;

            if (!string.IsNullOrWhiteSpace(kgRecogidosTextbox.Text) &&
                !int.TryParse(kgRecogidosTextbox.Text, out kgRecogidos))
            {
                MessageBox.Show("El valor de «kg recollits» no és vàlid. Ha de ser un nombre enter.", "Error d'entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(kgSiliceTextbox.Text) &&
                !int.TryParse(kgSiliceTextbox.Text, out kgSilice))
            {
                MessageBox.Show("El valor de «kg de sílice» no és vàlid. Ha de ser un nombre enter.", "Error d'entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(kgCarbonatoTextbox.Text) &&
                !int.TryParse(kgCarbonatoTextbox.Text, out kgCarbonato))
            {
                MessageBox.Show("El valor de «kg de carbonat» no és vàlid. Ha de ser un nombre enter.", "Error d'entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string obvs = null;

            if (string.IsNullOrEmpty(radTextBoxControl1.Text))
            {
                obvs = "";
            }
            else
            {
                obvs = radTextBoxControl1.Text;
            }

            nuevoControl.KgRecogidos = kgRecogidos;
            nuevoControl.KgSilice = kgSilice;
            nuevoControl.KgCarbonato = kgCarbonato;
            nuevoControl.Observaciones = obvs;

            try
            {
                if (control.Id == 0)
                {
                    repo.InsertControl(nuevoControl);
                }
                else
                {
                    nuevoControl.Id = control.Id;

                    repo.UpdateControl(nuevoControl);
                }

                Close(); 
            }
            catch (Exception ex)
            {
                RadMessageBox.Show($"Error en desar el control: {ex.Message}", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private void CargarDatosEnFormulario()
        {
            if (control == null) return;

            radDateTimePicker1.Value = control.Fecha;

            horaInicioTextbox.Text = control.HoraInicio;
            horaFinalTextBox.Text = control.HoraFinal;
            inicialesTextbox.Text = control.Iniciales;
            kgRecogidosTextbox.Text = control.KgRecogidos.ToString();
            kgSiliceTextbox.Text = control.KgSilice.ToString();
            loteSiliceTextBox.Text = control.LoteSilice;
            kgCarbonatoTextbox.Text = control.KgCarbonato.ToString();
            loteCarbonatoTextbox.Text = control.LoteCarbonato;
            lotetextbox.Text = control.Lote;

            comboBox1.SelectedItem = control.Accion;

            comboBox2.SelectedItem = control.Integridad;

            if (string.IsNullOrEmpty(control.Integridad))
            {
                comboBox2.SelectedIndex = 0;
            }

            radTextBoxControl1.Text = control.Observaciones;

        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            string horaActual = DateTime.Now.ToString("HH:mm");

            if (IsValidTime(horaActual))
            {
                horaInicioTextbox.Text = horaActual;
            }
            else
            {
                RadMessageBox.Show("Format d'hora no vàlid.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        public bool IsValidTime(string input)
        {

            if (string.IsNullOrEmpty(input)) return true;

            Regex regex = new Regex(@"^([01]?[0-9]|2[0-3]):([0-5][0-9])$");
            if (!regex.IsMatch(input))
            {
                return false;
            }

            string[] parts = input.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
            {
                return false;
            }

            return true;
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            string horaActual = DateTime.Now.ToString("HH:mm");

            if (IsValidTime(horaActual))
            {
                horaFinalTextBox.Text = horaActual;
            }
            else
            {
                RadMessageBox.Show("Format d'hora no vàlid.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }
    }
}
