namespace ASM_Registres_NET10.Modules
{
    public class Empleados
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Iniciales { get; set; }
        public string Activo { get; set; }
        public string Clave { get; set; }

        public Empleados(int idUsuario, string nombreUsuario, string password, string iniciales, string activo)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Password = password;
            Iniciales = iniciales;
            Activo = activo;
        }


        /*Constructor nuevo de Empleados, que recivirá la Clave*/
        public Empleados(int idUsuario, string nombreUsuario, string password, string iniciales, string activo, string clave) 
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Password = password;
            Iniciales = iniciales;
            Activo = activo;
            Clave = clave;
        }
    }
}
