using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace CoreUI.Controls;

public enum eNotificationType
{
    Accent,
    Information,
    Success,
    Warning,
    Error
}

[PseudoClasses(PC_Icon)]
[TemplatePart(PART_CloseButton, typeof(Button))]
public class Banner : HeaderedContentControl
{
    public const string PC_Icon = ":icon";
    public const string PART_CloseButton = "PART_CloseButton";

    private Button? _closeButton;

    public static readonly StyledProperty<bool> CanCloseProperty = AvaloniaProperty.Register<Banner, bool>(
        nameof(CanClose));

    public bool CanClose
    {
        get => GetValue(CanCloseProperty);
        set => SetValue(CanCloseProperty, value);
    }

    public static readonly StyledProperty<bool> ShowIconProperty = AvaloniaProperty.Register<Banner, bool>(
        nameof(ShowIcon), true);

    public bool ShowIcon
    {
        get => GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    public static readonly StyledProperty<string?> IconProperty = AvaloniaProperty.Register<Banner, string?>(
        nameof(Icon));

    public string? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<eNotificationType> TypeProperty = AvaloniaProperty.Register<Banner, eNotificationType>(
        nameof(Type));

    public eNotificationType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Button.ClickEvent.RemoveHandler(OnCloseClick, _closeButton);
        _closeButton = e.NameScope.Find<Button>(PART_CloseButton);
        Button.ClickEvent.AddHandler(OnCloseClick, _closeButton);
    }

    private void OnCloseClick(object? sender, RoutedEventArgs args)
    {
        IsVisible = false;
    }
}