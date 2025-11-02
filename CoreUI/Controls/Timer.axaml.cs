using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace CoreUI.Controls;

public class Timer : TemplatedControl
{
    private readonly DispatcherTimer _timer;
    private DateTime _startTime;
    private TextBlock _timeTextBlock;

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<Timer, string?>(nameof(Text), "Текст для примера:");

    public string? Text
    {
        get => this.GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<bool> IsTickingProperty =
        AvaloniaProperty.Register<Timer, bool>(nameof(IsTicking), false);

    public bool IsTicking
    {
        get => this.GetValue(IsTickingProperty);
        set => SetValue(IsTickingProperty, value);
    }

    public static readonly StyledProperty<string> TimeProperty =
        AvaloniaProperty.Register<Timer, string>(nameof(Time), "00:00");

    public string Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

#pragma warning disable CS8618

    public Timer()
#pragma warning restore CS8618
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick!;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _timeTextBlock = e.NameScope.Find<TextBlock>("textblock")!;
        Start();
        UpdateTime();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (IsTicking)
        {
            var elapsedTime = DateTime.Now - _startTime;
            Time = $"{elapsedTime:mm\\:ss}";
            UpdateTime();
        }
        else
        {
            Start();
        }
    }

    public void Start()
    {
        _startTime = DateTime.Now;
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void UpdateTime()
    {
        if (_timeTextBlock != null)
        {
            _timeTextBlock.Text = Time;
        }
    }
}