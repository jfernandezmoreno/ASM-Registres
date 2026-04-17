using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistroComun
    {
        public int Id { get; set; }             
        public int IdTasca { get; set; }         
        public int IdGrupTasques { get; set; }    
        public string NomTasca { get; set; }      
        public string NomGrup { get; set; }      
        public DateTime Data { get; set; }       
        public string Estat { get; set; }         
        public string Observacions { get; set; }
        public string FetaPer { get; set; }
    }

}
