namespace ASM_Registres
{
    partial class FormLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cancelButton = new Telerik.WinControls.UI.RadButton();
            loginButton = new Telerik.WinControls.UI.RadButton();
            passwordTextbox = new Telerik.WinControls.UI.RadTextBox();
            usernameTextbox = new Telerik.WinControls.UI.RadTextBox();
            passwordLabel = new Telerik.WinControls.UI.RadLabel();
            usernameLabel = new Telerik.WinControls.UI.RadLabel();
            pictureBoxAdditiusLogo = new System.Windows.Forms.PictureBox();
            telerikMetroBlueTheme1 = new Telerik.WinControls.Themes.TelerikMetroBlueTheme();
            radButton1 = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)cancelButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)loginButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)passwordTextbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)usernameTextbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)passwordLabel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)usernameLabel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAdditiusLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)radButton1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // cancelButton
            // 
            cancelButton.Location = new System.Drawing.Point(74, 240);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(87, 24);
            cancelButton.TabIndex = 13;
            cancelButton.Text = "Sortir";
            cancelButton.Click += cancelButton_Click;
            // 
            // loginButton
            // 
            loginButton.Location = new System.Drawing.Point(167, 240);
            loginButton.Name = "loginButton";
            loginButton.Size = new System.Drawing.Size(87, 24);
            loginButton.TabIndex = 7;
            loginButton.Text = "Login";
            loginButton.Click += loginButton_Click;
            // 
            // passwordTextbox
            // 
            passwordTextbox.Location = new System.Drawing.Point(94, 181);
            passwordTextbox.Name = "passwordTextbox";
            passwordTextbox.Size = new System.Drawing.Size(160, 24);
            passwordTextbox.TabIndex = 12;
            // 
            // usernameTextbox
            // 
            usernameTextbox.Location = new System.Drawing.Point(94, 141);
            usernameTextbox.Name = "usernameTextbox";
            usernameTextbox.Size = new System.Drawing.Size(160, 24);
            usernameTextbox.TabIndex = 11;
            // 
            // passwordLabel
            // 
            passwordLabel.Location = new System.Drawing.Point(12, 185);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(68, 18);
            passwordLabel.TabIndex = 10;
            passwordLabel.Text = "Contrasenya";
            // 
            // usernameLabel
            // 
            usernameLabel.Location = new System.Drawing.Point(13, 146);
            usernameLabel.Name = "usernameLabel";
            usernameLabel.Size = new System.Drawing.Size(38, 18);
            usernameLabel.TabIndex = 9;
            usernameLabel.Text = "Usuari";
            // 
            // pictureBoxAdditiusLogo
            // 
            pictureBoxAdditiusLogo.Location = new System.Drawing.Point(34, 29);
            pictureBoxAdditiusLogo.Name = "pictureBoxAdditiusLogo";
            pictureBoxAdditiusLogo.Size = new System.Drawing.Size(203, 70);
            pictureBoxAdditiusLogo.TabIndex = 8;
            pictureBoxAdditiusLogo.TabStop = false;
            // 
            // radButton1
            // 
            radButton1.Image = ASM_Registres_NET10.Properties.Resources.info;
            radButton1.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            radButton1.Location = new System.Drawing.Point(251, 10);
            radButton1.Name = "radButton1";
            radButton1.Size = new System.Drawing.Size(29, 24);
            radButton1.TabIndex = 14;
            radButton1.Click += radButton1_Click;
            // 
            // FormLogin
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(287, 287);
            Controls.Add(radButton1);
            Controls.Add(cancelButton);
            Controls.Add(loginButton);
            Controls.Add(passwordTextbox);
            Controls.Add(usernameTextbox);
            Controls.Add(passwordLabel);
            Controls.Add(usernameLabel);
            Controls.Add(pictureBoxAdditiusLogo);
            Name = "FormLogin";
            Text = "Registres";
            Load += LoginForm_Load;
            ((System.ComponentModel.ISupportInitialize)cancelButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)loginButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)passwordTextbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)usernameTextbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)passwordLabel).EndInit();
            ((System.ComponentModel.ISupportInitialize)usernameLabel).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAdditiusLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)radButton1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton cancelButton;
        private Telerik.WinControls.UI.RadButton loginButton;
        private Telerik.WinControls.UI.RadTextBox passwordTextbox;
        private Telerik.WinControls.UI.RadTextBox usernameTextbox;
        private Telerik.WinControls.UI.RadLabel passwordLabel;
        private Telerik.WinControls.UI.RadLabel usernameLabel;
        private System.Windows.Forms.PictureBox pictureBoxAdditiusLogo;
        private Telerik.WinControls.Themes.TelerikMetroBlueTheme telerikMetroBlueTheme1;
        private Telerik.WinControls.UI.RadButton radButton1;
    }
}
