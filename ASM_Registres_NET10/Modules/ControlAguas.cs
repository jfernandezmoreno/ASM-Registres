using System;

namespace ASM_Registres_NET10.Modules
{
    public class ControlAguas
    {
        public int Id { get; set; }           
        public DateTime Fecha { get; set; }    
        public double ControlAguasValue { get; set; } 
        public double CloroLibre { get; set; }  
        public double Nitritos { get; set; }   
        public double Ph { get; set; }          
        public string Dureza { get; set; }    
        public string Olor { get; set; }      
        public string Turbidez { get; set; }  
        public string Operario { get; set; }   
        public string DQS { get; set; }
        public string Observaciones { get; set; }
    }

}
