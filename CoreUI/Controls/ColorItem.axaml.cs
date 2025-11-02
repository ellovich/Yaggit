using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CoreUI.Controls;

public class ColorItem : TemplatedControl
{
    public static readonly StyledProperty<string> ResourceNameProperty =
    AvaloniaProperty.Register<ColorItem, string>(nameof(ResourceName), "Unknown");

    public static readonly StyledProperty<IBrush> ColorProperty =
        AvaloniaProperty.Register<ColorItem, IBrush>(nameof(Color));

    public string ResourceName
    {
        get { return GetValue(ResourceNameProperty); }
        set { SetValue(ResourceNameProperty, value); }
    }

    public IBrush Color
    {
        get { return GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }
}