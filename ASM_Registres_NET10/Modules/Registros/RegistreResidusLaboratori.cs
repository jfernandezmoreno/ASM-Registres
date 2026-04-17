using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistreResidusLaboratori
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Laborant { get; set; }
        public string Residu { get; set; }
        public string Quantitat { get; set; }
        public string QuantitatLitres { get; set; }
    }
}
