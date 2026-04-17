using System;

namespace ASM_Registres_NET10.Modules.Tasks
{
    public class TaskParticipation
    {
        public long Id { get; set; }
        public int IdTasca { get; set; }       // FK a tasks_schema.tasks(id)
        public int IdEmpleado { get; set; }    // FK a businesscentralsync.employees(id_usuario)
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Observaciones { get; set; }
    }

    public class TaskParticipationView
    {
        public long Id { get; set; }
        public int IdEmpleado { get; set; }
        public string Empleado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
