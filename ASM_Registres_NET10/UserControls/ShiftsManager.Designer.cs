namespace ASM_Registres.UserControls
{
    partial class ShiftsManager
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition3 = new Telerik.WinControls.UI.TableViewDefinition();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.radCalendar5 = new Telerik.WinControls.UI.RadCalendar();
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.radPanel2 = new Telerik.WinControls.UI.RadPanel();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCalendar5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).BeginInit();
            this.radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            this.SuspendLayout();
            // 
            // radGridView1
            // 
            this.radGridView1.Location = new System.Drawing.Point(1125, 6);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition3;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(1022, 806);
            this.radGridView1.TabIndex = 4;
            // 
            // radCalendar5
            // 
            this.radCalendar5.HeaderHeight = 28;
            this.radCalendar5.HeaderWidth = 28;
            this.radCalendar5.Location = new System.Drawing.Point(3, 6);
            this.radCalendar5.Name = "radCalendar5";
            this.radCalendar5.Size = new System.Drawing.Size(1116, 806);
            this.radCalendar5.TabIndex = 3;
            this.radCalendar5.SelectionChanged += new System.EventHandler(this.radCalendar5_SelectionChanged);
            // 
            // radPanel1
            // 
            this.radPanel1.Controls.Add(this.radPanel2);
            this.radPanel1.Location = new System.Drawing.Point(12, 835);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(2135, 76);
            this.radPanel1.TabIndex = 5;
            // 
            // radPanel2
            // 
            this.radPanel2.Controls.Add(this.radButton1);
            this.radPanel2.Location = new System.Drawing.Point(1938, 3);
            this.radPanel2.Name = "radPanel2";
            this.radPanel2.Size = new System.Drawing.Size(167, 70);
            this.radPanel2.TabIndex = 1;
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(12, 1);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(143, 67);
            this.radButton1.TabIndex = 0;
            this.radButton1.Text = "Guardar Turnos Semana";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // ShiftsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPanel1);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.radCalendar5);
            this.Name = "ShiftsManager";
            this.Size = new System.Drawing.Size(2154, 914);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radCalendar5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).EndInit();
            this.radPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadCalendar radCalendar5;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private Telerik.WinControls.UI.RadButton radButton1;
    }
}
