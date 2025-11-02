using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace CoreUI.Controls;

public interface IInnerContentControl
{
    object? InnerLeftContent { get; set; }

    object? InnerRightContent { get; set; }
}

public interface IPopupInnerContent
{
    object? PopupInnerTopContent { get; set; }

    object? PopupInnerBottomContent { get; set; }
}

/// <summary>
/// This control inherits from <see cref="SelectingItemsControl"/>, but it only supports MVVM pattern.
/// </summary>
[TemplatePart(PART_BackgroundBorder, typeof(Border))]
[PseudoClasses(PC_DropDownOpen, PC_Empty)]
public class MultiComboBox : SelectingItemsControl, IInnerContentControl, IPopupInnerContent
{
    public const string PART_BackgroundBorder = "PART_BackgroundBorder";
    public const string PC_DropDownOpen = ":dropdownopen";
    public const string PC_Empty = ":selection-empty";

    private static readonly ITemplate<Panel?> _defaultPanel =
        new FuncTemplate<Panel?>(() => new VirtualizingStackPanel());

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        ComboBox.IsDropDownOpenProperty.AddOwner<MultiComboBox>();

    public static readonly StyledProperty<double> MaxDropdownHeightProperty =
        AvaloniaProperty.Register<MultiComboBox, double>(
            nameof(MaxDropdownHeight));

    public static readonly StyledProperty<double> MaxSelectionBoxHeightProperty =
        AvaloniaProperty.Register<MultiComboBox, double>(
            nameof(MaxSelectionBoxHeight));

    public new static readonly StyledProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.Register<MultiComboBox, IList?>(
            nameof(SelectedItems));

    public static readonly StyledProperty<object?> InnerLeftContentProperty =
        AvaloniaProperty.Register<MultiComboBox, object?>(
            nameof(InnerLeftContent));

    public static readonly StyledProperty<object?> InnerRightContentProperty =
        AvaloniaProperty.Register<MultiComboBox, object?>(
            nameof(InnerRightContent));

    public static readonly StyledProperty<IDataTemplate?> SelectedItemTemplateProperty =
        AvaloniaProperty.Register<MultiComboBox, IDataTemplate?>(
            nameof(SelectedItemTemplate));

    public static readonly StyledProperty<string?> WatermarkProperty =
        TextBox.WatermarkProperty.AddOwner<MultiComboBox>();

    public static readonly StyledProperty<object?> PopupInnerTopContentProperty =
        AvaloniaProperty.Register<MultiComboBox, object?>(
            nameof(PopupInnerTopContent));

    public static readonly StyledProperty<object?> PopupInnerBottomContentProperty =
        AvaloniaProperty.Register<MultiComboBox, object?>(
            nameof(PopupInnerBottomContent));

    private Border? _rootBorder;

    static MultiComboBox()
    {
        FocusableProperty.OverrideDefaultValue<MultiComboBox>(true);
        ItemsPanelProperty.OverrideDefaultValue<MultiComboBox>(_defaultPanel);
        IsDropDownOpenProperty.AffectsPseudoClass<MultiComboBox>(PC_DropDownOpen);
        SelectedItemsProperty.Changed.AddClassHandler<MultiComboBox, IList?>((box, args) =>
            box.OnSelectedItemsChanged(args));
    }

    public MultiComboBox()
    {
        SelectedItems = new AvaloniaList<object>();
        if (SelectedItems is INotifyCollectionChanged c) c.CollectionChanged += OnSelectedItemsCollectionChanged;
    }

    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    public double MaxDropdownHeight
    {
        get => GetValue(MaxDropdownHeightProperty);
        set => SetValue(MaxDropdownHeightProperty, value);
    }

    public double MaxSelectionBoxHeight
    {
        get => GetValue(MaxSelectionBoxHeightProperty);
        set => SetValue(MaxSelectionBoxHeightProperty, value);
    }

    public new IList? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public IDataTemplate? SelectedItemTemplate
    {
        get => GetValue(SelectedItemTemplateProperty);
        set => SetValue(SelectedItemTemplateProperty, value);
    }

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
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

    private void OnSelectedItemsChanged(AvaloniaPropertyChangedEventArgs<IList?> args)
    {
        if (args.OldValue.Value is INotifyCollectionChanged old)
            old.CollectionChanged -= OnSelectedItemsCollectionChanged;
        if (args.NewValue.Value is INotifyCollectionChanged @new)
            @new.CollectionChanged += OnSelectedItemsCollectionChanged;
    }

