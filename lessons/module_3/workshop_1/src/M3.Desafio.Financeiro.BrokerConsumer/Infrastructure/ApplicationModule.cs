using Autofac;
using M3.Desafio.SeedWork.EfCore;
using M3.Desafio.SeedWork.ServiceBus.Silverback;
using M3.Desafio.SeedWork.ServiceBus;
using M3.Desafio.SeedWork;

namespace M3.Desafio.Financeiro.BrokerConsumer.Infrastructure;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterAssemblyTypes(typeof(Mensalidade).Assembly)
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

        builder
            .RegisterType<SilverbackServiceBus>()
            .As<IServiceBus>()
            .InstancePerLifetimeScope();
    }
}
