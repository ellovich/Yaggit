using Avalonia.Interactivity;

namespace CoreUI.Controls;

public class CalendarDayButtonEventArgs(DateTime? date) : RoutedEventArgs
{
    public DateTime? Date { get; private set; } = date;
}