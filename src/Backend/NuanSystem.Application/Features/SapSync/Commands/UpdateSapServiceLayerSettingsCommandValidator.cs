using FluentValidation;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class UpdateSapServiceLayerSettingsCommandValidator
    : AbstractValidator<UpdateSapServiceLayerSettingsCommand>
{
    public UpdateSapServiceLayerSettingsCommandValidator()
    {
        RuleFor(command => command.ServiceLayerUrl)
            .NotEmpty()
            .MaximumLength(500)
            .Must(BeSecureServiceLayerUrl)
            .WithMessage("La URL debe ser HTTPS, no incluir credenciales y apuntar a /b1s/v1 o /b1s/v2.");

        RuleFor(command => command.SapCompanyDb)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(command => command.SapUser)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(command => command.SapPassword)
            .MaximumLength(256)
            .When(command => !string.IsNullOrEmpty(command.SapPassword));

        RuleFor(command => command.MaxRetryCount)
            .InclusiveBetween(0, 10);
    }

    private static bool BeSecureServiceLayerUrl(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrEmpty(uri.UserInfo))
        {
            return false;
        }

        var path = uri.AbsolutePath.TrimEnd('/');
        return path.EndsWith("/b1s/v1", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith("/b1s/v2", StringComparison.OrdinalIgnoreCase);
    }
}
