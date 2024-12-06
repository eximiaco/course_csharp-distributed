using M3.Desafio.SeedWork;

namespace M3.Desafio.Inscricoes.Eventos;

public record InscricaoRealizadaEvento(Guid Id, string Responsavel) : INotification;
