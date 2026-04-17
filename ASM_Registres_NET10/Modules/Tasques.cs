using System;

namespace ASM_Registres_NET10.Modules
{
    public class Tasques
    {
        public int Id { get; set; }             
        public int IdIntern { get; set; }        
        public int IdGrup { get; set; }          
        public string Titol { get; set; }       
        public string Periodicitat { get; set; } 
        public DateTime Darrera { get; set; }    
        public string Zona { get; set; }


        public override string ToString()
        {
            return Titol + " " + Zona + " " + Darrera + " " + Periodicitat;
        }

    }

    

}
