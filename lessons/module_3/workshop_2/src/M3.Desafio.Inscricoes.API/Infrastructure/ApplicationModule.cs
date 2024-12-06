using Autofac;
using M3.Desafio.SeedWork;
using M3.Desafio.SeedWork.EfCore;

namespace M3.Desafio.Inscricoes.API.Infrastructure;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterAssemblyTypes(typeof(Inscricao).Assembly)
            .AsClosedTypesOf(typeof(IService<>))
            .InstancePerLifetimeScope();
        builder
            .RegisterType<OtelDbContextFactory>()
            .As<IEFDbContextFactory<OtelDbContext>>()
            .InstancePerLifetimeScope();
        builder
            .RegisterType<OtelDbContextAccessor>()
            .As<IEFDbContextAccessor<OtelDbContext>>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();
    }
}
