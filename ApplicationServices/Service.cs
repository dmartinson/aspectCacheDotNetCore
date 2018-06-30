using AspectCache.Cache;

namespace ApplicationServices
{
    public class Service : IService
    {
        [UseCache("week")]
        public string getValue(string id)
        {
            return "whateverTest";
        }
    }
}
