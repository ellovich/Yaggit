namespace ViewModels.Common;

public partial class VmMessageBoxYesNo : VmMessageBox
{
    public VmMessageBoxYesNo()
    {
        MessageBoxType = eMessageBoxType.Info;
        Header = "Подтверждение";
    }

    [RelayCommand]
    public void Yes()
    {
        OkCommand.Execute(null);
    }

    [RelayCommand]
    public void No()
    {
        CancelCommand.Execute(null);
    }
}