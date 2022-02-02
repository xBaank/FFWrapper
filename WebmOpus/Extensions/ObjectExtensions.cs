
namespace WebmOpus.Extensions
{
    internal static class ObjectExtensions
    {
        internal static bool IsNullOrWhiteSpaced(this object? obj)
        {
            return obj == null || obj.IsString() && string.IsNullOrWhiteSpace((string?)obj);
        }

        internal static bool IsString(this object? obj)
        {
            return obj?.GetType() == typeof(string);
        }
    }
}
