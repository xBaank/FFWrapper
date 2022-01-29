
namespace WebmOpus.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNullOrWhiteSpaced(this object? obj)
        {
            return obj == null || obj.IsString() && string.IsNullOrWhiteSpace((string?)obj);
        }

        public static bool IsString(this object? obj)
        {
            return obj?.GetType() == typeof(string) ;
        }
    }
}
