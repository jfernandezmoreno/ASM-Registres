using Telerik.WinControls;

namespace ASM_Registres.Forms
{
    partial class ReLogIn
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
            this.cancelButton = new Telerik.WinControls.UI.RadButton();
            this.loginButton = new Telerik.WinControls.UI.RadButton();
            this.passwordTextbox = new Telerik.WinControls.UI.RadTextBox();
            this.usernameTextbox = new Telerik.WinControls.UI.RadTextBox();
            this.passwordLabel = new Telerik.WinControls.UI.RadLabel();
            this.usernameLabel = new Telerik.WinControls.UI.RadLabel();
            this.pictureBoxAdditiusLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.cancelButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loginButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.passwordTextbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usernameTextbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.passwordLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usernameLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdditiusLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(74, 238);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 24);
            this.cancelButton.TabIndex = 20;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(167, 238);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(87, 24);
            this.loginButton.TabIndex = 14;
            this.loginButton.Text = "Login";
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click_1);
            // 
            // passwordTextbox
            // 
            this.passwordTextbox.Location = new System.Drawing.Point(94, 179);
            this.passwordTextbox.Name = "passwordTextbox";
            this.passwordTextbox.Size = new System.Drawing.Size(160, 24);
            this.passwordTextbox.TabIndex = 19;
            // 
            // usernameTextbox
            // 
            this.usernameTextbox.Location = new System.Drawing.Point(94, 139);
            this.usernameTextbox.Name = "usernameTextbox";
            this.usernameTextbox.Size = new System.Drawing.Size(160, 24);
            this.usernameTextbox.TabIndex = 18;
            // 
            // passwordLabel
            // 
            this.passwordLabel.Location = new System.Drawing.Point(21, 184);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 18);
            this.passwordLabel.TabIndex = 17;
            this.passwordLabel.Text = "Password";
            // 
            // usernameLabel
            // 
            this.usernameLabel.Location = new System.Drawing.Point(20, 144);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(56, 18);
            this.usernameLabel.TabIndex = 16;
            this.usernameLabel.Text = "Username";
            // 
            // pictureBoxAdditiusLogo
            // 
            this.pictureBoxAdditiusLogo.Location = new System.Drawing.Point(34, 27);
            this.pictureBoxAdditiusLogo.Name = "pictureBoxAdditiusLogo";
            this.pictureBoxAdditiusLogo.Size = new System.Drawing.Size(203, 70);
            this.pictureBoxAdditiusLogo.TabIndex = 15;
            this.pictureBoxAdditiusLogo.TabStop = false;
            // 
            // ReLogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 276);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.passwordTextbox);
            this.Controls.Add(this.usernameTextbox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.pictureBoxAdditiusLogo);
            this.Name = "ReLogIn";
            this.Text = "Log In";
            this.Load += new System.EventHandler(this.ReLogIn_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cancelButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loginButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.passwordTextbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usernameTextbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.passwordLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usernameLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdditiusLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private Telerik.WinControls.UI.RadButton cancelButton;
        private Telerik.WinControls.UI.RadButton loginButton;
        private Telerik.WinControls.UI.RadTextBox passwordTextbox;
        private Telerik.WinControls.UI.RadTextBox usernameTextbox;
        private Telerik.WinControls.UI.RadLabel passwordLabel;
        private Telerik.WinControls.UI.RadLabel usernameLabel;
        private System.Windows.Forms.PictureBox pictureBoxAdditiusLogo;
    }
}
