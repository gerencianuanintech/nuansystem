using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriDocuments.Commands;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Application.Features.SriDocuments.Services;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriDocumentQueueHandlerTests
{
    private readonly ISriDocumentQueuePolicy _policy = Substitute.For<ISriDocumentQueuePolicy>();
    private readonly ISriDocumentQueueRepository _repository = Substitute.For<ISriDocumentQueueRepository>();

    [Fact]
    public async Task Enqueue_ReturnsExistingQueue_WhenAccessKeyWasAlreadyRegistered()
    {
        var key = SriAccessKeyTests.BuildKey("01", '2');
        _policy.ValidateEnqueueAsync(SriEnvironmentCodes.Production, Arg.Any<CancellationToken>()).Returns(Result<bool>.Success(true));
        _repository.EnqueueAsync(Arg.Any<EnqueueSriDocumentData>(), Arg.Any<CancellationToken>())
            .Returns(new SriDocumentQueuePersistenceResult(Queue(21, key), IsCreated: false));
        var handler = new EnqueueSriDocumentCommandHandler(_policy, _repository);

        var result = await handler.Handle(new EnqueueSriDocumentCommand(
            " production ", key, " manual ", " source-1 ", null, 5, null, 7, " admin "), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(21);
        result.Message.Should().Contain("ya estaba encolada");
        await _repository.Received(1).EnqueueAsync(
            Arg.Is<EnqueueSriDocumentData>(data => data.Environment == "Production" && data.SourceType == "Manual" && data.DocumentTypeCode == "01"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Enqueue_DoesNotPersist_WhenTenantPolicyIsDisabled()
    {
        _policy.ValidateEnqueueAsync(SriEnvironmentCodes.Production, Arg.Any<CancellationToken>())
            .Returns(Result<bool>.Failure("disabled", [new ApiError("SRI_FEATURE_DISABLED", "disabled")]));
        var handler = new EnqueueSriDocumentCommandHandler(_policy, _repository);

        var result = await handler.Handle(new EnqueueSriDocumentCommand(
            SriEnvironmentCodes.Production, SriAccessKeyTests.BuildKey("04", '2'), SriSourceTypeCodes.Manual,
            "source-2", null, 5, null, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "SRI_FEATURE_DISABLED");
        await _repository.DidNotReceive().EnqueueAsync(Arg.Any<EnqueueSriDocumentData>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(SriDocumentQueueActionCode.ConcurrencyConflict, "SRI_QUEUE_CONCURRENCY_CONFLICT")]
    [InlineData(SriDocumentQueueActionCode.InvalidState, "SRI_QUEUE_INVALID_STATE")]
    public async Task Cancel_ReturnsStableError_ForRejectedDatabaseAction(SriDocumentQueueActionCode action, string errorCode)
    {
        _repository.CancelAsync(Arg.Any<SriDocumentQueueActionData>(), Arg.Any<CancellationToken>()).Returns(action);
        var handler = new CancelSriDocumentCommandHandler(_repository);

        var result = await handler.Handle(new CancelSriDocumentCommand(4, new byte[8], "reason", 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == errorCode);
    }

    private static SriDocumentQueueDetailDto Queue(long id, string key) => new()
    {
        Id = id,
        Environment = SriEnvironmentCodes.Production,
        AccessKey = key,
        DocumentTypeCode = "01",
        SourceType = SriSourceTypeCodes.Manual,
        SourceReference = "source-1",
        Status = SriDocumentQueueStatusCodes.Pending,
        Priority = 5,
        TraceId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        RowVersion = new byte[8]
    };
}
