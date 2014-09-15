using eTRIKS.Commons.WebAPI.DependencyResolution;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(WindsorActivator), "PreStart")]
[assembly: ApplicationShutdownMethod(typeof(WindsorActivator), "Shutdown")]

namespace eTRIKS.Commons.WebAPI.DependencyResolution
{
    public static class WindsorActivator
    {
        static ContainerBootstrapper _bootstrapper;

        public static void PreStart()
        {
            _bootstrapper = ContainerBootstrapper.Bootstrap();
        }
        
        public static void Shutdown()
        {
            if (_bootstrapper != null)
                _bootstrapper.Dispose();
        }
    }
}