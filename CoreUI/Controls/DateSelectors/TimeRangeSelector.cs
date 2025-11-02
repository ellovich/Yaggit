using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CoreUI.Controls;

[TemplatePart(PART_StartTextBox, typeof(TextBox))]
[TemplatePart(PART_EndTextBox, typeof(TextBox))]
[TemplatePart(PartNames.PART_Popup, typeof(Popup))]
[TemplatePart(PART_StartPresenter, typeof(TimeSelectorPresenter))]
[TemplatePart(PART_EndPresenter, typeof(TimeSelectorPresenter))]
[TemplatePart(PART_Button, typeof(Button))]
public class TimeRangeSelector : TimeSelectorBase, IClearControl
{
    public const string PART_StartTextBox = "PART_StartTextBox";
    public const string PART_EndTextBox = "PART_EndTextBox";
    public const string PART_StartPresenter = "PART_StartPresenter";
    public const string PART_EndPresenter = "PART_EndPresenter";
    public const string PART_Button = "PART_Button";

    public static readonly StyledProperty<TimeSpan?> StartTimeProperty =
        AvaloniaProperty.Register<TimeRangeSelector, TimeSpan?>(
            nameof(StartTime), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<TimeSpan?> EndTimeProperty =
        AvaloniaProperty.Register<TimeRangeSelector, TimeSpan?>(
            nameof(EndTime), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string?> StartWatermarkProperty =
        AvaloniaProperty.Register<TimeRangeSelector, string?>(
            nameof(StartWatermark));

    public static readonly StyledProperty<string?> EndWatermarkProperty =
        AvaloniaProperty.Register<TimeRangeSelector, string?>(
            nameof(EndWatermark));

    private Button? _button;
    private TimeSelectorPresenter? _endPresenter;
    private TextBox? _endTextBox;
    private TimeSelectorPresenter? _startPresenter;

    private TextBox? _startTextBox;
    private bool _suppressTextPresenterEvent;

    static TimeRangeSelector()
    {
        StartTimeProperty.Changed.AddClassHandler<TimeRangeSelector, TimeSpan?>((Selector, args) =>
            Selector.OnSelectionChanged(args));
        EndTimeProperty.Changed.AddClassHandler<TimeRangeSelector, TimeSpan?>((Selector, args) =>
            Selector.OnSelectionChanged(args, false));
        DisplayFormatProperty.Changed.AddClassHandler<TimeRangeSelector, string?>((Selector, args) => Selector.OnDisplayFormatChanged(args));
    }

    private void OnDisplayFormatChanged(AvaloniaPropertyChangedEventArgs<string?> args)
    {
        if (_startTextBox is not null)
        {
            SyncTimeToText(StartTime);
        }
        if (_endTextBox is not null)
        {
            SyncTimeToText(EndTime, false);
        }
    }

    public string? EndWatermark
    {
        get => GetValue(EndWatermarkProperty);
        set => SetValue(EndWatermarkProperty, value);
    }

    public string? StartWatermark
    {
        get => GetValue(StartWatermarkProperty);
        set => SetValue(StartWatermarkProperty, value);
    }

    public TimeSpan? StartTime
    {
        get => GetValue(StartTimeProperty);
        set => SetValue(StartTimeProperty, value);
    }

    public TimeSpan? EndTime
    {
        get => GetValue(EndTimeProperty);
        set => SetValue(EndTimeProperty, value);
    }

    public void Clear()
    {
        Focus(NavigationMethod.Pointer);
        _startPresenter?.SetValue(TimeSelectorPresenter.TimeProperty, null);
        _endPresenter?.SetValue(TimeSelectorPresenter.TimeProperty, null);
    }

    private void OnSelectionChanged(AvaloniaPropertyChangedEventArgs<TimeSpan?> args, bool start = true)
    {
        SyncTimeToText(args.NewValue.Value, start);
        _suppressTextPresenterEvent = true;
        TimeSelectorPresenter.TimeProperty.SetValue(args.NewValue.Value, start ? _startPresenter : _endPresenter);
        _suppressTextPresenterEvent = false;
    }

    private void SyncTimeToText(TimeSpan? time, bool start = true)
    {
        var textBox = start ? _startTextBox : _endTextBox;
        if (textBox is null) return;
        if (time is null)
        {
            textBox.Text = null;
            return;
        }

        var date = new DateTime(1, 1, 1, time.Value.Hours, time.Value.Minutes, time.Value.Seconds);
        var text = date.ToString(DisplayFormat);
        textBox.Text = text;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        GotFocusEvent.RemoveHandler(OnTextBoxGetFocus, _startTextBox, _endTextBox);
        PointerPressedEvent.RemoveHandler(OnTextBoxPointerPressed, _startTextBox, _endTextBox);
        Button.ClickEvent.RemoveHandler(OnButtonClick, _button);
        TimeSelectorPresenter.SelectedTimeChangedEvent.RemoveHandler(OnPresenterTimeChanged, _startPresenter,
            _endPresenter);

        e.NameScope.Find<Popup>(PartNames.PART_Popup);
        _startTextBox = e.NameScope.Find<TextBox>(PART_StartTextBox);
        _endTextBox = e.NameScope.Find<TextBox>(PART_EndTextBox);
        _startPresenter = e.NameScope.Find<TimeSelectorPresenter>(PART_StartPresenter);
        _endPresenter = e.NameScope.Find<TimeSelectorPresenter>(PART_EndPresenter);
        _button = e.NameScope.Find<Button>(PART_Button);

        GotFocusEvent.AddHandler(OnTextBoxGetFocus, _startTextBox, _endTextBox);
        PointerPressedEvent.AddHandler(OnTextBoxPointerPressed, RoutingStrategies.Tunnel, false, _startTextBox,
            _endTextBox);
        Button.ClickEvent.AddHandler(OnButtonClick, _button);
        TimeSelectorPresenter.SelectedTimeChangedEvent.AddHandler(OnPresenterTimeChanged, _startPresenter, _endPresenter);

        _startPresenter?.SetValue(TimeSelectorPresenter.TimeProperty, StartTime);
        _endPresenter?.SetValue(TimeSelectorPresenter.TimeProperty, EndTime);
        SyncTimeToText(StartTime);
        SyncTimeToText(EndTime, false);
    }

    private void OnPresenterTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (_suppressTextPresenterEvent) return;
        SetCurrentValue(Equals(sender, _startPresenter) ? StartTimeProperty : EndTimeProperty, e.NewTime);
    }

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        Focus(NavigationMethod.Pointer);
        SetCurrentValue(IsDropdownOpenProperty, !IsDropdownOpen);
    }

    private void OnTextBoxPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        SetCurrentValue(IsDropdownOpenProperty, true);
    }

    private void OnTextBoxGetFocus(object? sender, GotFocusEventArgs e)
    {
        SetCurrentValue(IsDropdownOpenProperty, true);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            SetCurrentValue(IsDropdownOpenProperty, false);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Down)
        {
            SetCurrentValue(IsDropdownOpenProperty, true);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Tab)
        {
            if (Equals(e.Source, _endTextBox)) SetCurrentValue(IsDropdownOpenProperty, false);
            return;
        }

        base.OnKeyDown(e);
    }

    public void Confirm()
    {
        _startPresenter?.Confirm();
        _endPresenter?.Confirm();
        SetCurrentValue(IsDropdownOpenProperty, false);
        Focus();
    }

    public void Dismiss()
    {
        SetCurrentValue(IsDropdownOpenProperty, false);
        Focus();
    }
}