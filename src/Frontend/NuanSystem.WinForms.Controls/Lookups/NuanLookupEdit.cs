using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Controls.Lookups;

[DefaultEvent(nameof(CreateButtonClick))]
public class NuanLookupEdit : LookUpEdit
{
    private EditorButton? clearButton;
    private EditorButton? createButton;

    public NuanLookupEdit()
    {
        EnsureActionButtons();
        Properties.ButtonClick += PropertiesButtonClick;
    }

    public event EventHandler? ClearButtonClick;

    public event EventHandler? CreateButtonClick;

    [DefaultValue(false)]
    public bool CreateButtonEnabled
    {
        get => createButton?.Enabled == true;
        set
        {
            EnsureActionButtons();
            if (createButton is not null)
            {
                createButton.Enabled = value;
            }
        }
    }

    [DefaultValue(true)]
    public bool ClearButtonEnabled
    {
        get => clearButton?.Enabled != false;
        set
        {
            EnsureActionButtons();
            if (clearButton is not null)
            {
                clearButton.Enabled = value;
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
        var buttons = Properties.Buttons.Cast<EditorButton>().ToList();
        if (buttons.All(button => button.Kind != ButtonPredefines.Combo))
        {
            Properties.Buttons.Insert(0, new EditorButton(ButtonPredefines.Combo));
        }

        clearButton = Properties.Buttons
            .Cast<EditorButton>()
            .FirstOrDefault(button => button.Kind == ButtonPredefines.Delete);
        if (clearButton is null)
        {
            clearButton = new EditorButton(ButtonPredefines.Delete)
            { 
                ToolTip = "Limpiar seleccion" 
            };
            Properties.Buttons.Add(clearButton);
        }
        else
        {
            clearButton.ToolTip = "Limpiar seleccion";
        }

        createButton = Properties.Buttons
            .Cast<EditorButton>()
            .FirstOrDefault(button => button.Kind == ButtonPredefines.Plus);
        if (createButton is null)
        {
            createButton = new EditorButton(ButtonPredefines.Plus)
            {
                ToolTip = "Crear nuevo",
                Enabled = false
            };
            Properties.Buttons.Add(createButton);
        }
        else
        {
            createButton.ToolTip = "Crear nuevo";
        }
    }

    private void PropertiesButtonClick(object sender, ButtonPressedEventArgs e)
    {
        if (ReferenceEquals(e.Button, clearButton))
        {
            EditValue = null;
            ClearButtonClick?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (ReferenceEquals(e.Button, createButton) && createButton.Enabled)
        {
            CreateButtonClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
