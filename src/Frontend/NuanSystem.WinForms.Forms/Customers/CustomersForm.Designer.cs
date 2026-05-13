using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Customers;

partial class CustomersForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlToolbar = new PanelControl();
        btnActualizar = new SimpleButton();
        btnNuevo = new SimpleButton();
        btnEditar = new SimpleButton();
        btnEliminar = new SimpleButton();
        grcClientes = new GridControl();
        grvClientes = new GridView();
        ((System.ComponentModel.ISupportInitialize)pnlToolbar).BeginInit();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grcClientes).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvClientes).BeginInit();
        SuspendLayout();
        pnlToolbar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        pnlToolbar.Controls.Add(btnActualizar);
        pnlToolbar.Controls.Add(btnNuevo);
        pnlToolbar.Controls.Add(btnEditar);
        pnlToolbar.Controls.Add(btnEliminar);
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Location = new Point(0, 0);
        pnlToolbar.Name = "pnlToolbar";
        pnlToolbar.Size = new Size(1000, 48);
        pnlToolbar.TabIndex = 0;
        btnActualizar.Location = new Point(12, 10);
        btnActualizar.Name = "btnActualizar";
        btnActualizar.Size = new Size(90, 28);
        btnActualizar.TabIndex = 0;
        btnActualizar.Text = "Actualizar";
        btnNuevo.Location = new Point(110, 10);
        btnNuevo.Name = "btnNuevo";
        btnNuevo.Size = new Size(90, 28);
        btnNuevo.TabIndex = 1;
        btnNuevo.Text = "Nuevo";
        btnEditar.Location = new Point(208, 10);
        btnEditar.Name = "btnEditar";
        btnEditar.Size = new Size(90, 28);
        btnEditar.TabIndex = 2;
        btnEditar.Text = "Editar";
        btnEliminar.Location = new Point(306, 10);
        btnEliminar.Name = "btnEliminar";
        btnEliminar.Size = new Size(90, 28);
        btnEliminar.TabIndex = 3;
        btnEliminar.Text = "Eliminar";
        grcClientes.Dock = DockStyle.Fill;
        grcClientes.Location = new Point(0, 48);
        grcClientes.MainView = grvClientes;
        grcClientes.Name = "grcClientes";
        grcClientes.Size = new Size(1000, 552);
        grcClientes.TabIndex = 1;
        grcClientes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvClientes });
        grvClientes.GridControl = grcClientes;
        grvClientes.Name = "grvClientes";
        grvClientes.OptionsBehavior.Editable = false;
        grvClientes.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvClientes.OptionsView.ShowGroupPanel = false;
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 600);
        Controls.Add(grcClientes);
        Controls.Add(pnlToolbar);
        LookAndFeel.SkinName = "Office 2019 White";
        LookAndFeel.UseDefaultLookAndFeel = false;
        MinimumSize = new Size(800, 460);
        Name = "CustomersForm";
        Text = "Clientes";
        ((System.ComponentModel.ISupportInitialize)pnlToolbar).EndInit();
        pnlToolbar.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grcClientes).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvClientes).EndInit();
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private PanelControl pnlToolbar;
    private SimpleButton btnActualizar;
    private SimpleButton btnNuevo;
    private SimpleButton btnEditar;
    private SimpleButton btnEliminar;
    private GridControl grcClientes;
    private GridView grvClientes;
}
