namespace EquipmentRepairLog.Core.Extension
{
    public static class EquipmentValidationExtension
    {
        public static bool KKSValidation(this string kss, out List<string> result)
        {
            result = new List<string>();
            if (string.IsNullOrEmpty(kss))
            {
                return false;
            }
            return true;
        }
    }
}
