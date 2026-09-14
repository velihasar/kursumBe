using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Utilities.TaskScheduler.Hangfire
{
    /// <summary>
    /// Hangfire için ASP.NET Core DI container'ını kullanan JobActivator.
    /// Bu activator, IServiceProvider üzerinden servisleri resolve eder.
    /// </summary>
    public class ServiceProviderJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override object ActivateJob(Type jobType)
        {
            return _serviceProvider.GetService(jobType) ?? 
                   Activator.CreateInstance(jobType);
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            return new ServiceProviderScope(_serviceProvider.CreateScope());
        }
    }

    /// <summary>
    /// ServiceProviderJobActivator için scope yönetimi.
    /// Her job için ayrı bir scope oluşturur ve dispose eder.
    /// </summary>
    internal class ServiceProviderScope : JobActivatorScope
    {
        private readonly IServiceScope _serviceScope;

        public ServiceProviderScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public override object Resolve(Type type)
        {
            return _serviceScope.ServiceProvider.GetService(type) ??
                   Activator.CreateInstance(type);
        }

        public override void DisposeScope()
        {
            _serviceScope?.Dispose();
        }
    }
}
