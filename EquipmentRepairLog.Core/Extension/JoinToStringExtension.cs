namespace EquipmentRepairLog.Core.Extension
{
    public static class JoinToStringExtension
    {
        public static string JoinToString<T>(this IEnumerable<T> self, string separator = ", ") => string.Join(separator, self);
    }
}
