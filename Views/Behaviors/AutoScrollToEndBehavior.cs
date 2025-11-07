using System.Collections.Specialized;
using Avalonia.Xaml.Interactivity;

namespace Views.Behaviors;

public partial class AutoScrollToEndBehavior : Behavior<ItemsControl>
{
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        if (AssociatedObject?.Items is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged += (_, _) =>
            {
                if (AssociatedObject.ItemCount > 0)
                {
                    var lastItem = AssociatedObject.Items[AssociatedObject.Items.Count - 1];
                    if (lastItem != null)
                        AssociatedObject.ScrollIntoView(lastItem);
                }
            };
        }
    }
}