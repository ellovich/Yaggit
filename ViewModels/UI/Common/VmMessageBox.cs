using System.ComponentModel;
using Avalonia.Media;

namespace ViewModels.Common;

public enum eMessageBoxType
{
    [Description("Не выбрано"), Comment("avares://CoreUI/Assets/Icons/bug.svg")]
    None,

    [Description("Информация"), Comment("avares://CoreUI/Assets/Icons/Colorful/infoCircle.svg")]
    Info,

    [Description("Предупреждение"), Comment("avares://CoreUI/Assets/Icons/Colorful/warnTriangle.svg")]
    Warn,

    [Description("Ошибка"), Comment("avares://CoreUI/Assets/Icons/Colorful/errorCircle.svg")]
    Error,
}

public partial class VmMessageBox : VmBase, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;

    public string Icon => MessageBoxType.GetComment()!;

    public IBrush BorderColor
    {
        get
        {
            return MessageBoxType switch
            {
                eMessageBoxType.Info => Brush.Parse("#4C62C9"),
                eMessageBoxType.Warn => Brush.Parse("#FFBB1E"),
                eMessageBoxType.Error => Brush.Parse("#EE4035"),
                _ => Brush.Parse("#4C62C9"),
            };
        }
    }

    [ObservableProperty] public partial eMessageBoxType MessageBoxType { get; set; }
    [ObservableProperty] public partial string? Header { get; set; }
    [ObservableProperty] public partial string? Text { get; set; }
    [ObservableProperty] public partial bool? DialogResult { get; set; }
    [ObservableProperty] public partial int? Width { get; set; }
    [ObservableProperty] public partial int? Height { get; set; }

    [RelayCommand]
    private void Ok()
    {
        DialogResult = true;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}