using ASM_Registres_NET10.DatabaseConnections.RepositoriesTasks;
using ASM_Registres_NET10.Modules.Tasks;
using System;
using System.Windows.Forms;

namespace ASM_Registres.Forms
{
    public partial class CopyTaskForm : Telerik.WinControls.UI.RadForm
    {
        Tasks task;

        ManTasksRecurrentRepository rep3;
        LabTasksRecurrentRepository rep2;
        ProdTasksRecurrentRepository rep1;

        int pos = 0;

        public CopyTaskForm(Tasks task, int pos)
        {
            InitializeComponent(); 

            rep1 = new ProdTasksRecurrentRepository();
            rep2 = new LabTasksRecurrentRepository();
            rep3 = new ManTasksRecurrentRepository();

            this.task = task;

            Icon = global::ASM_Registres_NET10.Properties.Resources.Logo_a_;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            radDateTimePicker1.Value = DateTime.Today;

            this.pos = pos;
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            task.Dia = radDateTimePicker1.Value;

            if (pos == 1)
            {
                rep1.AddTask(task);
            }
            else if (pos == 2) 
            { 
                rep2.AddTask(task);
            }
            else
            {
                rep3.AddTask(task);
            }
            Close();
        }
    }
}
