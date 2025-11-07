using Avalonia.Xaml.Interactivity;

namespace Views.Behaviors;

public sealed class ScrollToSelectedBehavior : Behavior<TreeView>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject!.PropertyChanged += (_, e) =>
        {
            if (e.Property == TreeView.SelectedItemProperty)
            {
                if (AssociatedObject!.SelectedItem is not null)
                    AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
            }
        };
    }
}
