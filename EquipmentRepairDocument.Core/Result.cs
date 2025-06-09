namespace EquipmentRepairDocument.Core
{
    public class Result<T>
    {
        public T? Value { get; }

        public string? ErrorMessage { get; }

        public bool HasError => ErrorMessage is not null;

        private Result(T? value, string? errorMessage)
        {
            Value = value;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Ok(T value) => new Result<T>(value, null);

        public static Result<T> Error(string? error) => new Result<T>(default, error ?? throw new ArgumentNullException(nameof(error)));

        public override string ToString() => HasError ? "Error: " + ErrorMessage : Value?.ToString() ?? "Error";
    }
}
