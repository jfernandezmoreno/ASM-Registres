namespace ASM_Registres.Forms
{
    partial class AgregarReprocesatMostresLaboratoriForm
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
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.lotTextBox = new Telerik.WinControls.UI.RadTextBoxControl();
            this.pesTextBox = new Telerik.WinControls.UI.RadTextBoxControl();
            this.producteTextBox = new Telerik.WinControls.UI.RadTextBoxControl();
            this.radPanel2 = new Telerik.WinControls.UI.RadPanel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lotTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pesTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.producteTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).BeginInit();
            this.radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.producteTextBox);
            this.radPanel1.Controls.Add(this.pesTextBox);
            this.radPanel1.Controls.Add(this.lotTextBox);
            this.radPanel1.Controls.Add(this.radLabel3);
            this.radPanel1.Controls.Add(this.radLabel2);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Controls.Add(this.radButton1);
            this.radPanel1.Location = new System.Drawing.Point(12, 64);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(360, 200);
            this.radPanel1.TabIndex = 0;
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(237, 159);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(110, 27);
            this.radButton1.TabIndex = 0;
            this.radButton1.Text = "Guardar";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(44, 31);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(24, 18);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "Lot:";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(42, 61);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(26, 18);
            this.radLabel2.TabIndex = 2;
            this.radLabel2.Text = "Pes:";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(14, 92);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(54, 18);
            this.radLabel3.TabIndex = 3;
            this.radLabel3.Text = "Producte:";
            // 
            // lotTextBox
            // 
            this.lotTextBox.Location = new System.Drawing.Point(72, 29);
            this.lotTextBox.Name = "lotTextBox";
            this.lotTextBox.Size = new System.Drawing.Size(275, 22);
            this.lotTextBox.TabIndex = 4;
            // 
            // pesTextBox
            // 
            this.pesTextBox.Location = new System.Drawing.Point(72, 59);
            this.pesTextBox.Name = "pesTextBox";
            this.pesTextBox.Size = new System.Drawing.Size(275, 22);
            this.pesTextBox.TabIndex = 5;
            // 
            // producteTextBox
            // 
            this.producteTextBox.Location = new System.Drawing.Point(72, 90);
            this.producteTextBox.Name = "producteTextBox";
            this.producteTextBox.Size = new System.Drawing.Size(275, 22);
            this.producteTextBox.TabIndex = 6;
            // 
            // radPanel2
            // 
            this.radPanel2.Controls.Add(this.radLabel4);
            this.radPanel2.Location = new System.Drawing.Point(12, 12);
            this.radPanel2.Name = "radPanel2";
            this.radPanel2.Size = new System.Drawing.Size(360, 46);
            this.radPanel2.TabIndex = 1;
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(58, 10);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(253, 25);
            this.radLabel4.TabIndex = 0;
            this.radLabel4.Text = "Reprocessat Mostres Laboratori";
            // 
            // AgregarReprocesatMostresLaboratoriForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 273);
            this.Controls.Add(this.radPanel2);
            this.Controls.Add(this.radPanel1);
            this.Name = "AgregarReprocesatMostresLaboratoriForm";
            this.Text = "Reprocessat Mostres Laboratori";
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lotTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pesTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.producteTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).EndInit();
            this.radPanel2.ResumeLayout(false);
            this.radPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadTextBoxControl lotTextBox;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadButton radButton1;
        private Telerik.WinControls.UI.RadTextBoxControl producteTextBox;
        private Telerik.WinControls.UI.RadTextBoxControl pesTextBox;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadLabel radLabel4;
    }
}
