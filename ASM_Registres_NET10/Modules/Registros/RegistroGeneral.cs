using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistroGeneral
    {
        public int Id { get; set; }               
        public string AccRealitzada { get; set; } 
        public DateTime Data { get; set; }        
        public string TipusMto { get; set; }      
        public double DuracioH { get; set; }      
        public string Observacions { get; set; }  
        public string Empresa { get; set; }       
        public bool Nateja { get; set; }
        public string FetaPer { get; set; }
    }

}
