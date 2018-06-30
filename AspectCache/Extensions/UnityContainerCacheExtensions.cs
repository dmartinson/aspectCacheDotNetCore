using AspectCache.Behaviors;
using AspectCache.Cache;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.Linq;

namespace AspectCache.Extensions
{
    public static class UnityContainerCacheExtensions
    {
        public static void ConfigureCache(this UnityContainer container, string namespaceStarsWith = "")
        {
            container.AddNewExtension<Interception>();
            container.RegisterInstance<ICache>(new Cache.Cache());

            var allClasesWithCacheAttribute = AllClasses.FromLoadedAssemblies().Where(c => c.Namespace != null && (string.IsNullOrEmpty(namespaceStarsWith) || c.Namespace.StartsWith(namespaceStarsWith)))
                    .SelectMany(t => t.GetMethods())
                    .Where(m => m.GetCustomAttributes(typeof(UseCacheAttribute), false).Any()).Select(m => m.DeclaringType).Distinct();

            container.RegisterTypes(
               allClasesWithCacheAttribute,
               WithMappings.FromMatchingInterface,
               getInjectionMembers: t => new InjectionMember[]
               {
                        new Interceptor<InterfaceInterceptor>(),
                        new InterceptionBehavior<CachingInterceptionBehavior>()
               });

        }
    }
}
