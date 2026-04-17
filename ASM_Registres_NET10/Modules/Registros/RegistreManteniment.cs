using System;

namespace ASM_Registres_NET10.Modules.Registros
{
    public class RegistreManteniment
    {
        public string Id { get; set; }                       
        public int IdTascaInterna { get; set; }          
        public int IdGrup { get; set; }                   
        public string NomTasca { get; set; }             
        public DateTime ProperaDataProgramada { get; set; } 
        public string Observacions { get; set; }          
        public string FetaPer { get; set; }              

        public RegistreManteniment() { }

    }
}
