using M3.Desafio.Financeiro.Telemetria;
using M3.Desafio.SeedWork;

namespace M3.Desafio.Financeiro.Comandos;

public class GerarMensalidadesParaNovaInscricaoHandler(
    MensalidadesRepositorio mensalidadesRepositorio,
    IUnitOfWork unitOfWork,
    GerarMensalidadeParaNovaInscricaoTelemetry gerarMensalidadeParaNovaInscricaoTelemetry) : IService<GerarMensalidadesParaNovaInscricaoHandler>
{
    public async Task Executar(GerarMensalidadesParaNovaInscricaoComando comando, CancellationToken cancellationToken)
    {
        gerarMensalidadeParaNovaInscricaoTelemetry.NovaInscricaoParaGeracaoMensalidadeRecebida(comando);

        List<Mensalidade> mensalidades =
        [
            Mensalidade.Criar(comando.InscricaoId, comando.Responsavel, 100).Value,
            Mensalidade.Criar(comando.InscricaoId, comando.Responsavel, 100).Value,
            Mensalidade.Criar(comando.InscricaoId, comando.Responsavel, 100).Value
        ];

        await mensalidadesRepositorio.Adicionar(mensalidades, cancellationToken).ConfigureAwait(false);
        await unitOfWork.Commit(cancellationToken).ConfigureAwait(false);
        gerarMensalidadeParaNovaInscricaoTelemetry.MensalidadesGeradasComSucesso(mensalidades);
    }
}
