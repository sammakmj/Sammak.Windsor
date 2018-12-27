using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sammak.Windsor.Controllers;
using Sammak.Windsor.Installers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Sammak.Windsor.Tests
{
    [TestClass]
    public class ControllersInstallerTests
    {
        #region Properties and Constructor

        private IWindsorContainer _containerWithControllers;
        
        public ControllersInstallerTests()
        {
            _containerWithControllers = new WindsorContainer()
                .Install(new ControllersInstaller());
        }

        #endregion

        #region Tests

        [TestMethod]
        public void All_controllers_implement_IController()
        {
            var allHandlers = GetAllHandlers(_containerWithControllers);
            var controllerHandlers = GetHandlersFor(typeof(IController), _containerWithControllers);

            Assert.IsNotNull(allHandlers);
            Assert.IsNotNull(controllerHandlers);
            CollectionAssert.AreEqual(allHandlers, controllerHandlers);
        }

        [TestMethod]
        public void All_controllers_are_registered()
        {
            // Is<TType> is an helper, extension method from Windsor in the Castle.Core.Internal namespace
            // which behaves like 'is' keyword in C# but at a Type, not instance level
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Is<IController>());
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);

            CollectionAssert.AreEqual(allControllers, registeredControllers);
        }

        [TestMethod]
        public void All_and_only_controllers_have_Controllers_suffix()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Name.EndsWith("Controller"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            CollectionAssert.AreEqual(allControllers, registeredControllers);
        }

        [TestMethod]
        public void All_and_only_controllers_live_in_Controllers_namespace()
        {
            var allControllers = GetPublicClassesFromApplicationAssembly(c => c.Namespace.Contains("Controllers"));
            var registeredControllers = GetImplementationTypesFor(typeof(IController), _containerWithControllers);
            CollectionAssert.AreEqual(allControllers, registeredControllers);
        }

        [TestMethod]
        public void All_controllers_are_transient()
        {
            var nonTransientControllers = GetHandlersFor(typeof(IController), _containerWithControllers)
                .Where(controller => controller.ComponentModel.LifestyleType != LifestyleType.Transient)
                .ToArray();

            Assert.AreEqual(nonTransientControllers.Length, 0);
        }

        [TestMethod]
        public void All_controllers_expose_themselves_as_service()
        {
            var controllersWithWrongName = GetHandlersFor(typeof(IController), _containerWithControllers)
                .Where(controller => controller.ComponentModel.Services.Single() != controller.ComponentModel.Implementation)
                .ToArray();

            Assert.AreEqual(controllersWithWrongName.Length, 0);
        }

        #endregion

        #region Private Methods

        private IHandler[] GetAllHandlers(IWindsorContainer container)
        {
            return GetHandlersFor(typeof(object), container);
        }

        private IHandler[] GetHandlersFor(Type type, IWindsorContainer container)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }

        private Type[] GetImplementationTypesFor(Type type, IWindsorContainer container)
        {
            return GetHandlersFor(type, container)
                .Select(h => h.ComponentModel.Implementation)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private Type[] GetPublicClassesFromApplicationAssembly(Predicate<Type> where)
        {
            return typeof(HomeController).Assembly.GetExportedTypes()
                .Where(t => t.IsClass)
                .Where(t => t.IsAbstract == false)
                .Where(where.Invoke)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        #endregion
    }
}
