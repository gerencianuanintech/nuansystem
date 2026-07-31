using System.Data;
using Dapper;
using FluentAssertions;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Persistence.Repositories.SapSync;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncScheduleCandidateRowTests
{
    [Fact]
    public void Dapper_MaterializesProfileRow_AndMapperCopiesTypedValues()
    {
        var sourceRowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var source = ValidRow();
        source.Direction = "SapToErp";
        source.ScheduleRowVersion = sourceRowVersion;
        source.SupportsSapToErp = true;
        source.SupportsErpToSap = false;
        source.SupportsFull = true;
        source.SupportsIncremental = false;

        var row = MaterializeWithDapper(source);
        var candidate = SapSyncScheduleCandidateRowMapper.Map(row);

        candidate.CandidateSource.Should().Be("Profile");
        candidate.Direction.Should().Be(SapSyncDirection.SapToErp);
        candidate.SupportsSapToErp.Should().BeTrue();
        candidate.SupportsErpToSap.Should().BeFalse();
        candidate.SupportsFull.Should().BeTrue();
        candidate.SupportsIncremental.Should().BeFalse();
        candidate.ScheduleRowVersion.Should().Equal(sourceRowVersion);
        candidate.ScheduleRowVersion.Should().NotBeSameAs(row.ScheduleRowVersion);
    }

    [Fact]
    public void Mapper_ConvertsErpToSapWithoutImplicitDapperEnumConversion()
    {
        var source = ValidRow();
        source.Direction = "ErpToSap";
        var row = MaterializeWithDapper(source);

        SapSyncScheduleCandidateRowMapper.Map(row).Direction
            .Should().Be(SapSyncDirection.ErpToSap);
    }

    [Fact]
    public void Mapper_PreservesBothForExistingFunctionalRejection()
    {
        var source = ValidRow();
        source.Direction = "Both";
        var row = MaterializeWithDapper(source);

        SapSyncScheduleCandidateRowMapper.Map(row).Direction
            .Should().Be(SapSyncDirection.Both);
    }

    [Fact]
    public void LegacyFallback_PreservesNullablesAndFalseCapabilities()
    {
        var source = ValidRow();
        source.CandidateSource = "LegacyFallback";
        source.ProfileId = null;
        source.ProfileEntityId = null;
        source.ScheduleId = null;
        source.ScheduleType = "LegacyFallback";
        source.IntervalMinutes = null;
        source.ExecutionTime = null;
        source.NextExecutionAtUtc = null;
        source.LastScheduledAtUtc = null;
        source.LastExecutionAtUtc = null;
        source.ScheduleRowVersion = null;
        source.SupportsSapToErp = false;
        source.SupportsErpToSap = false;
        source.SupportsFull = false;
        source.SupportsIncremental = false;
        source.CapabilityIsImplemented = false;
        source.CapabilityIsActive = false;
        source.LegacyFallbackEnabled = true;
        source.CompatibilityVersion = null;
        source.SortProfileId = 0;
        var row = MaterializeWithDapper(source);

        var candidate = SapSyncScheduleCandidateRowMapper.Map(row);

        candidate.IsLegacyFallback.Should().BeTrue();
        candidate.ProfileId.Should().BeNull();
        candidate.ProfileEntityId.Should().BeNull();
        candidate.ScheduleId.Should().BeNull();
        candidate.IntervalMinutes.Should().BeNull();
        candidate.ExecutionTime.Should().BeNull();
        candidate.NextExecutionAtUtc.Should().BeNull();
        candidate.LastScheduledAtUtc.Should().BeNull();
        candidate.LastExecutionAtUtc.Should().BeNull();
        candidate.ScheduleRowVersion.Should().BeNull();
        candidate.CompatibilityVersion.Should().BeNull();
        candidate.SupportsSapToErp.Should().BeFalse();
        candidate.SupportsErpToSap.Should().BeFalse();
        candidate.SupportsFull.Should().BeFalse();
        candidate.SupportsIncremental.Should().BeFalse();
        candidate.CapabilityIsImplemented.Should().BeFalse();
        candidate.CapabilityIsActive.Should().BeFalse();
    }

    [Fact]
    public void UnknownDirection_IsRejectedWithStableSanitizedCode()
    {
        var row = ValidRow();
        row.Direction = "UnexpectedSqlValue";

        var action = () => SapSyncScheduleCandidateRowMapper.Map(row);

        action.Should().Throw<DataException>()
            .WithMessage(SapSyncScheduleCandidateRowMapper.InvalidDirectionCode)
            .Which.Message.Should().NotContain("UnexpectedSqlValue");
    }

    [Theory]
    [InlineData(nameof(SapSyncScheduleCandidateRow.CandidateSource))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.CompanyCode))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.ProfileCode))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.ProfileName))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.EntityCode))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.Direction))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.SyncMode))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.ScheduleType))]
    [InlineData(nameof(SapSyncScheduleCandidateRow.TimeZoneId))]
    public void RequiredStrings_AreValidated(string propertyName)
    {
        var row = ValidRow();
        typeof(SapSyncScheduleCandidateRow).GetProperty(propertyName)!
            .SetValue(row, " ");

        var action = () => SapSyncScheduleCandidateRowMapper.Map(row);

        action.Should().Throw<DataException>()
            .WithMessage(
                $"{SapSyncScheduleCandidateRowMapper.InvalidRequiredValueCode}:{propertyName}");
    }

    [Fact]
    public void RowModel_IsInternalMutableParameterlessAndUsesOnlySqlPrimitiveTypes()
    {
        var type = typeof(SapSyncScheduleCandidateRow);
        var sqlPrimitiveTypes = new HashSet<Type>
        {
            typeof(string), typeof(int), typeof(long), typeof(bool),
            typeof(TimeSpan), typeof(DateTime), typeof(byte[])
        };

        type.IsNotPublic.Should().BeTrue();
        type.GetConstructor(Type.EmptyTypes).Should().NotBeNull();
        type.GetProperties().Should().OnlyContain(property =>
            property.SetMethod != null
            && property.SetMethod.IsPublic
            && sqlPrimitiveTypes.Contains(
                Nullable.GetUnderlyingType(property.PropertyType)
                ?? property.PropertyType));
    }

    private static SapSyncScheduleCandidateRow MaterializeWithDapper(
        SapSyncScheduleCandidateRow source)
    {
        var properties = typeof(SapSyncScheduleCandidateRow).GetProperties();
        using var table = new DataTable();
        foreach (var property in properties)
        {
            table.Columns.Add(
                property.Name,
                Nullable.GetUnderlyingType(property.PropertyType)
                ?? property.PropertyType);
        }

        table.Rows.Add(properties
            .Select(property => property.GetValue(source) ?? DBNull.Value)
            .ToArray());

        using var reader = table.CreateDataReader();
        return reader.Parse<SapSyncScheduleCandidateRow>().Single();
    }

    private static SapSyncScheduleCandidateRow ValidRow() => new()
    {
        CandidateSource = "Profile",
        CompanyId = 7,
        CompanyCode = "DEMO",
        ProfileId = 11,
        ProfileCode = "SAP-DEMO",
        ProfileName = "SAP DEMO",
        ProfileIsActive = true,
        ProfileEntityId = 13,
        EntityCode = "BusinessPartners",
        Direction = "SapToErp",
        SyncMode = "Full",
        BatchSize = 100,
        MaxAttempts = 3,
        ExecutionOrder = 10,
        ContinueOnError = true,
        ExecutionTimeoutMinutes = 15,
        EntityIsActive = true,
        ScheduleId = 17,
        ScheduleType = "Interval",
        IntervalMinutes = 30,
        ExecutionTime = new TimeSpan(8, 30, 0),
        TimeZoneId = "America/Guayaquil",
        PreventConcurrentExecutions = true,
        NextExecutionAtUtc = new DateTime(2026, 7, 31, 13, 30, 0, DateTimeKind.Utc),
        LastScheduledAtUtc = new DateTime(2026, 7, 31, 13, 0, 0, DateTimeKind.Utc),
        LastExecutionAtUtc = new DateTime(2026, 7, 31, 13, 1, 0, DateTimeKind.Utc),
        ScheduleIsActive = true,
        ScheduleRowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        SupportsSapToErp = true,
        SupportsErpToSap = true,
        SupportsFull = true,
        SupportsIncremental = true,
        CapabilityIsImplemented = true,
        CapabilityIsActive = true,
        LegacyFallbackEnabled = false,
        CompatibilityVersion = "v1",
        RequiredSuccessfulCycles = 2,
        SortProfileId = 11,
        SortEntityId = 13
    };
}
