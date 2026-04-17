namespace ASM_Registres.Forms
{
    partial class ControlResidusForm
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
            this.panel = new Telerik.WinControls.UI.RadPanel();
            this.plenaLabel = new Telerik.WinControls.UI.RadLabel();
            this.plenaCheckbox = new Telerik.WinControls.UI.RadCheckBox();
            this.qEnLitresTextBox = new Telerik.WinControls.UI.RadTextBoxControl();
            this.quantitatTextBox = new Telerik.WinControls.UI.RadTextBoxControl();
            this.residuCombobox = new System.Windows.Forms.ComboBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.quantitatLabel = new Telerik.WinControls.UI.RadLabel();
            this.residuLabel = new Telerik.WinControls.UI.RadLabel();
            this.saveButton = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plenaLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.plenaCheckbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.qEnLitresTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.quantitatTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.quantitatLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.residuLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.saveButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Controls.Add(this.plenaLabel);
            this.panel.Controls.Add(this.plenaCheckbox);
            this.panel.Controls.Add(this.qEnLitresTextBox);
            this.panel.Controls.Add(this.quantitatTextBox);
            this.panel.Controls.Add(this.residuCombobox);
            this.panel.Controls.Add(this.radLabel1);
            this.panel.Controls.Add(this.quantitatLabel);
            this.panel.Controls.Add(this.residuLabel);
            this.panel.Controls.Add(this.saveButton);
            this.panel.Location = new System.Drawing.Point(4, 2);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(374, 277);
            this.panel.TabIndex = 0;
            // 
            // plenaLabel
            // 
            this.plenaLabel.Location = new System.Drawing.Point(90, 185);
            this.plenaLabel.Name = "plenaLabel";
            this.plenaLabel.Size = new System.Drawing.Size(38, 18);
            this.plenaLabel.TabIndex = 8;
            this.plenaLabel.Text = "Plena?";
            // 
            // plenaCheckbox
            // 
            this.plenaCheckbox.Location = new System.Drawing.Point(145, 185);
            this.plenaCheckbox.Name = "plenaCheckbox";
            this.plenaCheckbox.Size = new System.Drawing.Size(18, 18);
            this.plenaCheckbox.TabIndex = 7;
            this.plenaCheckbox.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.plenaCheckbox_ToggleStateChanged);
            // 
            // qEnLitresTextBox
            // 
            this.qEnLitresTextBox.Location = new System.Drawing.Point(145, 127);
            this.qEnLitresTextBox.Name = "qEnLitresTextBox";
            this.qEnLitresTextBox.Size = new System.Drawing.Size(142, 22);
            this.qEnLitresTextBox.TabIndex = 6;
            // 
            // quantitatTextBox
            // 
            this.quantitatTextBox.Location = new System.Drawing.Point(145, 77);
            this.quantitatTextBox.Name = "quantitatTextBox";
            this.quantitatTextBox.Size = new System.Drawing.Size(142, 22);
            this.quantitatTextBox.TabIndex = 5;
            this.quantitatTextBox.TextChanged += new System.EventHandler(this.quantitatTextBox_TextChanged);
            // 
            // residuCombobox
            // 
            this.residuCombobox.FormattingEnabled = true;
            this.residuCombobox.Items.AddRange(new object[] {
            "Dissolvent",
            "Vidre",
            "Envassos"});
            this.residuCombobox.Location = new System.Drawing.Point(145, 22);
            this.residuCombobox.Name = "residuCombobox";
            this.residuCombobox.Size = new System.Drawing.Size(142, 21);
            this.residuCombobox.TabIndex = 4;
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(30, 132);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(98, 18);
            this.radLabel1.TabIndex = 3;
            this.radLabel1.Text = "Quantitat en litres:";
            // 
            // quantitatLabel
            // 
            this.quantitatLabel.Location = new System.Drawing.Point(72, 81);
            this.quantitatLabel.Name = "quantitatLabel";
            this.quantitatLabel.Size = new System.Drawing.Size(56, 18);
            this.quantitatLabel.TabIndex = 2;
            this.quantitatLabel.Text = "Quantitat:";
            // 
            // residuLabel
            // 
            this.residuLabel.Location = new System.Drawing.Point(86, 25);
            this.residuLabel.Name = "residuLabel";
            this.residuLabel.Size = new System.Drawing.Size(42, 18);
            this.residuLabel.TabIndex = 1;
            this.residuLabel.Text = "Residu:";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(251, 211);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(110, 48);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Guardar";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // ControlResidusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 282);
            this.Controls.Add(this.panel);
            this.Name = "ControlResidusForm";
            this.Text = "Control Residus";
            this.Load += new System.EventHandler(this.ControlResidusForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plenaLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.plenaCheckbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.qEnLitresTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.quantitatTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.quantitatLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.residuLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.saveButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanel panel;
        private Telerik.WinControls.UI.RadLabel quantitatLabel;
        private Telerik.WinControls.UI.RadLabel residuLabel;
        private Telerik.WinControls.UI.RadButton saveButton;
        private Telerik.WinControls.UI.RadLabel plenaLabel;
        private Telerik.WinControls.UI.RadCheckBox plenaCheckbox;
        private Telerik.WinControls.UI.RadTextBoxControl qEnLitresTextBox;
        private Telerik.WinControls.UI.RadTextBoxControl quantitatTextBox;
        private System.Windows.Forms.ComboBox residuCombobox;
        private Telerik.WinControls.UI.RadLabel radLabel1;
    }
}
