using FluentAssertions;
using NSubstitute;
using System.Text.Json;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriDocuments.Services;
using NuanSystem.Application.Features.SriTxtImports;
using NuanSystem.Application.Features.SriTxtImports.Commands;
using NuanSystem.Application.Features.SriTxtImports.Dtos;
using NuanSystem.Application.Features.SriTxtImports.Services;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Tests.Features.SriTxtImports;

public sealed class SriTxtImportCommandTests
{
    [Fact]
    public void UploadValidator_RejectsDeclaredIso88591()
    {
        using var stream = new MemoryStream([1]);
        var command = new UploadSriTxtImportCommand(
            "received.txt",
            1,
            "text/plain; charset=iso-8859-1",
            stream,
            Guid.NewGuid(),
            1,
            "tester");

        var result = new UploadSriTxtImportCommandValidator().Validate(command);

        result.Errors.Should().Contain(error => error.ErrorCode == "SRI_TXT_ENCODING_NOT_ALLOWED");
    }

    [Fact]
    public void UploadValidator_RejectsLengthDifferentFromStream()
    {
        using var stream = new MemoryStream([1, 2]);
        var command = new UploadSriTxtImportCommand(
            "received.txt",
            1,
            "text/plain",
            stream,
            Guid.NewGuid(),
            1,
            "tester");

        var result = new UploadSriTxtImportCommandValidator().Validate(command);

        result.Errors.Should().Contain(error => error.ErrorCode == "SRI_TXT_FILE_SIZE_MISMATCH");
    }

    [Fact]
    public void ParsedRow_DoesNotSerializeTransientAccessKey()
    {
        var row = ParsedFile(SriTxtRowValidationStatusCodes.Valid).Rows.Single();

        var json = JsonSerializer.Serialize(row);

        json.Should().NotContain(row.AccessKey!);
        json.Should().Contain(row.MaskedAccessKey!);
    }

    [Fact]
    public async Task Upload_PersistsParsedRowsWithoutReturningAccessKeys()
    {
        var parser = Substitute.For<ISriTxtFileParser>();
        var repository = Substitute.For<ISriTxtImportRepository>();
        using var content = new MemoryStream([1]);
        var parsed = ParsedFile(SriTxtRowValidationStatusCodes.Valid);
        parser.ParseAsync(content, Arg.Any<CancellationToken>()).Returns(parsed);
        repository.RegisterValidatedAsync(Arg.Any<RegisterValidatedSriTxtImportData>(), Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportPersistenceResult(Detail(), true));
        var handler = new UploadSriTxtImportCommandHandler(parser, repository);

        var result = await handler.Handle(
            new UploadSriTxtImportCommand(
                @"C:\unsafe\received.txt",
                1,
                "text/plain",
                content,
                Guid.NewGuid(),
                7,
                " tester "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.GetType().GetProperties().Select(property => property.Name)
            .Should().NotContain("AccessKey");
        await repository.Received(1).RegisterValidatedAsync(
            Arg.Is<RegisterValidatedSriTxtImportData>(data =>
                data.OriginalFileName == "received.txt" &&
                data.Rows.Single().AccessKey != null &&
                data.AuditUserName == "tester"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Enqueue_ValidatesEveryStagedEnvironmentBeforePersistence()
    {
        var policy = Substitute.For<ISriDocumentQueuePolicy>();
        var repository = Substitute.For<ISriTxtImportRepository>();
        repository.GetStagedEnvironmentsAsync(10, Arg.Any<CancellationToken>())
            .Returns([SriEnvironmentCodes.Production]);
        policy.ValidateEnqueueAsync(SriEnvironmentCodes.Production, Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Success(true));
        repository.EnqueueAsync(Arg.Any<EnqueueSriTxtImportData>(), Arg.Any<CancellationToken>())
            .Returns(new SriTxtImportEnqueuePersistenceResult(SriTxtImportEnqueueCode.Updated, Detail()));
        var handler = new EnqueueSriTxtImportCommandHandler(policy, repository);

        var result = await handler.Handle(
            new EnqueueSriTxtImportCommand(10, new byte[8], Guid.NewGuid(), 7, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await policy.Received(1).ValidateEnqueueAsync(
            SriEnvironmentCodes.Production,
            Arg.Any<CancellationToken>());
        await repository.Received(1).EnqueueAsync(
            Arg.Any<EnqueueSriTxtImportData>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Enqueue_DoesNotPersistWhenQueuePolicyFails()
    {
        var policy = Substitute.For<ISriDocumentQueuePolicy>();
        var repository = Substitute.For<ISriTxtImportRepository>();
        repository.GetStagedEnvironmentsAsync(10, Arg.Any<CancellationToken>())
            .Returns([SriEnvironmentCodes.Production]);
        policy.ValidateEnqueueAsync(SriEnvironmentCodes.Production, Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Failure(
                "disabled",
                [new ApiError("SRI_FEATURE_DISABLED", "disabled")]));
        var handler = new EnqueueSriTxtImportCommandHandler(policy, repository);

        var result = await handler.Handle(
            new EnqueueSriTxtImportCommand(10, new byte[8], Guid.NewGuid(), 7, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await repository.DidNotReceive().EnqueueAsync(
            Arg.Any<EnqueueSriTxtImportData>(),
            Arg.Any<CancellationToken>());
    }

    private static SriTxtParsedFile ParsedFile(string status) =>
        new(
            new byte[32],
            SriTxtEncodingCodes.Utf8,
            "header",
            [
                new SriTxtParsedRow(
                    Guid.NewGuid(),
                    2,
                    new byte[32],
                    new string('1', 49),
                    new byte[32],
                    "********11111111",
                    "0999999999001",
                    "Emisor",
                    "01",
                    "Factura",
                    "001-001-000000001",
                    SriEnvironmentCodes.Production,
                    DateTime.UtcNow,
                    new DateTime(2026, 7, 1),
                    "0199999999001",
                    1,
                    0.15m,
                    1.15m,
                    null,
                    status,
                    null,
                    null)
            ]);

    private static SriTxtImportDetailDto Detail() =>
        new()
        {
            Id = 10,
            GlobalId = Guid.NewGuid(),
            OriginalFileName = "received.txt",
            FileSha256Hex = new string('0', 64),
            EncodingCode = SriTxtEncodingCodes.Utf8,
            Status = SriTxtImportStatusCodes.Validated,
            TotalRows = 1,
            ValidRows = 1,
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[8]
        };
}
