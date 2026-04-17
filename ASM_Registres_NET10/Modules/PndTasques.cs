namespace ASM_Registres_NET10.Modules
{
    public class PndTasques
    {
        public int Id { get; set; }
        public int GrupTasquesId { get; set; }
        public string Zona { get; set; }
        public string Element { get; set; }
        public string Operacio { get; set; }
        public int AccCorrectores { get; set; }
        public string Frequencia { get; set; }
        public string Responsable { get; set; }

        public PndTasques() { }
    }

}
