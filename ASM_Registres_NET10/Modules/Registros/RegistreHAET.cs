using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistreHAET
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int LaborantId { get; set; }
        public bool HexaTick { get; set; }
        public bool AcetonaTick { get; set; }
        public bool EtanolTick { get; set; }
        public bool TouleTick { get; set; }
        public string HexaBatch { get; set; }
        public string AcetonaBatch { get; set; }
        public string EtanolBatch { get; set; }
        public string TolueBatch { get; set; }
    }
}


