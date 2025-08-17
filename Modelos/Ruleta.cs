namespace JEMP_API_HeyGIA.Modelos
{
    public class Ruleta
    {
        public int Id { get; set; }
        public bool EstaAbierta { get; set; } = false;
        public List<Apuesta> Apuestas { get; set; } = new();
    }

}
