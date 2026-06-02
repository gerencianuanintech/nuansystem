using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Companies.Models;
using NuanSystem.WinForms.Services.SecurityUsers.Models;

namespace NuanSystem.WinForms.Forms.SecurityUsers;

public sealed partial class UserCompanyAssignForm : XtraForm
{
    public UserCompanyAssignForm(UserAdminItem user, IReadOnlyCollection<CompanyAdminItem> companies)
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
        Text = $"Asignar empresa - {user.UserName}";
        lueEmpresa.Properties.DataSource = companies.ToList();
        lueEmpresa.Properties.DisplayMember = nameof(CompanyAdminItem.CommercialName);
        lueEmpresa.Properties.ValueMember = nameof(CompanyAdminItem.Id);
        lueEmpresa.Properties.NullText = "Seleccione una empresa";
        btnAsignar.Click += AssignButton_Click;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CompanyId { get; private set; }

    private void AssignButton_Click(object? sender, EventArgs e)
    {
        if (lueEmpresa.EditValue is not int companyId)
        {
            XtraMessageBox.Show(this, "Seleccione una empresa.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        CompanyId = companyId;
        DialogResult = DialogResult.OK;
        Close();
    }
}

