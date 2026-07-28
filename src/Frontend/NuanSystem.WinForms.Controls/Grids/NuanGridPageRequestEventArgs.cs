namespace NuanSystem.WinForms.Controls.Grids;

public sealed class NuanGridPageRequestEventArgs(int page, int pageSize) : EventArgs
{
    public int Page { get; } = page;

    public int PageSize { get; } = pageSize;
}
