using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Sammak.Windsor.Services;

namespace Sammak.Windsor.Installers
{
	public class ServiceInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromThisAssembly()
                                .Where(Component.IsInSameNamespaceAs<FormsAuthenticationService>())
			                   	.LifestyleTransient()
			                   	.WithService.DefaultInterfaces());
		}
	}
}
