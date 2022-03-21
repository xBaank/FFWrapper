
using System.Collections.Generic;

namespace FFmpegWrapper.Extensions
{
    internal static class ICollectionExtensions
    {
        internal static void AddRange(this ICollection<string> collection, IEnumerable<string> collectionToAdd)
        {
            foreach (var item in collectionToAdd)
            {
                collection.Add(item);
            }
        }
    }
}
