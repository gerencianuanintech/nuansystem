using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.SapIntegration.ServiceLayer;

public sealed class SapServiceLayerQueryClient(
    IHttpClientFactory httpClientFactory,
    ISapCompanySettingsRepository settingsRepository,
    ISecretProtector secretProtector)
{
    public async Task<IReadOnlyCollection<JsonElement>> ReadAllAsync(int companyId, string relativeQuery, CancellationToken cancellationToken)
    {
        var settings = await settingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        if (settings is null || !settings.IsEnabled || settings.IntegrationMode != SapIntegrationMode.ServiceLayer ||
            string.IsNullOrWhiteSpace(settings.ServiceLayerUrl) || string.IsNullOrWhiteSpace(settings.SapCompanyDb) ||
            string.IsNullOrWhiteSpace(settings.SapUser) || string.IsNullOrWhiteSpace(settings.SapPasswordEncrypted))
            throw new InvalidOperationException("La empresa no tiene configuracion completa de SAP Service Layer.");
        var baseUri = BuildBaseUri(settings.ServiceLayerUrl);
        var client = httpClientFactory.CreateClient("SapServiceLayer");
        var password = secretProtector.Unprotect(settings.SapPasswordEncrypted);
        using var login = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri,"Login"))
        {
            Content=JsonContent.Create(new Login(settings.SapCompanyDb,settings.SapUser,password))
        };
        using var loginResponse = await SendAsync(client,login,"iniciar sesion",cancellationToken);
        var cookie = ReadCookie(loginResponse,"B1SESSION") ?? throw new InvalidOperationException("SAP Service Layer no devolvio una sesion valida.");
        var route=ReadCookie(loginResponse,"ROUTEID");
        var cookieHeader=string.IsNullOrWhiteSpace(route)?$"B1SESSION={cookie}":$"B1SESSION={cookie}; ROUTEID={route}";
        try
        {
            var result=new List<JsonElement>();
            Uri? next=new(baseUri,relativeQuery);
            for(var page=0;next is not null && page<200;page++)
            {
                using var request=new HttpRequestMessage(HttpMethod.Get,next);request.Headers.TryAddWithoutValidation("Cookie",cookieHeader);
                using var response=await SendAsync(client,request,"consultar datos",cancellationToken);
                await using var stream=await response.Content.ReadAsStreamAsync(cancellationToken);
                using var json=await JsonDocument.ParseAsync(stream,cancellationToken:cancellationToken);
                if(!json.RootElement.TryGetProperty("value",out var value)||value.ValueKind!=JsonValueKind.Array)
                    throw new InvalidOperationException("SAP Service Layer devolvio un formato inesperado.");
                result.AddRange(value.EnumerateArray().Select(x=>x.Clone()));
                var link=OptionalString(json.RootElement,"odata.nextLink")??OptionalString(json.RootElement,"@odata.nextLink");
                next=string.IsNullOrWhiteSpace(link)?null:new Uri(baseUri,link);
                if(next is not null && (!string.Equals(next.Host,baseUri.Host,StringComparison.OrdinalIgnoreCase)||next.Port!=baseUri.Port))
                    throw new InvalidOperationException("SAP devolvio paginacion fuera del servidor configurado.");
            }
            return result;
        }
        finally
        {
            try { using var logout=new HttpRequestMessage(HttpMethod.Post,new Uri(baseUri,"Logout"));logout.Headers.TryAddWithoutValidation("Cookie",cookieHeader);using var ignored=await client.SendAsync(logout,cancellationToken); }
            catch(Exception e) when(e is HttpRequestException or TaskCanceledException) { }
        }
    }

    private static async Task<HttpResponseMessage> SendAsync(HttpClient client,HttpRequestMessage request,string operation,CancellationToken ct)
    {
        HttpResponseMessage response;
        try { response=await client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead,ct); }
        catch(TaskCanceledException) when(!ct.IsCancellationRequested) { throw new SapServiceLayerException(operation,sapErrorMessage:"La solicitud supero el tiempo de espera."); }
        catch(HttpRequestException e) { throw new SapServiceLayerException(operation,sapErrorMessage:"No fue posible conectar con SAP.",innerException:e); }
        if(response.IsSuccessStatusCode)return response;
        var code=(int)response.StatusCode;response.Dispose();throw new SapServiceLayerException(operation,code,null,"SAP rechazo la operacion.");
    }
    private static Uri BuildBaseUri(string url) => Uri.TryCreate(url.TrimEnd('/')+"/",UriKind.Absolute,out var uri)&&uri.Scheme==Uri.UriSchemeHttps&&string.IsNullOrEmpty(uri.UserInfo)
        ? uri : throw new InvalidOperationException("La URL de SAP debe ser HTTPS y no incluir credenciales.");
    private static string? ReadCookie(HttpResponseMessage response,string name)
    {
        if(!response.Headers.TryGetValues("Set-Cookie",out var values))return null;
        foreach(var segment in values.SelectMany(x=>x.Split(';',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries)))
        { var i=segment.IndexOf('=');if(i>0&&segment[..i].Equals(name,StringComparison.OrdinalIgnoreCase))return segment[(i+1)..]; }
        return null;
    }
    private static string? OptionalString(JsonElement e,string name)=>e.TryGetProperty(name,out var p)&&p.ValueKind==JsonValueKind.String?p.GetString():null;
    private sealed record Login([property:JsonPropertyName("CompanyDB")]string CompanyDb,[property:JsonPropertyName("UserName")]string User,[property:JsonPropertyName("Password")]string Password);
}
