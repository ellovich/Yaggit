namespace CoreUI.Controls;

public class AvroView : UserControl
{
    public static readonly StyledProperty<bool> IsTitleBarVisibleProperty =
        AvroWindow.IsTitleBarVisibleProperty.AddOwner<AvroView>();

    public bool IsTitleBarVisible
    {
        get => GetValue(IsTitleBarVisibleProperty);
        set => SetValue(IsTitleBarVisibleProperty, value);
    }

    public static readonly StyledProperty<object?> LeftContentProperty =
        AvroWindow.LeftContentProperty.AddOwner<AvroView>();

    public object? LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    public static readonly StyledProperty<object?> RightContentProperty =
        AvroWindow.RightContentProperty.AddOwner<AvroView>();

    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    public static readonly StyledProperty<object?> TitleBarContentProperty =
        AvroWindow.TitleBarContentProperty.AddOwner<AvroView>();

    public object? TitleBarContent
    {
        get => GetValue(TitleBarContentProperty);
        set => SetValue(TitleBarContentProperty, value);
    }

    public static readonly StyledProperty<Thickness> TitleBarMarginProperty =
        AvroWindow.TitleBarMarginProperty.AddOwner<AvroView>();

    public Thickness TitleBarMargin
    {
        get => GetValue(TitleBarMarginProperty);
        set => SetValue(TitleBarMarginProperty, value);
    }
}