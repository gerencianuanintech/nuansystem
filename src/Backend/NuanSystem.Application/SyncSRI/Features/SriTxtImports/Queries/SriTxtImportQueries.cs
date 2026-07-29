using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Application.Features.SriTxtImports.Queries;

public sealed record GetSriTxtImportsQuery(SriTxtImportFilter Filter)
    : IQuery<SriTxtImportPageDto>;

public sealed record GetSriTxtImportByIdQuery(long ImportId)
    : IQuery<SriTxtImportDetailDto>;

public sealed record GetSriTxtImportRowsQuery(long ImportId, SriTxtImportRowFilter Filter)
    : IQuery<SriTxtImportRowPageDto>;
