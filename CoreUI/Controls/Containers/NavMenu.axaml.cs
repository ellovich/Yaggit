using System.Windows.Input;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace CoreUI.Controls;

[PseudoClasses(PC_HorizontalCollapsed)]
public class NavMenu : ItemsControl
{
    public const string PC_HorizontalCollapsed = ":horizontal-collapsed";

    public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<NavMenu, object?>(
        nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IBinding?> IconBindingProperty =
        AvaloniaProperty.Register<NavMenu, IBinding?>(
            nameof(IconBinding));

    public static readonly StyledProperty<IBinding?> HeaderBindingProperty =
        AvaloniaProperty.Register<NavMenu, IBinding?>(
            nameof(HeaderBinding));

    public static readonly StyledProperty<IBinding?> DescriptionBindingProperty =
        AvaloniaProperty.Register<NavMenu, IBinding?>(
            nameof(DescriptionBinding));

    public static readonly StyledProperty<IBinding?> SubMenuBindingProperty =
        AvaloniaProperty.Register<NavMenu, IBinding?>(
            nameof(SubMenuBinding));

    public static readonly StyledProperty<IBinding?> CommandBindingProperty =
        AvaloniaProperty.Register<NavMenu, IBinding?>(
            nameof(CommandBinding));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<NavMenu, IDataTemplate?>(
            nameof(HeaderTemplate));

    public static readonly StyledProperty<IDataTemplate?> DescriptionTemplateProperty =
        AvaloniaProperty.Register<NavMenu, IDataTemplate?>(
            nameof(DescriptionTemplate));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavMenu, IDataTemplate?>(
            nameof(IconTemplate));

    public static readonly StyledProperty<double> SubMenuIndentProperty = AvaloniaProperty.Register<NavMenu, double>(
        nameof(SubMenuIndent));

    public static readonly StyledProperty<bool> IsHorizontalCollapsedProperty =
        AvaloniaProperty.Register<NavMenu, bool>(
            nameof(IsHorizontalCollapsed));

    public static readonly StyledProperty<object?> HeaderProperty =
        HeaderedContentControl.HeaderProperty.AddOwner<NavMenu>();

    public static readonly StyledProperty<object?> DescriptionProperty = AvaloniaProperty.Register<NavMenu, object?>(
        nameof(Description));

    public static readonly StyledProperty<object?> FooterProperty = AvaloniaProperty.Register<NavMenu, object?>(
        nameof(Footer));

    public static readonly StyledProperty<double> ExpandWidthProperty = AvaloniaProperty.Register<NavMenu, double>(
        nameof(ExpandWidth), double.NaN);

    public static readonly StyledProperty<double> CollapseWidthProperty = AvaloniaProperty.Register<NavMenu, double>(
        nameof(CollapseWidth), double.NaN);

    public static readonly AttachedProperty<bool> CanToggleProperty =
        AvaloniaProperty.RegisterAttached<NavMenu, InputElement, bool>("CanToggle");

    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<NavMenu, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    private bool _updateFromUI;

    static NavMenu()
    {
        SelectedItemProperty.Changed.AddClassHandler<NavMenu, object?>((o, e) => o.OnSelectedItemChange(e));
        IsHorizontalCollapsedProperty.AffectsPseudoClass<NavMenu>(PC_HorizontalCollapsed);
        CanToggleProperty.Changed.AddClassHandler<InputElement, bool>(OnInputRegisteredAsToggle);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? IconBinding
    {
        get => GetValue(IconBindingProperty);
        set => SetValue(IconBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? HeaderBinding
    {
        get => GetValue(HeaderBindingProperty);
        set => SetValue(HeaderBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? DescriptionBinding
    {
        get => GetValue(DescriptionBindingProperty);
        set => SetValue(DescriptionBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? SubMenuBinding
    {
        get => GetValue(SubMenuBindingProperty);
        set => SetValue(SubMenuBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IBinding? CommandBinding
    {
        get => GetValue(CommandBindingProperty);
        set => SetValue(CommandBindingProperty, value);
    }

    /// <summary>
    ///     Header Template is used for MenuItem headers, not menu header.
    /// </summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public IDataTemplate? DescriptionTemplate
    {
        get => GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public double SubMenuIndent
    {
        get => GetValue(SubMenuIndentProperty);
        set => SetValue(SubMenuIndentProperty, value);
    }

    public bool IsHorizontalCollapsed
    {
        get => GetValue(IsHorizontalCollapsedProperty);
        set => SetValue(IsHorizontalCollapsedProperty, value);
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public double ExpandWidth
    {
        get => GetValue(ExpandWidthProperty);
        set => SetValue(ExpandWidthProperty, value);
    }

    public double CollapseWidth
    {
        get => GetValue(CollapseWidthProperty);
        set => SetValue(CollapseWidthProperty, value);
    }

    public static void SetCanToggle(InputElement obj, bool value)
    {
        obj.SetValue(CanToggleProperty, value);
    }

    public static bool GetCanToggle(InputElement obj)
    {
        return obj.GetValue(CanToggleProperty);
    }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    private static void OnInputRegisteredAsToggle(InputElement input, AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.NewValue.Value)
            input.AddHandler(PointerPressedEvent, OnElementToggle);
        else
            input.RemoveHandler(PointerPressedEvent, OnElementToggle);
    }

    private static void OnElementToggle(object? sender, RoutedEventArgs args)
    {
        if (sender is not InputElement input) return;
        var nav = input.FindLogicalAncestorOfType<NavMenu>();
        if (nav is null) return;
        var collapsed = nav.IsHorizontalCollapsed;
        nav.IsHorizontalCollapsed = !collapsed;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        TryToSelectItem(SelectedItem);
    }

    /// <summary>
    ///     this implementation only works in the case that only leaf menu item is allowed to select. It will be changed if we
    ///     introduce parent level selection in the future.
    /// </summary>
    /// <param name="args"></param>
    private void OnSelectedItemChange(AvaloniaPropertyChangedEventArgs<object?> args)
    {
        var a = new SelectionChangedEventArgs(
            SelectionChangedEvent,
            new[] { args.OldValue.Value },
            new[] { args.NewValue.Value });
        if (_updateFromUI)
        {
            RaiseEvent(a);
            return;
        }

        var newValue = args.NewValue.Value;
        if (newValue is null)
        {
            ClearAll();
            RaiseEvent(a);
            return;
        }

        var found = TryToSelectItem(newValue);
        if (!found) ClearAll();
        RaiseEvent(a);
    }

    private bool TryToSelectItem(object? item)
    {
        if (item is null) return false;
        var leaves = GetLeafMenus();
        var found = false;
        foreach (var leaf in leaves)
            if (leaf == item || leaf.DataContext == item)
            {
                leaf.SelectItem(leaf);
                found = true;
            }

        return found;
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<NavMenuItem>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }

    internal void SelectItem(NavMenuItem item, NavMenuItem parent)
    {
        _updateFromUI = true;
        foreach (var child in LogicalChildren)
        {
            if (Equals(child, parent)) continue;
            if (child is NavMenuItem navMenuItem) navMenuItem.ClearSelection();
        }

        if (item.DataContext is not null && item.DataContext != DataContext)
            SelectedItem = item.DataContext;
        else
            SelectedItem = item;
        item.BringIntoView();
        _updateFromUI = false;
    }

    private IEnumerable<NavMenuItem> GetLeafMenus()
    {
        foreach (var child in LogicalChildren)
            if (child is NavMenuItem item)
            {
                var leafs = item.GetLeafMenus();
                foreach (var leaf in leafs) yield return leaf;
            }
    }

    private void ClearAll()
    {
        foreach (var child in LogicalChildren)
            if (child is NavMenuItem item)
                item.ClearSelection();
    }
}

/// <summary>
///     Navigation Menu Item
/// </summary>
[PseudoClasses(PC_Highlighted, PC_HorizontalCollapsed, PC_VerticalCollapsed, PC_FirstLevel, PC_Selector)]
public class NavMenuItem : HeaderedItemsControl
{
    public const string PC_Highlighted = ":highlighted";
    public const string PC_FirstLevel = ":first-level";
    public const string PC_HorizontalCollapsed = ":horizontal-collapsed";
    public const string PC_VerticalCollapsed = ":vertical-collapsed";
    public const string PC_Selector = ":selector";
    public const string PC_Selected = ":selected";

    private static readonly Point InvalidPoint = new(double.NaN, double.NaN);

    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<NavMenuItem, object?>(
        nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavMenuItem, IDataTemplate?>(
            nameof(IconTemplate));

    public static readonly StyledProperty<object?> DescriptionProperty = AvaloniaProperty.Register<NavMenuItem, object?>(
        nameof(Description));

    public static readonly StyledProperty<IDataTemplate?> DescriptionTemplateProperty =
        AvaloniaProperty.Register<NavMenuItem, IDataTemplate?>(
            nameof(DescriptionTemplate));

    public static readonly StyledProperty<ICommand?> CommandProperty = Button.CommandProperty.AddOwner<NavMenuItem>();

    public static readonly StyledProperty<object?> CommandParameterProperty =
        Button.CommandParameterProperty.AddOwner<NavMenuItem>();

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<NavMenuItem>();

    public static readonly RoutedEvent<RoutedEventArgs> IsSelectedChangedEvent =
        RoutedEvent.Register<SelectingItemsControl, RoutedEventArgs>("IsSelectedChanged", RoutingStrategies.Bubble);

    public static readonly DirectProperty<NavMenuItem, bool> IsHighlightedProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, bool>(
            nameof(IsHighlighted), o => o.IsHighlighted, (o, v) => o.IsHighlighted = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsHorizontalCollapsedProperty =
        NavMenu.IsHorizontalCollapsedProperty.AddOwner<NavMenuItem>();

    public static readonly StyledProperty<bool> IsVerticalCollapsedProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(
            nameof(IsVerticalCollapsed));

    public static readonly StyledProperty<double> SubMenuIndentProperty =
        NavMenu.SubMenuIndentProperty.AddOwner<NavMenuItem>();

    internal static readonly DirectProperty<NavMenuItem, int> LevelProperty =
        AvaloniaProperty.RegisterDirect<NavMenuItem, int>(
            nameof(Level), o => o.Level, (o, v) => o.Level = v);

    public static readonly StyledProperty<bool> IsSeparatorProperty = AvaloniaProperty.Register<NavMenuItem, bool>(
        nameof(IsSeparator));

    private bool _isHighlighted;
    private int _level;
    private Panel? _overflowPanel;
    private Point _pointerDownPoint = InvalidPoint;
    private Popup? _popup;

    private NavMenu? _rootMenu;

    static NavMenuItem()
    {
        // SelectableMixin.Attach<NavMenuItem>(IsSelectedProperty);
        PressedMixin.Attach<NavMenuItem>();
        FocusableProperty.OverrideDefaultValue<NavMenuItem>(true);
        LevelProperty.Changed.AddClassHandler<NavMenuItem, int>((item, args) => item.OnLevelChange(args));
        IsHighlightedProperty.AffectsPseudoClass<NavMenuItem>(PC_Highlighted);
        IsHorizontalCollapsedProperty.AffectsPseudoClass<NavMenuItem>(PC_HorizontalCollapsed);
        IsVerticalCollapsedProperty.AffectsPseudoClass<NavMenuItem>(PC_VerticalCollapsed);
        IsSelectedProperty.AffectsPseudoClass<NavMenuItem>(PC_Selected, IsSelectedChangedEvent);
        IsHorizontalCollapsedProperty.Changed.AddClassHandler<NavMenuItem, bool>((item, args) =>
            item.OnIsHorizontalCollapsedChanged(args));
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public IDataTemplate? DescriptionTemplate
    {
        get => GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        private set => SetAndRaise(IsHighlightedProperty, ref _isHighlighted, value);
    }

    public bool IsHorizontalCollapsed
    {
        get => GetValue(IsHorizontalCollapsedProperty);
        set => SetValue(IsHorizontalCollapsedProperty, value);
    }

    public bool IsVerticalCollapsed
    {
        get => GetValue(IsVerticalCollapsedProperty);
        set => SetValue(IsVerticalCollapsedProperty, value);
    }

    public double SubMenuIndent
    {
        get => GetValue(SubMenuIndentProperty);
        set => SetValue(SubMenuIndentProperty, value);
    }

    public int Level
    {
        get => _level;
        set => SetAndRaise(LevelProperty, ref _level, value);
    }

    public bool IsSeparator
    {
        get => GetValue(IsSeparatorProperty);
        set => SetValue(IsSeparatorProperty, value);
    }

    private void OnIsHorizontalCollapsedChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.NewValue.Value)
        {
            if (ItemsPanelRoot is OverflowStackPanel s) s.MoveChildrenToOverflowPanel();
        }
        else
        {
            if (ItemsPanelRoot is OverflowStackPanel s) s.MoveChildrenToMainPanel();
        }
    }

    private void OnLevelChange(AvaloniaPropertyChangedEventArgs<int> args)
    {
        PseudoClasses.Set(PC_FirstLevel, args.NewValue.Value == 1);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<NavMenuItem>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _rootMenu = GetRootMenu();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetCurrentValue(LevelProperty, CalculateDistanceFromLogicalParent<NavMenu>(this));
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _overflowPanel = e.NameScope.Find<Panel>("PART_OverflowPanel");
        if (_rootMenu is not null)
        {
            this.TryBind(IconProperty, _rootMenu.IconBinding);
            this.TryBind(HeaderProperty, _rootMenu.HeaderBinding);
            this.TryBind(DescriptionProperty, _rootMenu.DescriptionBinding);
            this.TryBind(ItemsSourceProperty, _rootMenu.SubMenuBinding);
            this.TryBind(CommandProperty, _rootMenu.CommandBinding);
            this[!IconTemplateProperty] = _rootMenu[!NavMenu.IconTemplateProperty];
            this[!HeaderTemplateProperty] = _rootMenu[!NavMenu.HeaderTemplateProperty];
            this[!DescriptionTemplateProperty] = _rootMenu[!NavMenu.DescriptionTemplateProperty];
            this[!SubMenuIndentProperty] = _rootMenu[!NavMenu.SubMenuIndentProperty];
            this[!IsHorizontalCollapsedProperty] = _rootMenu[!NavMenu.IsHorizontalCollapsedProperty];
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var root = ItemsPanelRoot;
        if (root is OverflowStackPanel stack) stack.OverflowPanel = _overflowPanel;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (IsSeparator)
        {
            e.Handled = true;
            return;
        }

        base.OnPointerPressed(e);
        if (e.Handled) return;

        var p = e.GetCurrentPoint(this);
        if (p.Properties.PointerUpdateKind is not (PointerUpdateKind.LeftButtonPressed
            or PointerUpdateKind.RightButtonPressed)) return;
        if (p.Pointer.Type == PointerType.Mouse)
        {
            if (ItemCount == 0)
            {
                SelectItem(this);
                Command?.Execute(CommandParameter);
                e.Handled = true;
            }
            else
            {
                if (!IsHorizontalCollapsed)
                {
                    SetCurrentValue(IsVerticalCollapsedProperty, !IsVerticalCollapsed);
                    e.Handled = true;
                }
                else
                {
                    if (_popup is null || e.Source is not Visual v || _popup.IsInsidePopup(v)) return;
                    if (_popup.IsOpen)
                        _popup.Close();
                    else
                        _popup.Open();
                }
            }
        }
        else
        {
            _pointerDownPoint = p.Position;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!e.Handled && !double.IsNaN(_pointerDownPoint.X) &&
            e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right)
        {
            var point = e.GetCurrentPoint(this);
            if (!new Rect(Bounds.Size).ContainsExclusive(point.Position) || e.Pointer.Type != PointerType.Touch) return;
            if (ItemCount == 0)
            {
                SelectItem(this);
                Command?.Execute(CommandParameter);
                e.Handled = true;
            }
            else
            {
                if (!IsHorizontalCollapsed)
                {
                    SetCurrentValue(IsVerticalCollapsedProperty, !IsVerticalCollapsed);
                    e.Handled = true;
                }
                else
                {
                    if (_popup is null || e.Source is not Visual v || _popup.IsInsidePopup(v)) return;
                    if (_popup.IsOpen)
                        _popup.Close();
                    else
                        _popup.Open();
                }
            }
        }
    }

    internal void SelectItem(NavMenuItem item)
    {
        if (item == this)
        {
            SetCurrentValue(IsSelectedProperty, true);
            SetCurrentValue(IsHighlightedProperty, true);
        }
        else
        {
            SetCurrentValue(IsSelectedProperty, false);
            SetCurrentValue(IsHighlightedProperty, true);
        }

        if (Parent is NavMenuItem menuItem)
        {
            menuItem.SelectItem(item);
            var items = menuItem.LogicalChildren.OfType<NavMenuItem>();
            foreach (var child in items)
                if (child != this)
                    child.ClearSelection();
        }
        else if (Parent is NavMenu menu)
        {
            menu.SelectItem(item, this);
        }

        if (_popup is not null) _popup.Close();
    }

    internal void ClearSelection()
    {
        SetCurrentValue(IsHighlightedProperty, false);
        SetCurrentValue(IsSelectedProperty, false);
        foreach (var child in LogicalChildren)
            if (child is NavMenuItem item)
                item.ClearSelection();
    }

    private NavMenu? GetRootMenu()
    {
        var root = this.FindAncestorOfType<NavMenu>() ?? this.FindLogicalAncestorOfType<NavMenu>();
        return root;
    }

    private static int CalculateDistanceFromLogicalParent<T>(ILogical? logical, int @default = -1) where T : class
    {
        var result = 0;

        while (logical != null && !(logical is T))
        {
            if (logical is NavMenuItem) result++;
            logical = logical.LogicalParent;
        }

        return logical != null ? result : @default;
    }

    internal IEnumerable<NavMenuItem> GetLeafMenus()
    {
        if (ItemCount == 0)
        {
            yield return this;
            yield break;
        }

        foreach (var child in LogicalChildren)
            if (child is NavMenuItem item)
            {
                var items = item.GetLeafMenus();
                foreach (var i in items) yield return i;
            }
    }
}

public static class BindingExtension
{
    public static ResultDisposable TryBind(this AvaloniaObject obj, AvaloniaProperty property, IBinding? binding)
    {
        if (binding == null)
        {
            return new ResultDisposable(new EmptyDisposable(), result: false);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        return new ResultDisposable(AvaloniaObjectExtensions.Bind(obj, property, binding), result: true);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}

public class ResultDisposable(IDisposable disposable, bool result) : IDisposable
{
    public bool Result { get; } = result;

    public void Dispose()
    {
        disposable?.Dispose();
    }
}

internal class EmptyDisposable : IDisposable
{
    public void Dispose()
    {
    }
}

public class OverflowStackPanel : StackPanel
{
    public Panel? OverflowPanel { get; set; }

    public void MoveChildrenToOverflowPanel()
    {
        var children = Children.ToList();
        foreach (var child in children)
        {
            Children.Remove(child);
            OverflowPanel?.Children.Add(child);
        }
    }

    public void MoveChildrenToMainPanel()
    {
        var children = OverflowPanel?.Children.ToList();
        if (children is not null && children.Count > 0)
            foreach (var child in children)
            {
                OverflowPanel?.Children.Remove(child);
                Children.Add(child);
            }
    }
}