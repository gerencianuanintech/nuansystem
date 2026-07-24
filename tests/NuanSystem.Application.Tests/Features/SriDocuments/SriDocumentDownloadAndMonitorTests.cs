using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Application.Features.SriDocuments.Commands;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.SriDocuments;
using NuanSystem.WinForms.Services.SriDocuments.Models;
using NuanSystem.WinForms.ViewModels.SriDocuments;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriDocumentDownloadAndMonitorTests
{
    [Fact]
    public async Task DownloadAuthorizedXml_ReturnsBytesWithoutChangingStateContract()
    {
        var repository=Substitute.For<ISriDocumentQueueRepository>();
        repository.DownloadAuthorizedXmlAsync(Arg.Any<SriAuthorizedXmlDownloadData>(),Arg.Any<CancellationToken>())
            .Returns(new SriAuthorizedXmlPersistenceResult(SriAuthorizedXmlDownloadCode.Success,42,7,[1,2,3],"application/xml",3));
        var result=await new DownloadAuthorizedSriXmlCommandHandler(repository).Handle(new DownloadAuthorizedSriXmlCommand(7,9,"tester",Guid.NewGuid()),CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value!.Content.Should().Equal(1,2,3);
        result.Value.ContentType.Should().Be("application/xml");
        result.Value.FileName.Should().Be("sri-7.xml");
        await repository.DidNotReceive().CancelAsync(Arg.Any<SriDocumentQueueActionData>(),Arg.Any<CancellationToken>());
        await repository.DidNotReceive().ReprocessAsync(Arg.Any<SriDocumentQueueActionData>(),Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(SriAuthorizedXmlDownloadCode.NotFound,"SRI_DOCUMENT_NOT_FOUND")]
    [InlineData(SriAuthorizedXmlDownloadCode.NotAuthorized,"SRI_DOCUMENT_NOT_AUTHORIZED")]
    [InlineData(SriAuthorizedXmlDownloadCode.MissingContent,"SRI_DOCUMENT_XML_MISSING")]
    public async Task DownloadAuthorizedXml_ReturnsStableFunctionalErrors(SriAuthorizedXmlDownloadCode code,string expected)
    {
        var repository=Substitute.For<ISriDocumentQueueRepository>();
        repository.DownloadAuthorizedXmlAsync(Arg.Any<SriAuthorizedXmlDownloadData>(),Arg.Any<CancellationToken>()).Returns(new SriAuthorizedXmlPersistenceResult(code,null,7,[],null,0));
        var result=await new DownloadAuthorizedSriXmlCommandHandler(repository).Handle(new DownloadAuthorizedSriXmlCommand(7,null,null,Guid.NewGuid()),CancellationToken.None);
        result.IsSuccess.Should().BeFalse(); result.Errors.Should().ContainSingle(x=>x.Code==expected);
    }

    [Fact]
    public async Task RepeatedDownload_UsesOneDocumentAndAuditsEveryRequestThroughProcedure()
    {
        var repository=Substitute.For<ISriDocumentQueueRepository>();
        repository.DownloadAuthorizedXmlAsync(Arg.Any<SriAuthorizedXmlDownloadData>(),Arg.Any<CancellationToken>()).Returns(new SriAuthorizedXmlPersistenceResult(SriAuthorizedXmlDownloadCode.Success,42,7,[1],"application/xml",1));
        var handler=new DownloadAuthorizedSriXmlCommandHandler(repository);
        await handler.Handle(new DownloadAuthorizedSriXmlCommand(7,1,"user",Guid.NewGuid()),CancellationToken.None);
        await handler.Handle(new DownloadAuthorizedSriXmlCommand(7,1,"user",Guid.NewGuid()),CancellationToken.None);
        await repository.Received(2).DownloadAuthorizedXmlAsync(Arg.Is<SriAuthorizedXmlDownloadData>(x=>x.QueueId==7),Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Download_PropagatesCancellationToken()
    {
        var repository=Substitute.For<ISriDocumentQueueRepository>();
        using var source=new CancellationTokenSource();
        repository.DownloadAuthorizedXmlAsync(Arg.Any<SriAuthorizedXmlDownloadData>(),source.Token).Returns(new SriAuthorizedXmlPersistenceResult(SriAuthorizedXmlDownloadCode.NotFound,null,7,[],null,0));
        await new DownloadAuthorizedSriXmlCommandHandler(repository).Handle(new DownloadAuthorizedSriXmlCommand(7,null,null,Guid.NewGuid()),source.Token);
        await repository.Received(1).DownloadAuthorizedXmlAsync(Arg.Any<SriAuthorizedXmlDownloadData>(),source.Token);
    }

    [Fact]
    public void SqlScript_IsIdempotentPagedTenantScopedAndAuditsWithoutDuplicatingDocument()
    {
        var sql=Read("database","sql","118_tenant_sri_document_monitor_and_download.sql");
        sql.Should().Contain("CREATE OR ALTER PROCEDURE dbo.SP_NA_POST_SRIDOCUMENTAUTORIZADO_DESCARGAR");
        sql.Should().Contain("N'DownloadXml'").And.Contain("OFFSET (@Page-1)*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY");
        sql.Should().Contain("IF NOT EXISTS (SELECT 1 FROM sys.indexes");
        sql.Should().NotContain("INSERT dbo.SriAuthorizedDocuments");
        sql.Should().NotContain("UPDATE dbo.SriDocumentQueue SET Status");
    }

    [Fact]
    public void Api_RequiresPermissionsAndEmitsProtectedFileHeaders()
    {
        var source=Read("src","Backend","NuanSystem.Api","Endpoints","SriDocumentEndpoints.cs");
        source.Should().Contain("PermissionCodes.SriDocumentsDownloadXml").And.Contain("PermissionCodes.SriDocumentsViewPayload");
        source.Should().Contain("Results.NotFound(response)").And.Contain("Headers.CacheControl = \"no-store\"");
        source.Should().Contain("Results.File(result.Value!.Content, \"application/xml\", result.Value.FileName)");
    }

    [Fact]
    public void Persistence_UsesOnlyAuthenticatedTenantConnectionAndSafeProjections()
    {
        var repository=Read("src","Backend","NuanSystem.Persistence","Repositories","SriDocumentQueueRepository.cs");
        var sql=Read("database","sql","118_tenant_sri_document_monitor_and_download.sql");
        var dtos=Read("src","Backend","NuanSystem.Application","Features","SriDocuments","Dtos","SriDocumentQueueDtos.cs");
        repository.Should().Contain("ITenantConnectionFactory").And.NotContain("IMasterConnectionFactory");
        sql.Should().NotContain("NuanSystem_Master").And.NotContain("AccessKey");
        var monitorSection=dtos[dtos.IndexOf("SriDocumentMonitorSummaryDto",StringComparison.Ordinal)..];
        monitorSection.Should().NotContain("AccessKey").And.NotContain("ILogger");
    }

    [Fact]
    public void Frontend_UsesTypedTransportDesignerControlsAndSaveFileDialog()
    {
        var client=Read("src","Frontend","NuanSystem.WinForms.Services","SriDocuments","SriDocumentMonitorClient.cs");
        var form=Read("src","Frontend","NuanSystem.WinForms.Forms","SriDocuments","SriDocumentMonitorForm.cs");
        var designer=Read("src","Frontend","NuanSystem.WinForms.Forms","SriDocuments","SriDocumentMonitorForm.Designer.cs");
        client.Should().Contain("INuanApiClient").And.NotContain("new HttpClient");
        form.Should().Contain("SaveFileDialog").And.Contain("DialogResult.OK").And.Contain("File.WriteAllBytesAsync").And.NotContain("Path.GetTempPath");
        designer.Should().Contain("NuanDataGridControl").And.Contain("NuanKpiCardControl").And.Contain("NuanActionButton");
    }

    [Theory]
    [InlineData("Authorized",true,true)]
    [InlineData("Authorized",false,false)]
    [InlineData("Pending",true,false)]
    public async Task FrontendDownload_RequiresPermissionAuthorizedStateAndContent(string status,bool hasXml,bool expected)
    {
        var client=new FakeMonitorClient(new SriDocumentMonitorItem(7,"Production","01","Manual","SAFE-REF",null,status,1,DateTime.UtcNow,null,hasXml,1));
        var viewModel=new SriDocumentMonitorViewModel(client,canViewDetail:false,canDownload:true);
        await viewModel.LoadAsync(); await viewModel.LoadDetailAsync(7);
        viewModel.CanDownload.Should().Be(expected);
    }

    [Fact]
    public async Task FrontendDownload_IsBlockedWithoutPermission()
    {
        var item=new SriDocumentMonitorItem(7,"Production","01","Manual","SAFE-REF",null,"Authorized",1,DateTime.UtcNow,null,true,1);
        var viewModel=new SriDocumentMonitorViewModel(new FakeMonitorClient(item),canViewDetail:false,canDownload:false);
        await viewModel.LoadAsync(); await viewModel.LoadDetailAsync(7);
        viewModel.CanDownload.Should().BeFalse();
        await FluentActions.Invoking(()=>viewModel.DownloadAsync()).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FrontendHealthModel_DeserializesWorkerVersionThroughRealClientOptions()
    {
        var backendReport=WorkerHealthEvaluator.Evaluate(
            [BackendSnapshot("6.0.0.0")],
            new(),
            new DateTime(2026,7,23,12,0,0,DateTimeKind.Utc));
        var json=JsonSerializer.Serialize(ApiResponse<WorkerHealthReportDto>.Ok(backendReport),
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        using var httpClient=new HttpClient(new JsonResponseHandler(json)) { BaseAddress=new Uri("http://localhost") };
        var client=new SriDocumentMonitorClient(new NuanApiClient(httpClient,new ApiSession()));

        var result=await client.GetWorkerHealthAsync();

        result.Instances.Should().ContainSingle();
        result.Instances.Single().WorkerVersion.Should().Be("6.0.0.0");
    }

    [Fact]
    public async Task FrontendMonitor_RendersReportedWorkerVersionWithoutSensitiveData()
    {
        var health=FrontendHealth("6.0.0.0");
        var viewModel=new SriDocumentMonitorViewModel(new FakeMonitorClient(MonitorItem(),health),
            canViewDetail:false,canDownload:false,canViewWorkerHealth:true);

        await viewModel.LoadAsync();

        viewModel.WorkerHealthText.Should().Contain("Versión: 6.0.0.0");
        foreach(var sensitiveName in new[] { "ConnectionString","SigningKey","AccessKey","XmlContent","JWT" })
            viewModel.WorkerHealthText.Contains(sensitiveName,StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FrontendMonitor_RendersMissingWorkerVersionAsNotReported(string? workerVersion)
    {
        var viewModel=new SriDocumentMonitorViewModel(
            new FakeMonitorClient(MonitorItem(),FrontendHealth(workerVersion)),
            canViewDetail:false,canDownload:false,canViewWorkerHealth:true);

        await viewModel.LoadAsync();

        viewModel.WorkerHealthText.Should().Contain("Versión: no informada");
    }

    private static WorkerHeartbeatSnapshotDto BackendSnapshot(string? workerVersion) =>
        new(WorkerTypes.Sri,"HOST","pilot",WorkerLifecycleStates.Disabled,false,workerVersion,
            new DateTime(2026,7,23,12,0,0,DateTimeKind.Utc),null,null,null,null,null,null,null,null,
            0,0,0,0,0,0,0,null,null,null);

    private static SriWorkerHealthReport FrontendHealth(string? workerVersion) =>
        new("Disabled",new DateTime(2026,7,23,12,0,0,DateTimeKind.Utc),
            [new SriWorkerHealthInstance(WorkerTypes.Sri,"HOST","pilot",WorkerLifecycleStates.Disabled,
                "Disabled",[],new DateTime(2026,7,23,12,0,0,DateTimeKind.Utc),null,0,0,0,0,0,0,
                null,null,workerVersion)]);

    private static SriDocumentMonitorItem MonitorItem() =>
        new(7,"Production","01","Manual","SAFE-REF",null,"Authorized",1,DateTime.UtcNow,null,true,1);

    private static string Read(params string[] parts)
    {
        var directory=new DirectoryInfo(AppContext.BaseDirectory);
        while(directory is not null && !File.Exists(Path.Combine(directory.FullName,"NuanSystem.sln"))) directory=directory.Parent;
        return File.ReadAllText(Path.Combine(new[] { directory!.FullName }.Concat(parts).ToArray()));
    }

    private sealed class FakeMonitorClient(SriDocumentMonitorItem item,SriWorkerHealthReport? workerHealth=null) : ISriDocumentMonitorClient
    {
        public Task<SriDocumentMonitorSummary> GetSummaryAsync(CancellationToken cancellationToken=default)=>Task.FromResult(new SriDocumentMonitorSummary(1,0,0,1,0));
        public Task<SriWorkerHealthReport> GetWorkerHealthAsync(CancellationToken cancellationToken=default)=>
            Task.FromResult(workerHealth ?? new SriWorkerHealthReport("Unknown",DateTime.UtcNow,[]));
        public Task<IReadOnlyCollection<SriDocumentMonitorItem>> SearchAsync(NuanSystem.WinForms.Services.SriDocuments.Models.SriDocumentMonitorFilter filter,CancellationToken cancellationToken=default)=>Task.FromResult<IReadOnlyCollection<SriDocumentMonitorItem>>([item]);
        public Task<SriDocumentMonitorDetail> GetDetailAsync(long queueId,CancellationToken cancellationToken=default)=>throw new NotSupportedException();
        public Task<IReadOnlyCollection<SriDocumentAttempt>> GetAttemptsAsync(long queueId,CancellationToken cancellationToken=default)=>Task.FromResult<IReadOnlyCollection<SriDocumentAttempt>>([]);
        public Task<IReadOnlyCollection<SriDocumentAudit>> GetAuditAsync(long queueId,CancellationToken cancellationToken=default)=>Task.FromResult<IReadOnlyCollection<SriDocumentAudit>>([]);
        public Task<ApiFileResponse> DownloadXmlAsync(long queueId,CancellationToken cancellationToken=default)=>Task.FromResult(new ApiFileResponse([1],"application/xml","sri-7.xml"));
    }

    private sealed class JsonResponseHandler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage=request,
                Content=new StringContent(json,Encoding.UTF8,"application/json")
            });
    }
}
