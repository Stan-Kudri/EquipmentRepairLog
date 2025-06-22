using System.Collections;

namespace EquipmentRepairDocument.Core.Exceptions.AppException
{
    public class NotFoundException : BusinessLogicException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static NotFoundException Create<TEntity, TValue>(string propertyName, TValue propertyValue)
            => new NotFoundException(FormatMessage(propertyName, propertyValue, typeof(TEntity).Name));

        public static NotFoundException Create<TEntity>(params (string PropertyName, object? PropertyValue)[] properties)
            => new NotFoundException(FormatMessage(properties, typeof(TEntity).Name));

        public static string FormatMessage<T>(string propertyName, T propertyValue, string entityName)
            => $"Unknown {entityName} {propertyName} '{propertyValue}'";

        public static string FormatMessage(IEnumerable<(string PropertyName, object? PropertyValue)> properties, string entityName)
        {
            var propDetail = string.Join(", ", properties.Select(p => $"{p.PropertyName} '{FormatValue(p.PropertyValue)}'"));
            return $"Unknown {entityName} [{propDetail}]";

            static string FormatValue(object? value)
            {
                return value switch
                {
                    null => "null",
                    IEnumerable enumerable => "[" + string.Join(", ", enumerable.Cast<object>()) + "]",
                    _ => value.ToString() ?? string.Empty,
                };
            }
        }
    }
}