    private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PseudoClasses.Set(PC_Empty, SelectedItems?.Count is null or 0);
        //return;
        var containers = Presenter?.Panel?.Children;
        if (containers is null) return;
        foreach (var container in containers)
        {
            if (container is MultiComboBoxItem i)
            {
                i.UpdateSelection();
            }
        }
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = item;
        return item is not MultiComboBoxItem;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new MultiComboBoxItem();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        PointerPressedEvent.RemoveHandler(OnBackgroundPointerPressed, _rootBorder);
        _rootBorder = e.NameScope.Find<Border>(PART_BackgroundBorder);
        PointerPressedEvent.AddHandler(OnBackgroundPointerPressed, _rootBorder);
        PseudoClasses.Set(PC_Empty, SelectedItems?.Count == 0);
    }

    private void OnBackgroundPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
    }

    internal void ItemFocused(MultiComboBoxItem dropDownItem)
    {
        if (IsDropDownOpen && dropDownItem.IsFocused && dropDownItem.IsArrangeValid) dropDownItem.BringIntoView();
    }

    public void Remove(object? o)
    {
        if (o is StyledElement s)
        {
            var data = s.DataContext;
            SelectedItems?.Remove(data);
            var item = Items.FirstOrDefault(a => ReferenceEquals(a, data));
            if (item is not null)
            {
                var container = ContainerFromItem(item);
                if (container is MultiComboBoxItem t) t.IsSelected = false;
            }
        }
    }

    public void Clear()
    {
        this.SelectedItems?.Clear();
        var containers = Presenter?.Panel?.Children;
        if (containers is null) return;
        foreach (var container in containers)
            if (container is MultiComboBoxItem t)
                t.IsSelected = false;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (SelectedItems is INotifyCollectionChanged c) c.CollectionChanged -= OnSelectedItemsCollectionChanged;
    }
}

public class MultiComboBoxItem : ContentControl
{
    private MultiComboBox? _parent;
    private static readonly Point s_invalidPoint = new(double.NaN, double.NaN);
    private Point _pointerDownPoint = s_invalidPoint;
    private bool _updateInternal;

    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<MultiComboBoxItem, bool>(
        nameof(IsSelected));

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    static MultiComboBoxItem()
    {
        IsSelectedProperty.AffectsPseudoClass<MultiComboBoxItem>(":selected");
        PressedMixin.Attach<MultiComboBoxItem>();
        FocusableProperty.OverrideDefaultValue<MultiComboBoxItem>(true);
        IsSelectedProperty.Changed.AddClassHandler<MultiComboBoxItem, bool>((item, args) =>
            item.OnSelectionChanged(args));
    }

    private void OnSelectionChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (_updateInternal) return;
        var parent = this.FindLogicalAncestorOfType<MultiComboBox>();
        if (args.NewValue.Value)
        {
            parent?.SelectedItems?.Add(this.DataContext);
        }
        else
        {
            parent?.SelectedItems?.Remove(this.DataContext);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _parent = this.FindLogicalAncestorOfType<MultiComboBox>();
        if (this.IsSelected)
            _parent?.SelectedItems?.Add(this.DataContext);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _pointerDownPoint = e.GetPosition(this);
        if (e.Handled)
        {
            return;
        }
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var p = e.GetCurrentPoint(this);
            if (p.Properties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed
                or PointerUpdateKind.RightButtonPressed)
            {
                if (p.Pointer.Type == PointerType.Mouse)
                {
                    this.IsSelected = !this.IsSelected;
                    e.Handled = true;
                }
                else
                {
                    _pointerDownPoint = p.Position;
                }
            }
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!e.Handled && !double.IsNaN(_pointerDownPoint.X) &&
            e.InitialPressMouseButton is MouseButton.Left or MouseButton.Right)
        {
            var point = e.GetCurrentPoint(this);
            if (new Rect(Bounds.Size).ContainsExclusive(point.Position) && e.Pointer.Type == PointerType.Touch)
            {
                this.IsSelected = !this.IsSelected;
                e.Handled = true;
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateSelection();
    }

    internal void UpdateSelection()
    {
        _updateInternal = true;
        if (_parent?.ItemsPanelRoot is VirtualizingPanel)
        {
            IsSelected = _parent?.SelectedItems?.Contains(DataContext) ?? false;
        }
        _updateInternal = false;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ListItemAutomationPeer(this);
    }
}

public class MultiComboBoxSelectedItemList : ItemsControl
{
    public static readonly StyledProperty<ICommand?> RemoveCommandProperty = AvaloniaProperty.Register<MultiComboBoxSelectedItemList, ICommand?>(
        nameof(RemoveCommand));

    public ICommand? RemoveCommand
    {
        get => GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<ClosableTag>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ClosableTag();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ClosableTag tag)
        {
            tag.Command = RemoveCommand;
        }
    }
}

[TemplatePart(PART_CloseButton, typeof(PathIcon))]
public class ClosableTag : ContentControl
{
    public const string PART_CloseButton = "PART_CloseButton";
    private PathIcon? _icon;

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<ClosableTag, ICommand?>(
        nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_icon != null)
        {
            _icon.PointerPressed -= OnPointerPressed;
        }
        _icon = e.NameScope.Find<PathIcon>(PART_CloseButton);
        if (_icon != null)
        {
            _icon.PointerPressed += OnPointerPressed;
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (Command != null && Command.CanExecute(null))
        {
            Command.Execute(this);
        }
    }
}