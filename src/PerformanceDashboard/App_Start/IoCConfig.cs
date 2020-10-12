using PerformanceDashboard.Entity;
using PerformanceDashboard.IoC;
using PerformanceDashboard.Service;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;

namespace PerformanceDashboard
{
    public static class IoCConfig
    {
        /// <summary>
        /// Sets up initial configuration for the MVC part of the application.
        /// </summary>
        public static void Register()
        {
            ConfigureDIContainer();
        }

        /// <summary>
        /// Registers types for dependency injection.
        /// </summary>
        private static void ConfigureDIContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<DashboardContext>(new TransientLifetimeManager());
            container.RegisterType<IDataService, RealtimeDataService>(new TransientLifetimeManager());
            DependencyResolver.SetResolver(new UnityResolver(container));
        }
    }

}