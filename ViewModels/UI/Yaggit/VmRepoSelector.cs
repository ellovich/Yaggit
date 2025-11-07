using Models.Services.Yaggit.Contracts;

namespace ViewModels.UI.Yaggit;

/// <summary>
/// ViewModel для выбора или создания репозитория.
/// </summary>
public partial class VmRepoSelector : VmBase
{
    private readonly ILogger<VmRepoSelector> _logger;
    private readonly IDialogService _dialogService;
    private readonly IGitRepositoryContext _gitRepositoryContext;
    private readonly IGitRepositoryInitializer _gitInitializer;

#pragma warning disable CS9264, CS8618
    public VmRepoSelector() { }
#pragma warning restore CS9264, CS8618

    public VmRepoSelector(
        ILogger<VmRepoSelector> logger,
        IDialogService dialogService,
        IGitRepositoryContext gitRepositoryContext,
        IGitRepositoryInitializer gitInitializer
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _gitRepositoryContext = gitRepositoryContext ?? throw new ArgumentNullException(nameof(gitRepositoryContext));
        _gitInitializer = gitInitializer ?? throw new ArgumentNullException(nameof(gitInitializer));
    }


    #region PROPS

    [ObservableProperty]
    public required partial string? RepoPath { get; set; } = string.Empty;

    [ObservableProperty]
    public required partial string? RepoName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsRepositoryInitialized { get; set; }

    partial void OnRepoPathChanged(string? value)
    {
        if (value is not null)
        {
            RepoName = new DirectoryInfo(value).Name;
            IsRepositoryInitialized = _gitInitializer.IsRepositoryInitialized(value);
        }
    }

    #endregion PROPS


    #region COMMANDS

    [RelayCommand]
    private async Task SelectRepo()
    {
        try
        {
            var res = await _dialogService.ShowOpenFolderDialogAsync(MainVm);
            if (res is null)
                return;

            RepoPath = res.Path.LocalPath;

            if (!_gitInitializer.IsRepositoryInitialized(RepoPath))
            {
                var confirm = await _dialogService.ShowConfirmDialog(
                    MainVm,
                    "Папка не является Git-репозиторием",
                    "Создать новый репозиторий?"
                );

                if (confirm == true)
                {
                    await _gitInitializer.InitializeRepositoryAsync(RepoPath);
                    IsRepositoryInitialized = true;
                    await _dialogService.ShowInfoAsync(MainVm, Lang.Git.Result_RepoInitialized, RepoPath);
                }
                else
                {
                    _logger.LogInformation("Отменён выбор неинициализированной папки {Path}", RepoPath);
                    return;
                }
            }
            else
            {
                IsRepositoryInitialized = true;
            }

            _gitRepositoryContext.SetRepository(RepoPath);
            _logger.LogInformation("Выбран репозиторий {Path}", RepoPath);
        }
        catch (GitException ex)
        {
            _logger.LogError(ex, "Ошибка git при выборе репозитория.");
            await _dialogService.ShowErrorAsync(MainVm, "Ошибка Git", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка выбора репозитория.");
            await _dialogService.ShowErrorAsync(MainVm, "Ошибка выбора папки", ex.Message);
        }
    }

    /// <summary>
    /// Создание нового git-репозитория.
    /// </summary>
    [RelayCommand]
    private async Task InitRepo()
    {
        try
        {
            var res = await _dialogService.ShowOpenFolderDialogAsync(MainVm);
            if (res is null)
                return;

            RepoPath = res.Path.AbsolutePath;

            if (_gitInitializer.IsRepositoryInitialized(RepoPath))
            {
                await _dialogService.ShowInfoAsync(MainVm, "Репозиторий уже существует", RepoPath);
                _gitRepositoryContext.SetRepository(RepoPath);
                return;
            }

            await _gitInitializer.InitializeRepositoryAsync(RepoPath);
            _gitRepositoryContext.SetRepository(RepoPath);

            IsRepositoryInitialized = true;
            _logger.LogInformation("Репозиторий успешно инициализирован: {Path}", RepoPath);
            await _dialogService.ShowInfoAsync(MainVm, "Репозиторий создан", RepoPath);
        }
        catch (GitException ex)
        {
            _logger.LogError(ex, "Ошибка git при инициализации репозитория.");
            await _dialogService.ShowErrorAsync(MainVm, "Ошибка Git", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации репозитория.");
            await _dialogService.ShowErrorAsync(MainVm, "Ошибка", ex.Message);
        }
    }

    #endregion COMMANDS
}

public class MockVmRepoSelector : VmRepoSelector
{
    public MockVmRepoSelector()
    {
        RepoPath = @"C:\Users\ello\Desktop\Projects\AvaMarketSimulationOld";
        RepoName = @"AvaMarketSimulationOld";
    }
}