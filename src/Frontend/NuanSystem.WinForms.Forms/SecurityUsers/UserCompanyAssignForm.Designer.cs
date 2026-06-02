using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SecurityUsers;

partial class UserCompanyAssignForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblEmpresa = new LabelControl();
        lueEmpresa = new LookUpEdit();
        btnAsignar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)lueEmpresa.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblEmpresa
        // 
        lblEmpresa.Location = new Point(24, 28);
        lblEmpresa.Name = "lblEmpresa";
        lblEmpresa.Size = new Size(42, 13);
        lblEmpresa.TabIndex = 0;
        lblEmpresa.Text = "Empresa";
        // 
        // lueEmpresa
        // 
        lueEmpresa.Location = new Point(24, 54);
        lueEmpresa.Name = "lueEmpresa";
        lueEmpresa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueEmpresa.Size = new Size(420, 22);
        lueEmpresa.TabIndex = 1;
        // 
        // btnAsignar
        // 
        btnAsignar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnAsignar.Appearance.ForeColor = Color.White;
        btnAsignar.Appearance.Options.UseBackColor = true;
        btnAsignar.Appearance.Options.UseForeColor = true;
        btnAsignar.AppearanceHovered.BackColor = Color.FromArgb(0, 161, 132);
        btnAsignar.AppearanceHovered.ForeColor = Color.White;
        btnAsignar.AppearanceHovered.Options.UseBackColor = true;
        btnAsignar.AppearanceHovered.Options.UseForeColor = true;
        btnAsignar.Location = new Point(238, 106);
        btnAsignar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAsignar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnAsignar.Name = "btnAsignar";
        btnAsignar.Size = new Size(100, 32);
        btnAsignar.TabIndex = 2;
        btnAsignar.Text = "Asignar";
        // 
        // btnCancelar
        // 
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(344, 106);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 32);
        btnCancelar.TabIndex = 3;
        btnCancelar.Text = "Cancelar";
        // 
        // UserCompanyAssignForm
        // 
        AcceptButton = btnAsignar;
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(480, 180);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Controls.Add(btnCancelar);
        Controls.Add(btnAsignar);
        Controls.Add(lueEmpresa);
        Controls.Add(lblEmpresa);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MaximizeBox = false;
        Name = "UserCompanyAssignForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Asignar empresa";
        ((System.ComponentModel.ISupportInitialize)lueEmpresa.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private LabelControl lblEmpresa;
    private LookUpEdit lueEmpresa;
    private SimpleButton btnAsignar;
    private SimpleButton btnCancelar;
}


