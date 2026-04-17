namespace ASM_Registres_NET10.Modules
{
    public class CBascPetites
    {
        public int Id { get; set; }
        public int GrupTasquesId { get; set; }
        public string TipusEquip { get; set; }
        public string NomEquip { get; set; }
        public double ValorMin { get; set; }
        public double ValorMax { get; set; }
        public string CaracteristiquesControl { get; set; }
        public string Periodicitat { get; set; }
        public int AccCorrectores { get; set; }
        public string Responsable { get; set; }

        public CBascPetites() { }
    }

}
