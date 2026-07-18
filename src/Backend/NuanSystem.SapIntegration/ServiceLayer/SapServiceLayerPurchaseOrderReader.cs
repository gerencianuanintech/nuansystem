using System.Globalization;
using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.SapIntegration.ServiceLayer;

public sealed class SapServiceLayerPurchaseOrderReader(SapServiceLayerQueryClient client) : ISapPurchaseOrderReader
{
    public async Task<IReadOnlyCollection<SapPurchaseOrderRecord>> GetPurchaseOrdersAsync(int companyId, DateTime? modifiedSince, CancellationToken cancellationToken = default)
    {
        var filter=modifiedSince.HasValue?$"&$filter=UpdateDate ge '{modifiedSince.Value:yyyy-MM-dd}'":string.Empty;
        var query="PurchaseOrders?$top=50&$orderby=DocEntry desc"+filter;
        var rows=await client.ReadAllAsync(companyId,query,cancellationToken);
        return rows.Select(Map).ToArray();
    }
    private static SapPurchaseOrderRecord Map(JsonElement e)
    {
        var lines=e.TryGetProperty("DocumentLines",out var a)&&a.ValueKind==JsonValueKind.Array?a.EnumerateArray().Select(MapLine).ToArray():[];
        var updated=Date(e,"UpdateDate",DateTime.UtcNow).Date+Time(e,"UpdateTime");
        return new(Int(e,"DocEntry"),Int(e,"DocNum"),Date(e,"DocDate",DateTime.UtcNow),Date(e,"DocDueDate",DateTime.UtcNow),
            Str(e,"CardCode"),Str(e,"CardName"),Str(e,"DocCurrency"),Dec(e,"DocRate",1),Dec(e,"DocTotal"),Dec(e,"VatSum"),
            Dec(e,"DiscountPercent"),MapStatus(Str(e,"DocumentStatus")),Yes(e,"Cancelled"),updated,Opt(e,"Comments"),lines);
    }
    private static SapPurchaseOrderLineRecord MapLine(JsonElement e)=>new(Int(e,"LineNum"),Str(e,"ItemCode"),Str(e,"ItemDescription"),
        Dec(e,"Quantity"),Dec(e,"RemainingOpenQuantity"),Dec(e,"UnitPrice"),Dec(e,"DiscountPercent"),Str(e,"TaxCode"),Dec(e,"TaxPercentagePerRow"),
        Opt(e,"MeasureUnit"),Str(e,"WarehouseCode"),Date(e,"ShipDate",DateTime.UtcNow),MapStatus(Str(e,"LineStatus")));
    private static string MapStatus(string s)=>s.ToUpperInvariant() switch { "BOST_CLOSE" or "CLOSED"=>"Closed","BOST_OPEN" or "OPEN"=>"Open",_=>s };
    private static string Str(JsonElement e,string n)=>Opt(e,n)??string.Empty;
    private static string? Opt(JsonElement e,string n)=>e.TryGetProperty(n,out var p)&&p.ValueKind==JsonValueKind.String?string.IsNullOrWhiteSpace(p.GetString())?null:p.GetString()!.Trim():null;
    private static int Int(JsonElement e,string n)=>e.TryGetProperty(n,out var p)&&(p.ValueKind==JsonValueKind.Number?p.TryGetInt32(out var v):int.TryParse(p.GetString(),out v))?v:0;
    private static decimal Dec(JsonElement e,string n,decimal d=0)=>e.TryGetProperty(n,out var p)&&(p.ValueKind==JsonValueKind.Number?p.TryGetDecimal(out var v):decimal.TryParse(p.GetString(),NumberStyles.Any,CultureInfo.InvariantCulture,out v))?v:d;
    private static DateTime Date(JsonElement e,string n,DateTime d)=>DateTime.TryParse(Opt(e,n),CultureInfo.InvariantCulture,DateTimeStyles.AssumeLocal,out var v)?v:d;
    private static TimeSpan Time(JsonElement e,string n)
    {
        if(!e.TryGetProperty(n,out var p))return TimeSpan.Zero;
        if(p.ValueKind==JsonValueKind.Number&&p.TryGetInt32(out var number))return TimeSpan.FromHours(number/100).Add(TimeSpan.FromMinutes(number%100));
        var text=p.ValueKind==JsonValueKind.String?p.GetString():null;
        if(TimeSpan.TryParse(text,CultureInfo.InvariantCulture,out var time))return time;
        return int.TryParse(text,out number)?TimeSpan.FromHours(number/100).Add(TimeSpan.FromMinutes(number%100)):TimeSpan.Zero;
    }
    private static bool Yes(JsonElement e,string n)=>Str(e,n).ToUpperInvariant() is "TYE S" or "TYES" or "Y" or "YES";
}
