using System;

namespace AspectCache.Cache
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UseCacheAttribute : Attribute
    {
        public string PolicyKey { get; }

        public UseCacheAttribute(string policyKey)
        {
            if (string.IsNullOrEmpty(policyKey))
            {
                throw new ArgumentException($"{nameof(policyKey)} parameter is empty.", nameof(policyKey));
            }
            this.PolicyKey = policyKey;
        }
    }
}
