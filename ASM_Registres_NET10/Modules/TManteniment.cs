using System;

namespace ASM_Registres_NET10.Modules
{
    public class TManteniment
    {
        public int Id { get; set; }                
        public string Nom { get; set; }            
        public string Periodicitat { get; set; }    
        public string Zona { get; set; }      
        public DateTime Previst {  get; set; }

        public TManteniment() { }
    }
}
