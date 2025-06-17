namespace EquipmentRepairDocument.Core.Extension
{
    public static class JoinErrorExtension
    {
        public static string JoinErrorToString<T>(this IEnumerable<Result<T>> result, string? separation = null)
            => result.Where(e => e.HasError).Select(e => e.ErrorMessage).JoinToString(separation ?? Environment.NewLine);
    }
}
