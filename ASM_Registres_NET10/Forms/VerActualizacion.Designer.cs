namespace ASM_Registres_NET10.Forms
{
    partial class VerActualizacion
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
            radPdfViewer1 = new Telerik.WinControls.UI.RadPdfViewer();
            ((System.ComponentModel.ISupportInitialize)radPdfViewer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this).BeginInit();
            SuspendLayout();
            // 
            // radPdfViewer1
            // 
            radPdfViewer1.Location = new System.Drawing.Point(0, 1);
            radPdfViewer1.Name = "radPdfViewer1";
            radPdfViewer1.Size = new System.Drawing.Size(1107, 791);
            radPdfViewer1.TabIndex = 0;
            radPdfViewer1.ThumbnailsScaleFactor = 0.15F;
            // 
            // VerActualizacion
            // 
            AutoScaleBaseSize = new System.Drawing.Size(7, 15);
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1108, 794);
            Controls.Add(radPdfViewer1);
            Name = "VerActualizacion";
            Text = "Última actualització";
            ((System.ComponentModel.ISupportInitialize)radPdfViewer1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPdfViewer radPdfViewer1;
    }
}
