using JEMP_API_HeyGIA.Transport;

namespace JEMP_API_HeyGIA.Services
{
    public interface IRuletaService
    {
        Task<int> CrearRuletaAsync();
        Task<bool> AbrirRuletaAsync(int ruletaId);
        Task<(bool ok, string message)> ApostarAsync(int ruletaId, string usuarioId, BetRequestDto bet);
        Task<CierreRuletaResponse> CerrarRuletaAsync(int ruletaId);
    }
}
