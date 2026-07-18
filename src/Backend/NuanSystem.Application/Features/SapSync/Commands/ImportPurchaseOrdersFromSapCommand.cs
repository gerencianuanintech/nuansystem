using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Abstractions.Tenancy;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed record ImportPurchaseOrdersFromSapCommand(DateTime? ModifiedSince=null,int? AuditUserId=null,string? AuditUserName=null)
    : ICommand<SapPurchaseOrderImportResultDto>;

public sealed class ImportPurchaseOrdersFromSapCommandHandler(ICompanyContext companyContext,ISapPurchaseOrderImportService service)
    : ICommandHandler<ImportPurchaseOrdersFromSapCommand,SapPurchaseOrderImportResultDto>
{
    public async Task<Result<SapPurchaseOrderImportResultDto>> Handle(ImportPurchaseOrdersFromSapCommand request,CancellationToken cancellationToken)
    {
        if(!companyContext.HasActiveCompany||companyContext.CurrentCompany is null)
            return Result<SapPurchaseOrderImportResultDto>.Failure("Debe seleccionar una empresa activa.");
        var value=await service.ImportAsync(companyContext.CurrentCompany.CompanyId,request.ModifiedSince,request.AuditUserId,request.AuditUserName,cancellationToken);
        return value.Failed==0?Result<SapPurchaseOrderImportResultDto>.Success(value,"Ordenes SAP importadas."):
            Result<SapPurchaseOrderImportResultDto>.Failure("Una o mas ordenes SAP no pudieron importarse.");
    }
}
