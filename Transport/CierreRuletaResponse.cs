using JEMP_API_HeyGIA.Modelos;

namespace JEMP_API_HeyGIA.Transport
{
    public class CierreRuletaResponse
    {
        public int NumeroGanador { get; set; }
        public string ColorGanador { get; set; }
        public List<ResultadoApuesta> Resultados { get; set; } = new();
    }
}
