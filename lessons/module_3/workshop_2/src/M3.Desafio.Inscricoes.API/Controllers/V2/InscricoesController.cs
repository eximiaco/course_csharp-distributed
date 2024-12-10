using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace M3.Desafio.Inscricoes.API.Controllers.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2")]
[ApiController]
public class InscricoesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetV2()
        => Ok(new { version = "2", datetime = DateTime.Now });
}
