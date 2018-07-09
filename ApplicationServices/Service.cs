using AspectCache.Cache;

namespace ApplicationServices
{
    public class Service : IService
    {
        private string returnValue = "WhatEver";
        [UseCache("week")]
        public string getValue(string id)
        {
            return returnValue;
        }

        [UseCache("week")]
        public string getValue()
        {
            return returnValue;
        }
    }
}
