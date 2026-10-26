using System.Xml.Linq;
using MediatR;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Connections;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class BackendDependencyTests
{
    private static readonly Dictionary<string, string[]> AllowedProjects = new(StringComparer.Ordinal)
    {
        ["Domain"] = [],
        ["Shared"] = [],
        ["Application"] = ["Domain", "Shared"],
        ["Persistence"] = ["Application", "Domain", "Shared"],
        ["Infrastructure"] = ["Application", "Domain", "Shared"],
        ["SapIntegration"] = ["Application", "Shared"],
        ["Api"] = ["Application", "Infrastructure", "Persistence", "SapIntegration", "Shared"],
        ["SyncWorker"] = ["Application", "Infrastructure", "Persistence", "SapIntegration", "Shared"],
        ["MasterBranchSyncWorker"] = ["Application", "Infrastructure", "Persistence", "Shared"],
        ["SriWorker"] = ["Application", "Infrastructure", "Persistence", "Shared"]
    };

    [Theory]
    [InlineData("DatabaseEngine")]
    [InlineData("Server")]
    [InlineData("Port")]
    [InlineData("DatabaseName")]
    [InlineData("DatabaseUser")]
    [InlineData("DatabasePasswordEncrypted")]
    [InlineData("SapIntegrationMode")]
    public void Business_company_does_not_expose_technical_configuration(string property)
    {
        Assert.Null(typeof(Company).GetProperty(property));
    }

    [Fact]
    public void Business_company_does_not_depend_on_technical_selector_types()
    {
        Assert.DoesNotContain(typeof(Company).GetProperties(), property =>
            property.PropertyType == typeof(DatabaseEngine) || property.PropertyType == typeof(SapIntegrationMode));
    }

    public static TheoryData<string> BackendProjects => new(AllowedProjects.Keys);

    [Theory]
    [MemberData(nameof(BackendProjects))]
    public void Project_references_respect_layer_boundaries(string layer)
    {
        var project = LoadProject(layer);
        var references = project.Descendants().Where(element => element.Name.LocalName == "ProjectReference")
            .Select(element => Path.GetFileNameWithoutExtension(
                ((string?)element.Attribute("Include") ?? string.Empty).Replace('\\', '/')));

        Assert.Empty(FindUnexpectedProjects(layer, references));
    }

    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    [InlineData("Shared")]
    public void Core_projects_do_not_declare_provider_or_transport_packages_even_when_unused(string layer)
    {
        var packages = LoadProject(layer).Descendants()
            .Where(element => element.Name.LocalName == "PackageReference")
            .Select(element => (string?)element.Attribute("Include") ?? (string?)element.Attribute("Update"))
            .Where(package => package is not null && IsForbiddenAssembly(layer, package));

        Assert.Empty(packages);
    }

    [Theory]
    [InlineData("Domain", "NuanSystem.Application")]
    [InlineData("Application", "NuanSystem.Persistence")]
    [InlineData("Application", "NuanSystem.SapIntegration")]
    [InlineData("SriWorker", "NuanSystem.SapIntegration")]
    public void Project_policy_rejects_a_forbidden_reference(string layer, string reference)
    {
        Assert.Equal([reference], FindUnexpectedProjects(layer, [reference]));
    }

    [Theory]
    [InlineData("Application", "NuanSystem.Domain")]
    [InlineData("Api", "NuanSystem.Persistence")]
    [InlineData("SyncWorker", "NuanSystem.SapIntegration")]
    public void Project_policy_allows_the_composition_roots_and_inward_dependencies(string layer, string reference)
    {
        Assert.Empty(FindUnexpectedProjects(layer, [reference]));
    }

    [Theory]
    [InlineData("Domain")]
    [InlineData("Application")]
    [InlineData("Shared")]
    public void Compiled_core_assemblies_do_not_reference_external_adapters_or_transports(string layer)
    {
        var assembly = layer switch
        {
            "Domain" => typeof(Company).Assembly,
            "Application" => typeof(ApplicationServiceRegistration).Assembly,
            _ => typeof(ApiError).Assembly
        };
        var violations = assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name!)
            .Where(reference => IsForbiddenAssembly(layer, reference)).ToArray();

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData("Domain", "System.Net.Http")]
    [InlineData("Domain", "System.Data.Common")]
    [InlineData("Application", "Microsoft.Data.SqlClient")]
    [InlineData("Application", "Dapper")]
    [InlineData("Application", "NuanSystem.Infrastructure")]
    [InlineData("Application", "Sap.Data.Hana")]
    [InlineData("Shared", "NuanSystem.SapIntegration")]
    public void Assembly_policy_detects_forbidden_dependencies(string layer, string reference)
    {
        Assert.True(IsForbiddenAssembly(layer, reference), $"{layer} must reject {reference}.");
    }

    [Theory]
    [InlineData("Application", "System.Data.Common")]
    [InlineData("Application", "MediatR")]
    [InlineData("Application", "NuanSystem.Domain")]
    [InlineData("Domain", "System.Runtime")]
    public void Assembly_policy_preserves_documented_existing_contracts(string layer, string reference)
    {
        Assert.False(IsForbiddenAssembly(layer, reference));
    }

    [Fact]
    public void All_application_handlers_depend_on_core_contracts_instead_of_external_implementations()
    {
        var handlers = typeof(ApplicationServiceRegistration).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && type.GetInterfaces().Any(IsRequestHandler)).ToArray();
        Assert.NotEmpty(handlers);
        var violations = handlers.SelectMany(handler => handler.GetConstructors()
            .SelectMany(constructor => constructor.GetParameters())
            .Where(parameter => ContainsExternalAdapter(parameter.ParameterType))
            .Select(parameter => $"{handler.FullName}: {parameter.ParameterType}")).ToArray();

        Assert.Empty(violations);
    }

    [Fact]
    public void Constructor_policy_detects_adapters_inside_generic_and_array_parameters()
    {
        Assert.True(ContainsExternalAdapter(typeof(MasterConnectionFactory)));
        Assert.True(ContainsExternalAdapter(typeof(IEnumerable<MasterConnectionFactory>)));
        Assert.True(ContainsExternalAdapter(typeof(MasterConnectionFactory[])));
        Assert.False(ContainsExternalAdapter(typeof(ApplicationServiceRegistration)));
    }

    private static string[] FindUnexpectedProjects(string layer, IEnumerable<string> references)
    {
        var allowed = AllowedProjects[layer].Select(name => $"NuanSystem.{name}").ToHashSet(StringComparer.Ordinal);
        return references.Where(reference => !allowed.Contains(reference)).ToArray();
    }

    private static bool IsForbiddenAssembly(string layer, string reference)
    {
        if (reference.StartsWith("NuanSystem.", StringComparison.Ordinal))
            return reference != $"NuanSystem.{layer}"
                && !AllowedProjects[layer].Any(name => reference == $"NuanSystem.{name}");

        string[] forbiddenPrefixes =
        [
            "Dapper", "Microsoft.Data.SqlClient", "System.Data.SqlClient",
            "Microsoft.EntityFrameworkCore", "MySqlConnector", "MySql.Data", "Npgsql",
            "Sap.Data.Hana", "SAPbobsCOM", "SAPbouiCOM", "System.Windows.Forms",
            "DevExpress", "Microsoft.AspNetCore", "Microsoft.Extensions.Http",
            "System.Net.Http", "System.Net.Sockets"
        ];
        return forbiddenPrefixes.Any(prefix => reference == prefix
                || reference.StartsWith(prefix + ".", StringComparison.Ordinal))
            || (layer is "Domain" or "Shared"
                && (reference == "System.Data" || reference.StartsWith("System.Data.", StringComparison.Ordinal)));
    }

    private static bool ContainsExternalAdapter(Type type)
    {
        return IsForbiddenAssembly("Application", type.Assembly.GetName().Name!)
            || (type.HasElementType && ContainsExternalAdapter(type.GetElementType()!))
            || (type.IsGenericType && type.GetGenericArguments().Any(ContainsExternalAdapter));
    }

    internal static bool IsRequestHandler(Type type) => type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(IRequestHandler<,>);

    private static XDocument LoadProject(string layer)
    {
        var root = FindWorkspaceRoot();
        var directory = layer == "SriWorker"
            ? Path.Combine(root, "src", "Backend", "SyncSRI", "NuanSystem.SriWorker")
            : Path.Combine(root, "src", "Backend", $"NuanSystem.{layer}");
        return XDocument.Load(Path.Combine(directory, $"NuanSystem.{layer}.csproj"));
    }

    private static string FindWorkspaceRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
                return directory.FullName;
        }
        throw new DirectoryNotFoundException("NuanSystem.sln was not found above the test assembly.");
    }
}
