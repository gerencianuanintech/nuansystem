namespace NuanSystem.Application.Features.SriTxtImports.Services;

public sealed class SriTxtParseException(string code, string message, string field = "File")
    : Exception(message)
{
    public string Code { get; } = code;
    public string Field { get; } = field;
}
