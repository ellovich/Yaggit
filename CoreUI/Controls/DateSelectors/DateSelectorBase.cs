using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace CoreUI.Controls;

public class DateSelectorBase : TemplatedControl, IInnerContentControl, IPopupInnerContent
{
    public static readonly StyledProperty<string?> DisplayFormatProperty =
        AvaloniaProperty.Register<TimeSelector, string?>(
            nameof(DisplayFormat), "dd.MM.yyyy");

    public static readonly StyledProperty<AvaloniaList<DateRange>> BlackoutDatesProperty =
        AvaloniaProperty.Register<DateSelectorBase, AvaloniaList<DateRange>>(nameof(BlackoutDates));

    public static readonly StyledProperty<IDateSelector?> BlackoutDateRuleProperty =
        AvaloniaProperty.Register<DateSelectorBase, IDateSelector?>(nameof(BlackoutDateRule));

    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<DateSelectorBase, DayOfWeek>(
            nameof(FirstDayOfWeek), DateTimeHelper.GetCurrentDateTimeFormatInfo().FirstDayOfWeek);

    public static readonly StyledProperty<bool> IsTodayHighlightedProperty =
        AvaloniaProperty.Register<DateSelectorBase, bool>(nameof(IsTodayHighlighted), true);

    public static readonly StyledProperty<object?> InnerLeftContentProperty =
        AvaloniaProperty.Register<DateSelectorBase, object?>(
            nameof(InnerLeftContent));

    public static readonly StyledProperty<object?> InnerRightContentProperty =
        AvaloniaProperty.Register<DateSelectorBase, object?>(
            nameof(InnerRightContent));

    public static readonly StyledProperty<object?> PopupInnerTopContentProperty =
        AvaloniaProperty.Register<DateSelectorBase, object?>(
            nameof(PopupInnerTopContent));

    public static readonly StyledProperty<object?> PopupInnerBottomContentProperty =
        AvaloniaProperty.Register<DateSelectorBase, object?>(
            nameof(PopupInnerBottomContent));

    public static readonly StyledProperty<bool> IsDropdownOpenProperty = AvaloniaProperty.Register<DateSelectorBase, bool>(
        nameof(IsDropdownOpen), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsReadonlyProperty = AvaloniaProperty.Register<DateSelectorBase, bool>(
        nameof(IsReadonly));

    public AvaloniaList<DateRange> BlackoutDates
    {
        get => GetValue(BlackoutDatesProperty);
        set => SetValue(BlackoutDatesProperty, value);
    }

    public IDateSelector? BlackoutDateRule
    {
        get => GetValue(BlackoutDateRuleProperty);
        set => SetValue(BlackoutDateRuleProperty, value);
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    public bool IsTodayHighlighted
    {
        get => GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }

    public bool IsReadonly
    {
        get => GetValue(IsReadonlyProperty);
        set => SetValue(IsReadonlyProperty, value);
    }

    public bool IsDropdownOpen
    {
        get => GetValue(IsDropdownOpenProperty);
        set => SetValue(IsDropdownOpenProperty, value);
    }

    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }

    public object? PopupInnerTopContent
    {
        get => GetValue(PopupInnerTopContentProperty);
        set => SetValue(PopupInnerTopContentProperty, value);
    }

    public object? PopupInnerBottomContent
    {
        get => GetValue(PopupInnerBottomContentProperty);
        set => SetValue(PopupInnerBottomContentProperty, value);
    }

    public string? DisplayFormat
    {
        get => GetValue(DisplayFormatProperty);
        set => SetValue(DisplayFormatProperty, value);
    }
}