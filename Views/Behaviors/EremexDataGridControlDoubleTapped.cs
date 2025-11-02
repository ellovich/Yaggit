using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Views.Behaviors;

public class EremexDataGridControlDoubleTapped : Behavior<Avalonia.Controls.ComboBox>
{
    public static readonly StyledProperty<ICommand?> LoadCommandProperty =
        AvaloniaProperty.Register<EremexDataGridControlDoubleTapped, ICommand?>(nameof(LoadCommand));

    public ICommand? LoadCommand
    {
        get => GetValue(LoadCommandProperty);
        set => SetValue(LoadCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            // Подписка на событие получения фокуса
            AssociatedObject.GotFocus += OnGotFocus;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
        {
            // Отписка от события получения фокуса
            AssociatedObject.GotFocus -= OnGotFocus;
        }
    }

    private void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        // Проверяем, что список ComboBox пуст и он активен
        if (AssociatedObject?.ItemCount == 0 && LoadCommand?.CanExecute(null) == true)
        {
            LoadCommand.Execute(null);
        }
    }
}