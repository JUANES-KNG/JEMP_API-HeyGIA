namespace JEMP_API_HeyGIA.Modelos
{
    public class Apuesta
    {
        public int Id { get; set; }
        public int RuletaId { get; set; }
        public Ruleta Ruleta { get; set; }
        public string UsuarioId { get; set; } // desde headers
        public int? Numero { get; set; } // 0-36
        public string Color { get; set; } // "rojo" o "negro"
        public decimal Monto { get; set; }
    }

}
