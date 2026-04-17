using System;

namespace ASM_Registres_NET10.Modules
{
    public class RegistroTurno
    {
        public int IdRegistro { get; set; }
        public DateTime date { get; set; }
        public int IdUsuario { get; set; }
        public string IdTurno { get; set; }
        public string NombreUsuario { get; set; }

        public RegistroTurno(int idRegistro, DateTime date, int idUsuario, string idTurno, string nombreUsuario)
        {
            IdRegistro = idRegistro;
            this.date = date;
            IdUsuario = idUsuario;
            IdTurno = idTurno;
            NombreUsuario = nombreUsuario;
        }

        public RegistroTurno(DateTime date, int idUsuario, string idTurno, string nombreUsuario)
        {
            this.date = date;
            IdUsuario = idUsuario;
            IdTurno = idTurno;
            NombreUsuario = nombreUsuario;
        }
    }
}
