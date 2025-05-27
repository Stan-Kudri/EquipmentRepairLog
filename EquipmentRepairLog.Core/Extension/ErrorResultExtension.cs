namespace EquipmentRepairLog.Core.Extension
{
    public static class ErrorResultExtension
    {
        public static string ErrorListMassage<T>(this List<Result<T>> result, string separation = ";\n")
            => result.Where(e => e.HasError).Select(e => e.ErrorMessage).JoinToString($";{Environment.NewLine}");
    }
}
