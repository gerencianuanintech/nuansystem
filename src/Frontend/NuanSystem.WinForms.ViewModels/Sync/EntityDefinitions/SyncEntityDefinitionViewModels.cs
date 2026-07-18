using System.ComponentModel;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Sync.EntityDefinitions;

public sealed class SyncEntityDefinitionsViewModel(ISyncEntityDefinitionClient client) : ViewModelBase
{
    private IReadOnlyCollection<SyncEntityDefinitionListItem> definitions = Array.Empty<SyncEntityDefinitionListItem>();
    private int totalCount;
    private bool isBusy;

    public SyncEntityDefinitionListFilter Filter { get; } = new();

    public IReadOnlyCollection<SyncEntityDefinitionListItem> Definitions
    {
        get => definitions;
        private set => SetProperty(ref definitions, value);
    }

    public int TotalCount
    {
        get => totalCount;
        private set => SetProperty(ref totalCount, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            var page = await client.SearchAsync(Filter, cancellationToken);
            Definitions = page.Items;
            TotalCount = page.TotalCount;
            Filter.PageNumber = page.PageNumber;
            Filter.PageSize = page.PageSize;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<SyncEntityDefinitionDetail> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.GetAsync(id, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync(id, cancellationToken);
    }
}

public sealed class SyncEntityDefinitionEditViewModel(ISyncEntityDefinitionClient client) : ViewModelBase
{
    private SyncEntityDefinitionEditorState state = SyncEntityDefinitionEditorState.CreateNew([]);
    private bool isBusy;

    public SyncEntityDefinitionEditorState State
    {
        get => state;
        private set => SetProperty(ref state, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task InitializeAsync(int? id, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            var lookup = await client.GetLookupAsync(id, cancellationToken);
            if (!id.HasValue)
            {
                State = SyncEntityDefinitionEditorState.CreateNew(lookup);
                return;
            }

            var detail = await client.GetAsync(id.Value, cancellationToken);
            State = SyncEntityDefinitionEditorState.FromDetail(detail, lookup);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<SyncEntityDefinitionDetail> SaveAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            var saved = State.Id > 0
                ? await client.UpdateAsync(State.Id, State.ToUpdateRequest(), cancellationToken)
                : await client.CreateAsync(State.ToCreateRequest(), cancellationToken);
            var lookup = await client.GetLookupAsync(saved.Id, cancellationToken);
            State = SyncEntityDefinitionEditorState.FromDetail(saved, lookup);
            return saved;
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public sealed class SyncEntityDefinitionEditorState
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DefaultExecutionOrder { get; set; }
    public bool SupportsIncremental { get; set; }
    public bool SupportsInsert { get; set; } = true;
    public bool SupportsUpdate { get; set; } = true;
    public bool SupportsDeactivate { get; set; } = true;
    public string? DefaultKeyField { get; set; }
    public string? DefaultModifiedAtField { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public bool HasProducer { get; set; }
    public bool HasApplier { get; set; }
    public bool IsOperative => HasProducer && HasApplier;
    public bool IsCodeReadOnly => Id > 0;
    public BindingList<SyncEntityDefinitionDependencyOption> Dependencies { get; } = [];

    public static SyncEntityDefinitionEditorState CreateNew(
        IEnumerable<SyncEntityDefinitionLookupItem> lookup)
    {
        var state = new SyncEntityDefinitionEditorState();
        state.AddLookupOptions(lookup, new HashSet<int>());
        return state;
    }

    public static SyncEntityDefinitionEditorState FromDetail(
        SyncEntityDefinitionDetail detail,
        IEnumerable<SyncEntityDefinitionLookupItem> lookup)
    {
        var selectedIds = detail.Dependencies
            .Select(dependency => dependency.DependencyDefinitionId)
            .ToHashSet();
        var state = new SyncEntityDefinitionEditorState
        {
            Id = detail.Id,
            Code = detail.Code,
            Name = detail.Name,
            Description = detail.Description,
            DefaultExecutionOrder = detail.DefaultExecutionOrder,
            SupportsIncremental = detail.SupportsIncremental,
            SupportsInsert = detail.SupportsInsert,
            SupportsUpdate = detail.SupportsUpdate,
            SupportsDeactivate = detail.SupportsDeactivate,
            DefaultKeyField = detail.DefaultKeyField,
            DefaultModifiedAtField = detail.DefaultModifiedAtField,
            IsSystem = detail.IsSystem,
            IsActive = detail.IsActive,
            HasProducer = detail.HasProducer,
            HasApplier = detail.HasApplier
        };

        state.AddLookupOptions(
            lookup.Where(item => item.Id != detail.Id),
            selectedIds);

        var existingIds = state.Dependencies.Select(item => item.DefinitionId).ToHashSet();
        foreach (var dependency in detail.Dependencies.Where(item => !existingIds.Contains(item.DependencyDefinitionId)))
        {
            state.Dependencies.Add(new SyncEntityDefinitionDependencyOption
            {
                DefinitionId = dependency.DependencyDefinitionId,
                Code = dependency.DependencyCode,
                Name = dependency.DependencyName,
                IsAvailable = false,
                IsSelected = true
            });
        }

        return state;
    }

    public CreateSyncEntityDefinitionRequest ToCreateRequest()
    {
        return new CreateSyncEntityDefinitionRequest
        {
            Code = Code,
            Name = Name,
            Description = Description,
            DefaultExecutionOrder = DefaultExecutionOrder,
            SupportsIncremental = SupportsIncremental,
            SupportsInsert = SupportsInsert,
            SupportsUpdate = SupportsUpdate,
            SupportsDeactivate = SupportsDeactivate,
            DefaultKeyField = DefaultKeyField,
            DefaultModifiedAtField = DefaultModifiedAtField,
            IsActive = IsActive,
            DependencyDefinitionIds = SelectedDependencyIds()
        };
    }

    public UpdateSyncEntityDefinitionRequest ToUpdateRequest()
    {
        return new UpdateSyncEntityDefinitionRequest
        {
            Name = Name,
            Description = Description,
            DefaultExecutionOrder = DefaultExecutionOrder,
            SupportsIncremental = SupportsIncremental,
            SupportsInsert = SupportsInsert,
            SupportsUpdate = SupportsUpdate,
            SupportsDeactivate = SupportsDeactivate,
            DefaultKeyField = DefaultKeyField,
            DefaultModifiedAtField = DefaultModifiedAtField,
            IsActive = IsActive,
            DependencyDefinitionIds = SelectedDependencyIds()
        };
    }

    private void AddLookupOptions(
        IEnumerable<SyncEntityDefinitionLookupItem> lookup,
        IReadOnlySet<int> selectedIds)
    {
        foreach (var item in lookup.OrderBy(item => item.DefaultExecutionOrder).ThenBy(item => item.Code))
        {
            Dependencies.Add(new SyncEntityDefinitionDependencyOption
            {
                DefinitionId = item.Id,
                Code = item.Code,
                Name = item.Name,
                IsAvailable = item.IsActive,
                IsSelected = selectedIds.Contains(item.Id)
            });
        }
    }

    private IReadOnlyCollection<int> SelectedDependencyIds()
    {
        return Dependencies
            .Where(item => item.IsSelected)
            .Select(item => item.DefinitionId)
            .Distinct()
            .Order()
            .ToArray();
    }
}

public sealed class SyncEntityDefinitionDependencyOption
{
    public int DefinitionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
    public bool IsSelected { get; set; }
    public string DisplayName => $"{Code} - {Name}";
}
