using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Sammak.Windsor.Repositories;

namespace Sammak.Windsor.Installers
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                   .Where(Component.IsInSameNamespaceAs<EventRepository>())
                                   .WithService.DefaultInterfaces()
                                   .LifestyleTransient()
                                   .Configure(c => c.DependsOn(new { pageSize = 20 })));
        }
    }
}