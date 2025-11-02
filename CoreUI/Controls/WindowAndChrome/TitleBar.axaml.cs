using System.Collections.ObjectModel;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CoreUI.Controls;

[PseudoClasses(":active")]
public class TitleBar : ContentControl
{
    private CoreUI.Controls.CaptionButtons? _captionButtons;
    private InputElement? _background;
    private Window? _visualRoot;
    private IDisposable? _activeSubscription;
    
    public static readonly StyledProperty<object?> LeftContentProperty = AvaloniaProperty.Register<TitleBar, object?>(
        nameof(LeftContent));

    public object? LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    public static readonly StyledProperty<object?> RightContentProperty = AvaloniaProperty.Register<TitleBar, object?>(
        nameof(RightContent));

    public object? RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    public static readonly StyledProperty<bool> IsTitleVisibleProperty = AvaloniaProperty.Register<TitleBar, bool>(
        nameof(IsTitleVisible));

    public bool IsTitleVisible
    {
        get => GetValue(IsTitleVisibleProperty);
        set => SetValue(IsTitleVisibleProperty, value);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this._captionButtons?.Detach();
        this._captionButtons = e.NameScope.Get<CoreUI.Controls.CaptionButtons>("PART_CaptionButtons");
        this._background = e.NameScope.Get<InputElement>("PART_Background");
        DoubleTappedEvent.AddHandler(OnDoubleTapped, _background);
        PointerPressedEvent.AddHandler(OnPointerPressed, _background);
        this._captionButtons?.Attach(_visualRoot);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _visualRoot = this.VisualRoot as Window;
        if (_visualRoot is not null)
        {
            _activeSubscription = _visualRoot.GetObservable(WindowBase.IsActiveProperty).Subscribe(isActive =>
            {
                PseudoClasses.Set(":active", isActive);
            });
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if(_visualRoot is not null && _visualRoot.WindowState == WindowState.FullScreen)
        {
            return;
        }
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (e.ClickCount < 2) 
            {
                _visualRoot?.BeginMoveDrag(e);
            }
        }
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_visualRoot is null) return;
        if (!_visualRoot.CanResize) return;
        if ( _visualRoot.WindowState == WindowState.FullScreen) return;
        _visualRoot.WindowState = _visualRoot.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void UpdateSize(Window window)
    {
        Thickness offScreenMargin = window.OffScreenMargin;
        var left = offScreenMargin.Left;
        offScreenMargin = window.OffScreenMargin;
        double top = offScreenMargin.Top;
        offScreenMargin = window.OffScreenMargin;
        double right = offScreenMargin.Right;
        offScreenMargin = window.OffScreenMargin;
        double bottom = offScreenMargin.Bottom;
        this.Margin = new Thickness(left, top, right, bottom);
        if (window.WindowState != WindowState.FullScreen)
        {
            this.Height = window.WindowDecorationMargin.Top;
            if (this._captionButtons != null)
                this._captionButtons.Height = this.Height;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _captionButtons?.Detach();
        _activeSubscription?.Dispose();
    }
}




#region HELPERS

internal static class RoutedEventExtension
{
    public static void AddHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.AddHandler(routedEvent, handler);
        }
    }

    public static void AddHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params TControl?[] controls) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.AddHandler(routedEvent, handler);
        }
    }

    public static void AddHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.AddHandler(routedEvent, handler, strategies, handledEventsToo);
        }
    }

    public static void AddHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false, params TControl?[] controls) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.AddHandler(routedEvent, handler, strategies, handledEventsToo);
        }
    }

    public static void AddHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, IEnumerable<TControl?> controls, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        foreach (var control in controls)
        {
            control?.AddHandler(routedEvent, handler, strategies, handledEventsToo);
        }
    }

    public static void RemoveHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.RemoveHandler(routedEvent, handler);
        }
    }

    public static void RemoveHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params TControl?[] controls) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i]?.RemoveHandler(routedEvent, handler);
        }
    }

    public static void RemoveHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, IEnumerable<TControl?> controls) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        foreach (var control in controls)
        {
            control?.RemoveHandler(routedEvent, handler);
        }
    }

    public static IDisposable AddDisposableHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        var list = new List<IDisposable?>(controls.Length);
        for (int i = 0; i < controls.Length; i++)
        {
            var disposable = controls[i]?.AddDisposableHandler(routedEvent, handler);
            if (disposable != null)
            {
                list.Add(disposable);
            }
        }

        return new ReadonlyDisposableCollection(list);
    }

    public static IDisposable AddDisposableHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, params TControl?[] controls) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        var list = new List<IDisposable?>(controls.Length);
        for (int i = 0; i < controls.Length; i++)
        {
            var disposable = controls[i]?.AddDisposableHandler(routedEvent, handler);
            if (disposable != null)
            {
                list.Add(disposable);
            }
        }

        return new ReadonlyDisposableCollection(list);
    }

    public static IDisposable AddDisposableHandler<TArgs>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false, params Interactive?[] controls) where TArgs : RoutedEventArgs
    {
        var list = new List<IDisposable?>(controls.Length);
        for (int i = 0; i < controls.Length; i++)
        {
            var disposable = controls[i]?.AddDisposableHandler(routedEvent, handler, strategies, handledEventsToo);
            if (disposable != null)
            {
                list.Add(disposable);
            }
        }

        return new ReadonlyDisposableCollection(list);
    }

    public static IDisposable AddDisposableHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false, params TControl?[] controls) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        var list = new List<IDisposable?>(controls.Length);
        for (int i = 0; i < controls.Length; i++)
        {
            var disposable = controls[i]?.AddDisposableHandler(routedEvent, handler, strategies, handledEventsToo);
            if (disposable != null)
            {
                list.Add(disposable);
            }
        }

        return new ReadonlyDisposableCollection(list);
    }

    public static IDisposable AddDisposableHandler<TArgs, TControl>(this RoutedEvent<TArgs> routedEvent, EventHandler<TArgs> handler, IEnumerable<TControl> controls, RoutingStrategies strategies = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false) where TArgs : RoutedEventArgs where TControl : Interactive
    {
        var list = new List<IDisposable?>();
        foreach (TControl control in controls)
        {
            var disposable = control?.AddDisposableHandler(routedEvent, handler, strategies, handledEventsToo);
            if (disposable != null)
            {
                list.Add(disposable);
            }
        }

        return new ReadonlyDisposableCollection(list);
    }
}

