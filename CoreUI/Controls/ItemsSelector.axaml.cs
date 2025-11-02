using Avalonia.Controls.Primitives;

namespace CoreUI.Controls;

public class ItemsSelector : TemplatedControl
{
    public static readonly StyledProperty<string?> SelectedValuesTextProperty =
        AvaloniaProperty.Register<ItemsSelector, string?>(nameof(SelectedValuesText), null);

    public string? SelectedValuesText
    {
        get => this.GetValue(SelectedValuesTextProperty);
        set => SetValue(SelectedValuesTextProperty, value);
    }
}