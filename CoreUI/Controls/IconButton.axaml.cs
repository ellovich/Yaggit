using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace CoreUI.Controls;

[TemplatePart(PART_RootPanel, typeof(Panel))]
[PseudoClasses(PC_Right, PC_Left, PC_Top, PC_Bottom, PC_Empty, PC_EmptyContent)]
public class IconButton : Button
{
    public const string PC_Right = ":right";
    public const string PC_Left = ":left";
    public const string PC_Top = ":top";
    public const string PC_Bottom = ":bottom";
    public const string PC_Empty = ":empty";
    public const string PC_EmptyContent = ":empty-content";
    public const string PART_RootPanel = "PART_RootPanel";
    
    private Panel? _rootPanel;

    public static readonly StyledProperty<string?> SvgPathProperty =
        AvaloniaProperty.Register<IconButton, string?>(nameof(SvgPath));

    public static readonly StyledProperty<double> SvgSizeProperty =
        AvaloniaProperty.Register<IconButton, double>(nameof(SvgSize), 22);

    public static readonly StyledProperty<bool> IsCollapsedProperty =
        AvaloniaProperty.Register<IconButton, bool>(nameof(IsCollapsed), false);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<IconButton, bool>(nameof(IsLoading));

    public static readonly StyledProperty<ePosition> IconPlacementProperty =
        AvaloniaProperty.Register<IconButton, ePosition>(nameof(IconPlacement), defaultValue: ePosition.Left);

    public string? SvgPath
    {
        get => this.GetValue(SvgPathProperty);
        set => SetValue(SvgPathProperty, value);
    }

    public double SvgSize
    {
        get => this.GetValue(SvgSizeProperty);
        set => SetValue(SvgSizeProperty, value);
    }

    public bool IsCollapsed
    {
        get => this.GetValue(IsCollapsedProperty) || this.GetValue(ContentProperty) is null;
        set => SetValue(IsCollapsedProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public ePosition IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    static IconButton()
    {
        IconPlacementProperty.Changed.AddClassHandler<IconButton, ePosition>((o, e) =>
        {
            o.SetPlacement(e.NewValue.Value, o.SvgPath);
            o.InvalidateRootPanel();
        });
        SvgPathProperty.Changed.AddClassHandler<IconButton, string?>((o, e) =>
        {
            o.SetPlacement(o.IconPlacement, e.NewValue.Value);
        });
        ContentProperty.Changed.AddClassHandler<IconButton>((o, e) => o.SetEmptyContent());
    }
    
    private void InvalidateRootPanel() => _rootPanel?.InvalidateArrange();

    private void SetEmptyContent()
    {
        PseudoClasses.Set(PC_EmptyContent, Presenter?.Content is null);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _rootPanel = e.NameScope.Find<Panel>(PART_RootPanel);
        SetEmptyContent();
        SetPlacement(IconPlacement, SvgPath);
    }

    private void SetPlacement(ePosition placement, object? icon)
    {
        if (icon is null)
        {
            PseudoClasses.Set(PC_Empty, true);
            PseudoClasses.Set(PC_Left, false);
            PseudoClasses.Set(PC_Right, false);
            PseudoClasses.Set(PC_Top, false);
            PseudoClasses.Set(PC_Bottom, false);
            return;
        }
        PseudoClasses.Set(PC_Empty, false);
        PseudoClasses.Set(PC_Left, placement == ePosition.Left);
        PseudoClasses.Set(PC_Right, placement == ePosition.Right);
        PseudoClasses.Set(PC_Top, placement == ePosition.Top);
        PseudoClasses.Set(PC_Bottom, placement == ePosition.Bottom);
    }
}