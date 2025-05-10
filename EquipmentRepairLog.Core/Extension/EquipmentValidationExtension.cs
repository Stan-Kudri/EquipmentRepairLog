using System.Text.RegularExpressions;

namespace EquipmentRepairLog.Core.Extension
{
    public static class EquipmentValidationExtension
    {
        private static Regex regexKKS = new Regex(@"^[0-9]{2}[A-Z]{3}[0-9]{2}[A-Z]{2}[0-9]{3}$");

        public static bool KKSValidation(this string str, out List<string> result)
        {
            result = new List<string>();
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            var arrayKKS = str.Split([',', ' ', '-', '.'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var item in arrayKKS)
            {
                if (!regexKKS.IsMatch(item))
                {
                    return false;
                }
            }

            result = arrayKKS.ToList();
            return true;
        }

        public static bool ValidateEquipment(this string str, out string type)
        {
            type = string.Empty;

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            type = str.TrimEnd('.', ' ', ',', '-', '_').TrimStart('.', ' ', ',', '-', '_');
            return true;
        }
    }
}
