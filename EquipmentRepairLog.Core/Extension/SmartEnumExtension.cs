using Ardalis.SmartEnum;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.Extension
{
    public static class SmartEnumExtension
    {
        public static void SmartEnumConversion<T>(this PropertyBuilder<T> type)
            where T : SmartEnum<T>
            => type.HasConversion(x => x.Name, x => SmartEnum<T>.FromName(x, false));
    }
}
