using System;

namespace ASM_Registres_NET10.Modules
{
    public class Incidencia
    {
        public int Id { get; set; }
        public int IdTasca { get; set; }
        public int IdGrupTasques { get; set; }
        public DateTime Data { get; set; }
        public string NomTasca { get; set; }
        public string NomGrup { get; set; }
        public string DescripcioIncidencia { get; set; }
        public bool Resolta { get; set; }
        public string Solucio {  get; set; }

        public bool esResolta()
        {
            if (Resolta) return true;
            else return false;
        }
    }
}
