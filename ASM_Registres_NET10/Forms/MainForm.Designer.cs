namespace ASM_Registres.Forms
{
    partial class MainForm
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
            this.radRibbonBar = new Telerik.WinControls.UI.RadRibbonBar();
            this.statusLabel = new Telerik.WinControls.UI.RadStatusStrip();
            this.radLabelElement1 = new Telerik.WinControls.UI.RadLabelElement();
            this.showPanel = new System.Windows.Forms.Panel();
            this.radWaitingBar1 = new Telerik.WinControls.UI.RadWaitingBar();
            this.lineRingWaitingBarIndicatorElement1 = new Telerik.WinControls.UI.LineRingWaitingBarIndicatorElement();
            this.pageViewer = new Telerik.WinControls.UI.RadPageView();
            ((System.ComponentModel.ISupportInitialize)(this.radRibbonBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusLabel)).BeginInit();
            this.showPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radWaitingBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pageViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radRibbonBar
            // 
            this.radRibbonBar.Location = new System.Drawing.Point(0, 0);
            this.radRibbonBar.Name = "radRibbonBar";
            // 
            // 
            // 
            this.radRibbonBar.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.radRibbonBar.Size = new System.Drawing.Size(2374, 146);
            this.radRibbonBar.TabIndex = 0;
            this.radRibbonBar.Text = "MainForm";
            this.radRibbonBar.Click += new System.EventHandler(this.radRibbonBar_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radLabelElement1});
            this.statusLabel.Location = new System.Drawing.Point(0, 985);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(2374, 24);
            this.statusLabel.SizingGrip = false;
            this.statusLabel.TabIndex = 1;
            // 
            // radLabelElement1
            // 
            this.radLabelElement1.Name = "radLabelElement1";
            this.statusLabel.SetSpring(this.radLabelElement1, false);
            this.radLabelElement1.Text = "radLabelElement1";
            this.radLabelElement1.TextWrap = true;
            // 
            // showPanel
            // 
            this.showPanel.Controls.Add(this.radWaitingBar1);
            this.showPanel.Controls.Add(this.pageViewer);
            this.showPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showPanel.Location = new System.Drawing.Point(0, 146);
            this.showPanel.Name = "showPanel";
            this.showPanel.Size = new System.Drawing.Size(2374, 839);
            this.showPanel.TabIndex = 2;
            // 
            // radWaitingBar1
            // 
            this.radWaitingBar1.Location = new System.Drawing.Point(1136, 74);
            this.radWaitingBar1.Name = "radWaitingBar1";
            this.radWaitingBar1.Size = new System.Drawing.Size(70, 70);
            this.radWaitingBar1.TabIndex = 1;
            this.radWaitingBar1.Text = "radWaitingBar1";
            this.radWaitingBar1.WaitingIndicators.Add(this.lineRingWaitingBarIndicatorElement1);
            this.radWaitingBar1.WaitingIndicatorSize = new System.Drawing.Size(100, 14);
            this.radWaitingBar1.WaitingSpeed = 50;
            this.radWaitingBar1.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;
            // 
            // lineRingWaitingBarIndicatorElement1
            // 
            this.lineRingWaitingBarIndicatorElement1.Name = "lineRingWaitingBarIndicatorElement1";
            // 
            // pageViewer
            // 
            this.pageViewer.Location = new System.Drawing.Point(3, 6);
            this.pageViewer.Name = "pageViewer";
            this.pageViewer.Size = new System.Drawing.Size(2368, 827);
            this.pageViewer.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2374, 1009);
            this.Controls.Add(this.showPanel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.radRibbonBar);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.radRibbonBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusLabel)).EndInit();
            this.showPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radWaitingBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pageViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadRibbonBar radRibbonBar;
        private Telerik.WinControls.UI.RadStatusStrip statusLabel;
        private System.Windows.Forms.Panel showPanel;
        private Telerik.WinControls.UI.RadPageView pageViewer;
        private Telerik.WinControls.UI.RadWaitingBar radWaitingBar1;
        private Telerik.WinControls.UI.LineRingWaitingBarIndicatorElement lineRingWaitingBarIndicatorElement1;
        private Telerik.WinControls.UI.RadLabelElement radLabelElement1;
    }
}
