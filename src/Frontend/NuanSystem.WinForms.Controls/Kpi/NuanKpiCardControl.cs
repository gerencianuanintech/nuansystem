using System.ComponentModel;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Controls.Kpi;

public sealed class NuanKpiCardControl : XtraUserControl
{
    private const int DefaultWidth = 220;
    private const int DefaultHeight = 118;
    private string title = "Pending";
    private string description = "Pendientes de procesamiento";
    private string valueText = "0";
    private Color headerColor = Color.FromArgb(71, 85, 105);
    private Color footerBackColor = Color.White;
    private Color borderColor = Color.FromArgb(221, 226, 240);
    private Color titleColor = Color.White;
    private Color valueColor = Color.White;
    private Color descriptionColor = Color.FromArgb(51, 65, 85);
    private Image? iconImage;
    private SvgImage? svgIcon;
    private bool useSvgIcon;
    private int cornerRadius = 10;
    private int headerHeight = 78;
    private bool showBorder = true;
    private bool showShadow;
    private string fallbackIconText = string.Empty;

    public NuanKpiCardControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.Transparent;
        Size = new Size(DefaultWidth, DefaultHeight);
        MinimumSize = new Size(160, 88);
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
    }

    [Category("Nuan KPI")]
    [DefaultValue("Pending")]
    public string Title
    {
        get => title;
        set
        {
            title = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
    [DefaultValue("Pendientes de procesamiento")]
    public string Description
    {
        get => description;
        set
        {
            description = value ?? string.Empty;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
    public Color HeaderColor
    {
        get => headerColor;
        set
        {
            headerColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
    public Color FooterBackColor
    {
        get => footerBackColor;
        set
        {
            footerBackColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
    public Color BorderColor
    {
        get => borderColor;
        set
        {
            borderColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
    public Color TitleColor
    {
        get => titleColor;
        set
        {
            titleColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
    public Color ValueColor
    {
        get => valueColor;
        set
        {
            valueColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
    public Color DescriptionColor
    {
        get => descriptionColor;
        set
        {
            descriptionColor = value;
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
    [DefaultValue(78)]
    public int HeaderHeight
    {
        get => headerHeight;
        set
        {
            headerHeight = Math.Max(40, value);
            Invalidate();
        }
    }

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
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

    [Category("Nuan KPI")]
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

    public void ApplyStyle(NuanKpiCardStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        Title = style.Title;
        Description = style.Description;
        HeaderColor = style.HeaderColor;
        FooterBackColor = style.FooterBackColor;
        BorderColor = style.BorderColor;
        DescriptionColor = style.DescriptionColor;
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
        using var cardPath = DrawRoundedRectangle(cardBounds, cornerRadius);
        e.Graphics.SetClip(cardPath);

        DrawFooter(e.Graphics, cardBounds);
        DrawHeader(e.Graphics, cardBounds);
        e.Graphics.ResetClip();

        if (showShadow)
        {
            DrawShadow(e.Graphics, cardBounds);
        }

        if (showBorder)
        {
            using var borderPen = new Pen(borderColor);
            e.Graphics.DrawPath(borderPen, cardPath);
        }

        DrawIcon(e.Graphics, HeaderBounds(cardBounds));
        DrawText(e.Graphics, cardBounds);
    }

    private GraphicsPath DrawRoundedRectangle(Rectangle bounds, int radius)
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

    private void DrawHeader(Graphics graphics, Rectangle cardBounds)
    {
        using var brush = new SolidBrush(headerColor);
        graphics.FillRectangle(brush, HeaderBounds(cardBounds));
    }

    private void DrawFooter(Graphics graphics, Rectangle cardBounds)
    {
        using var brush = new SolidBrush(footerBackColor);
        graphics.FillRectangle(brush, cardBounds);
    }

    private void DrawIcon(Graphics graphics, Rectangle headerBounds)
    {
        var iconSize = Math.Max(34, Math.Min(48, headerBounds.Height - 24));
        var iconBounds = new Rectangle(18, headerBounds.Top + (headerBounds.Height - iconSize) / 2, iconSize, iconSize);

        if (useSvgIcon && svgIcon is not null && TryDrawSvgIcon(graphics, iconBounds))
        {
            return;
        }

        if (iconImage is not null)
        {
            graphics.DrawImage(iconImage, iconBounds);
            return;
        }

        if (string.IsNullOrWhiteSpace(fallbackIconText))
        {
            return;
        }

        using var iconFont = new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point);
        using var iconBrush = new SolidBrush(Color.FromArgb(226, 232, 240));
        using var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        graphics.DrawString(fallbackIconText, iconFont, iconBrush, iconBounds, format);
    }

    private void DrawText(Graphics graphics, Rectangle cardBounds)
    {
        var headerBounds = HeaderBounds(cardBounds);
        var rightX = Math.Max(78, cardBounds.Width - 134);
        var valueBounds = new Rectangle(rightX, headerBounds.Top + 12, cardBounds.Width - rightX - 20, 38);
        var titleBounds = new Rectangle(rightX, headerBounds.Top + 50, cardBounds.Width - rightX - 20, 18);
        var footerBounds = new Rectangle(16, headerBounds.Bottom + 9, cardBounds.Width - 32, Math.Max(16, cardBounds.Bottom - headerBounds.Bottom - 12));

        using var valueFont = new Font("Segoe UI Semibold", 23F, FontStyle.Bold, GraphicsUnit.Point);
        using var titleFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        using var descriptionFont = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
        using var valueBrush = new SolidBrush(valueColor);
        using var titleBrush = new SolidBrush(titleColor);
        using var descriptionBrush = new SolidBrush(descriptionColor);
        using var rightFormat = new StringFormat
        {
            Alignment = StringAlignment.Far,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        using var descriptionFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        graphics.DrawString(valueText, valueFont, valueBrush, valueBounds, rightFormat);
        graphics.DrawString(title, titleFont, titleBrush, titleBounds, rightFormat);
        graphics.DrawString(description, descriptionFont, descriptionBrush, footerBounds, descriptionFormat);
    }

    private void DrawShadow(Graphics graphics, Rectangle cardBounds)
    {
        using var shadowPath = DrawRoundedRectangle(new Rectangle(cardBounds.X + 1, cardBounds.Y + 2, cardBounds.Width - 1, cardBounds.Height - 1), cornerRadius);
        using var shadowPen = new Pen(Color.FromArgb(28, 15, 23, 42));
        graphics.DrawPath(shadowPen, shadowPath);
    }

    private bool TryDrawSvgIcon(Graphics graphics, Rectangle iconBounds)
    {
        try
        {
            var createMethod = typeof(SvgImage).GetMethod("Create", new[] { typeof(SvgImage), typeof(Size) });
            var svgBitmap = createMethod?.Invoke(null, new object[] { svgIcon!, iconBounds.Size });
            var renderMethod = svgBitmap?.GetType().GetMethod("Render", Type.EmptyTypes);
            using var rendered = renderMethod?.Invoke(svgBitmap, Array.Empty<object>()) as Image;
            if (rendered is null)
            {
                return false;
            }

            graphics.DrawImage(rendered, iconBounds);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Rectangle HeaderBounds(Rectangle cardBounds)
    {
        var effectiveHeaderHeight = Math.Min(Math.Max(40, headerHeight), Math.Max(40, cardBounds.Height - 28));
        return new Rectangle(cardBounds.Left, cardBounds.Top, cardBounds.Width, effectiveHeaderHeight);
    }

    private bool ShouldSerializeHeaderColor() => headerColor != Color.FromArgb(71, 85, 105);

    private bool ShouldSerializeFooterBackColor() => footerBackColor != Color.White;

    private bool ShouldSerializeBorderColor() => borderColor != Color.FromArgb(221, 226, 240);

    private bool ShouldSerializeTitleColor() => titleColor != Color.White;

    private bool ShouldSerializeValueColor() => valueColor != Color.White;

    private bool ShouldSerializeDescriptionColor() => descriptionColor != Color.FromArgb(51, 65, 85);
}
