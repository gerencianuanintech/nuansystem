using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Controls.Lookups;

[DefaultEvent(nameof(CreateButtonClick))]
public class NuanLookupEdit : LookUpEdit
{
    public NuanLookupEdit()
    {
        EnsureActionButtons();
        Properties.ButtonClick += PropertiesButtonClick;
    }

    public event EventHandler? ClearButtonClick;

    public event EventHandler? CreateButtonClick;

    public event EventHandler? EditButtonClick;

    [DefaultValue(false)]
    public bool CreateButtonEnabled
    {
        get => ActionButtons(ButtonPredefines.Plus).Any(button => button.Enabled);
        set
        {
            EnsureActionButtons();
            foreach (var button in ActionButtons(ButtonPredefines.Plus))
            {
                button.Enabled = value;
            }
        }
    }

    [DefaultValue(true)]
    public bool ClearButtonEnabled
    {
        get => ActionButtons(ButtonPredefines.Delete).All(button => button.Enabled);
        set
        {
            EnsureActionButtons();
            foreach (var button in ActionButtons(ButtonPredefines.Delete))
            {
                button.Enabled = value;
            }
        }
    }

    [DefaultValue(false)]
    public bool EditButtonEnabled
    {
        get => ActionButtons(ButtonPredefines.Ellipsis).Any(button => button.Enabled);
        set
        {
            EnsureActionButtons();
            foreach (var button in ActionButtons(ButtonPredefines.Ellipsis))
            {
                button.Enabled = value;
            }
        }
    }

    public void RefreshButtons()
    {
        EnsureActionButtons();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Properties.ButtonClick -= PropertiesButtonClick;
        }

        base.Dispose(disposing);
    }

    private void EnsureActionButtons()
    {
        if (!ActionButtons(ButtonPredefines.Combo).Any())
        {
            Properties.Buttons.Insert(0, new EditorButton(ButtonPredefines.Combo));
        }

        NormalizeActionButton(ButtonPredefines.Delete, "Limpiar seleccion", enabledByDefault: true);
        NormalizeActionButton(ButtonPredefines.Plus, "Crear nuevo", enabledByDefault: false);
        NormalizeActionButton(ButtonPredefines.Ellipsis, "Editar seleccionado", enabledByDefault: false);
    }

    private void NormalizeActionButton(
        ButtonPredefines kind,
        string toolTip,
        bool enabledByDefault)
    {
        var buttons = ActionButtons(kind).ToList();
        var button = buttons.FirstOrDefault();
        if (button is null)
        {
            Properties.Buttons.Add(new EditorButton(kind)
            {
                ToolTip = toolTip,
                Enabled = enabledByDefault
            });
            return;
        }

        foreach (var duplicate in buttons.Skip(1))
        {
            Properties.Buttons.Remove(duplicate);
        }

        button.ToolTip = toolTip;
        if (buttons.Count > 1 && !buttons.Any(item => item.Enabled))
        {
            button.Enabled = enabledByDefault;
        }

    }

    private IEnumerable<EditorButton> ActionButtons(ButtonPredefines kind)
    {
        return Properties.Buttons
            .Cast<EditorButton>()
            .Where(button => button.Kind == kind);
    }

    private void PropertiesButtonClick(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button.Kind == ButtonPredefines.Delete && e.Button.Enabled)
        {
            EditValue = null;
            ClearButtonClick?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (e.Button.Kind == ButtonPredefines.Plus && e.Button.Enabled)
        {
            CreateButtonClick?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (e.Button.Kind == ButtonPredefines.Ellipsis && e.Button.Enabled)
        {
            EditButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
