using Avalonia.Animation;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;

namespace CoreUI.Controls;

[TemplatePart(PART_ClockTicks, typeof(ClockTicks))]
public class Clock : TemplatedControl
{
    public const string PART_ClockTicks = "PART_ClockTicks";

    public static readonly StyledProperty<DateTime> TimeProperty = AvaloniaProperty.Register<Clock, DateTime>(
        nameof(Time), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> ShowHourTicksProperty =
        ClockTicks.ShowHourTicksProperty.AddOwner<Clock>();

    public static readonly StyledProperty<bool> ShowMinuteTicksProperty =
        ClockTicks.ShowMinuteTicksProperty.AddOwner<Clock>();

    public static readonly StyledProperty<IBrush?> HandBrushProperty = AvaloniaProperty.Register<Clock, IBrush?>(
        nameof(HandBrush));

    public static readonly StyledProperty<bool> ShowHourHandProperty = AvaloniaProperty.Register<Clock, bool>(
        nameof(ShowHourHand), true);

    public static readonly StyledProperty<bool> ShowMinuteHandProperty = AvaloniaProperty.Register<Clock, bool>(
        nameof(ShowMinuteHand), true);

    public static readonly StyledProperty<bool> ShowSecondHandProperty = AvaloniaProperty.Register<Clock, bool>(
        nameof(ShowSecondHand), true);

    public static readonly StyledProperty<bool> IsSmoothProperty = AvaloniaProperty.Register<Clock, bool>(
        nameof(IsSmooth));

    public static readonly DirectProperty<Clock, double> HourAngleProperty =
        AvaloniaProperty.RegisterDirect<Clock, double>(
            nameof(HourAngle), o => o.HourAngle);

    public static readonly DirectProperty<Clock, double> MinuteAngleProperty =
        AvaloniaProperty.RegisterDirect<Clock, double>(
            nameof(MinuteAngle), o => o.MinuteAngle);

    public static readonly DirectProperty<Clock, double> SecondAngleProperty =
        AvaloniaProperty.RegisterDirect<Clock, double>(
            nameof(SecondAngle), o => o.SecondAngle, (o, v) => o.SecondAngle = v);

    private double _hourAngle;
    private double _minuteAngle;

    private double _secondAngle;

    static Clock()
    {
        TimeProperty.Changed.AddClassHandler<Clock, DateTime>((clock, args) => clock.OnTimeChanged(args));
        IsSmoothProperty.Changed.AddClassHandler<Clock, bool>((clock, args) => clock.OnIsSmoothChanged(args));
    }

    private void OnIsSmoothChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.NewValue.Value && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }
    }

    public DateTime Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public bool ShowHourTicks
    {
        get => GetValue(ShowHourTicksProperty);
        set => SetValue(ShowHourTicksProperty, value);
    }

    public bool ShowMinuteTicks
    {
        get => GetValue(ShowMinuteTicksProperty);
        set => SetValue(ShowMinuteTicksProperty, value);
    }

    public IBrush? HandBrush
    {
        get => GetValue(HandBrushProperty);
        set => SetValue(HandBrushProperty, value);
    }

    public bool ShowHourHand
    {
        get => GetValue(ShowHourHandProperty);
        set => SetValue(ShowHourHandProperty, value);
    }

    public bool ShowMinuteHand
    {
        get => GetValue(ShowMinuteHandProperty);
        set => SetValue(ShowMinuteHandProperty, value);
    }

    public bool ShowSecondHand
    {
        get => GetValue(ShowSecondHandProperty);
        set => SetValue(ShowSecondHandProperty, value);
    }

    public bool IsSmooth
    {
        get => GetValue(IsSmoothProperty);
        set => SetValue(IsSmoothProperty, value);
    }

    public double HourAngle
    {
        get => _hourAngle;
        private set => SetAndRaise(HourAngleProperty, ref _hourAngle, value);
    }

    public double MinuteAngle
    {
        get => _minuteAngle;
        private set => SetAndRaise(MinuteAngleProperty, ref _minuteAngle, value);
    }

    public double SecondAngle
    {
        get => _secondAngle;
        private set => SetAndRaise(SecondAngleProperty, ref _secondAngle, value);
    }

    private Animation _secondsAnimation = new Animation()
    {
        FillMode = FillMode.Forward,
        Duration = TimeSpan.FromSeconds(1),
        Children =
        {
            new KeyFrame
            {
                Cue = new Cue(0.0),
                Setters = { new Setter { Property = SecondAngleProperty } }
            },
            new KeyFrame
            {
                Cue = new Cue(1.0),
                Setters = { new Setter { Property = SecondAngleProperty } }
            }
        }
    };

    private CancellationTokenSource _cts = new CancellationTokenSource();

    private void OnTimeChanged(AvaloniaPropertyChangedEventArgs<DateTime> args)
    {
        var oldSeconds = args.OldValue.Value.Second;
        var time = args.NewValue.Value;
        var hour = time.Hour;
        var minute = time.Minute;
        var second = time.Second;
        var hourAngle = 360.0 / 12 * hour + 360.0 / 12 / 60 * minute;
        var minuteAngle = 360.0 / 60 * minute + 360.0 / 60 / 60 * second;
        if (second == 0) second = 60;
        var oldSecondAngle = 360.0 / 60 * oldSeconds;
        var secondAngle = 360.0 / 60 * second;
        HourAngle = hourAngle;
        MinuteAngle = minuteAngle;
        if (!IsLoaded || !IsSmooth)
        {
            SecondAngle = secondAngle;
        }
        else
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            if (_secondsAnimation.Children[0].Setters[0] is Setter start)
            {
                start.Value = oldSecondAngle;
            }
            if (_secondsAnimation.Children[1].Setters[0] is Setter end)
            {
                end.Value = secondAngle;
            }
            _secondsAnimation.RunAsync(this, _cts.Token);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var min = Math.Min(availableSize.Height, availableSize.Width);
        var newSize = new Size(min, min);
        var size = base.MeasureOverride(newSize);
        return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var min = Math.Min(finalSize.Height, finalSize.Width);
        var newSize = new Size(min, min);
        var size = base.ArrangeOverride(newSize);
        return size;
    }
}

