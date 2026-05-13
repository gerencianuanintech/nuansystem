namespace NuanSystem.WinForms.ViewModels.Common;

public abstract class CrudViewModel<TItem, TRequest> : ViewModelBase
{
    private IReadOnlyCollection<TItem> items = Array.Empty<TItem>();
    private bool isBusy;

    public IReadOnlyCollection<TItem> Items
    {
        get => items;
        protected set => SetProperty(ref items, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        protected set => SetProperty(ref isBusy, value);
    }

    public abstract Task LoadAsync(CancellationToken cancellationToken = default);

    public abstract Task CreateAsync(TRequest request, CancellationToken cancellationToken = default);

    public abstract Task UpdateAsync(int id, TRequest request, CancellationToken cancellationToken = default);

    public abstract Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    protected async Task LoadItemsAsync(Func<CancellationToken, Task<IReadOnlyCollection<TItem>>> load, CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            Items = await load(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
