using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistroLimpiezaProduccion
    {
        public int Id { get; set; }
        public string Operario { get; set; }
        public DateTime Fecha { get; set; }
        public bool Finalizada {  get; set; }
        public string Tipo { get; set; }
    }
}
