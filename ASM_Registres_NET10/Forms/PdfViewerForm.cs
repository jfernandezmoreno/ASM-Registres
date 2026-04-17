using System;
using System.Windows.Forms;

namespace ASM_Registres.Forms
{
    public partial class PdfViewerForm : Telerik.WinControls.UI.RadForm
    {
        //MailSenderService service;
        string path;

        public PdfViewerForm(string pdfFilePath)
        {
            InitializeComponent();

            //service = new MailSenderService();

            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;

            radPdfViewerNavigator1.AssociatedViewer = radPdfViewer1;
            radPdfViewer1.LoadDocument(pdfFilePath);

            radPdfViewerNavigator1.Dock = DockStyle.Top;
            radPdfViewer1.Dock = DockStyle.Fill;

            path = pdfFilePath;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            MailSenderForm form = new MailSenderForm(path);
            form.FormClosed += Form_FormClosed;
            form.ShowDialog();
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}
