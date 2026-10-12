namespace Ipco.Domain;

public enum ErrorType
{
    None,
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden
}

public sealed record Error
{
    public static readonly Error None = new("", "", ErrorType.None, allowNone: true);

    public Error(string code, string description, ErrorType type) : this(code, description, type, allowNone: false) { }

    private Error(string code, string description, ErrorType type, bool allowNone)
    {
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(description);

        if (type == ErrorType.None && !allowNone)
            throw new ArgumentException("ErrorType.None is reserved for Error.None.", nameof(type));

        if (!allowNone && string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("An error code is required.", nameof(code));

        if (!allowNone && string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("An error description is required.", nameof(description));

        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
    public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);
    public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);
}
