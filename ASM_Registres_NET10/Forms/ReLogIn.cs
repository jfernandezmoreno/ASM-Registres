using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls;

// Aliases limpios para Resources y Settings
using Res = ASM_Registres_NET10.Properties.Resources;
using Sett = ASM_Registres_NET10.Properties.Settings;

namespace ASM_Registres.Forms
{
    public partial class ReLogIn : Telerik.WinControls.UI.RadForm
    {
        public User LoggedInUser { get; private set; }

        private const string DefaultUsernameText = "example@additius.com";
        private const string DefaultPasswordText = "examplePassword";

        private readonly NPGSQLService NPGSQLService = new NPGSQLService(DatabaseCredentials.ConnectionStringLocal);

        private string lastEmail = string.Empty;
        private string lastContra = string.Empty;

        private string email = "";
        private string password = "";

        private UsuarioRepository usuarioRepository;

        public ReLogIn()
        {
            InitializeComponent();
            SetupWindow();
            SetupPicture();
            SetupPlaceholders();

            // Carga de Settings tipados del proyecto net10
            lastEmail = Sett.Default.UltimoUsuario ?? string.Empty;
            lastContra = Sett.Default.UltimaContra ?? string.Empty;

            usernameTextbox.Enter += usernameTextbox_Enter;
            usernameTextbox.Leave += usernameTextbox_Leave;
            passwordTextbox.Enter += passwordTextbox_Enter;
            passwordTextbox.Leave += passwordTextbox_Leave;
        }

        private void SetupWindow()
        {
            // Asigna el Icon desde resources (.ico directo o Bitmap convertido)
            Icon = EnsureIconFromResource(Res.Logo_a_);

            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;

            AcceptButton = loginButton;
            usuarioRepository = new UsuarioRepository(NPGSQLService);
        }

        private void SetupPicture()
        {
            // Carga la imagen del logo desde resources
            pictureBoxAdditiusLogo.Image = Res.logo;
            pictureBoxAdditiusLogo.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void SetupPlaceholders()
        {
            usernameTextbox.Text = string.IsNullOrWhiteSpace(lastEmail) ? DefaultUsernameText : lastEmail;
            usernameTextbox.ForeColor = string.IsNullOrWhiteSpace(lastEmail) ? Color.Gray : Color.Black;

            passwordTextbox.Text = string.IsNullOrWhiteSpace(lastContra) ? DefaultPasswordText : lastContra;
            passwordTextbox.ForeColor = string.IsNullOrWhiteSpace(lastContra) ? Color.Gray : Color.Black;
            passwordTextbox.PasswordChar = string.IsNullOrWhiteSpace(lastContra) ? '\0' : '*';
            passwordTextbox.UseSystemPasswordChar = !string.IsNullOrWhiteSpace(lastContra);
        }

        private void usernameTextbox_Enter(object sender, EventArgs e)
        {
            if (isDefaultUsernameText())
            {
                usernameTextbox.Text = "";
                usernameTextbox.ForeColor = Color.Black;
            }
        }

        private bool isDefaultUsernameText()
        {
            return usernameTextbox.Text == DefaultUsernameText;
        }

        private void passwordTextbox_Enter(object sender, EventArgs e)
        {
            if (isDefaultPasswordText())
            {
                passwordTextbox.Text = "";
                passwordTextbox.ForeColor = Color.Black;
                passwordTextbox.PasswordChar = '*';
                passwordTextbox.UseSystemPasswordChar = true;
            }
        }

        private bool isDefaultPasswordText()
        {
            return passwordTextbox.Text == DefaultPasswordText;
        }

        private void usernameTextbox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextbox.Text))
            {
                usernameTextbox.Text = DefaultUsernameText;
                usernameTextbox.ForeColor = Color.Gray;
            }
        }

        private void passwordTextbox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordTextbox.Text))
            {
                passwordTextbox.Text = DefaultPasswordText;
                passwordTextbox.ForeColor = Color.Gray;
                passwordTextbox.PasswordChar = '\0';
                passwordTextbox.UseSystemPasswordChar = false;
            }
        }

        private void loginButton_Click_1(object sender, EventArgs e)
        {
            if (textBoxesHaveCorrectContent())
            {
                logIn();
            }
            else
            {
                RadMessageBox.Show("Por favor, ingrese su correo electrónico y contraseña.", "Campos vacíos", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
            }
        }

        private void logIn()
        {
            email = usernameTextbox.Text?.Trim() ?? string.Empty;
            password = passwordTextbox.Text ?? string.Empty;

            try
            {
                string storedPassword = usuarioRepository.GetPasswordHashByEmail(email);

                if (!string.IsNullOrEmpty(storedPassword) && BCrypt.Net.BCrypt.Verify(password, storedPassword))
                {
                    setUpUser();
                }
                else
                {
                    RadMessageBox.Show("Correo electrónico o contraseña incorrectos.", "Error de inicio de sesión", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Exclamation);
            }
        }

        private void setUpUser()
        {
            LoggedInUser = usuarioRepository.GetUserByEmail(email);
            if (LoggedInUser != null)
            {
                LoggedInUser.nivel = usuarioRepository.getActualLevel(LoggedInUser.Id);
            }

            // Persistimos email (no guardamos contraseña)
            Sett.Default.UltimoUsuario = email;
            Sett.Default.UltimaContra = string.Empty;
            Sett.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool textBoxesHaveCorrectContent()
        {
            return !string.IsNullOrWhiteSpace(passwordTextbox.Text) &&
                   !string.IsNullOrWhiteSpace(usernameTextbox.Text) &&
                   passwordTextbox.Text != DefaultPasswordText &&
                   usernameTextbox.Text != DefaultUsernameText;
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void ReLogIn_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lastEmail))
            {
                usernameTextbox.Text = lastEmail;
                usernameTextbox.ForeColor = Color.Black;
            }
            if (!string.IsNullOrWhiteSpace(lastContra))
            {
                passwordTextbox.Text = lastContra;
                passwordTextbox.ForeColor = Color.Black;
                passwordTextbox.PasswordChar = '*';
                passwordTextbox.UseSystemPasswordChar = true;
            }
        }

        // ---------- Helpers para asegurar Icon válido y evitar leaks ----------

        // Si el recurso ya es Icon, úsalo tal cual.
        private static Icon EnsureIconFromResource(Icon iconResource) => iconResource;

        // Si el recurso es Bitmap (PNG/JPG), conviértelo a Icon y libera el handle.
        private static Icon EnsureIconFromResource(Bitmap bitmapResource)
        {
            if (bitmapResource == null) throw new ArgumentNullException(nameof(bitmapResource));
            IntPtr hIcon = bitmapResource.GetHicon();
            try
            {
                using (var icon = Icon.FromHandle(hIcon))
                {
                    return (Icon)icon.Clone(); // clon para no depender del handle
                }
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
    }
}
