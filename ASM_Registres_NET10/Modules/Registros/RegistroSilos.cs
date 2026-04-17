using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistroSilos
    {
        public int Id { get; set; }               
        public DateTime Fecha { get; set; }       
        public float Precinto { get; set; }         
        public string PrecintadoPor { get; set; } 
        public string NombreTarea { get; set; }   
        public string NombreGrupo { get; set; }  
    }

}
