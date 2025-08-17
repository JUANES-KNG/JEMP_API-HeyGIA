using JEMP_API_HeyGIA.Services;
using JEMP_API_HeyGIA.Transport;
using Microsoft.AspNetCore.Mvc;

namespace RuletaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuletaController : ControllerBase
    {
        private readonly IRuletaService _svc;

        public RuletaController(IRuletaService svc)
        {
            _svc = svc;
        }

        // POST: api/ruleta/crear
        [HttpPost("crear")]
        public async Task<IActionResult> CrearRuleta()
        {
            var id = await _svc.CrearRuletaAsync();
            return Ok(new { Id = id });
        }

        // PUT: api/ruleta/abrir/5
        [HttpPut("abrir/{id:int}")]
        public async Task<IActionResult> AbrirRuleta(int id)
        {
            var ok = await _svc.AbrirRuletaAsync(id);
            return ok
                ? Ok(new { Estado = "Ruleta abierta" })
                : NotFound(new { Error = "Ruleta no encontrada" });
        }

        // POST: api/ruleta/apostar/5
        [HttpPost("apostar/{id:int}")]
        public async Task<IActionResult> Apostar(int id, [FromBody] BetRequestDto apuesta)
        {
            var usuarioId = Request.Headers["UsuarioId"].ToString();
            var (ok, mensaje) = await _svc.ApostarAsync(id, usuarioId, apuesta);

            return ok
                ? Ok(new { Estado = mensaje })
                : BadRequest(new { Error = mensaje });
        }

        // PUT: api/ruleta/cerrar/5
        [HttpPut("cerrar/{id:int}")]
        public async Task<IActionResult> CerrarRuleta(int id)
        {
            try
            {
                var resultado = await _svc.CerrarRuletaAsync(id);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
