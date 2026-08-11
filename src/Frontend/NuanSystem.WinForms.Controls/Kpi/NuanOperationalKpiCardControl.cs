using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Controls.Kpi;

public sealed class NuanOperationalKpiCardControl : XtraUserControl
{
    private const int DefaultWidth = 360;
    private const int DefaultHeight = 160;
    private string title = "Indicador";
    private string valueText = "0";
    private string unitText = string.Empty;
    private string statusText = string.Empty;
    private string fallbackIconText = string.Empty;
    private Color accentColor = Color.FromArgb(0, 184, 148);
    private Color cardBackColor = Color.White;
    private Color borderColor = Color.FromArgb(221, 226, 240);
    private Color titleColor = Color.FromArgb(23, 32, 51);
    private Color statusBackColor = Color.FromArgb(230, 250, 246);
    private Color statusForeColor = Color.FromArgb(0, 137, 111);
    private Image? iconImage;
    private SvgImage? svgIcon;
    private bool useSvgIcon;
    private int cornerRadius = 10;
    private bool showBorder = true;
    private bool showShadow;

    public NuanOperationalKpiCardControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.Transparent;
        Size = new Size(DefaultWidth, DefaultHeight);
        MinimumSize = new Size(140, 68);
        AccessibleRole = AccessibleRole.StaticText;
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue("Indicador")]
    public string Title
    {
        get => title;
        set
        {
            title = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue("0")]
    public string ValueText
    {
        get => valueText;
        set
        {
            valueText = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue("")]
    public string UnitText
    {
        get => unitText;
        set
        {
            unitText = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue("")]
    public string StatusText
    {
        get => statusText;
        set
        {
            statusText = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue("")]
    public string FallbackIconText
    {
        get => fallbackIconText;
        set
        {
            fallbackIconText = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    public Color AccentColor
    {
        get => accentColor;
        set
        {
            accentColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    public Color CardBackColor
    {
        get => cardBackColor;
        set
        {
            cardBackColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    public Color BorderColor
    {
        get => borderColor;
        set
        {
            borderColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    public Color TitleColor
    {
        get => titleColor;
        set
        {
            titleColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    public Color StatusBackColor
    {
        get => statusBackColor;
        set
        {
            statusBackColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    public Color StatusForeColor
    {
        get => statusForeColor;
        set
        {
            statusForeColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue(null)]
    public Image? IconImage
    {
        get => iconImage;
        set
        {
            iconImage = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue(null)]
    public SvgImage? SvgIcon
    {
        get => svgIcon;
        set
        {
            svgIcon = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue(false)]
    public bool UseSvgIcon
    {
        get => useSvgIcon;
        set
        {
            useSvgIcon = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue(10)]
    public int CornerRadius
    {
        get => cornerRadius;
        set
        {
            cornerRadius = Math.Max(0, value);
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue(true)]
    public bool ShowBorder
    {
        get => showBorder;
        set
        {
            showBorder = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI operativo")]
    [DefaultValue(false)]
    public bool ShowShadow
    {
        get => showShadow;
        set
        {
            showShadow = value;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (Width <= 1 || Height <= 1)
        {
            return;
        }

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var cardBounds = new Rectangle(0, 0, Width - 1, Height - 1);
        using var cardPath = RoundedRectangle(cardBounds, cornerRadius);
        e.Graphics.SetClip(cardPath);
        using (var backgroundBrush = new SolidBrush(cardBackColor))
        {
            e.Graphics.FillRectangle(backgroundBrush, cardBounds);
        }

        e.Graphics.ResetClip();

        if (showShadow)
        {
            using var shadowPath = RoundedRectangle(
                new Rectangle(cardBounds.X + 1, cardBounds.Y + 2, cardBounds.Width - 1, cardBounds.Height - 1),
                cornerRadius);
            using var shadowPen = new Pen(Color.FromArgb(28, 15, 23, 42));
            e.Graphics.DrawPath(shadowPen, shadowPath);
        }

        if (showBorder)
        {
            using var borderPen = new Pen(borderColor);
            e.Graphics.DrawPath(borderPen, cardPath);
        }

        DrawContent(e.Graphics, cardBounds);
    }

    private void DrawContent(Graphics graphics, Rectangle cardBounds)
    {
        var compact = cardBounds.Width < 220 || cardBounds.Height < 84;
        var medium = !compact && (cardBounds.Width < 300 || cardBounds.Height < 120);
        var horizontalPadding = compact ? 6 : medium ? 12 : 20;
        var iconSize = compact
            ? Math.Max(30, Math.Min(36, cardBounds.Height - 24))
            : medium
                ? Math.Max(38, Math.Min(42, cardBounds.Height - 38))
            : Math.Max(50, Math.Min(66, cardBounds.Height - 42));
        var iconBounds = new Rectangle(
            horizontalPadding,
            cardBounds.Top + (cardBounds.Height - iconSize) / 2,
            iconSize,
            iconSize);

        using (var iconBrush = new SolidBrush(accentColor))
        {
            graphics.FillEllipse(iconBrush, iconBounds);
        }

        DrawIcon(graphics, iconBounds, compact, medium);

        var contentLeft = iconBounds.Right + (compact ? 6 : medium ? 14 : 20);
        var contentRight = cardBounds.Right - horizontalPadding;
        var contentWidth = Math.Max(40, contentRight - contentLeft);
        var titleBounds = new Rectangle(
            contentLeft,
            cardBounds.Top + (compact ? 6 : medium ? 7 : 18),
            contentWidth,
            compact ? 16 : medium ? 17 : 22);
        var valueBounds = new Rectangle(
            contentLeft,
            titleBounds.Bottom + (compact || medium ? 0 : 3),
            contentWidth,
            compact ? 25 : medium ? 28 : 42);
        var statusBounds = new Rectangle(
            contentLeft,
            valueBounds.Bottom + (compact ? 0 : medium ? 5 : 7),
            contentWidth,
            compact ? 19 : medium ? 22 : 28);

        using var titleFont = new Font("Segoe UI Semibold", compact ? 7F : medium ? 8.5F : 10F, FontStyle.Bold, GraphicsUnit.Point);
        using var valueFont = CreateFittedValueFont(graphics, valueBounds, compact, medium);
        using var statusFont = new Font("Segoe UI Semibold", compact ? 6F : medium ? 7F : 8.5F, FontStyle.Bold, GraphicsUnit.Point);
        using var titleBrush = new SolidBrush(titleColor);
        using var valueBrush = new SolidBrush(accentColor);
        using var nearFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
            Trimming = StringTrimming.EllipsisCharacter
        };

        graphics.DrawString(title, titleFont, titleBrush, titleBounds, nearFormat);
        DrawValue(graphics, valueBounds, valueFont, valueBrush);
        DrawStatus(graphics, statusBounds, statusFont, compact, medium);
    }

    private void DrawIcon(Graphics graphics, Rectangle iconBounds, bool compact, bool medium)
    {
        var inset = compact ? 8 : medium ? 10 : 13;
        var contentBounds = Rectangle.Inflate(iconBounds, -inset, -inset);

        if (useSvgIcon && svgIcon is not null && TryDrawSvgIcon(graphics, contentBounds))
        {
            return;
        }

        if (iconImage is not null)
        {
            graphics.DrawImage(iconImage, contentBounds);
            return;
        }

        if (string.IsNullOrWhiteSpace(fallbackIconText))
        {
            return;
        }

        using var iconFont = new Font("Segoe UI Semibold", compact ? 10F : medium ? 12F : 15F, FontStyle.Bold, GraphicsUnit.Point);
        using var iconBrush = new SolidBrush(Color.White);
        using var iconFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        graphics.DrawString(fallbackIconText, iconFont, iconBrush, contentBounds, iconFormat);
    }

    private void DrawValue(
        Graphics graphics,
        Rectangle bounds,
        Font valueFont,
        Brush valueBrush)
    {
        var displayText = string.IsNullOrWhiteSpace(unitText)
            ? valueText
            : $"{valueText} {unitText}";
        using var valueFormat = new StringFormat
        {
            Alignment = IsNumericValue(valueText) ? StringAlignment.Far : StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
            Trimming = StringTrimming.None
        };

        graphics.DrawString(displayText, valueFont, valueBrush, bounds, valueFormat);
    }

    private void DrawStatus(Graphics graphics, Rectangle bounds, Font font, bool compact, bool medium)
    {
        if (string.IsNullOrWhiteSpace(statusText) || bounds.Width <= 8)
        {
            return;
        }

        var measured = graphics.MeasureString(statusText, font, int.MaxValue, StringFormat.GenericTypographic);
        var badgeWidth = compact || medium
            ? bounds.Width
            : Math.Min(bounds.Width, (int)Math.Ceiling(measured.Width) + 30);
        var badgeBounds = new Rectangle(bounds.Left, bounds.Top, badgeWidth, bounds.Height);
        using var badgePath = RoundedRectangle(badgeBounds, compact ? 4 : medium ? 5 : 6);
        using var backgroundBrush = new SolidBrush(statusBackColor);
        using var borderPen = new Pen(statusForeColor);
        graphics.FillPath(backgroundBrush, badgePath);
        graphics.DrawPath(borderPen, badgePath);

        var textBounds = new Rectangle(
            badgeBounds.Left + (compact ? 4 : medium ? 7 : 12),
            badgeBounds.Top,
            Math.Max(1, badgeBounds.Width - (compact ? 8 : medium ? 14 : 18)),
            badgeBounds.Height);
        using var textBrush = new SolidBrush(statusForeColor);
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap,
            Trimming = StringTrimming.EllipsisCharacter
        };
        graphics.DrawString(statusText, font, textBrush, textBounds, format);
    }

    private Font CreateFittedValueFont(Graphics graphics, Rectangle bounds, bool compact, bool medium)
    {
        var fontSize = compact ? 12F : medium ? 16F : 25F;
        var minimumSize = compact ? 8F : medium ? 11F : 14F;
        var displayText = string.IsNullOrWhiteSpace(unitText)
            ? valueText
            : $"{valueText} {unitText}";

        while (fontSize > minimumSize)
        {
            using var candidate = new Font("Segoe UI Semibold", fontSize, FontStyle.Bold, GraphicsUnit.Point);
            var valueWidth = graphics.MeasureString(displayText, candidate, int.MaxValue, StringFormat.GenericTypographic).Width;
            if (valueWidth <= bounds.Width - (compact ? 6 : 0))
            {
                break;
            }

            fontSize -= 0.5F;
        }

        return new Font("Segoe UI Semibold", Math.Max(fontSize, minimumSize), FontStyle.Bold, GraphicsUnit.Point);
    }

    private static bool IsNumericValue(string text)
    {
        const NumberStyles styles = NumberStyles.Number | NumberStyles.AllowParentheses;
        return decimal.TryParse(text, styles, CultureInfo.CurrentCulture, out _)
            || decimal.TryParse(text, styles, CultureInfo.InvariantCulture, out _);
    }

    private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        var diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    private bool TryDrawSvgIcon(Graphics graphics, Rectangle iconBounds)
    {
        try
        {
            using var rendered = svgIcon!.Render(
                iconBounds.Size,
                paletteProvider: null,
                DefaultBoolean.Default,
                DefaultBoolean.Default);
            graphics.DrawImage(rendered, iconBounds);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool ShouldSerializeAccentColor() => accentColor != Color.FromArgb(0, 184, 148);

    private bool ShouldSerializeCardBackColor() => cardBackColor != Color.White;

    private bool ShouldSerializeBorderColor() => borderColor != Color.FromArgb(221, 226, 240);

    private bool ShouldSerializeTitleColor() => titleColor != Color.FromArgb(23, 32, 51);

    private bool ShouldSerializeStatusBackColor() => statusBackColor != Color.FromArgb(230, 250, 246);

    private bool ShouldSerializeStatusForeColor() => statusForeColor != Color.FromArgb(0, 137, 111);
}
