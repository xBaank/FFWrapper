
using System.Collections.Generic;

namespace FFmpegWrapper.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddRange(this ICollection<string> collection, IEnumerable<string> collectionToAdd)
        {
            foreach (var item in collectionToAdd)
            {
                collection.Add(item);
            }
        }
    }
}
