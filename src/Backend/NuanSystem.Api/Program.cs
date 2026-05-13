using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Companies.Commands;
using NuanSystem.Application.Features.Companies.Queries;
using NuanSystem.Application.Features.Audit.Dtos;
using NuanSystem.Application.Features.ConfigurationCompanies.Commands;
using NuanSystem.Application.Features.ConfigurationCompanies.Queries;
using NuanSystem.Application.Features.ConfigurationSettings.Commands;
using NuanSystem.Application.Features.ConfigurationSettings.Queries;
using NuanSystem.Application.Features.Audit.Queries;
using NuanSystem.Application.Features.Customers.Commands;
using NuanSystem.Application.Features.Customers.Queries;
using NuanSystem.Application.Features.Documents.Commands;
using NuanSystem.Application.Features.Documents.Queries;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Queries;
using NuanSystem.Application.Features.SapSync.Commands;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Application.Features.Settings.Commands;
using NuanSystem.Application.Features.Settings.Queries;
using NuanSystem.Application.Features.SecurityUsers.Commands;
using NuanSystem.Application.Features.SecurityUsers.Queries;
using NuanSystem.Application.Features.SecurityRoles.Commands;
using NuanSystem.Application.Features.SecurityRoles.Queries;
using NuanSystem.Application.Features.Roles.Commands;
using NuanSystem.Application.Features.Roles.Queries;
using NuanSystem.Application.Features.SecurityOperations.Commands;
using NuanSystem.Application.Features.SecurityOperations.Queries;
using NuanSystem.Application.Features.SecurityMenus.Commands;
using NuanSystem.Application.Features.SecurityMenus.Queries;
using NuanSystem.Application.Features.SecurityForms.Commands;
using NuanSystem.Application.Features.SecurityForms.Queries;
using NuanSystem.Application.Features.SecurityFields.Commands;
using NuanSystem.Application.Features.SecurityFields.Queries;
using NuanSystem.Application.Features.SecurityAccess.Commands;
using NuanSystem.Application.Features.SecurityAccess.Queries;
using NuanSystem.Application.Features.GridColumnSettings.Dtos;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.Shared.Responses;
using Serilog;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/nuansystem-api-.log", rollingInterval: RollingInterval.Day);
    });

    builder.Services.AddNuanSystemServices(builder.Configuration);

    var app = builder.Build();

    var initOnly = args.Contains("--init-only", StringComparer.OrdinalIgnoreCase);
    var initializeMasterOnStartup = builder.Configuration.GetValue<bool>("DatabaseInitialization:InitializeMasterOnStartup");
    if (initOnly || initializeMasterOnStartup)
    {
        await InitializeMasterDatabaseAsync(app.Services, app.Lifetime.ApplicationStopping);
    }

    if (initOnly)
    {
        Log.Information("Inicializacion de base master completada. Finalizando por --init-only.");
        return;
    }

    app.UseGlobalExceptionHandling();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "NuanSystem API v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseCompanyContext();
    app.UseAuthorization();
    app.UseAuditLogging();

    app.MapHealthChecks("/health");
    app.MapGet("/", () => ApiResponse<object>.Ok(new
    {
        Service = "NuanSystem.Api",
        Status = "Running",
        Swagger = "/swagger"
    }));

    app.MapPost("/api/auth/login", async (
        LoginRequest request,
        IAuthService authService,
        CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(ApiResponse<object>.Fail("Usuario y contrasena son requeridos."));
        }

        var result = await authService.LoginAsync(
            request.UserNameOrEmail,
            request.Password,
            cancellationToken);

        if (result is null)
        {
            return Results.Unauthorized();
        }

        var response = new LoginResponse(
            result.UserId,
            result.UserName,
            result.DisplayName,
            result.AccessToken,
            result.ExpiresAtUtc,
            result.MustChangePassword,
            result.Roles,
            result.Permissions,
            result.Companies.Select(company => new UserCompanyResponse(
                company.Id,
                company.Code,
                company.CommercialName,
                company.LogoImage,
                company.LogoImageContentType,
                company.LogoImageFileName)).ToArray());

        return Results.Ok(ApiResponse<LoginResponse>.Ok(response, "Login correcto."));
    })
    .AllowAnonymous();

    app.MapPost("/api/auth/change-password", async (
        ChangePasswordRequest request,
        ClaimsPrincipal user,
        IMasterConnectionFactory connectionFactory,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken) =>
    {
        if (!TryGetUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return Results.BadRequest(ApiResponse<object>.Fail("Clave actual y nueva clave son requeridas."));
        }

        if (request.NewPassword.Length < 8)
        {
            return Results.BadRequest(ApiResponse<object>.Fail("La nueva clave debe tener al menos 8 caracteres."));
        }

        var changed = await ChangePasswordAsync(
            connectionFactory,
            passwordHasher,
            userId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        return changed
            ? Results.Ok(ApiResponse<object>.Ok(new { }, "Clave actualizada correctamente."))
            : Results.BadRequest(ApiResponse<object>.Fail("La clave actual no es correcta."));
    })
    .RequireAuthorization();

    app.MapGet("/api/companies/my-companies", async (
        ClaimsPrincipal user,
        IAuthService authService,
        CancellationToken cancellationToken) =>
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return Results.Unauthorized();
        }

        var companies = await authService.GetCompaniesForUserAsync(userId, cancellationToken);
        var response = companies
            .Select(company => new UserCompanyResponse(
                company.Id,
                company.Code,
                company.CommercialName,
                company.LogoImage,
                company.LogoImageContentType,
                company.LogoImageFileName))
            .ToArray();

        return Results.Ok(ApiResponse<IReadOnlyCollection<UserCompanyResponse>>.Ok(response));
    })
    .RequirePermission(PermissionCodes.UsersManage);

    app.MapGet("/api/companies", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetCompaniesQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CompaniesManage);

    app.MapPost("/api/companies", async (
        CreateCompanyCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CompaniesManage);

    app.MapPost("/api/companies/validate-connection", async (
        ValidateCompanyConnectionCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CompaniesManage);

    app.MapPost("/api/companies/assign-user", async (
        AssignUserCompanyCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CompaniesManage);

    app.MapGet("/api/configuration/companies", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetConfigurationCompaniesQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-companies", "refresh");

    app.MapGet("/api/configuration/companies/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetConfigurationCompanyByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-companies", "consult");

    app.MapPost("/api/configuration/companies", async (
        CreateConfigurationCompanyCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-companies", "create");

    app.MapPut("/api/configuration/companies/{id:int}", async (
        int id,
        UpdateConfigurationCompanyCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-companies", "update");

    app.MapDelete("/api/configuration/companies/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteConfigurationCompanyCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-companies", "delete");

    app.MapGet("/api/tenancy/current", (NuanSystem.Application.Abstractions.Tenancy.ICompanyContext companyContext) =>
    {
        if (!companyContext.HasActiveCompany)
        {
            return Results.BadRequest(ApiResponse<object>.Fail("No hay empresa activa."));
        }

        return Results.Ok(ApiResponse<object>.Ok(new
        {
            companyContext.CurrentCompany!.CompanyId,
            companyContext.CurrentCompany.CompanyCode,
            companyContext.CurrentCompany.CommercialName,
            DatabaseEngine = companyContext.CurrentCompany.DatabaseEngine.ToString(),
            SapIntegrationMode = companyContext.CurrentCompany.SapIntegrationMode.ToString()
        }));
    })
    .RequirePermission(PermissionCodes.CustomersRead);

    app.MapPost("/api/tenancy/initialize-database", async (
        ITenantDatabaseInitializer initializer,
        CancellationToken cancellationToken) =>
    {
        await initializer.InitializeCurrentTenantAsync(cancellationToken);

        return Results.Ok(ApiResponse<object>.Ok(new
        {
            Initialized = true
        }, "Base de datos tenant validada correctamente."));
    })
    .RequirePermission(PermissionCodes.CustomersRead);

    app.MapGet("/api/customers", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetCustomersQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CustomersRead);

    app.MapGet("/api/customers/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetCustomerByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CustomersRead);

    app.MapPost("/api/customers", async (
        CreateCustomerCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CustomersManage);

    app.MapPut("/api/customers/{id:int}", async (
        int id,
        UpdateCustomerCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CustomersManage);

    app.MapDelete("/api/customers/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new DeleteCustomerCommand(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.CustomersManage);

    app.MapGet("/api/items", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetItemsQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.ItemsRead);

    app.MapGet("/api/items/lookups", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetItemLookupsQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.ItemsRead);

    app.MapGet("/api/items/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetItemByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.ItemsRead);

    app.MapPost("/api/items", async (
        CreateItemCommand command,
        ISender sender,
        ClaimsPrincipal user,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.ItemsManage);

    app.MapPut("/api/items/{id:int}", async (
        int id,
        UpdateItemCommand command,
        ISender sender,
        ClaimsPrincipal user,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.ItemsManage);

    app.MapDelete("/api/items/{id:int}", async (
        int id,
        ISender sender,
        ClaimsPrincipal user,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteItemCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.ItemsManage);

    app.MapGet("/api/documents", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetDocumentsQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.DocumentsRead);

    app.MapGet("/api/documents/{id:long}", async (
        long id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetDocumentByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.DocumentsRead);

    app.MapPost("/api/documents", async (
        CreateDocumentCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.DocumentsManage);

    app.MapPost("/api/sap/send-document/{documentId:long}", async (
        long documentId,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new SendDocumentToSapCommand(documentId), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.SapManage);

    app.MapGet("/api/sap/sync-logs", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSapSyncLogsQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.SapRead);

    app.MapGet("/api/settings/parameters", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetCompanyParametersQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.SettingsManage);

    app.MapPut("/api/settings/parameters/{key}", async (
        string key,
        UpsertCompanyParameterCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command with { Key = key }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.SettingsManage);

    app.MapGet("/api/configuration/settings", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetConfigurationSettingsQuery(), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-settings", "refresh");

    app.MapGet("/api/configuration/settings/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetConfigurationSettingByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-settings", "consult");

    app.MapPost("/api/configuration/settings", async (
        CreateConfigurationSettingCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-settings", "create");

    app.MapPut("/api/configuration/settings/{id:int}", async (
        int id,
        UpdateConfigurationSettingCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-settings", "update");

    app.MapDelete("/api/configuration/settings/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteConfigurationSettingCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("configuration-settings", "delete");

    app.MapGet("/api/users", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetUsersQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.UsersManage);

    app.MapGet("/api/security/users", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetUsersQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("users", "refresh");

    app.MapGet("/api/security/users/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("users", "consult");

    app.MapPost("/api/users", async (
        CreateUserCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.UsersManage);

    app.MapPost("/api/security/users", async (
        CreateUserCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("users", "create");

    app.MapPut("/api/security/users/{id:int}", async (
        int id,
        UpdateUserCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("users", "update");

    app.MapDelete("/api/security/users/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteUserCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("users", "delete");

    app.MapGet("/api/users/roles", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.UsersManage);

    app.MapGet("/api/security/users/roles", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("users", "refresh");

    app.MapGet("/api/roles", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetRolesAdminQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.RolesManage);

    app.MapPost("/api/roles", async (
        CreateRoleCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.RolesManage);

    app.MapGet("/api/roles/permissions", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetPermissionsQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.RolesManage);

    app.MapPost("/api/roles/assign-permission", async (
        AssignRolePermissionCommand command,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.RolesManage);

    app.MapGet("/api/security/roles", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityRolesQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-roles", "refresh");

    app.MapGet("/api/security/roles/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityRoleByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-roles", "consult");

    app.MapPost("/api/security/roles", async (
        CreateSecurityRoleCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-roles", "create");

    app.MapPut("/api/security/roles/{id:int}", async (
        int id,
        UpdateSecurityRoleCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-roles", "update");

    app.MapDelete("/api/security/roles/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteSecurityRoleCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-roles", "delete");

    app.MapGet("/api/security/operations", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityOperationsQuery(), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-operations", "refresh");

    app.MapGet("/api/security/operations/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityOperationByIdQuery(id), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-operations", "consult");

    app.MapPost("/api/security/operations", async (
        CreateSecurityOperationCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-operations", "create");

    app.MapPut("/api/security/operations/{id:int}", async (
        int id,
        UpdateSecurityOperationCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-operations", "update");

    app.MapDelete("/api/security/operations/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(
            new DeleteSecurityOperationCommand(id, auditUser.UserId, auditUser.UserName),
            cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-operations", "delete");

    app.MapGet("/api/security/menus", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityMenusQuery(), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-menus", "refresh");

    app.MapGet("/api/security/menus/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityMenuByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-menus", "consult");

    app.MapPost("/api/security/menus", async (
        CreateSecurityMenuCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-menus", "create");

    app.MapPut("/api/security/menus/{id:int}", async (
        int id,
        UpdateSecurityMenuCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-menus", "update");

    app.MapDelete("/api/security/menus/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteSecurityMenuCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-menus", "delete");

    app.MapGet("/api/security/forms", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityFormsQuery(), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-forms", "refresh");

    app.MapGet("/api/security/forms/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityFormByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-forms", "consult");

    app.MapPost("/api/security/forms", async (
        CreateSecurityFormCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-forms", "create");

    app.MapPut("/api/security/forms/{id:int}", async (
        int id,
        UpdateSecurityFormCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-forms", "update");

    app.MapDelete("/api/security/forms/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteSecurityFormCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-forms", "delete");

    app.MapGet("/api/security/fields", async (
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityFieldsQuery(), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-fields", "refresh");

    app.MapGet("/api/security/fields/{id:int}", async (
        int id,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityFieldByIdQuery(id), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-fields", "consult");

    app.MapPost("/api/security/fields", async (
        CreateSecurityFieldCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-fields", "create");

    app.MapPut("/api/security/fields/{id:int}", async (
        int id,
        UpdateSecurityFieldCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            Id = id,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-fields", "update");

    app.MapDelete("/api/security/fields/{id:int}", async (
        int id,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(new DeleteSecurityFieldCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-fields", "delete");

    app.MapGet("/api/security/navigation/me", async (
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        if (!TryGetUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await sender.Send(new GetNavigationQuery(userId), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireAuthorization();

    app.MapGet("/api/security/forms/{formKey}/operations/me", async (
        string formKey,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        if (!TryGetUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await sender.Send(new GetCurrentFormOperationsQuery(userId, formKey), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireAuthorization();

    app.MapGet("/api/security/roles/{roleId:int}/access", async (
        int roleId,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetRoleAccessQuery(roleId), cancellationToken);
        return result.ToHttpResult();
    })
    .RequireFormOperation("security-access", "consult");

    app.MapPut("/api/security/roles/{roleId:int}/access", async (
        int roleId,
        SaveRoleAccessCommand command,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var result = await sender.Send(command with
        {
            RoleId = roleId,
            AuditUserId = auditUser.UserId,
            AuditUserName = auditUser.UserName
        }, cancellationToken);

        return result.ToHttpResult();
    })
    .RequireFormOperation("security-access", "update");

    app.MapGet("/api/security/grid-columns/{formKey}/{gridName}/me", async (
        string formKey,
        string gridName,
        ClaimsPrincipal user,
        IGridColumnSettingsRepository repository,
        CancellationToken cancellationToken) =>
    {
        if (!TryGetUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var settings = await repository.GetUserSettingsAsync(userId, formKey, gridName, cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyCollection<GridColumnSettingDto>>.Ok(settings));
    })
    .RequireAuthorization();

    app.MapPut("/api/security/grid-columns/{formKey}/{gridName}/me", async (
        string formKey,
        string gridName,
        IReadOnlyCollection<SaveGridColumnSettingData> columns,
        ClaimsPrincipal user,
        IGridColumnSettingsRepository repository,
        CancellationToken cancellationToken) =>
    {
        if (!TryGetUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var auditUser = GetAuditUser(user);
        await repository.SaveUserSettingsAsync(userId, formKey, gridName, columns, auditUser.UserId, auditUser.UserName, cancellationToken);
        return Results.Ok(ApiResponse<bool>.Ok(true, "Configuracion de columnas guardada correctamente."));
    })
    .RequireFormOperation("{formKey}", "customizecolumns");

    app.MapGet("/api/audit/logs", async (
        int? take,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetAuditLogsQuery(take ?? 200), cancellationToken);

        return result.ToHttpResult();
    })
    .RequirePermission(PermissionCodes.AuditRead);

    app.MapGet("/api/audit/security-changes", async (
        string entityName,
        string recordId,
        int? take,
        ISender sender,
        CancellationToken cancellationToken) =>
    {
        var result = await sender.Send(new GetSecurityChangesQuery(entityName, recordId, take ?? 200), cancellationToken);

        return result.ToHttpResult();
    })
    .RequireSecurityHistoryOperation();

    app.MapGet("/api/audit/inventory-changes", async (
        string entityName,
        string recordId,
        int? take,
        IInventoryAuditRepository repository,
        CancellationToken cancellationToken) =>
    {
        var changes = await repository.GetChangesAsync(
            entityName.Trim(),
            recordId.Trim(),
            Math.Clamp(take ?? 200, 1, 500),
            cancellationToken);

        return Results.Ok(ApiResponse<IReadOnlyCollection<SecurityChangeDto>>.Ok(changes));
    })
    .RequirePermission(PermissionCodes.ItemsRead);

    app.MapGet("/api/audit/error-logs", async (
        int? take,
        IAuditLogRepository repository,
        CancellationToken cancellationToken) =>
    {
        var logs = await repository.GetRecentErrorsAsync(Math.Clamp(take ?? 200, 1, 500), cancellationToken);
        return Results.Ok(ApiResponse<IReadOnlyCollection<AuditErrorLogDto>>.Ok(logs));
    })
    .RequirePermission(PermissionCodes.AuditRead);

    app.MapPost("/api/audit/error-logs", async (
        CreateAuditErrorLogData request,
        ClaimsPrincipal user,
        IAuditLogRepository repository,
        CancellationToken cancellationToken) =>
    {
        var auditUser = GetAuditUser(user);
        var errorLog = new CreateAuditErrorLogData(
            Trim(request.Source, 30) ?? "WinForms",
            request.UserId ?? auditUser.UserId,
            Trim(request.UserName, 120) ?? auditUser.UserName,
            Trim(request.CompanyCode, 50),
            Trim(request.ModuleKey, 120),
            Trim(request.FormName, 180),
            Trim(request.ActionName, 120),
            Trim(request.HttpMethod, 12),
            Trim(request.Path, 500),
            Trim(request.QueryString, 1000),
            request.StatusCode,
            Trim(request.ErrorMessage, 2000) ?? "Error no controlado en cliente.",
            Trim(request.ExceptionType, 300),
            request.StackTrace,
            Trim(request.TraceId, 120),
            Trim(request.IpAddress, 64),
            Trim(request.MachineName, 120),
            Trim(request.UserAgent, 500));

        await repository.AddErrorAsync(errorLog, cancellationToken);
        return Results.Ok(ApiResponse<bool>.Ok(true, "Error registrado correctamente."));
    })
    .AllowAnonymous();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "NuanSystem API finalizo inesperadamente.");
    Environment.ExitCode = 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static async Task InitializeMasterDatabaseAsync(IServiceProvider services, CancellationToken cancellationToken)
{
    using var scope = services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<IMasterDatabaseInitializer>();
    await initializer.InitializeAsync(cancellationToken);
}

static (int? UserId, string? UserName) GetAuditUser(ClaimsPrincipal user)
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var userId = int.TryParse(userIdValue, out var parsedUserId) ? parsedUserId : (int?)null;

    return (userId, user.FindFirstValue(ClaimTypes.Name) ?? user.Identity?.Name);
}

static string? Trim(string? value, int maxLength)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return null;
    }

    return value.Length <= maxLength ? value : value[..maxLength];
}

static bool TryGetUserId(ClaimsPrincipal user, out int userId)
{
    var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
    return int.TryParse(userIdValue, out userId);
}

static async Task<bool> ChangePasswordAsync(
    IMasterConnectionFactory connectionFactory,
    IPasswordHasher passwordHasher,
    int userId,
    string currentPassword,
    string newPassword,
    CancellationToken cancellationToken)
{
    using var connection = connectionFactory.CreateConnection();
    connection.Open();

    using var getCommand = connection.CreateCommand();
    getCommand.CommandText = "SELECT PasswordHash FROM dbo.Users WHERE Id = @UserId AND IsActive = 1 AND IsDeleted = 0;";
    AddParameter(getCommand, "@UserId", userId);

    var currentHash = (string?)getCommand.ExecuteScalar();
    if (string.IsNullOrWhiteSpace(currentHash) || !passwordHasher.VerifyPassword(currentPassword, currentHash))
    {
        return false;
    }

    using var updateCommand = connection.CreateCommand();
    updateCommand.CommandText = """
UPDATE dbo.Users
SET PasswordHash = @PasswordHash,
    MustChangePassword = 0,
    FailedAccessCount = 0,
    LockoutEndAt = NULL,
    UpdatedByUserId = @UserId,
    UpdatedByUserName = @UserName,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @UserId
  AND IsActive = 1
  AND IsDeleted = 0;
""";
    AddParameter(updateCommand, "@PasswordHash", passwordHasher.HashPassword(newPassword));
    AddParameter(updateCommand, "@UserId", userId);
    AddParameter(updateCommand, "@UserName", null);

    await Task.Run(updateCommand.ExecuteNonQuery, cancellationToken);
    return true;
}

static void AddParameter(System.Data.IDbCommand command, string name, object? value)
{
    var parameter = command.CreateParameter();
    parameter.ParameterName = name;
    parameter.Value = value ?? DBNull.Value;
    command.Parameters.Add(parameter);
}

