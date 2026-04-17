using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistroTemperaturaCamara
    {
        public int Id { get; set; }
        public string Camara { get; set; }
        public DateTime DiaHora { get; set; }
        public bool Paro { get; set; }
        public bool Marcha { get; set; }
        public bool CambioTemperatura { get; set; }
        public float TemperaturaCamara { get; set; }
        public string Operario { get; set; }
    }

}
