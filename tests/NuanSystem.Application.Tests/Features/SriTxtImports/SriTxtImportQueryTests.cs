using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriTxtImports;
using NuanSystem.Application.Features.SriTxtImports.Dtos;
using NuanSystem.Application.Features.SriTxtImports.Queries;

namespace NuanSystem.Application.Tests.Features.SriTxtImports;

public sealed class SriTxtImportQueryTests
{
    [Fact]
    public void ListValidator_RejectsInvalidPagingDateStatusAndEnvironment()
    {
        var query = new GetSriTxtImportsQuery(
            new SriTxtImportFilter(
                new DateTime(2026, 7, 28),
                new DateTime(2026, 7, 27),
                "Unknown",
                new string('x', 261),
                "Unknown",
                0,
                501));

        var result = new GetSriTxtImportsQueryValidator().Validate(query);

        result.Errors.Select(error => error.ErrorCode).Should().Contain(
        [
            "SRI_TXT_PAGE_INVALID",
            "SRI_TXT_PAGE_SIZE_INVALID",
            "SRI_TXT_STATUS_INVALID",
            "SRI_TXT_ENVIRONMENT_INVALID",
            "SRI_TXT_FILE_NAME_LENGTH",
            "SRI_TXT_DATE_RANGE_INVALID"
        ]);
    }

    [Fact]
    public async Task ListHandler_NormalizesFiltersAndReturnsServerPage()
    {
        var repository = Substitute.For<ISriTxtImportRepository>();
        var expected = new SriTxtImportPageDto(
            [new SriTxtImportListItemDto { Id = 42, OriginalFileName = "fixture.txt" }],
            61,
            2,
            25,
            new SriTxtImportSummaryDto { TotalRows = 123 });
        repository.SearchAsync(Arg.Any<SriTxtImportFilter>(), Arg.Any<CancellationToken>())
            .Returns(expected);
        var handler = new GetSriTxtImportsQueryHandler(repository);

        var result = await handler.Handle(
            new GetSriTxtImportsQuery(
                new SriTxtImportFilter(
                    null,
                    null,
                    " validated ",
                    " fixture ",
                    " production ",
                    2,
                    25)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(expected);
        await repository.Received(1).SearchAsync(
            Arg.Is<SriTxtImportFilter>(filter =>
                filter.Status == SriTxtImportStatusCodes.Validated
                && filter.FileName == "fixture"
                && filter.Environment == SriEnvironmentCodes.Production
                && filter.Page == 2
                && filter.PageSize == 25),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DetailHandler_ReturnsStableNotFoundWithoutCrossTenantFallback()
    {
        var repository = Substitute.For<ISriTxtImportRepository>();
        repository.GetByIdAsync(91, Arg.Any<CancellationToken>())
            .Returns((SriTxtImportDetailDto?)null);
        var handler = new GetSriTxtImportByIdQueryHandler(repository);

        var result = await handler.Handle(new GetSriTxtImportByIdQuery(91), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "SRI_TXT_IMPORT_NOT_FOUND");
        await repository.Received(1).GetByIdAsync(91, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RowsHandler_NormalizesInvalidFilterAndPreservesQueueNavigationId()
    {
        var repository = Substitute.For<ISriTxtImportRepository>();
        var expected = new SriTxtImportRowPageDto(
            [new SriTxtImportRowDto { Id = 7, QueueId = 10004, MaskedAccessKey = "********12345678" }],
            1,
            1,
            100);
        repository.GetRowsAsync(Arg.Any<long>(), Arg.Any<SriTxtImportRowFilter>(), Arg.Any<CancellationToken>())
            .Returns(expected);
        var handler = new GetSriTxtImportRowsQueryHandler(repository);

        var result = await handler.Handle(
            new GetSriTxtImportRowsQuery(5, new SriTxtImportRowFilter(" invalid ", 1, 100)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Single().QueueId.Should().Be(10004);
        await repository.Received(1).GetRowsAsync(
            5,
            Arg.Is<SriTxtImportRowFilter>(filter => filter.Validity == SriTxtRowValidityCodes.Invalid),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void PublicRowContract_ContainsNoCompleteKeyOrRawPayloadMembers()
    {
        var names = typeof(SriTxtImportRowDto).GetProperties().Select(property => property.Name);

        names.Should().NotContain(["AccessKey", "HeaderLine", "OriginalLine", "Xml", "Jwt", "ConnectionString"]);
        names.Should().Contain(["MaskedAccessKey", "QueueId", "QueueStatus"]);
    }
}
