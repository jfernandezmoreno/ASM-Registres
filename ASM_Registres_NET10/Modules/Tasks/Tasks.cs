using System;

namespace ASM_Registres_NET10.Modules.Tasks
{
    public class Tasks
    {
        public int Id { get; set; }
        public DateTime Dia { get; set; }
        public string Turno { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; }
        public string PersonaAsignada { get; set; }
        public bool Completada { get; set; }
        public string Comentario { get; set; }
        public string Estado { get; set; } = "No empezada";
        public int? Tasca_Id { get; set; }
        public int? Employee_Id { get; set; }

        public Tasks() { }

        public Tasks(DateTime dia, string turno, string tipo, string titulo, int prioridad,
                     string descripcion = null, string comentario = null, string personaAsignada = null, bool completada = false)
            : this(dia, turno, tipo, titulo, prioridad, descripcion, comentario, personaAsignada, completada,
                   null, null, null)
        { }

        public Tasks(DateTime dia, string turno, string tipo, string titulo, int prioridad,
                     string descripcion = null, string comentario = null, string personaAsignada = null, bool completada = false,
                     string estado = null, int? tasca_id = null, int? employee_id = null)
        {
            Dia = dia;
            Turno = turno;
            Tipo = tipo;
            Titulo = titulo;
            Prioridad = prioridad;
            Descripcion = descripcion;
            Comentario = comentario;
            PersonaAsignada = personaAsignada;
            Completada = completada;

            Estado = estado ?? Estado;
            Tasca_Id = tasca_id;
            Employee_Id = employee_id;
        }
    }
}
