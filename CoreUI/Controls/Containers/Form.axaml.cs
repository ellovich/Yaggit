using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace CoreUI.Controls;

public enum ePosition
{
    Left,
    Top,
    Right,
    Bottom,
}

public interface IFormGroup
{
}

[PseudoClasses(PC_FixedWidth)]
public class Form : ItemsControl
{
    public const string PC_FixedWidth = ":fixed-width";

    public static readonly StyledProperty<GridLength> LabelWidthProperty = AvaloniaProperty.Register<Form, GridLength>(
        nameof(LabelWidth));

    /// <summary>
    /// Behavior:
    /// <para>Fixed Width: all labels are with fixed length. </para>
    /// <para>Star: all labels are aligned by max length. </para>
    /// <para>Auto: labels are not aligned. </para>
    /// </summary>
    public GridLength LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    public static readonly StyledProperty<ePosition> LabelPositionProperty = AvaloniaProperty.Register<Form, ePosition>(
        nameof(LabelPosition), defaultValue: ePosition.Left);

    public ePosition LabelPosition
    {
        get => GetValue(LabelPositionProperty);
        set => SetValue(LabelPositionProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> LabelAlignmentProperty = AvaloniaProperty.Register<Form, HorizontalAlignment>(
        nameof(LabelAlignment), defaultValue: HorizontalAlignment.Left);

    public HorizontalAlignment LabelAlignment
    {
        get => GetValue(LabelAlignmentProperty);
        set => SetValue(LabelAlignmentProperty, value);
    }

    static Form()
    {
        LabelWidthProperty.Changed.AddClassHandler<Form, GridLength>((x, args) => x.LabelWidthChanged(args));
    }

    private void LabelWidthChanged(AvaloniaPropertyChangedEventArgs<GridLength> args)
    {
        var newValue = args.NewValue.Value;
        bool isFixed = newValue.IsStar || newValue.IsAbsolute;
        PseudoClasses.Set(PC_FixedWidth, isFixed);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not FormItem && item is not FormGroup;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is not Control control)
        {
            if (item is IFormGroup) return new FormGroup();
            return new FormItem();
        }
        return new FormItem()
        {
            Content = control,
            [!FormItem.LabelProperty] = control[!FormItem.LabelProperty],
            [!FormItem.IsRequiredProperty] = control[!FormItem.IsRequiredProperty],
            [!FormItem.NoLabelProperty] = control[!FormItem.NoLabelProperty],
        };
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is FormItem formItem && !formItem.IsSet(ContentControl.ContentTemplateProperty))
        {
            formItem.ContentTemplate = ItemTemplate;
        }
        if (container is FormGroup group && !group.IsSet(FormGroup.ItemTemplateProperty))
        {
            group.ItemTemplate = ItemTemplate;
        }
    }
}

public class FormGroup : HeaderedItemsControl
{
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not FormItem;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is not Control control) return new FormItem();
        return new FormItem
        {
            Content = control,
            [!FormItem.LabelProperty] = control[!FormItem.LabelProperty],
            [!FormItem.IsRequiredProperty] = control[!FormItem.IsRequiredProperty],
        };
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is FormItem formItem && !formItem.IsSet(ContentControl.ContentTemplateProperty))
        {
            formItem.ContentTemplate = ItemTemplate;
        }
    }
}

[PseudoClasses(PC_Horizontal, PC_NoLabel)]
public class FormItem : ContentControl
{
    public const string PC_Horizontal = ":horizontal";
    public const string PC_NoLabel = ":no-label";

    #region Attached Properties

    public static readonly AttachedProperty<object?> LabelProperty =
        AvaloniaProperty.RegisterAttached<FormItem, Control, object?>("Label");

    public static void SetLabel(Control obj, object? value) => obj.SetValue(LabelProperty, value);

    public static object? GetLabel(Control obj) => obj.GetValue(LabelProperty);

    public static readonly AttachedProperty<bool> IsRequiredProperty =
        AvaloniaProperty.RegisterAttached<FormItem, Control, bool>("IsRequired");

    public static void SetIsRequired(Control obj, bool value) => obj.SetValue(IsRequiredProperty, value);

    public static bool GetIsRequired(Control obj) => obj.GetValue(IsRequiredProperty);

    public static readonly AttachedProperty<bool> NoLabelProperty =
        AvaloniaProperty.RegisterAttached<FormItem, Control, bool>("NoLabel");

    public static void SetNoLabel(Control obj, bool value) => obj.SetValue(NoLabelProperty, value);

    public static bool GetNoLabel(Control obj) => obj.GetValue(NoLabelProperty);

    #endregion Attached Properties

    private List<IDisposable> _formSubscriptions = new List<IDisposable>();

    public static readonly StyledProperty<double> LabelWidthProperty = AvaloniaProperty.Register<FormItem, double>(
        nameof(LabelWidth));

    public double LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> LabelAlignmentProperty = AvaloniaProperty.Register<FormItem, HorizontalAlignment>(
        nameof(LabelAlignment));

    public HorizontalAlignment LabelAlignment
    {
        get => GetValue(LabelAlignmentProperty);
        set => SetValue(LabelAlignmentProperty, value);
    }

    static FormItem()
    {
        NoLabelProperty.AffectsPseudoClass<FormItem>(PC_NoLabel);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var form = this.GetVisualAncestors().OfType<Form>().FirstOrDefault();
        if (form is not null)
        {
            _formSubscriptions.Clear();
            var labelSubscription = form
                .GetObservable(Form.LabelWidthProperty)
                .Subscribe(new AnonymousObserver<GridLength>(length => { LabelWidth = length.IsAbsolute ? length.Value : double.NaN; }));
            var positionSubscription = form
                .GetObservable(Form.LabelPositionProperty)
                .Subscribe(new AnonymousObserver<ePosition>(position => { PseudoClasses.Set(PC_Horizontal, position == ePosition.Left); }));
            var alignmentSubscription = form
                .GetObservable(Form.LabelAlignmentProperty)
                .Subscribe(new AnonymousObserver<HorizontalAlignment>(alignment => { LabelAlignment = alignment; }));
            _formSubscriptions.Add(labelSubscription);
            _formSubscriptions.Add(positionSubscription);
            _formSubscriptions.Add(alignmentSubscription);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        foreach (var subscription in _formSubscriptions)
        {
            subscription.Dispose();
        }
    }
}