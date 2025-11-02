using Avalonia.Media;

namespace CoreUI.Controls;

public class Avatar : Button
{
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<Avatar, IImage?>(nameof(Source));

    public static readonly StyledProperty<object?> HoverMaskProperty =
        AvaloniaProperty.Register<Avatar, object?>(nameof(HoverMask));

    public static readonly StyledProperty<eStatus?> StatusProperty =
        AvaloniaProperty.Register<Avatar, eStatus?>(nameof(Status), eStatus.Unknown);

    public static readonly StyledProperty<bool> IsStatusVisibleProperty =
        AvaloniaProperty.Register<Avatar, bool>(nameof(IsStatusVisible), false);

    public static readonly StyledProperty<int> SizeProperty =
        AvaloniaProperty.Register<Avatar, int>(nameof(Size), 40);

    public static readonly StyledProperty<SolidColorBrush> UserColorBrushProperty =
        AvaloniaProperty.Register<Avatar, SolidColorBrush>(nameof(UserColorBrush), new SolidColorBrush(Colors.Red));

    public static readonly StyledProperty<string?> UsernameProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(Username));

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public object? HoverMask
    {
        get => GetValue(HoverMaskProperty);
        set => SetValue(HoverMaskProperty, value);
    }

    public eStatus? Status
    {
        get => this.GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public bool IsStatusVisible
    {
        get => this.GetValue(IsStatusVisibleProperty);
        set => SetValue(IsStatusVisibleProperty, value);
    }

    public int Size
    {
        get => this.GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public SolidColorBrush UserColorBrush
    {
        get => this.GetValue(UserColorBrushProperty);
        set => SetValue(UserColorBrushProperty, value);
    }

    public string? Username
    {
        get => this.GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }
}