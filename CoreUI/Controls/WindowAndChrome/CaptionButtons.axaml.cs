using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace CoreUI.Controls;

[TemplatePart(PART_CloseButton, typeof(Button))]
[TemplatePart(PART_RestoreButton, typeof(Button))]
[TemplatePart(PART_MinimizeButton, typeof(Button))]
[TemplatePart(PART_FullScreenButton, typeof(Button))]
[TemplatePart(PART_ThemeButton, typeof(ThemeToggleButton))]
[PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen")]
public class CaptionButtons : Avalonia.Controls.Chrome.CaptionButtons
{
    private const string PART_CloseButton = "PART_CloseButton";
    private const string PART_RestoreButton = "PART_RestoreButton";
    private const string PART_MinimizeButton = "PART_MinimizeButton";
    private const string PART_FullScreenButton = "PART_FullScreenButton";
    private const string PART_ThemeButton = "PART_ThemeButton";

    private Button? _closeButton;
    private Button? _restoreButton;
    private Button? _minimizeButton;
    private Button? _fullScreenButton;
    private ThemeToggleButton? _themeButton;

    private IDisposable? _windowStateSubscription;
    private IDisposable? _fullScreenSubscription;
    private IDisposable? _minimizeSubscription;
    private IDisposable? _restoreSubscription;
    private IDisposable? _closeSubscription;
    private IDisposable? _themeSubscription;

    /// <summary>
    /// 切换进入全屏前 窗口的状态
    /// </summary>
    private WindowState? _oldWindowState;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _closeButton = e.NameScope.Get<Button>(PART_CloseButton);
        _restoreButton = e.NameScope.Get<Button>(PART_RestoreButton);
        _minimizeButton = e.NameScope.Get<Button>(PART_MinimizeButton);
        _fullScreenButton = e.NameScope.Get<Button>(PART_FullScreenButton);
        _themeButton = e.NameScope.Get<ThemeToggleButton>(PART_ThemeButton);
        Button.ClickEvent.AddHandler((_, _) => OnClose(), _closeButton);
        Button.ClickEvent.AddHandler((_, _) => OnRestore(), _restoreButton);
        Button.ClickEvent.AddHandler((_, _) => OnMinimize(), _minimizeButton);
        Button.ClickEvent.AddHandler((_, _) => OnToggleFullScreen(), _fullScreenButton);
        //Button.ClickEvent.AddHandler((_, _) => OnToggleTheme(), _themeButton);

        Window.WindowStateProperty.Changed.AddClassHandler<Window, WindowState>(WindowStateChanged);
        if (this.HostWindow is not null && !HostWindow.CanResize)
        {
            _restoreButton.IsEnabled = false;
        }
        UpdateVisibility();
    }

    private void WindowStateChanged(Window window, AvaloniaPropertyChangedEventArgs<WindowState> e)
    {
        if (window != HostWindow) return;
        if (e.NewValue.Value == WindowState.FullScreen)
        {
            _oldWindowState = e.OldValue.Value;
        }
    }

    protected override void OnToggleFullScreen()
    {
        if (HostWindow != null)
        {
            if (HostWindow.WindowState != WindowState.FullScreen)
            {
                HostWindow.WindowState = WindowState.FullScreen;
            }
            else
            {
                HostWindow.WindowState = _oldWindowState.HasValue ? _oldWindowState.Value : WindowState.Normal;
            }
        }
    }

    public override void Attach(Window? hostWindow)
    {
        if (hostWindow is null) return;
        base.Attach(hostWindow);
        _windowStateSubscription = HostWindow?.GetObservable(Window.WindowStateProperty).Subscribe(_ =>
        {
            UpdateVisibility();
        });
        Action<bool> a = (_) => UpdateVisibility();
        _fullScreenSubscription = HostWindow?.GetObservable(AvroWindow.IsFullScreenButtonVisibleProperty).Subscribe(a);
        _minimizeSubscription = HostWindow?.GetObservable(AvroWindow.IsMinimizeButtonVisibleProperty).Subscribe(a);
        _restoreSubscription = HostWindow?.GetObservable(AvroWindow.IsRestoreButtonVisibleProperty).Subscribe(a);
        _closeSubscription = HostWindow?.GetObservable(AvroWindow.IsCloseButtonVisibleProperty).Subscribe(a);
        _themeSubscription = HostWindow?.GetObservable(AvroWindow.IsThemeButtonVisibleProperty).Subscribe(a);
    }

    private void UpdateVisibility()
    {
        if (HostWindow is not AvroWindow u)
        {
            return;
        }

        IsVisibleProperty.SetValue(u.IsCloseButtonVisible, _closeButton);
        IsVisibleProperty.SetValue(u.WindowState != WindowState.FullScreen && u.IsRestoreButtonVisible,
            _restoreButton);
        IsVisibleProperty.SetValue(u.WindowState != WindowState.FullScreen && u.IsMinimizeButtonVisible,
            _minimizeButton);
        IsVisibleProperty.SetValue(u.IsFullScreenButtonVisible, _fullScreenButton);
        IsVisibleProperty.SetValue(u.IsThemeButtonVisible, _themeButton);
    }

    public override void Detach()
    {
        base.Detach();
        _windowStateSubscription?.Dispose();
        _fullScreenSubscription?.Dispose();
        _minimizeSubscription?.Dispose();
        _restoreSubscription?.Dispose();
        _closeSubscription?.Dispose();
        _themeSubscription?.Dispose();
    }

    public static readonly StyledProperty<bool> IsDebugProperty
        = AvaloniaProperty.Register<CaptionButtons, bool>(nameof(IsDebug), IsDebugDefault);

    public bool IsDebug => GetValue(IsDebugProperty);

    public static bool IsDebugDefault
    {
        get
        {
#pragma warning disable CS0162
#if DEBUG
            return true;
#endif
            return false;
#pragma warning restore CS0162
        }
    }
}