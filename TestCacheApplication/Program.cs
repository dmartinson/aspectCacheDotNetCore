using ApplicationServices;
using AspectCache.Configuration;
using AspectCache.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Practices.Unity;
using System;

namespace TestCacheApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //Init dotnet service provider
            //I could have used Unity container directly for this test. Just wanted check how Unity works with .NET core ServiceProvider.
            var serviceProvider = new ServiceCollection()
                .AddScoped<IService, Service>()
                .BuildServiceProvider();

            //Use unity
            var container = new UnityContainer
            {
                AlternativeServiceProvider = serviceProvider
            };

            container.ConfigureCache();

            var service = container.Resolve<IService>();

            //Fix cache behavior only working for methods with parameters
            Console.WriteLine($"First call: { service.getValue() }");

            Console.WriteLine($"This comes from the cache: { service.getValue() }");

            Console.Read();

        }
    }
}
