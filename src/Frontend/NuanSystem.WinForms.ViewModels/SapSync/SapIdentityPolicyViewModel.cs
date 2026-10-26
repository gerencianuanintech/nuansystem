using NuanSystem.WinForms.Services.SapSync.IdentityPolicy;
using NuanSystem.WinForms.Services.SapSync.IdentityPolicy.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SapSync;

public sealed class SapIdentityPolicyViewModel(ISapIdentityPolicyClient client) : ViewModelBase
{
    private SapIdentityPolicyItem? policy;
    private bool isBusy;
    public SapIdentityPolicyItem? Policy { get => policy; private set => SetProperty(ref policy, value); }
    public bool IsBusy { get => isBusy; private set => SetProperty(ref isBusy, value); }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try { Policy = await client.GetAsync(cancellationToken); }
        finally { IsBusy = false; }
    }

    public async Task SaveAsync(SaveSapIdentityPolicyRequest request, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try { await client.SaveAsync(request, cancellationToken); }
        finally { IsBusy = false; }
    }
}
