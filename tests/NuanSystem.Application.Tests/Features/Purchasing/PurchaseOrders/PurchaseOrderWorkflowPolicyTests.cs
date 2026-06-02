using FluentAssertions;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Tests.Features.Purchasing.PurchaseOrders;

public sealed class PurchaseOrderWorkflowPolicyTests
{
    [Fact]
    public void Draft_ShouldAllowEditableAndConfirmationActions()
    {
        const string status = PurchaseOrderStatuses.Draft;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.GetStatusAfterConfirmation(status, true).Should().Be(PurchaseOrderStatuses.PendingApproval);
        PurchaseOrderWorkflowPolicy.GetStatusAfterConfirmation(status, false).Should().Be(PurchaseOrderStatuses.Approved);
        PurchaseOrderWorkflowPolicy.CanSendToApproval(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanApprove(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanReject(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
    }

    [Fact]
    public void PendingApproval_ShouldOnlyAllowApprovalDecisionAndConsultation()
    {
        const string status = PurchaseOrderStatuses.PendingApproval;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanApprove(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanReject(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanViewRelatedDocuments(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanViewAttachments(status).Should().BeTrue();
    }

    [Fact]
    public void Approved_ShouldOnlyAllowSapRequestAndAllowedCollections()
    {
        const string status = PurchaseOrderStatuses.Approved;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanApprove(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanReject(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanModifyRelatedDocuments(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanModifyAttachments(status).Should().BeTrue();
    }

    [Fact]
    public void Rejected_ShouldAllowCorrectionAndConfirmationAgain()
    {
        const string status = PurchaseOrderStatuses.Rejected;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.GetStatusAfterConfirmation(status, true).Should().Be(PurchaseOrderStatuses.PendingApproval);
        PurchaseOrderWorkflowPolicy.GetStatusAfterConfirmation(status, false).Should().Be(PurchaseOrderStatuses.Approved);
        PurchaseOrderWorkflowPolicy.CanSendToApproval(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanApprove(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
    }

    [Fact]
    public void SapPending_ShouldNotAllowOperationalActions()
    {
        const string status = PurchaseOrderStatuses.SapPending;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanModifyRelatedDocuments(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanModifyAttachments(status).Should().BeFalse();
    }

    [Fact]
    public void SapSynced_ShouldNotAllowOperationalActions()
    {
        const string status = PurchaseOrderStatuses.SapSynced;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanModifyRelatedDocuments(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanModifyAttachments(status).Should().BeFalse();
    }

    [Fact]
    public void SapError_ShouldAllowControlledCorrectionAndSapRetry()
    {
        const string status = PurchaseOrderStatuses.SapError;

        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanRetrySapSync(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.EnsureCanRequestSapSync(status).IsSuccess.Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanModifyRelatedDocuments(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanModifyAttachments(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeFalse();
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Closed)]
    [InlineData(PurchaseOrderStatuses.Cancelled)]
    public void TerminalStatuses_ShouldNotAllowOperationalActions(string status)
    {
        PurchaseOrderWorkflowPolicy.CanEdit(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanDelete(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanConfirm(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanSendToApproval(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanApprove(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanReject(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRequestSapSync(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanRetrySapSync(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanModifyRelatedDocuments(status).Should().BeFalse();
        PurchaseOrderWorkflowPolicy.CanModifyAttachments(status).Should().BeFalse();
    }

    [Theory]
    [InlineData(PurchaseOrderStatuses.Draft)]
    [InlineData(PurchaseOrderStatuses.PendingApproval)]
    [InlineData(PurchaseOrderStatuses.Approved)]
    [InlineData(PurchaseOrderStatuses.Rejected)]
    [InlineData(PurchaseOrderStatuses.SapPending)]
    [InlineData(PurchaseOrderStatuses.SapSynced)]
    [InlineData(PurchaseOrderStatuses.SapError)]
    [InlineData(PurchaseOrderStatuses.Closed)]
    [InlineData(PurchaseOrderStatuses.Cancelled)]
    public void KnownStatuses_ShouldAllowConsultation(string status)
    {
        PurchaseOrderWorkflowPolicy.CanViewRelatedDocuments(status).Should().BeTrue();
        PurchaseOrderWorkflowPolicy.CanViewAttachments(status).Should().BeTrue();
    }

    [Fact]
    public void GetStatusAfterConfirmation_ShouldRejectInvalidSourceStatus()
    {
        var act = () => PurchaseOrderWorkflowPolicy.GetStatusAfterConfirmation(PurchaseOrderStatuses.Approved, true);

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(PurchaseOrderWorkflowPolicy.ConfirmInvalidMessage);
    }

    [Fact]
    public void EnsureMethods_ShouldReturnExpectedMessages()
    {
        PurchaseOrderWorkflowPolicy.EnsureCanSendToApproval(PurchaseOrderStatuses.Approved).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.SendToApprovalInvalidMessage);
        PurchaseOrderWorkflowPolicy.EnsureCanApprove(PurchaseOrderStatuses.Draft).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.ApproveInvalidMessage);
        PurchaseOrderWorkflowPolicy.EnsureCanReject(PurchaseOrderStatuses.Rejected).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.RejectInvalidMessage);
        PurchaseOrderWorkflowPolicy.EnsureCanRequestSapSync(PurchaseOrderStatuses.Draft).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.SapSyncInvalidMessage);
        PurchaseOrderWorkflowPolicy.EnsureCanEdit(PurchaseOrderStatuses.Approved).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.EditInvalidMessage);
        PurchaseOrderWorkflowPolicy.EnsureCanDelete(PurchaseOrderStatuses.SapError).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.DeleteInvalidMessage);
        PurchaseOrderWorkflowPolicy.EnsureCanModifyAttachmentsOrRelatedDocuments(PurchaseOrderStatuses.SapPending).Message
            .Should().Be(PurchaseOrderWorkflowPolicy.ModifyCollectionsInvalidMessage);
    }
}
