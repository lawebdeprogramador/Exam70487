using System;
using System.Runtime.Caching;
using Xunit;

namespace Exam70487._1._2___Implement_caching
{
    public class Cache
    {
        [Theory]
        [InlineData("Cache1", 1)]
        [InlineData("Cache2", 2)]
        [InlineData("Cache3", 3)]
        public void CanCache(string key, int value)
        {
            ObjectCache cache = MemoryCache.Default;
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1)),
                Priority = CacheItemPriority.Default
            };

            cache.Remove(key);
            cache.Add(key, value, policy);
            int fetchedValue = (int)cache.Get(key);

            //"Uh oh!"
            Assert.Equal(value,fetchedValue);
        }
    }

}
