using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Application.Features.SriTxtImports.Services;

public interface ISriTxtFileParser
{
    Task<SriTxtParsedFile> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}
