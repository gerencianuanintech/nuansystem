using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.SapIntegration.DependencyInjection;
using NuanSystem.Application.Features.SapSync.Dtos;
using Microsoft.Data.SqlClient;
using Xunit.Abstractions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapPurchaseOrderPilotIntegrationTests(ITestOutputHelper output)
{
    [Fact]
    public async Task ImportPurchaseOrdersFromConfiguredDemo_WhenPilotExplicitlyEnabled()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("NUAN_RUN_SAP_PO_PILOT"), "1", StringComparison.Ordinal))
            return;

        var root=FindRoot();
        var api=Path.Combine(root,"src","Backend","NuanSystem.Api");
        var config=new ConfigurationBuilder().SetBasePath(api).AddJsonFile("appsettings.json").AddJsonFile("appsettings.Local.json",optional:false)
            .AddInMemoryCollection(new Dictionary<string,string?> {
                ["SqlConnectionPolicy:Encrypt"]="false", ["SqlConnectionPolicy:TrustServerCertificate"]="true",
                ["MasterBranchSyncWorker:Enabled"]="true", ["MasterBranchSyncWorker:SkeletonMode"]="false",
                ["MasterBranchSyncWorker:BatchSize"]="500", ["MasterBranchSyncWorker:ErrorDelaySeconds"]="1",
                ["MasterBranchSyncWorker:WorkerInstance"]="fase9-pilot-test",
                ["MasterBranchSyncWorker:EnabledEntityAppliers:0"]="Countries", ["MasterBranchSyncWorker:EnabledEntityAppliers:1"]="Provinces",
                ["MasterBranchSyncWorker:EnabledEntityAppliers:2"]="Cities", ["MasterBranchSyncWorker:EnabledEntityAppliers:3"]="Currencies",
                ["MasterBranchSyncWorker:EnabledEntityAppliers:4"]="Tax", ["MasterBranchSyncWorker:EnabledEntityAppliers:5"]="UnitOfMeasure",
                ["MasterBranchSyncWorker:EnabledEntityAppliers:6"]="PriceList", ["MasterBranchSyncWorker:EnabledEntityAppliers:7"]="BusinessPartner",
                ["MasterBranchSyncWorker:EnabledEntityAppliers:8"]="ItemGroups", ["MasterBranchSyncWorker:EnabledEntityAppliers:9"]="Item",
                ["MasterBranchSyncWorker:EnabledEntityAppliers:10"]="Warehouse", ["MasterBranchSyncWorker:EnabledEntityAppliers:11"]="PurchaseOrder"
            }).Build();
        var services=new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);services.AddLogging();
        services.AddApplicationServices().AddInfrastructureServices().AddPersistenceServices(config).AddSapIntegrationServices(config);
        services.Configure<MasterBranchSyncWorkerOptions>(config.GetSection(MasterBranchSyncWorkerOptions.SectionName));
        services.AddScoped<ISyncEntityEventApplier,CountrySyncEventApplier>();services.AddScoped<ISyncEntityEventApplier,ProvinceSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier,CitySyncEventApplier>();services.AddScoped<ISyncEntityEventApplier,CurrencySyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier,ReferenceCatalogSyncEventApplier>();services.AddScoped<ISyncEntityEventApplier,BusinessPartnerSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier,ItemGroupSyncEventApplier>();services.AddScoped<ISyncEntityEventApplier,ItemSyncEventApplier>();
        services.AddScoped<ISyncEntityEventApplier,WarehouseSyncEventApplier>();services.AddScoped<ISyncEntityEventApplier,PurchaseOrderSyncEventApplier>();
        services.AddScoped<ISyncEventApplier,SyncEventApplierDispatcher>();services.AddScoped<IMasterBranchSyncWorkerProcessor,MasterBranchSyncWorkerProcessor>();
        await using var provider=services.BuildServiceProvider();using var scope=provider.CreateScope();
        var resolver=scope.ServiceProvider.GetRequiredService<ICompanyResolver>();
        var demo=await resolver.ResolveByCodeAsync("DEMO")??throw new InvalidOperationException("DEMO no existe.");
        scope.ServiceProvider.GetRequiredService<ICompanyContext>().SetCurrentCompany(demo);
        var remigio=await resolver.ResolveByCodeAsync("DEMO-REMIGIO")??throw new InvalidOperationException("DEMO-REMIGIO no existe.");
        var canaris=await resolver.ResolveByCodeAsync("DEMO-CANARIS")??throw new InvalidOperationException("DEMO-CANARIS no existe.");
        var poReader=scope.ServiceProvider.GetRequiredService<ISapPurchaseOrderReader>();
        var sourceOrders=await poReader.GetPurchaseOrdersAsync(demo.CompanyId,null);
        var supplierImport=scope.ServiceProvider.GetRequiredService<ISapSupplierImportService>();
        var supplierRows=sourceOrders.GroupBy(x=>x.SupplierCode,StringComparer.OrdinalIgnoreCase).Select(g=>new SapSupplierRecord(
            g.Key,g.First().SupplierName,null,"S",null,null,null,g.First().CurrencyCode,true,null,g.Max(x=>x.UpdatedAt))).ToArray();
        var suppliers=await supplierImport.ImportBatchAsync(demo.CompanyId,supplierRows,new(null,"PilotIntegrationTest",true,true,false,"pilot-test",Guid.NewGuid().ToString("N")));
        var warehouseImport=scope.ServiceProvider.GetRequiredService<ISapWarehouseImportService>();
        var warehouses=await warehouseImport.ImportAsync(demo.CompanyId,[new("20","DEMO-REMIGIO"),new("11","DEMO-CANARIS")],null,"PilotIntegrationTest");
        var itemImport=scope.ServiceProvider.GetRequiredService<ISapItemImportService>();
        var itemCodes=sourceOrders.SelectMany(x=>x.Lines).Select(x=>x.ItemCode).Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var items=await itemImport.ImportAsync(demo.CompanyId,itemCodes,null,"PilotIntegrationTest");
        output.WriteLine($"Dependencies Suppliers Created={suppliers.Created} Updated={suppliers.Updated} Failed={suppliers.Failed}; Warehouses Created={warehouses.Created} Updated={warehouses.Updated} Failed={warehouses.Failed}; Items Selected={items.Selected} Created={items.Created} Updated={items.Updated} Failed={items.Failed}");
        var service=scope.ServiceProvider.GetRequiredService<ISapPurchaseOrderImportService>();
        var first=await service.ImportAsync(demo.CompanyId,null,null,"PilotIntegrationTest");
        var second=await service.ImportAsync(demo.CompanyId,null,null,"PilotIntegrationTest");
        output.WriteLine($"First Total={first.TotalRead} Created={first.Created} Updated={first.Updated} Unchanged={first.Unchanged} Failed={first.Failed}");
        output.WriteLine($"Second Total={second.TotalRead} Created={second.Created} Updated={second.Updated} Unchanged={second.Unchanged} Failed={second.Failed}");
        Assert.Equal(0,first.Failed);
        Assert.Equal(0,second.Created);
        Assert.Equal(0,second.Failed);
        Assert.Equal(second.TotalRead,second.Unchanged);

        if(await CountItemsAsync(remigio.ConnectionString)<itemCodes.Length||await CountItemsAsync(canaris.ConnectionString)<itemCodes.Length)
        {
            var dependencyExecution=scope.ServiceProvider.GetRequiredService<ISyncProfileExecutionService>();
            var requested=await dependencyExecution.RequestExecutionAsync(3002,new SyncProfileExecutionRequest{
                ExecutionType="Manual",RequestedBy="Fase9Pilot",EntityCodes=[SyncMasterBranchEntityCodes.BusinessPartner,SyncMasterBranchEntityCodes.Item,
                    SyncMasterBranchEntityCodes.Warehouse,SyncMasterBranchEntityCodes.Currencies,SyncMasterBranchEntityCodes.Taxes,
                    SyncMasterBranchEntityCodes.UnitOfMeasures,SyncMasterBranchEntityCodes.PriceLists]});
            Assert.True(requested.IsSuccess,requested.Message+" "+string.Join(" | ",requested.Errors.Select(x=>$"{x.Code}: {x.Message}")));
            await dependencyExecution.ProcessPendingAsync();
        }

        var order20=sourceOrders.First(x=>x.Lines.Any(l=>l.WarehouseCode=="20"));
        var order11=sourceOrders.First(x=>x.Lines.Any(l=>l.WarehouseCode=="11"));
        var routing=scope.ServiceProvider.GetRequiredService<IPurchaseOrderRoutingService>();
        var route20=await routing.RouteAsync(await FindOrderIdAsync(demo.ConnectionString,order20.DocEntry),null,"Fase9Pilot");
        var route11=await routing.RouteAsync(await FindOrderIdAsync(demo.ConnectionString,order11.DocEntry),null,"Fase9Pilot");
        Assert.Equal("Routed",route20.Status);Assert.Equal(1002,route20.BranchCompanyId);
        Assert.Equal("Routed",route11.Status);Assert.Equal(1003,route11.BranchCompanyId);

        var line20=order20.Lines.First(x=>x.WarehouseCode=="20") with { LineNumber=0 };
        var line11=order11.Lines.First(x=>x.WarehouseCode=="11") with { LineNumber=1 };
        var mixed=order20 with { DocEntry=-20260718,DocNum=-20260718,UpdatedAt=DateTime.UtcNow,
            Comments="FASE 9 PILOT - orden mixta deliberada para validar NeedsApproval",Lines=[line20,line11] };
        var mixedApply=await scope.ServiceProvider.GetRequiredService<ISapPurchaseOrderImportRepository>()
            .UpsertAsync(new SapPurchaseOrderImportData(Guid.Parse("f9090000-0000-0000-0000-000000000001"),mixed,DateTime.UtcNow.Ticks,null,"Fase9Pilot"));
        Assert.NotNull(mixedApply.PurchaseOrderId);
        var mixedRoute=await routing.RouteAsync(mixedApply.PurchaseOrderId!.Value,null,"Fase9Pilot");
        Assert.Equal("NeedsApproval",mixedRoute.Status);Assert.Null(mixedRoute.OutboxId);

        var worker=scope.ServiceProvider.GetRequiredService<IMasterBranchSyncWorkerProcessor>();
        var processed=0;
        for(var pass=0;pass<10;pass++){var count=await worker.ProcessOnceAsync();processed+=count;if(count==0)break;}
        output.WriteLine($"Worker processed={processed}; routes: 20->{route20.BranchCompanyId}, 11->{route11.BranchCompanyId}; mixed={mixedRoute.Status}");
        Assert.Equal(1,await CountOrderAsync(remigio.ConnectionString,order20.DocEntry));Assert.Equal(0,await CountOrderAsync(canaris.ConnectionString,order20.DocEntry));
        Assert.Equal(1,await CountOrderAsync(canaris.ConnectionString,order11.DocEntry));Assert.Equal(0,await CountOrderAsync(remigio.ConnectionString,order11.DocEntry));
        Assert.Equal(0,await CountOrderAsync(remigio.ConnectionString,mixed.DocEntry));Assert.Equal(0,await CountOrderAsync(canaris.ConnectionString,mixed.DocEntry));
    }
    private static async Task<int> FindOrderIdAsync(string connectionString,int docEntry)=>await ScalarAsync(connectionString,"SELECT Id FROM dbo.PurchaseOrderHeaders WHERE SapDocEntry=@Value",docEntry);
    private static async Task<int> CountOrderAsync(string connectionString,int docEntry)=>await ScalarAsync(connectionString,"SELECT COUNT(*) FROM dbo.PurchaseOrderHeaders WHERE SapDocEntry=@Value AND IsDeleted=0",docEntry);
    private static async Task<int> CountItemsAsync(string connectionString)=>await ScalarAsync(connectionString,"SELECT COUNT(*) FROM dbo.Items WHERE IsDeleted=0",0,false);
    private static async Task<int> ScalarAsync(string connectionString,string sql,int value,bool addValue=true){var builder=new SqlConnectionStringBuilder(connectionString){Encrypt=false};await using var cn=new SqlConnection(builder.ConnectionString);await cn.OpenAsync();await using var cmd=cn.CreateCommand();cmd.CommandText=sql;if(addValue)cmd.Parameters.AddWithValue("@Value",value);return Convert.ToInt32(await cmd.ExecuteScalarAsync());}
    private static string FindRoot(){var d=new DirectoryInfo(AppContext.BaseDirectory);while(d is not null&&!File.Exists(Path.Combine(d.FullName,"nuansystem.sln")))d=d.Parent;return d?.FullName??throw new InvalidOperationException("Root no encontrado.");}
}
