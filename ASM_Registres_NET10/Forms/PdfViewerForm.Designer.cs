namespace ASM_Registres.Forms
{
    partial class PdfViewerForm
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
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.radPdfViewerNavigator1 = new Telerik.WinControls.UI.RadPdfViewerNavigator();
            this.radPdfViewer1 = new Telerik.WinControls.UI.RadPdfViewer();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPdfViewerNavigator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPdfViewer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radButton1
            // 
            this.radButton1.Location = new System.Drawing.Point(1243, 906);
            this.radButton1.Name = "radButton1";
            this.radButton1.Size = new System.Drawing.Size(140, 39);
            this.radButton1.TabIndex = 6;
            this.radButton1.Text = "Enviar PDF";
            this.radButton1.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radPdfViewerNavigator1
            // 
            this.radPdfViewerNavigator1.Location = new System.Drawing.Point(22, 35);
            this.radPdfViewerNavigator1.Name = "radPdfViewerNavigator1";
            this.radPdfViewerNavigator1.Size = new System.Drawing.Size(1361, 40);
            this.radPdfViewerNavigator1.TabIndex = 5;
            // 
            // radPdfViewer1
            // 
            this.radPdfViewer1.Location = new System.Drawing.Point(22, 81);
            this.radPdfViewer1.Name = "radPdfViewer1";
            this.radPdfViewer1.Size = new System.Drawing.Size(1361, 816);
            this.radPdfViewer1.TabIndex = 4;
            this.radPdfViewer1.ThumbnailsScaleFactor = 0.15F;
            // 
            // PdfViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1432, 973);
            this.Controls.Add(this.radButton1);
            this.Controls.Add(this.radPdfViewerNavigator1);
            this.Controls.Add(this.radPdfViewer1);
            this.Name = "PdfViewerForm";
            this.Text = "Veure PDF";
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPdfViewerNavigator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPdfViewer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton radButton1;
        private Telerik.WinControls.UI.RadPdfViewerNavigator radPdfViewerNavigator1;
        private Telerik.WinControls.UI.RadPdfViewer radPdfViewer1;
    }
}
