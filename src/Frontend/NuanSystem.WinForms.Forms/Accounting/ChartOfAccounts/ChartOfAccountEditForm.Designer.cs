using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Accounting.ChartOfAccounts;

partial class ChartOfAccountEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCodigo = new LabelControl();
        txtCodigo = new TextEdit();
        lblNombre = new LabelControl();
        txtNombre = new TextEdit();
        lblDescripcion = new LabelControl();
        memDescripcion = new MemoEdit();
        lblCodigoExterno = new LabelControl();
        txtCodigoExterno = new TextEdit();
        lblTipoCuenta = new LabelControl();
        lueTipoCuenta = new LookUpEdit();
        lblClaseCuenta = new LabelControl();
        lueClaseCuenta = new LookUpEdit();
        lblCuentaPadre = new LabelControl();
        lueCuentaPadre = new LookUpEdit();
        lblMoneda = new LabelControl();
        txtMoneda = new TextEdit();
        lblSaldo = new LabelControl();
        txtSaldo = new TextEdit();
        lblNivel = new LabelControl();
        txtNivel = new TextEdit();
        chkTitulo = new CheckEdit();
        chkPermiteMovimiento = new CheckEdit();
        chkConfidencial = new CheckEdit();
        chkCuentaMonetaria = new CheckEdit();
        chkCuentaAsociada = new CheckEdit();
        chkRevaluaIndice = new CheckEdit();
        chkBloquearManual = new CheckEdit();
        chkFlujoCaja = new CheckEdit();
        chkCentroCosto = new CheckEdit();
        chkTercero = new CheckEdit();
        chkProyecto = new CheckEdit();
        chkActivo = new CheckEdit();
        trlAccounts = new TreeList();
        colAccountDisplay = new TreeListColumn();
        btnTipoActivo = new SimpleButton();
        btnTipoPasivo = new SimpleButton();
        btnTipoPatrimonio = new SimpleButton();
        btnTipoIngreso = new SimpleButton();
        btnTipoCosto = new SimpleButton();
        btnTipoGasto = new SimpleButton();
        btnTipoOrden = new SimpleButton();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigoExterno.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTipoCuenta.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueClaseCuenta.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCuentaPadre.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtMoneda.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSaldo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtNivel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkTitulo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkPermiteMovimiento.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkConfidencial.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkCuentaMonetaria.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkCuentaAsociada.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkRevaluaIndice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkBloquearManual.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkFlujoCaja.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkCentroCosto.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkTercero.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkProyecto.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)trlAccounts).BeginInit();
        SuspendLayout();
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Location = new Point(12, 14);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(39, 15);
        lblCodigo.TabIndex = 1;
        lblCodigo.Text = "Codigo";
        // 
        // txtCodigo
        // 
        txtCodigo.Location = new Point(139, 12);
        txtCodigo.Name = "txtCodigo";
        txtCodigo.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCodigo.Properties.Appearance.Options.UseFont = true;
        txtCodigo.Properties.AutoHeight = false;
        txtCodigo.Properties.MaxLength = 50;
        txtCodigo.Size = new Size(236, 22);
        txtCodigo.TabIndex = 2;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Location = new Point(12, 43);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(44, 15);
        lblNombre.TabIndex = 3;
        lblNombre.Text = "Nombre";
        // 
        // txtNombre
        // 
        txtNombre.Location = new Point(139, 40);
        txtNombre.Name = "txtNombre";
        txtNombre.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtNombre.Properties.Appearance.Options.UseFont = true;
        txtNombre.Properties.AutoHeight = false;
        txtNombre.Properties.MaxLength = 200;
        txtNombre.Size = new Size(236, 22);
        txtNombre.TabIndex = 4;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Location = new Point(12, 71);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(62, 15);
        lblDescripcion.TabIndex = 5;
        lblDescripcion.Text = "Descripcion";
        // 
        // memDescripcion
        // 
        memDescripcion.Location = new Point(139, 68);
        memDescripcion.Name = "memDescripcion";
        memDescripcion.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescripcion.Properties.Appearance.Options.UseFont = true;
        memDescripcion.Properties.MaxLength = 500;
        memDescripcion.Size = new Size(236, 55);
        memDescripcion.TabIndex = 6;
        // 
        // lblCodigoExterno
        // 
        lblCodigoExterno.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigoExterno.Appearance.Options.UseFont = true;
        lblCodigoExterno.Location = new Point(12, 131);
        lblCodigoExterno.Name = "lblCodigoExterno";
        lblCodigoExterno.Size = new Size(81, 15);
        lblCodigoExterno.TabIndex = 7;
        lblCodigoExterno.Text = "Codigo externo";
        // 
        // txtCodigoExterno
        // 
        txtCodigoExterno.Location = new Point(139, 129);
        txtCodigoExterno.Name = "txtCodigoExterno";
        txtCodigoExterno.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCodigoExterno.Properties.Appearance.Options.UseFont = true;
        txtCodigoExterno.Properties.AutoHeight = false;
        txtCodigoExterno.Properties.MaxLength = 50;
        txtCodigoExterno.Size = new Size(236, 22);
        txtCodigoExterno.TabIndex = 8;
        // 
        // lblTipoCuenta
        // 
        lblTipoCuenta.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipoCuenta.Appearance.Options.UseFont = true;
        lblTipoCuenta.Location = new Point(12, 159);
        lblTipoCuenta.Name = "lblTipoCuenta";
        lblTipoCuenta.Size = new Size(83, 15);
        lblTipoCuenta.TabIndex = 9;
        lblTipoCuenta.Text = "Clase de cuenta";
        // 
        // lueTipoCuenta
        // 
        lueTipoCuenta.Location = new Point(139, 157);
        lueTipoCuenta.Name = "lueTipoCuenta";
        lueTipoCuenta.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTipoCuenta.Properties.Appearance.Options.UseFont = true;
        lueTipoCuenta.Properties.AutoHeight = false;
        lueTipoCuenta.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTipoCuenta.Size = new Size(236, 22);
        lueTipoCuenta.TabIndex = 10;
        // 
        // lblClaseCuenta
        // 
        lblClaseCuenta.Appearance.Font = new Font("Segoe UI", 9F);
        lblClaseCuenta.Appearance.Options.UseFont = true;
        lblClaseCuenta.Location = new Point(12, 187);
        lblClaseCuenta.Name = "lblClaseCuenta";
        lblClaseCuenta.Size = new Size(93, 15);
        lblClaseCuenta.TabIndex = 11;
        lblClaseCuenta.Text = "Propiedad cuenta";
        // 
        // lueClaseCuenta
        // 
        lueClaseCuenta.Location = new Point(139, 185);
        lueClaseCuenta.Name = "lueClaseCuenta";
        lueClaseCuenta.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueClaseCuenta.Properties.Appearance.Options.UseFont = true;
        lueClaseCuenta.Properties.AutoHeight = false;
        lueClaseCuenta.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueClaseCuenta.Size = new Size(236, 22);
        lueClaseCuenta.TabIndex = 12;
        // 
        // lblCuentaPadre
        // 
        lblCuentaPadre.Appearance.Font = new Font("Segoe UI", 9F);
        lblCuentaPadre.Appearance.Options.UseFont = true;
        lblCuentaPadre.Location = new Point(12, 215);
        lblCuentaPadre.Name = "lblCuentaPadre";
        lblCuentaPadre.Size = new Size(71, 15);
        lblCuentaPadre.TabIndex = 13;
        lblCuentaPadre.Text = "Cuenta padre";
        // 
        // lueCuentaPadre
        // 
        lueCuentaPadre.Location = new Point(139, 213);
        lueCuentaPadre.Name = "lueCuentaPadre";
        lueCuentaPadre.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        lueCuentaPadre.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCuentaPadre.Properties.Appearance.Options.UseFont = true;
        lueCuentaPadre.Properties.AutoHeight = false;
        lueCuentaPadre.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCuentaPadre.Size = new Size(236, 22);
        lueCuentaPadre.TabIndex = 14;
        // 
        // lblMoneda
        // 
        lblMoneda.Appearance.Font = new Font("Segoe UI", 9F);
        lblMoneda.Appearance.Options.UseFont = true;
        lblMoneda.Location = new Point(12, 243);
        lblMoneda.Name = "lblMoneda";
        lblMoneda.Size = new Size(44, 15);
        lblMoneda.TabIndex = 15;
        lblMoneda.Text = "Moneda";
        // 
        // txtMoneda
        // 
        txtMoneda.Location = new Point(139, 241);
        txtMoneda.Name = "txtMoneda";
        txtMoneda.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtMoneda.Properties.Appearance.Options.UseFont = true;
        txtMoneda.Properties.AutoHeight = false;
        txtMoneda.Properties.MaxLength = 3;
        txtMoneda.Size = new Size(80, 22);
        txtMoneda.TabIndex = 16;
        // 
        // lblSaldo
        // 
        lblSaldo.Appearance.Font = new Font("Segoe UI", 9F);
        lblSaldo.Appearance.Options.UseFont = true;
        lblSaldo.Location = new Point(12, 266);
        lblSaldo.Name = "lblSaldo";
        lblSaldo.Size = new Size(29, 15);
        lblSaldo.TabIndex = 17;
        lblSaldo.Text = "Saldo";
        // 
        // txtSaldo
        // 
        txtSaldo.Location = new Point(139, 269);
        txtSaldo.Name = "txtSaldo";
        txtSaldo.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSaldo.Properties.Appearance.Options.UseFont = true;
        txtSaldo.Properties.AutoHeight = false;
        txtSaldo.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
        txtSaldo.Properties.MaskSettings.Set("mask", "n2");
        txtSaldo.Size = new Size(114, 22);
        txtSaldo.TabIndex = 18;
        // 
        // lblNivel
        // 
        lblNivel.Appearance.Font = new Font("Segoe UI", 9F);
        lblNivel.Appearance.Options.UseFont = true;
        lblNivel.Location = new Point(270, 272);
        lblNivel.Name = "lblNivel";
        lblNivel.Size = new Size(27, 15);
        lblNivel.TabIndex = 19;
        lblNivel.Text = "Nivel";
        // 
        // txtNivel
        // 
        txtNivel.Location = new Point(318, 269);
        txtNivel.Name = "txtNivel";
        txtNivel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtNivel.Properties.Appearance.Options.UseFont = true;
        txtNivel.Properties.AutoHeight = false;
        txtNivel.Properties.ReadOnly = true;
        txtNivel.Size = new Size(57, 22);
        txtNivel.TabIndex = 20;
        // 
        // chkTitulo
        // 
        chkTitulo.Location = new Point(139, 297);
        chkTitulo.Name = "chkTitulo";
        chkTitulo.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkTitulo.Properties.Appearance.Options.UseFont = true;
        chkTitulo.Properties.Caption = "Titulo / cuenta agrupadora";
        chkTitulo.Size = new Size(165, 20);
        chkTitulo.TabIndex = 21;
        // 
        // chkPermiteMovimiento
        // 
        chkPermiteMovimiento.Location = new Point(139, 319);
        chkPermiteMovimiento.Name = "chkPermiteMovimiento";
        chkPermiteMovimiento.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkPermiteMovimiento.Properties.Appearance.Options.UseFont = true;
        chkPermiteMovimiento.Properties.Caption = "Permite movimientos";
        chkPermiteMovimiento.Size = new Size(208, 20);
        chkPermiteMovimiento.TabIndex = 22;
        // 
        // chkConfidencial
        // 
        chkConfidencial.Location = new Point(139, 342);
        chkConfidencial.Name = "chkConfidencial";
        chkConfidencial.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkConfidencial.Properties.Appearance.Options.UseFont = true;
        chkConfidencial.Properties.Caption = "Confidencial";
        chkConfidencial.Size = new Size(105, 20);
        chkConfidencial.TabIndex = 23;
        // 
        // chkCuentaMonetaria
        // 
        chkCuentaMonetaria.Location = new Point(258, 343);
        chkCuentaMonetaria.Name = "chkCuentaMonetaria";
        chkCuentaMonetaria.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkCuentaMonetaria.Properties.Appearance.Options.UseFont = true;
        chkCuentaMonetaria.Properties.Caption = "Cuenta monetaria";
        chkCuentaMonetaria.Size = new Size(128, 20);
        chkCuentaMonetaria.TabIndex = 24;
        // 
        // chkCuentaAsociada
        // 
        chkCuentaAsociada.Location = new Point(139, 364);
        chkCuentaAsociada.Name = "chkCuentaAsociada";
        chkCuentaAsociada.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkCuentaAsociada.Properties.Appearance.Options.UseFont = true;
        chkCuentaAsociada.Properties.Caption = "Cuenta asociada";
        chkCuentaAsociada.Size = new Size(114, 20);
        chkCuentaAsociada.TabIndex = 25;
        // 
        // chkRevaluaIndice
        // 
        chkRevaluaIndice.Location = new Point(258, 365);
        chkRevaluaIndice.Name = "chkRevaluaIndice";
        chkRevaluaIndice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkRevaluaIndice.Properties.Appearance.Options.UseFont = true;
        chkRevaluaIndice.Properties.Caption = "Reval. segun indice";
        chkRevaluaIndice.Size = new Size(128, 20);
        chkRevaluaIndice.TabIndex = 26;
        // 
        // chkBloquearManual
        // 
        chkBloquearManual.Location = new Point(139, 387);
        chkBloquearManual.Name = "chkBloquearManual";
        chkBloquearManual.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkBloquearManual.Properties.Appearance.Options.UseFont = true;
        chkBloquearManual.Properties.Caption = "Bloquear contabilizacion manual";
        chkBloquearManual.Size = new Size(228, 20);
        chkBloquearManual.TabIndex = 27;
        // 
        // chkFlujoCaja
        // 
        chkFlujoCaja.Location = new Point(139, 409);
        chkFlujoCaja.Name = "chkFlujoCaja";
        chkFlujoCaja.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkFlujoCaja.Properties.Appearance.Options.UseFont = true;
        chkFlujoCaja.Properties.Caption = "Relevante para flujo de caja";
        chkFlujoCaja.Size = new Size(228, 20);
        chkFlujoCaja.TabIndex = 28;
        // 
        // chkCentroCosto
        // 
        chkCentroCosto.Location = new Point(139, 454);
        chkCentroCosto.Name = "chkCentroCosto";
        chkCentroCosto.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkCentroCosto.Properties.Appearance.Options.UseFont = true;
        chkCentroCosto.Properties.Caption = "Centro de costo";
        chkCentroCosto.Size = new Size(111, 20);
        chkCentroCosto.TabIndex = 29;
        // 
        // chkTercero
        // 
        chkTercero.Location = new Point(258, 454);
        chkTercero.Name = "chkTercero";
        chkTercero.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkTercero.Properties.Appearance.Options.UseFont = true;
        chkTercero.Properties.Caption = "Tercero";
        chkTercero.Size = new Size(77, 20);
        chkTercero.TabIndex = 30;
        // 
        // chkProyecto
        // 
        chkProyecto.Location = new Point(139, 476);
        chkProyecto.Name = "chkProyecto";
        chkProyecto.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkProyecto.Properties.Appearance.Options.UseFont = true;
        chkProyecto.Properties.Caption = "Proyecto";
        chkProyecto.Size = new Size(85, 20);
        chkProyecto.TabIndex = 31;
        // 
        // chkActivo
        // 
        chkActivo.Location = new Point(258, 476);
        chkActivo.Name = "chkActivo";
        chkActivo.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkActivo.Properties.Appearance.Options.UseFont = true;
        chkActivo.Properties.Caption = "Activo";
        chkActivo.Size = new Size(69, 20);
        chkActivo.TabIndex = 32;
        // 
        // trlAccounts
        // 
        trlAccounts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        trlAccounts.Appearance.Row.Options.UseFont = true;
        trlAccounts.Columns.AddRange(new TreeListColumn[] { colAccountDisplay });
        trlAccounts.KeyFieldName = "Id";
        trlAccounts.Location = new Point(392, 13);
        trlAccounts.MinWidth = 17;
        trlAccounts.Name = "trlAccounts";
        trlAccounts.OptionsBehavior.Editable = false;
        trlAccounts.OptionsView.ShowColumns = false;
        trlAccounts.OptionsView.ShowHorzLines = false;
        trlAccounts.OptionsView.ShowIndicator = false;
        trlAccounts.OptionsView.ShowVertLines = false;
        trlAccounts.ParentFieldName = "ParentAccountId";
        trlAccounts.Size = new Size(425, 483);
        trlAccounts.TabIndex = 20;
        trlAccounts.TreeLevelWidth = 15;
        // 
        // colAccountDisplay
        // 
        colAccountDisplay.Caption = "Cuenta";
        colAccountDisplay.FieldName = "DisplayText";
        colAccountDisplay.MinWidth = 17;
        colAccountDisplay.Name = "colAccountDisplay";
        colAccountDisplay.Visible = true;
        colAccountDisplay.VisibleIndex = 0;
        colAccountDisplay.Width = 64;
        // 
        // btnTipoActivo
        // 
        btnTipoActivo.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoActivo.Appearance.Options.UseFont = true;
        btnTipoActivo.Location = new Point(834, 13);
        btnTipoActivo.Name = "btnTipoActivo";
        btnTipoActivo.Size = new Size(111, 47);
        btnTipoActivo.TabIndex = 21;
        btnTipoActivo.Text = "Activos";
        // 
        // btnTipoPasivo
        // 
        btnTipoPasivo.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoPasivo.Appearance.Options.UseFont = true;
        btnTipoPasivo.Location = new Point(834, 65);
        btnTipoPasivo.Name = "btnTipoPasivo";
        btnTipoPasivo.Size = new Size(111, 47);
        btnTipoPasivo.TabIndex = 22;
        btnTipoPasivo.Text = "Pasivos";
        // 
        // btnTipoPatrimonio
        // 
        btnTipoPatrimonio.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoPatrimonio.Appearance.Options.UseFont = true;
        btnTipoPatrimonio.Location = new Point(834, 117);
        btnTipoPatrimonio.Name = "btnTipoPatrimonio";
        btnTipoPatrimonio.Size = new Size(111, 47);
        btnTipoPatrimonio.TabIndex = 23;
        btnTipoPatrimonio.Text = "Patrimonio";
        // 
        // btnTipoIngreso
        // 
        btnTipoIngreso.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoIngreso.Appearance.Options.UseFont = true;
        btnTipoIngreso.Location = new Point(834, 169);
        btnTipoIngreso.Name = "btnTipoIngreso";
        btnTipoIngreso.Size = new Size(111, 47);
        btnTipoIngreso.TabIndex = 24;
        btnTipoIngreso.Text = "Ingresos";
        // 
        // btnTipoCosto
        // 
        btnTipoCosto.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoCosto.Appearance.Options.UseFont = true;
        btnTipoCosto.Location = new Point(834, 221);
        btnTipoCosto.Name = "btnTipoCosto";
        btnTipoCosto.Size = new Size(111, 47);
        btnTipoCosto.TabIndex = 25;
        btnTipoCosto.Text = "Costos";
        // 
        // btnTipoGasto
        // 
        btnTipoGasto.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoGasto.Appearance.Options.UseFont = true;
        btnTipoGasto.Location = new Point(834, 273);
        btnTipoGasto.Name = "btnTipoGasto";
        btnTipoGasto.Size = new Size(111, 47);
        btnTipoGasto.TabIndex = 26;
        btnTipoGasto.Text = "Gastos";
        // 
        // btnTipoOrden
        // 
        btnTipoOrden.Appearance.Font = new Font("Segoe UI", 9F);
        btnTipoOrden.Appearance.Options.UseFont = true;
        btnTipoOrden.Location = new Point(834, 325);
        btnTipoOrden.Name = "btnTipoOrden";
        btnTipoOrden.Size = new Size(111, 47);
        btnTipoOrden.TabIndex = 27;
        btnTipoOrden.Text = "Orden";
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.Font = new Font("Segoe UI", 9F);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseFont = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnGuardar.Location = new Point(851, 503);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.TabIndex = 29;
        btnGuardar.Text = "Guardar";
        // 
        // btnCancelar
        // 
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.Font = new Font("Segoe UI", 9F);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseFont = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnCancelar.Location = new Point(745, 503);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.TabIndex = 28;
        btnCancelar.Text = "Cancelar";
        // 
        // ChartOfAccountEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.None;
        CancelButton = btnCancelar;
        ClientSize = new Size(963, 551);
        Controls.Add(lblCodigo);
        Controls.Add(txtCodigo);
        Controls.Add(lblNombre);
        Controls.Add(txtNombre);
        Controls.Add(lblDescripcion);
        Controls.Add(memDescripcion);
        Controls.Add(lblCodigoExterno);
        Controls.Add(txtCodigoExterno);
        Controls.Add(lblTipoCuenta);
        Controls.Add(lueTipoCuenta);
        Controls.Add(lblClaseCuenta);
        Controls.Add(lueClaseCuenta);
        Controls.Add(lblCuentaPadre);
        Controls.Add(lueCuentaPadre);
        Controls.Add(lblMoneda);
        Controls.Add(txtMoneda);
        Controls.Add(lblSaldo);
        Controls.Add(txtSaldo);
        Controls.Add(lblNivel);
        Controls.Add(txtNivel);
        Controls.Add(chkTitulo);
        Controls.Add(chkPermiteMovimiento);
        Controls.Add(chkConfidencial);
        Controls.Add(chkCuentaMonetaria);
        Controls.Add(chkCuentaAsociada);
        Controls.Add(chkRevaluaIndice);
        Controls.Add(chkBloquearManual);
        Controls.Add(chkFlujoCaja);
        Controls.Add(chkCentroCosto);
        Controls.Add(chkTercero);
        Controls.Add(chkProyecto);
        Controls.Add(chkActivo);
        Controls.Add(trlAccounts);
        Controls.Add(btnTipoActivo);
        Controls.Add(btnTipoPasivo);
        Controls.Add(btnTipoPatrimonio);
        Controls.Add(btnTipoIngreso);
        Controls.Add(btnTipoCosto);
        Controls.Add(btnTipoGasto);
        Controls.Add(btnTipoOrden);
        Controls.Add(btnGuardar);
        Controls.Add(btnCancelar);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ChartOfAccountEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Plan de cuentas";
        ((System.ComponentModel.ISupportInitialize)txtCodigo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNombre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescripcion.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCodigoExterno.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTipoCuenta.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueClaseCuenta.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCuentaPadre.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtMoneda.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSaldo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtNivel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkTitulo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkPermiteMovimiento.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkConfidencial.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkCuentaMonetaria.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkCuentaAsociada.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkRevaluaIndice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkBloquearManual.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkFlujoCaja.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkCentroCosto.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkTercero.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkProyecto.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkActivo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)trlAccounts).EndInit();
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
    private LabelControl lblCodigo;
    private TextEdit txtCodigo;
    private LabelControl lblNombre;
    private TextEdit txtNombre;
    private LabelControl lblDescripcion;
    private MemoEdit memDescripcion;
    private LabelControl lblCodigoExterno;
    private TextEdit txtCodigoExterno;
    private LabelControl lblTipoCuenta;
    private LookUpEdit lueTipoCuenta;
    private LabelControl lblClaseCuenta;
    private LookUpEdit lueClaseCuenta;
    private LabelControl lblCuentaPadre;
    private LookUpEdit lueCuentaPadre;
    private LabelControl lblMoneda;
    private TextEdit txtMoneda;
    private LabelControl lblSaldo;
    private TextEdit txtSaldo;
    private LabelControl lblNivel;
    private TextEdit txtNivel;
    private CheckEdit chkTitulo;
    private CheckEdit chkPermiteMovimiento;
    private CheckEdit chkConfidencial;
    private CheckEdit chkCuentaMonetaria;
    private CheckEdit chkCuentaAsociada;
    private CheckEdit chkRevaluaIndice;
    private CheckEdit chkBloquearManual;
    private CheckEdit chkFlujoCaja;
    private CheckEdit chkCentroCosto;
    private CheckEdit chkTercero;
    private CheckEdit chkProyecto;
    private CheckEdit chkActivo;
    private TreeList trlAccounts;
    private TreeListColumn colAccountDisplay;
    private SimpleButton btnTipoActivo;
    private SimpleButton btnTipoPasivo;
    private SimpleButton btnTipoPatrimonio;
    private SimpleButton btnTipoIngreso;
    private SimpleButton btnTipoCosto;
    private SimpleButton btnTipoGasto;
    private SimpleButton btnTipoOrden;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}


