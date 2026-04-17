using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistreReprocesatMostresLaboratori
    {
        public int Id { get; set; }
        public string Lot { get; set; }
        public DateTime Data { get; set; }
        public double Pes { get; set; }
        public string Laborant { get; set; }
        public string Producte { get; set; }

        public RegistreReprocesatMostresLaboratori() { }
    }
}