public class ClockTicks : Control
{
    private Matrix _hourRotationMatrix = Matrix.CreateRotation(Math.PI / 6);
    private Matrix _minuteRotationMatrix = Matrix.CreateRotation(Math.PI / 30);

    public static readonly StyledProperty<bool> ShowHourTicksProperty = AvaloniaProperty.Register<ClockTicks, bool>(
        nameof(ShowHourTicks), true);

    public bool ShowHourTicks
    {
        get => GetValue(ShowHourTicksProperty);
        set => SetValue(ShowHourTicksProperty, value);
    }

    public static readonly StyledProperty<bool> ShowMinuteTicksProperty = AvaloniaProperty.Register<ClockTicks, bool>(
        nameof(ShowMinuteTicks), true);

    public bool ShowMinuteTicks
    {
        get => GetValue(ShowMinuteTicksProperty);
        set => SetValue(ShowMinuteTicksProperty, value);
    }

    public static readonly StyledProperty<IBrush?> HourTickForegroundProperty = AvaloniaProperty.Register<ClockTicks, IBrush?>(
        nameof(HourTickForeground));

    public IBrush? HourTickForeground
    {
        get => GetValue(HourTickForegroundProperty);
        set => SetValue(HourTickForegroundProperty, value);
    }

    public static readonly StyledProperty<IBrush?> MinuteTickForegroundProperty = AvaloniaProperty.Register<ClockTicks, IBrush?>(
        nameof(MinuteTickForeground));

    public IBrush? MinuteTickForeground
    {
        get => GetValue(MinuteTickForegroundProperty);
        set => SetValue(MinuteTickForegroundProperty, value);
    }

    public static readonly StyledProperty<double> HourTickLengthProperty = AvaloniaProperty.Register<ClockTicks, double>(
        nameof(HourTickLength), 10);

    public double HourTickLength
    {
        get => GetValue(HourTickLengthProperty);
        set => SetValue(HourTickLengthProperty, value);
    }

    public static readonly StyledProperty<double> MinuteTickLengthProperty = AvaloniaProperty.Register<ClockTicks, double>(
        nameof(MinuteTickLength), 5);

    public double MinuteTickLength
    {
        get => GetValue(MinuteTickLengthProperty);
        set => SetValue(MinuteTickLengthProperty, value);
    }

    public static readonly StyledProperty<double> HourTickWidthProperty = AvaloniaProperty.Register<ClockTicks, double>(
        nameof(HourTickWidth), 2);

    public double HourTickWidth
    {
        get => GetValue(HourTickWidthProperty);
        set => SetValue(HourTickWidthProperty, value);
    }

    public static readonly StyledProperty<double> MinuteTickWidthProperty = AvaloniaProperty.Register<ClockTicks, double>(
        nameof(MinuteTickWidth), 1);

    public double MinuteTickWidth
    {
        get => GetValue(MinuteTickWidthProperty);
        set => SetValue(MinuteTickWidthProperty, value);
    }

    static ClockTicks()
    {
        AffectsRender<ClockTicks>(ShowHourTicksProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double minSize = Math.Min(availableSize.Width, availableSize.Height);
        return new Size(minSize, minSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var minSize = Math.Min(finalSize.Width, finalSize.Height);
        return new Size(minSize, minSize);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var size = Math.Min(Bounds.Width, Bounds.Height);
        var center = size / 2;
        IPen hourTickPen = new Pen(HourTickForeground, HourTickWidth);
        IPen minuteTickPen = new Pen(MinuteTickForeground, MinuteTickWidth);
        double hourTickLength = Math.Min(center, HourTickLength);
        double minuteTickLength = Math.Min(center, MinuteTickLength);
        context.PushTransform(Matrix.CreateTranslation(center, center));
        if (ShowHourTicks)
        {
            for (int i = 0; i < 12; i++)
            {
                DrawTick(context, hourTickPen, center, hourTickLength);
                context.PushTransform(_hourRotationMatrix);
            }
        }

        if (ShowMinuteTicks)
        {
            for (int i = 0; i < 60; i++)
            {
                if (i % 5 != 0)
                {
                    DrawTick(context, minuteTickPen, center, minuteTickLength);
                }
                context.PushTransform(_minuteRotationMatrix);
            }
        }
    }

    private void DrawTick(DrawingContext context, IPen pen, double center, double length)
    {
        var start = new Point(0, -center);
        var end = new Point(0, length - center);
        context.DrawLine(pen, start, end);
    }
}