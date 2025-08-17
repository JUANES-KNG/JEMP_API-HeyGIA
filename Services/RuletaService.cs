using JEMP_API_HeyGIA.Data;
using JEMP_API_HeyGIA.Modelos;
using JEMP_API_HeyGIA.Transport;
using Microsoft.EntityFrameworkCore;

namespace JEMP_API_HeyGIA.Services
{
    namespace RuletaApi.Services
    {
        public class RuletaService : IRuletaService
        {
            private readonly AppDbContext _db;
            private static readonly Random _rng = new();
            private static readonly object _rngLock = new();

            public RuletaService(AppDbContext db)
            {
                _db = db;
            }

            public async Task<int> CrearRuletaAsync()
            {
                var r = new Ruleta { EstaAbierta = false };
                _db.Ruletas.Add(r);
                await _db.SaveChangesAsync();
                return r.Id;
            }

            /// <summary>
            /// Abre una ruleta (si ya está abierta, no hace nada).
            /// </summary>
            public async Task<bool> AbrirRuletaAsync(int ruletaId)
            {
                var r = await _db.Ruletas.FindAsync(ruletaId);
                if (r is null) return false;
                if (r.EstaAbierta) return true; // ya abierta
                r.EstaAbierta = true;
                await _db.SaveChangesAsync();
                return true;
            }

            public async Task<(bool ok, string message)> ApostarAsync(int ruletaId, string usuarioId, BetRequestDto bet)
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(usuarioId))
                    return (false, "Falta UsuarioId en headers.");

                if (bet is null) return (false, "Body inválido.");

                var tieneNumero = bet.Numero.HasValue;
                var tieneColor = !string.IsNullOrWhiteSpace(bet.Color);

                if (tieneNumero == tieneColor)
                    return (false, "Debes apostar a un número O a un color (no ambos, no ninguno).");

                if (tieneNumero && (bet.Numero < 0 || bet.Numero > 36))
                    return (false, "Número fuera de rango (0-36).");

                if (tieneColor)
                {
                    bet.Color = bet.Color!.Trim().ToLowerInvariant();
                    if (bet.Color != "rojo" && bet.Color != "negro")
                        return (false, "Color inválido. Usa 'rojo' o 'negro'.");
                }

                if (bet.Monto <= 0) return (false, "El monto debe ser mayor a 0.");
                if (bet.Monto > 10_000) return (false, "El monto supera el máximo permitido (10.000).");

                // Verificar estado de la ruleta
                var r = await _db.Ruletas.Include(x => x.Apuestas).FirstOrDefaultAsync(x => x.Id == ruletaId);
                if (r is null) return (false, "La ruleta no existe.");
                if (!r.EstaAbierta) return (false, "La ruleta está cerrada.");

                // Registrar apuesta
                var ap = new Apuesta
                {
                    RuletaId = ruletaId,
                    UsuarioId = usuarioId,
                    Numero = bet.Numero,
                    Color = tieneColor ? bet.Color : null,
                    Monto = bet.Monto
                };

                _db.Apuestas.Add(ap);
                await _db.SaveChangesAsync();
                return (true, "Apuesta registrada.");
            }

            public async Task<CierreRuletaResponse> CerrarRuletaAsync(int ruletaId)
            {
                var r = await _db.Ruletas.Include(x => x.Apuestas).FirstOrDefaultAsync(x => x.Id == ruletaId);
                if (r is null) throw new InvalidOperationException("La ruleta no existe.");
                if (!r.EstaAbierta) throw new InvalidOperationException("La ruleta ya está cerrada.");

                // Generar número ganador
                int numeroGanador;
                lock (_rngLock)
                {
                    numeroGanador = _rng.Next(0, 37);
                }
                var colorGanador = ColorPorNumero(numeroGanador);

                // Calcular resultados
                var resultados = new List<ResultadoApuesta>();
                foreach (var a in r.Apuestas)
                {
                    var ganador = false;
                    decimal ganancia = 0m;

                    if (a.Numero.HasValue && a.Numero.Value == numeroGanador)
                    {
                        ganador = true;
                        ganancia = a.Monto * 5m;
                    }
                    else if (!string.IsNullOrWhiteSpace(a.Color) &&
                             a.Color!.Trim().ToLowerInvariant() == colorGanador)
                    {
                        ganador = true;
                        ganancia = a.Monto * 1.8m;
                    }

                    resultados.Add(new ResultadoApuesta
                    {
                        UsuarioId = a.UsuarioId,
                        Ganador = ganador,
                        Ganancia = Decimal.Round(ganancia, 2, MidpointRounding.ToEven)
                    });
                }

                //Marca ruleta como "Cerrada"
                r.EstaAbierta = false;
                await _db.SaveChangesAsync();

                return new CierreRuletaResponse
                {
                    NumeroGanador = numeroGanador,
                    ColorGanador = colorGanador,
                    Resultados = resultados
                };
            }

            /// <summary>
            /// Determina color ganador según paridad del número.
            /// </summary>
            private static string ColorPorNumero(int n) =>
                (n % 2 == 0) ? "rojo" : "negro";
        }
    }

}
