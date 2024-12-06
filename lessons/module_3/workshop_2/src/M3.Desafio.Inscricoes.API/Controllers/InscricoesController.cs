using Asp.Versioning;
using M3.Desafio.Inscricoes.Comandos;
using Microsoft.AspNetCore.Mvc;

namespace M3.Desafio.Inscricoes.API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
[ApiController]
public class InscricoesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RealizarInscricao(
       [FromServices] RealizarInscricaoHandler realizarInscricaoHandler,
       [FromBody] NovaInscricaoModel input,
       CancellationToken cancellationToken)
    {
        RealizarInscricaoComando comando = new(
            input.CpfAluno,
            input.CpfResponsavel,
            input.CodigoTurma);

        var resultado = await realizarInscricaoHandler.Executar(comando, cancellationToken);
        if (resultado.IsFailure)
            return BadRequest(resultado.Error);
        return Ok();
    }
}

public record NovaInscricaoModel(string CpfAluno, string CpfResponsavel, int CodigoTurma);
