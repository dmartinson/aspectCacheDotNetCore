using AspectCache.Cache;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspectCache.Behaviors
{
    public class CachingInterceptionBehavior : IInterceptionBehavior
    {
        private readonly ICache cache;

        public CachingInterceptionBehavior(ICache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public bool WillExecute => true;

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var cacheAttr = GetCacheAttribute(input);

            if (cacheAttr == null)
            {
                // Not to be cached.
                return getNext()(input, getNext);
            }

            GetKeys(input, out var methodArgumentsToKey, out var cacheItemKey);

            if (!string.IsNullOrEmpty(methodArgumentsToKey))
            {
                var objectFromCache = this.cache.Get(cacheItemKey);
                if (objectFromCache != null)
                {
                    return input.CreateMethodReturn(objectFromCache);
                }
            }

            var result = getNext()(input, getNext);


            if (string.IsNullOrEmpty(methodArgumentsToKey))
            {
                return result;
            }

            if (result?.ReturnValue == null)
            {
                return result;
            }

            var method = input.MethodBase as MethodInfo;
            if (method != null && typeof(Task) == method.ReturnType)
            {
                //If method is async we should wait for the task to be completed
                ((Task)result.ReturnValue).GetAwaiter().GetResult();
            }

            this.cache.Add(cacheItemKey, result.ReturnValue, cacheAttr);

            return result;
        }

        private static UseCacheAttribute GetCacheAttribute(IMethodInvocation input)
        {
            var attribute = input.Target.GetType().GetMethods().FirstOrDefault(m => m.Name == input.MethodBase.Name)
                        ?.GetCustomAttributes(typeof(UseCacheAttribute), false).FirstOrDefault() as UseCacheAttribute;
            return attribute;
        }

        private static void GetKeys(IMethodInvocation input, out string methodArgumentsToKey, out string cacheItemKey)
        {
            methodArgumentsToKey = string.Empty;
            var count = 0;
            foreach (var argument in input.Arguments)
            {
                if (count > 0)
                {
                    methodArgumentsToKey += "_";
                }
                methodArgumentsToKey = $"{methodArgumentsToKey}{input.Arguments.GetParameterInfo(count).Name}:{argument}";
                count++;
            }

            cacheItemKey = $"{input.Target}.{input.MethodBase}:{methodArgumentsToKey}";
        }
    }
}
