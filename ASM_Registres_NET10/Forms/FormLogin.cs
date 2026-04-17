using ASM_Registres.Forms;
using ASM_Registres_NET10.Constants;
using ASM_Registres_NET10.DatabaseConnections.RepositoriesEntidades;
using ASM_Registres_NET10.Forms;
using ASM_Registres_NET10.Modules;
using ASM_Registres_NET10.Services;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.util;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Themes;
using Telerik.WinControls.UI;

using Res = ASM_Registres_NET10.Properties.Resources;
using Sett = ASM_Registres_NET10.Properties.Settings;

namespace ASM_Registres
{
    public partial class FormLogin : RadForm
    {
        private const string UsernameNullText = "exemple@additius.com";
        private const string PasswordNullText = "Contrasenya";

        private string _lastEmail = string.Empty;

        private readonly NPGSQLService _npqsqlService;
        private readonly UsuarioRepository _usuarioRepository;

        private User _user;

        public FormLogin()
            : this(
                new UsuarioRepository(new NPGSQLService(DatabaseCredentials.ConnectionStringLocal))
              )
        {
        }

        public FormLogin(UsuarioRepository usuarioRepository)
        {
            InitializeComponent();

            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));

            var serviceField = typeof(UsuarioRepository).GetField("_npgsqlService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            _npqsqlService = serviceField != null ? (NPGSQLService)serviceField.GetValue(usuarioRepository) : null;

            SetupWindow();
            SetupPicture();
            LoadPersistedState();

            Text = "Registres v2.0.1";

            var original = Res.info;
            var resized = new Bitmap(original, new Size(24, 24));
            radButton1.Image = resized;

        }

        private void SetupWindow()
        {
            Icon = EnsureIconFromResource(Res.Logo_a_);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.White;

            AcceptButton = loginButton;
        }

        private void SetupPicture()
        {
            pictureBoxAdditiusLogo.Image = Res.logo;
            pictureBoxAdditiusLogo.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void LoadPersistedState()
        {
            _lastEmail = Sett.Default.UltimoUsuario ?? string.Empty;

            if (!string.IsNullOrEmpty(Sett.Default.UltimaContra))
            {
                Sett.Default.UltimaContra = string.Empty;
                Sett.Default.Save();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "TelerikMetroBlue");
            var theme = new TelerikMetroBlueTheme();
            ThemeResolutionService.ApplicationThemeName = "TelerikMetroBlue";

            SetupInputs();
        }

        private void SetupInputs()
        {
            var userRad = usernameTextbox as RadTextBox;
            var passRad = passwordTextbox as RadTextBox;

            if (userRad != null)
            {
                userRad.NullText = UsernameNullText;
                userRad.Text = string.IsNullOrWhiteSpace(_lastEmail) ? string.Empty : _lastEmail;
            }
            else
            {
                usernameTextbox.Text = string.IsNullOrWhiteSpace(_lastEmail) ? string.Empty : _lastEmail;
            }

            if (passRad != null)
            {
                passRad.NullText = PasswordNullText;
            }

            passwordTextbox.UseSystemPasswordChar = true;
            passwordTextbox.Text = string.Empty;

            if (!string.IsNullOrWhiteSpace(_lastEmail))
                usernameTextbox.ForeColor = Color.Black;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            if (!CamposValidos())
            {
                RadMessageBox.Show("Si us plau, introdueix un correu electrònic vàlid i una contrasenya.", "Camps buits", MessageBoxButtons.OK, RadMessageIcon.Error);
                return;
            }

            loginButton.Enabled = false;
            try
            {
                await LogInAsync();
            }
            finally
            {
                loginButton.Enabled = true;
            }
        }

        private bool CamposValidos()
        {
            var email = (usernameTextbox.Text ?? string.Empty).Trim();
            var pwd = passwordTextbox.Text ?? string.Empty;

            return !string.IsNullOrWhiteSpace(email)
                   && !string.IsNullOrWhiteSpace(pwd)
                   && EsEmailValido(email);
        }

        private static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private async Task LogInAsync()
        {
            var email = (usernameTextbox.Text ?? string.Empty).Trim();  
            var password = passwordTextbox.Text ?? string.Empty;

            try
            {
                var storedHash = await Task.Run(() => _usuarioRepository.GetPasswordHashByEmail(email));

                if (string.IsNullOrEmpty(storedHash))
                {
                    RadMessageBox.Show("Correu electrònic o contrasenya incorrectes", "Error d'inici de sessió", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                bool ok;
                try
                {
                    ok = BCrypt.Net.BCrypt.Verify(password, storedHash);
                }
                catch
                {
                    ok = false;
                }

                if (!ok)
                {
                    RadMessageBox.Show("Correu electrònic o contrasenya incorrectes", "Error d'inici de sessió", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                _user = await Task.Run(() =>
                {
                    var u = _usuarioRepository.GetUserByEmail(email);
                    if (u != null)
                    {
                        u.nivel = _usuarioRepository.getActualLevel(u.Id);
                    }
                    return u;
                });

                if (_user == null)
                {
                    RadMessageBox.Show("No s'ha pogut carregar el perfil de l'usuari.", "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
                    return;
                }

                Sett.Default.UltimoUsuario = email;
                Sett.Default.UltimaContra = string.Empty;
                Sett.Default.Save();

                await OpenMainMenuAsync();
            }
            catch (DatabaseException dbEx)
            {
                RadMessageBox.Show("Error en la base de dades: " + dbEx.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("Error inesperat: " + ex.Message, "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        private async Task OpenMainMenuAsync()
        {
            Hide();
            try
            {
                using (var main = new MainForm(_user))
                {
                    main.ShowDialog();
                }
            }
            finally
            {
                await Task.Yield();
                Close();
            }
        }

        // -------- Helpers para asegurar Icon válido y evitar leaks --------

        // Si el recurso ya es Icon, devuélvelo tal cual.
        private static Icon EnsureIconFromResource(Icon iconResource) => iconResource;

        // Si el recurso es Bitmap (PNG/JPG), conviértelo a Icon.
        private static Icon EnsureIconFromResource(Bitmap bitmapResource)
        {
            if (bitmapResource == null) throw new ArgumentNullException(nameof(bitmapResource));
            IntPtr hIcon = bitmapResource.GetHicon();
            try
            {
                using (var icon = Icon.FromHandle(hIcon))
                {
                    return (Icon)icon.Clone(); // clona para poder liberar el handle
                }
            }
            finally
            {
                DestroyIcon(hIcon);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private void radButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new VerActualizacion())
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show("No s'ha pogut obrir el visor de PDF:\n" + ex.Message,
                                   "Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }
    }
}
