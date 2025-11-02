using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace ViewModels.UI.Common;

// добавить возможность закрыть форму по клике в листовой форме

public enum eOpeningMode
{
    ReadOnly,
    Editable
}

public abstract class VmBase : ObservableObject
{
    private static int _pageId = 0;
    public int PageId { get; set; }

    public eOpeningMode OpeningMode { get; set; } = eOpeningMode.ReadOnly;

    public static ObservableCollection<ActiveLoading> ActiveLoadings { get; } = [];

    protected VmBase()
    {
        PageId = _pageId++;
    }

    public static bool IsDebug
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

    protected bool HaveRights()
    {
        return true;
        // return user.HasRight();
    }

    public static ILogger<T> CreateMockLoggerForDesigner<T>()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug);
        });
        var logger = loggerFactory.CreateLogger<T>();
        logger.LogInformation("Design-time logger created");
        return logger;
    }
}

public abstract class VmBaseDbLoad : VmBase
{
    /// <summary>
    /// Метод чтение из базы, вызывается после конструктора
    /// </summary>
    /// <returns></returns>
  //  public virtual async Task ReadFromDbAsync() => await Task.CompletedTask; // Todo rename + async

    protected VmBaseDbLoad()
    {
        //Dispatcher.UIThread.Post(async () =>
        //{
        //    try
        //    {
        //        await ReadFromDbAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Здесь можно обработать исключения (например, логирование)
        //        Console.Error.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
        //    }
        //});
    }

    protected TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            var visualRoot = viewApp.MainView?.GetVisualRoot();
            return visualRoot as TopLevel;
        }
        return null;
    }
}

public abstract class VmBaseEdit : VmBase
{
    public void Cancel()
    { }

    public void Save()
    { }
}

public abstract class VmBaseSelectableList<T> : VmBase
{
}

public interface IEditable
{
    void BeginEdit();

    void CancelEdit();

    void EndEdit();
}

public abstract class VmEditable<T> : VmBase, IEditable where T : VmEditable<T>
{
    private string? _backup;

    protected VmEditable()
    {
        BeginEdit();
    }

    public void BeginEdit()
    {
      //  _backup = JsonSerializer.Serialize(this);
    }

    public void CancelEdit()
    {
        if (_backup is not null)
        {
            var restored = JsonSerializer.Deserialize<string>(_backup);
            if (restored != null)
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.CanWrite && prop.CanRead)
                    {
                        var value = prop.GetValue(restored);
                        prop.SetValue(this, value);
                    }
                }

                // присвоить поля
            }
        }
    }

    public void EndEdit()
    {
        _backup = null;
    }
}