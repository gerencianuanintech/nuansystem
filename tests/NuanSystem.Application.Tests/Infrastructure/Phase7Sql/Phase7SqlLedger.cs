using System.Text.RegularExpressions;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlLedgerEntry(string Case, string Role, string Code, int? Id = null, Guid? GlobalId = null, Guid? EventId = null);

internal sealed class Phase7SqlLedger(Guid runId)
{
    private readonly Dictionary<(string Case, string Role), Phase7SqlLedgerEntry> entries = [];
    public IReadOnlyList<Phase7SqlLedgerEntry> Entries => entries.Values.ToArray();

    // Candidates must come from an authorized read-only lookup, scoped to the target database.
    public static int ResolveLookup(IReadOnlyList<int> candidates) => candidates.Count == 1 && candidates[0] > 0
        ? candidates[0] : throw new Phase7SqlGuardException("PHASE7_SQL_LOOKUP_MISSING_OR_AMBIGUOUS");

    public Phase7SqlLedgerEntry Add(string caseName, string role)
    {
        if (runId == Guid.Empty || role is not ("A" or "B") || !Regex.IsMatch(caseName ?? "", "\\AS[0-9]{2}[A-Za-z0-9]{0,4}\\z"))
            throw new Phase7SqlGuardException("PHASE7_SQL_LEDGER_KEY");
        var item = new Phase7SqlLedgerEntry(caseName!, role, $"P7_{runId:N}"[..11] + $"_{caseName}");
        if (!entries.TryAdd((caseName!, role), item)) throw new Phase7SqlGuardException("PHASE7_SQL_LEDGER_DUPLICATE");
        return item;
    }

    public void Record(string caseName, string role, int id, Guid globalId, Guid? eventId)
    {
        if (!entries.TryGetValue((caseName, role), out var item) || id <= 0 || globalId == Guid.Empty || eventId == Guid.Empty)
            throw new Phase7SqlGuardException("PHASE7_SQL_LEDGER_IDENTITY");
        entries[(caseName, role)] = item with { Id = id, GlobalId = globalId, EventId = eventId };
    }
}
