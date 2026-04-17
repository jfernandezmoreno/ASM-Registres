using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistreFaseMovil
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int LaborantId { get; set; }
        public bool? HexaTick { get; set; }
        public bool? AcetatEtilTick { get; set; }
        public bool? AcetonaTick { get; set; }
        public string HexaBatch { get; set; } = string.Empty;
        public string AcetatBatch { get; set; } = string.Empty;
        public string AcetonaBatch { get; set; } = string.Empty;
    }
}


