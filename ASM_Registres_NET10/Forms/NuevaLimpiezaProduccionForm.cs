using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesRegistros;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Modules.Registros;
using ASM_Registres_NET10.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    public partial class NuevaLimpiezaProduccionForm : Telerik.WinControls.UI.RadForm
    {

        NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);
        RegistroLimpiezaProduccionRepository repo;
        UsuarioRepository UsuarioRepository;
        TipoLimpiezaProduccionRepository TipoLimpiezaProduccionRepository;
        List<string> iniciales = new List<string>();

        User u;

        public event EventHandler LimpiezaCreada;

        public NuevaLimpiezaProduccionForm(User user)
        {
            InitializeComponent();

            repo = new RegistroLimpiezaProduccionRepository(NPGSQLService);
            UsuarioRepository = new UsuarioRepository(NPGSQLService);
            TipoLimpiezaProduccionRepository = new TipoLimpiezaProduccionRepository(NPGSQLService);

            u = user;

            iniciales = UsuarioRepository.GetIniciales();

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Clear();
            try
            {
                comboBox1.Items.AddRange(TipoLimpiezaProduccionRepository.GetTipos().ToArray());
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("No s'han pogut carregar els tipus de neteja des de la base de dades: " + ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            radDateTimePicker1.Value = DateTime.Today;

            SetupWindow();
        }

        private void SetupWindow()
        {
            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            string inicial = radTextBoxControl1.Text;
            DateTime d;

            if (u.IsAdmin())
            {
                d = radDateTimePicker1.Value;
            }
            else
            {
                d = DateTime.Today;
            }


            if (!iniciales.Contains(inicial))
            {
                RadMessageBox.Show("Les inicials " + inicial + " no coincideixen amb cap inicial a la base de dades. Si es tracta d’un error, contacta amb l’administrador IT: 233.", "Advertència", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }
            else
            {
                RegistroLimpiezaProduccion registro = new RegistroLimpiezaProduccion();
                registro.Operario = inicial;
                registro.Fecha = d;
                registro.Tipo = comboBox1.Text;

                repo.InsertRegistro(registro);

                LimpiezaCreada?.Invoke(this, EventArgs.Empty);

                Close();
            }
            
        }
    }
}
