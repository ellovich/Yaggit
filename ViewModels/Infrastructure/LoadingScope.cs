namespace ViewModels.Infrastructure;

public partial class ActiveLoading : ObservableObject
{
    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ElapsedTime { get; set; } = default!;

    private readonly DateTime _startTime;
    private readonly Timer _timer;

    public ActiveLoading()
    {
        _startTime = DateTime.Now;
        _timer = new Timer(UpdateElapsedTime, null, 1000, 1000);
    }

    private void UpdateElapsedTime(object? state)
    {
        var elapsed = DateTime.Now - _startTime;
        string timeDisplay = $"{(int)elapsed.TotalSeconds} сек";

        if (timeDisplay != "0 сек")
            ElapsedTime = timeDisplay;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}

/// <summary>
/// Класс LoadingScope используется для индикации загрузки в виде контекста,
/// который автоматически добавляет индикатор загрузки в список активных загрузок при создании
/// и удаляет его при завершении работы (Dispose).
///
/// Рекомендуется использовать в блоке using, чтобы гарантировать вызов Dispose и избежать утечек памяти.
/// </summary>
public sealed class LoadingScope : IDisposable
{
    private readonly ActiveLoading _loading;

    /// <summary>
    /// Создает новый экземпляр LoadingScope с заданным сообщением и регистрирует его в списке активных загрузок.
    /// </summary>
    /// <param name="message">Сообщение, отображаемое во время загрузки.</param>
    public LoadingScope(string message)
    {
        _loading = new ActiveLoading { Message = message };
        VmBase.ActiveLoadings.Add(_loading);
    }

    /// <summary>
    /// Удаляет индикатор загрузки из списка активных загрузок.
    /// Метод Dispose обязательно должен вызываться (например, через using),
    /// чтобы не удерживать ссылки на объекты и не создавать утечек памяти.
    /// </summary>
    public void Dispose()
    {
        VmBase.ActiveLoadings.Remove(_loading);
        _loading.Dispose();
    }
}