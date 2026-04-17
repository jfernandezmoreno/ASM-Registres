using ASM_Registres.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class EscogerTurno : Telerik.WinControls.UI.RadForm
    {
        ShiftsManager previous;

        int idRegistro;
        int idUser;
        string idTurno;
        string name;

        public EscogerTurno(string name, string idTurno, int idUser, int idRegistro, ShiftsManager form)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            this.name = name;
            this.idUser = idUser;
            this.idRegistro = idRegistro;
            this.idTurno = idTurno;

            this.comboBox1.Items.AddRange(new object[] {
                "Noche",
                "Mañana",
                "Tarde",
                "Vacaciones",
                "Baja"});

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            label1.Text += name;
            CenterLabelHorizontally(label1);

            previous = form;

            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void CenterLabelHorizontally(Label label)
        {
            int formWidth = ClientSize.Width;
            int labelWidth = label.Width;

            int x = (formWidth - labelWidth) / 2;
            int y = label.Location.Y;

            label.Location = new Point(x, y);
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
            {
                RadMessageBox.Show("El Turno está vacío", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            else
            {
                previous.getInfo(this.comboBox1.Text, this.idUser, this.idRegistro, this.name);
                Close();
            }
        }
    }
}
