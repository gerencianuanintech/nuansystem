namespace NuanSystem.Application.Abstractions.Diagnostics;

public interface IErrorSanitizer
{
    string? Sanitize(string? value);
}
