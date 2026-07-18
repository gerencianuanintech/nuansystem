using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;

namespace NuanSystem.Application.Features.Sync.Execution.Services;

public static class SyncEntityDependencyPlanner
{
    public static IReadOnlyCollection<SyncProfileEntityRecord> Plan(
        IReadOnlyCollection<SyncProfileEntityRecord> profileEntities,
        IReadOnlyCollection<string> requestedCodes,
        IEnumerable<string> registeredSourceCodes,
        IReadOnlyCollection<SyncEntityDefinitionLookupDto> catalog)
    {
        var availableSources = registeredSourceCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidates = profileEntities
            .Where(entity => entity.IsActive && availableSources.Contains(entity.EntityCode))
            .ToDictionary(entity => entity.EntityCode, StringComparer.OrdinalIgnoreCase);
        var definitions = catalog.ToDictionary(definition => definition.Code, StringComparer.OrdinalIgnoreCase);
        var selectedCodes = requestedCodes.Count == 0
            ? candidates.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase)
            : DependencyClosure(requestedCodes, candidates, definitions);
        EnsureDependenciesAvailable(selectedCodes, candidates, definitions);
        var state = new Dictionary<string, VisitState>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<SyncProfileEntityRecord>(selectedCodes.Count);

        foreach (var code in selectedCodes
                     .OrderBy(code => candidates[code].ExecutionOrder)
                     .ThenBy(code => code, StringComparer.OrdinalIgnoreCase))
        {
            Visit(code, candidates, definitions, selectedCodes, state, ordered);
        }

        return ordered;
    }

    private static void EnsureDependenciesAvailable(
        IReadOnlySet<string> selectedCodes,
        IReadOnlyDictionary<string, SyncProfileEntityRecord> candidates,
        IReadOnlyDictionary<string, SyncEntityDefinitionLookupDto> definitions)
    {
        foreach (var code in selectedCodes)
        {
            if (!definitions.TryGetValue(code, out var definition))
            {
                continue;
            }

            var missing = definition.Dependencies.FirstOrDefault(dependency => !candidates.ContainsKey(dependency));
            if (missing is not null)
            {
                throw new InvalidOperationException(
                    $"La entidad {code} no puede ejecutarse porque la dependencia {missing} no esta activa u operativa en el perfil.");
            }
        }
    }

    private static HashSet<string> DependencyClosure(
        IReadOnlyCollection<string> requestedCodes,
        IReadOnlyDictionary<string, SyncProfileEntityRecord> candidates,
        IReadOnlyDictionary<string, SyncEntityDefinitionLookupDto> definitions)
    {
        var selected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pending = new Stack<string>(requestedCodes
            .Where(candidates.ContainsKey)
            .OrderByDescending(code => code, StringComparer.OrdinalIgnoreCase));

        while (pending.Count > 0)
        {
            var code = pending.Pop();
            if (!selected.Add(code) || !definitions.TryGetValue(code, out var definition))
            {
                continue;
            }

            foreach (var dependency in definition.Dependencies
                         .Where(candidates.ContainsKey)
                         .OrderByDescending(item => item, StringComparer.OrdinalIgnoreCase))
            {
                pending.Push(dependency);
            }
        }

        return selected;
    }

    private static void Visit(
        string code,
        IReadOnlyDictionary<string, SyncProfileEntityRecord> candidates,
        IReadOnlyDictionary<string, SyncEntityDefinitionLookupDto> definitions,
        IReadOnlySet<string> selectedCodes,
        IDictionary<string, VisitState> state,
        ICollection<SyncProfileEntityRecord> ordered)
    {
        if (state.TryGetValue(code, out var current))
        {
            if (current == VisitState.Visiting)
            {
                throw new InvalidOperationException($"El catalogo de sincronizacion contiene un ciclo que involucra {code}.");
            }

            return;
        }

        state[code] = VisitState.Visiting;
        if (definitions.TryGetValue(code, out var definition))
        {
            foreach (var dependency in definition.Dependencies
                         .Where(selectedCodes.Contains)
                         .OrderBy(item => candidates[item].ExecutionOrder)
                         .ThenBy(item => item, StringComparer.OrdinalIgnoreCase))
            {
                Visit(dependency, candidates, definitions, selectedCodes, state, ordered);
            }
        }

        state[code] = VisitState.Visited;
        ordered.Add(candidates[code]);
    }

    private enum VisitState
    {
        Visiting,
        Visited
    }
}
