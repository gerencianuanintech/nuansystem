using NuanSystem.WinForms.Controls.Buttons;

namespace NuanSystem.WinForms.Forms.Common;

partial class BaseEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        btnCancelar = new NuanActionButton();
        btnGuardar = new NuanActionButton();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancelar.ButtonKind = NuanActionButtonKind.Cancel;
        btnCancelar.ButtonText = "Cancelar";
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(298, 52);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 0;
        btnCancelar.Text = "Cancelar";
        // 
        // btnGuardar
        // 
        btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnGuardar.ButtonKind = NuanActionButtonKind.Save;
        btnGuardar.ButtonText = "Guardar";
        btnGuardar.Location = new Point(404, 52);
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 1;
        btnGuardar.Text = "Guardar";
        // 
        // BaseEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(528, 110);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(528, 149);
        Name = "BaseEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Mantenimiento";
        ResumeLayout(false);
    }

    protected NuanActionButton btnCancelar;
    protected NuanActionButton btnGuardar;
}