internal class ReadonlyDisposableCollection(IList<IDisposable?> list) : ReadOnlyCollection<IDisposable?>(list), IDisposable
{
    private readonly IList<IDisposable?> _list = list;

    public void Dispose()
    {
        foreach (var item in _list)
        {
            item?.Dispose();
        }
    }
}

internal static class AvaloniaPropertyExtension
{
    public static void SetValue<T>(this AvaloniaProperty<T> property, T value, params AvaloniaObject?[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i]?.SetValue(property, value);
        }
    }

    public static void SetValue<T, TControl>(this AvaloniaProperty<T> property, T value, IEnumerable<TControl?> objects) where TControl : AvaloniaObject
    {
        foreach (var @object in objects)
        {
            @object?.SetValue(property, value);
        }
    }

    public static void AffectsPseudoClass<TControl>(this AvaloniaProperty<bool> property, string pseudoClass, RoutedEvent<RoutedEventArgs>? routedEvent = null) where TControl : Control
    {
        string pseudoClass2 = pseudoClass;
        var routedEvent2 = routedEvent;
        property.Changed.AddClassHandler(delegate (TControl control, AvaloniaPropertyChangedEventArgs<bool> args)
        {
            OnPropertyChanged(control, args, pseudoClass2, routedEvent2);
        });
    }

    private static void OnPropertyChanged<TControl, TArgs>(TControl control, AvaloniaPropertyChangedEventArgs<bool> args, string pseudoClass, RoutedEvent<TArgs>? routedEvent) where TControl : Control where TArgs : RoutedEventArgs, new()
    {
        PseudolassesExtensions.Set(control.Classes, pseudoClass, args.NewValue.Value);
        if (routedEvent != null)
        {
            control.RaiseEvent(new TArgs
            {
                RoutedEvent = routedEvent
            });
        }
    }

    public static void AffectsPseudoClass<TControl, TArgs>(this AvaloniaProperty<bool> property, string pseudoClass, RoutedEvent<TArgs>? routedEvent = null) where TControl : Control where TArgs : RoutedEventArgs, new()
    {
        string pseudoClass2 = pseudoClass;
        var routedEvent2 = routedEvent;
        property.Changed.AddClassHandler(delegate (TControl control, AvaloniaPropertyChangedEventArgs<bool> args)
        {
            OnPropertyChanged(control, args, pseudoClass2, routedEvent2);
        });
    }
}

#endregion HELPERS