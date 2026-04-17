using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class ControlRegistroLimpiezaProduccion
    {
        public int Id { get; set; }
        public int IdRegistroLimpiezaProduccion { get; set; }
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFinal { get; set; }
        public string Accion { get; set; }
        public string Iniciales { get; set; }
        public int KgRecogidos { get; set; }
        public int KgSilice { get; set; }
        public string LoteSilice { get; set; }
        public int KgCarbonato { get; set; }
        public string LoteCarbonato { get; set; }
        public string Lote { get; set; }
        public string Integridad { get; set; }
        public bool Finalizada { get; set; }
        public string Observaciones { get; set; }
    }
}
