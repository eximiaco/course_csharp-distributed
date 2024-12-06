using CSharpFunctionalExtensions;
using Dapper;
using M3.Desafio.SeedWork;
using M3.Desafio.SeedWork.EfCore;
using Microsoft.EntityFrameworkCore;

namespace M3.Desafio.Inscricoes;

public class InscricoesRepositorio(IEFDbContextAccessor<OtelDbContext> dbContext) : IService<InscricoesRepositorio>
{
    public async Task<bool> AlunoExiste(string aluno)
    {
        var result = await dbContext.Get().Database.GetDbConnection()
            .QueryFirstOrDefaultAsync<string>("SELECT codigo FROM Alunos WHERE codigo = @aluno",
            new { aluno });

        return result == aluno;
    }

    public async Task<bool> ResponsavelExiste(string responsavel)
    {
        var result = await dbContext.Get().Database.GetDbConnection()
            .QueryFirstOrDefaultAsync<string>("SELECT codigo FROM Responsaveis WHERE codigo = @responsavel",
                new { responsavel });
        return result == responsavel;
    }

    public async Task<Maybe<Turma>> RecuperarTurma(int id, CancellationToken cancellationToken)
    {
        var turma = await dbContext.Get().Turmas.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return turma ?? Maybe<Turma>.None;
    }

    public async Task Adicionar(Inscricao inscricao, CancellationToken cancellationToken)
    {
        await dbContext.Get().Inscricoes.AddAsync(inscricao, cancellationToken);
    }
}
