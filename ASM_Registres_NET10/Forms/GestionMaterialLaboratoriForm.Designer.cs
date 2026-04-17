namespace ASM_Registres.Forms
{
    partial class GestionMaterialLaboratoriForm
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
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.textLabel = new Telerik.WinControls.UI.RadLabel();
            this.textBox = new Telerik.WinControls.UI.RadTextBoxControl();
            this.cancelarButton = new Telerik.WinControls.UI.RadButton();
            this.enviarButton = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cancelarButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.enviarButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.textLabel);
            this.radPanel1.Controls.Add(this.textBox);
            this.radPanel1.Controls.Add(this.cancelarButton);
            this.radPanel1.Controls.Add(this.enviarButton);
            this.radPanel1.Location = new System.Drawing.Point(3, 12);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(597, 299);
            this.radPanel1.TabIndex = 0;
            // 
            // textLabel
            // 
            this.textLabel.Location = new System.Drawing.Point(9, 22);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(42, 18);
            this.textLabel.TabIndex = 3;
            this.textLabel.Text = "Petició:";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(9, 49);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(579, 174);
            this.textBox.TabIndex = 2;
            // 
            // cancelarButton
            // 
            this.cancelarButton.Location = new System.Drawing.Point(351, 243);
            this.cancelarButton.Name = "cancelarButton";
            this.cancelarButton.Size = new System.Drawing.Size(110, 47);
            this.cancelarButton.TabIndex = 1;
            this.cancelarButton.Text = "Cancelar";
            this.cancelarButton.Click += new System.EventHandler(this.cancelarButton_Click);
            // 
            // enviarButton
            // 
            this.enviarButton.Location = new System.Drawing.Point(478, 243);
            this.enviarButton.Name = "enviarButton";
            this.enviarButton.Size = new System.Drawing.Size(110, 47);
            this.enviarButton.TabIndex = 0;
            this.enviarButton.Text = "Enviar";
            this.enviarButton.Click += new System.EventHandler(this.enviarButton_Click);
            // 
            // GestionMaterialLaboratoriForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 314);
            this.Controls.Add(this.radPanel1);
            this.Name = "GestionMaterialLaboratoriForm";
            this.Text = "Tiquet Material";
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cancelarButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.enviarButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadButton enviarButton;
        private Telerik.WinControls.UI.RadButton cancelarButton;
        private Telerik.WinControls.UI.RadLabel textLabel;
        private Telerik.WinControls.UI.RadTextBoxControl textBox;
    }
}
