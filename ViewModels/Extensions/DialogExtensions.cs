namespace ViewModels.Extensions;

internal static class DialogExtensions
{
    public static async Task<bool?> ShowConfirmDialog(this IDialogService dialogService, VmBase ownerViewModel, string header, string text)
    {
        //dialogService.CheckNotNull(nameof(dialogService)); // Использование HanumanInstitute.Validators

        var vm = dialogService.CreateViewModel<VmMessageBoxYesNo>();
        vm.Header = header;
        vm.Text = text;

        return await dialogService
            .ShowDialogAsync(ownerViewModel, vm)
            .ConfigureAwait(true);
    }

    public static async Task ShowInfoAsync(
        this IDialogService dialogService,
        VmBase ownerViewModel,
        string header,
        string text,
        int width = 700,
        int height = 400)
    {
        var vm = dialogService.CreateViewModel<VmMessageBox>();
        vm.MessageBoxType = eMessageBoxType.Info;
        vm.Header = header;
        vm.Text = text;

        vm.Width = width;
        vm.Height = height;

        await dialogService
            .ShowDialogAsync(ownerViewModel, vm)
            .ConfigureAwait(true);
    }

    public static async Task<bool?> ShowWarningAsync(
        this IDialogService dialogService,
        VmBase ownerViewModel,
        string text,
        int width = 700,
        int height = 400)
    {
        var vm = dialogService.CreateViewModel<VmMessageBox>();
        vm.MessageBoxType = eMessageBoxType.Warn;
        vm.Header = "Предупреждение";
        vm.Text = text;

        vm.Width = width;
        vm.Height = height;

        var result = await dialogService
            .ShowDialogAsync(ownerViewModel, vm)
            .ConfigureAwait(true);

        return result;
    }

    public static async Task<bool?> ShowErrorAsync(
        this IDialogService dialogService,
        VmBase ownerViewModel,
        string title,
        string text,
        int width = 700,
        int height = 400)
    {
        var vm = dialogService.CreateViewModel<VmMessageBox>();
        vm.MessageBoxType = eMessageBoxType.Error;
        vm.Header = title;
        vm.Text = text;

        vm.Width = width;
        vm.Height = height;

        var result = await dialogService
            .ShowDialogAsync(ownerViewModel, vm)
            .ConfigureAwait(true);

        return result;
    }
}