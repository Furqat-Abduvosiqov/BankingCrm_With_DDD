namespace Utilities.Shared.Results;

/// <summary>
/// Represents an error with a code and message.
/// </summary>
public class Error
{
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error NotFound(string entity) => new Error("NotFound", $"{entity} not found.");
    public static Error Validation(string message) => new Error("Validation", message);
}
