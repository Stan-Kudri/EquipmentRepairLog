using System.Text.RegularExpressions;
using EquipmentRepairDocument.Core.Exceptions;

namespace EquipmentRepairDocument.Core.Data.ValidationData
{
    public class KKS
    {
        public string Value { get; }

        public KKS(string? value)
        {
            value = value?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                throw new BusinessLogicException("KKS cannot be null or empty");
            }

            if (!Regex.IsMatch(value, "^[0-9]{2}[A-Z]{3}[0-9]{2}[A-Z]{2}[0-9]{3}$", RegexOptions.Compiled))
            {
                throw new BusinessLogicException($"KKS should be valid '{value}'");
            }

            Value = value;
        }

        public static IEnumerable<Result<KKS?>> CreateArray(string str)
        {
            var arrayKKS = str.Split([',', ' ', '-', '.'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var item in arrayKKS)
            {
                KKS? obj = null;
                string? errorMessage = null;
                try
                {
                    obj = new KKS(item);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                if (obj is not null)
                {
                    yield return Result<KKS?>.Ok(obj);
                }
                else
                {
                    yield return Result<KKS?>.Error(errorMessage);
                }
            }
        }

        public override string ToString() => Value;
    }
}
