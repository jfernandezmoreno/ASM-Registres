namespace ASM_Registres.Forms
{
    partial class RegistroComunForm
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
            this.dateTimePicker = new Telerik.WinControls.UI.RadDateTimePicker();
            this.dataLabel = new Telerik.WinControls.UI.RadLabel();
            this.estatLabel = new Telerik.WinControls.UI.RadLabel();
            this.observacionsLabel = new Telerik.WinControls.UI.RadLabel();
            this.textBoxObsv = new Telerik.WinControls.UI.RadTextBoxControl();
            this.tituloLabel = new Telerik.WinControls.UI.RadLabel();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel2 = new Telerik.WinControls.UI.RadPanel();
            this.estadoCombo = new System.Windows.Forms.ComboBox();
            this.radButton2 = new Telerik.WinControls.UI.RadButton();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimePicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.estatLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observacionsLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxObsv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tituloLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).BeginInit();
            this.radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CalendarSize = new System.Drawing.Size(290, 320);
            this.dateTimePicker.Location = new System.Drawing.Point(106, 17);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(164, 24);
            this.dateTimePicker.TabIndex = 6;
            this.dateTimePicker.TabStop = false;
            this.dateTimePicker.Text = "martes, 24 de septiembre de 2024";
            this.dateTimePicker.Value = new System.DateTime(2024, 9, 24, 10, 9, 51, 672);
            // 
            // dataLabel
            // 
            this.dataLabel.Location = new System.Drawing.Point(65, 22);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(32, 18);
            this.dataLabel.TabIndex = 1;
            this.dataLabel.Text = "Data:";
            // 
            // estatLabel
            // 
            this.estatLabel.Location = new System.Drawing.Point(55, 67);
            this.estatLabel.Name = "estatLabel";
            this.estatLabel.Size = new System.Drawing.Size(33, 18);
            this.estatLabel.TabIndex = 2;
            this.estatLabel.Text = "Estat:";
            // 
            // observacionsLabel
            // 
            this.observacionsLabel.Location = new System.Drawing.Point(22, 108);
            this.observacionsLabel.Name = "observacionsLabel";
            this.observacionsLabel.Size = new System.Drawing.Size(76, 18);
            this.observacionsLabel.TabIndex = 3;
            this.observacionsLabel.Text = "Observacions:";
            // 
            // textBoxObsv
            // 
            this.textBoxObsv.Location = new System.Drawing.Point(106, 109);
            this.textBoxObsv.Name = "textBoxObsv";
            this.textBoxObsv.Size = new System.Drawing.Size(164, 127);
            this.textBoxObsv.TabIndex = 0;
            // 
            // tituloLabel
            // 
            this.tituloLabel.Location = new System.Drawing.Point(3, 10);
            this.tituloLabel.Name = "tituloLabel";
            this.tituloLabel.Size = new System.Drawing.Size(55, 18);
            this.tituloLabel.TabIndex = 5;
            this.tituloLabel.Text = "radLabel1";
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.tituloLabel);
            this.radPanel1.Location = new System.Drawing.Point(0, 3);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(303, 38);
            this.radPanel1.TabIndex = 6;
            // 
            // radPanel2
            // 
            this.radPanel2.Controls.Add(this.estadoCombo);
            this.radPanel2.Controls.Add(this.radButton2);
            this.radPanel2.Controls.Add(this.radButton1);
            this.radPanel2.Controls.Add(this.dataLabel);
            this.radPanel2.Controls.Add(this.dateTimePicker);
            this.radPanel2.Controls.Add(this.estatLabel);
            this.radPanel2.Controls.Add(this.textBoxObsv);
            this.radPanel2.Controls.Add(this.observacionsLabel);
            this.radPanel2.Location = new System.Drawing.Point(0, 47);
            this.radPanel2.Name = "radPanel2";
            this.radPanel2.Size = new System.Drawing.Size(303, 297);
            this.radPanel2.TabIndex = 7;
            // 
            // estadoCombo
            // 
            this.estadoCombo.FormattingEnabled = true;
            this.estadoCombo.Location = new System.Drawing.Point(106, 67);
            this.estadoCombo.Name = "estadoCombo";
            this.estadoCombo.Size = new System.Drawing.Size(164, 21);
            this.estadoCombo.TabIndex = 3;
            // 
            // radButton2
            // 
            this.radButton2.Location = new System.Drawing.Point(86, 255);
            this.radButton2.Name = "radButton2";
            this.radButton2.Size = new System.Drawing.Size(96, 32);
            this.radButton2.TabIndex = 6;
            this.radButton2.Text = "Descartar";
            this.radButton2.Click += new System.EventHandler(this.radButton2_Click);
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(197, 255);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(96, 32);
            this.radButton1.TabIndex = 5;
            this.radButton1.Text = "Guardar";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // RegistroComunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 345);
            this.Controls.Add(this.radPanel2);
            this.Controls.Add(this.radPanel1);
            this.Name = "RegistroComunForm";
            this.Text = "Registre Comú";
            ((System.ComponentModel.ISupportInitialize)(this.dateTimePicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.estatLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observacionsLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxObsv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tituloLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).EndInit();
            this.radPanel2.ResumeLayout(false);
            this.radPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadDateTimePicker dateTimePicker;
        private Telerik.WinControls.UI.RadLabel dataLabel;
        private Telerik.WinControls.UI.RadLabel estatLabel;
        private Telerik.WinControls.UI.RadLabel observacionsLabel;
        private Telerik.WinControls.UI.RadTextBoxControl textBoxObsv;
        private Telerik.WinControls.UI.RadLabel tituloLabel;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadButton radButton2;
        private Telerik.WinControls.UI.RadButton radButton1;
        private System.Windows.Forms.ComboBox estadoCombo;
    }
}
