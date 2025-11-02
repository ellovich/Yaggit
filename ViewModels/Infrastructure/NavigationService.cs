namespace ViewModels.Infrastructure;

public interface INavigationService
{
    VmBase? CurrentPage { get; }

    event Action? CurrentPageChanged;

    void GoBack();

    void NavigateToExisting(VmBase vm);

    VmBase NavigateTo<T>(Action<T>? afterCreation = null) where T : VmBase;

    VmBase NavigateTo(Type pageType, Action<VmBase>? afterCreation = null);

    Task<VmBase> NavigateToAsync<T>(Func<T, Task>? afterCreation = null) where T : VmBase;

    Task<VmBase> NavigateToAsync(Type pageType, Func<VmBase, Task>? afterCreation = null);
}

public partial class NavigationService(IVmFactory vmFactory, ILogger<NavigationService> logger) : ObservableObject, INavigationService
{
    private readonly Stack<VmBase> _pageStack = new();

    [ObservableProperty] public partial VmBase? CurrentPage { get; set; }

    public event Action? CurrentPageChanged;

    private void OnCurrentPageChanged()
    {
        CurrentPage = _pageStack.TryPeek(out var page) ? page : null;
        CurrentPageChanged?.Invoke();
    }

    public void NavigateToExisting(VmBase vm)
    {
        if (_pageStack.Any(x => x.PageId == vm.PageId))
        {
            while (_pageStack.Peek().PageId != vm.PageId)
            {
                _pageStack.Pop();
            }
            CurrentPage = vm;
        }
        else
        {
            logger.LogError("Экземпляр страницы не найден в стеке.");
        }
    }

    public void GoBack()
    {
        if (_pageStack.Count > 1) _pageStack.Pop();
        OnCurrentPageChanged();
        LogStack();
    }

    public VmBase NavigateTo<T>(Action<T>? afterCreation = null) where T : VmBase
    {
        if (TryGetExistingPage(typeof(T), out var page))
            return page!;
        else
            return PushPage(vmFactory.GetVm<T>(afterCreation));
    }

    public VmBase NavigateTo(Type pageType, Action<VmBase>? afterCreation = null) =>
        TryGetExistingPage(pageType, out var page) ? page! : PushPage(vmFactory.GetVm(pageType, afterCreation));

    public async Task<VmBase> NavigateToAsync<T>(Func<T, Task>? afterCreation = null) where T : VmBase =>
        TryGetExistingPage(typeof(T), out var page) ? page! : PushPage(await vmFactory.GetVmAsync<T>(afterCreation));

    public async Task<VmBase> NavigateToAsync(Type pageType, Func<VmBase, Task>? afterCreation = null) =>
        TryGetExistingPage(pageType, out var page) ? page! : PushPage(await vmFactory.GetVmAsync(pageType, afterCreation));

    private void LogStack() => logger.LogInformation("Stack: {stack}", string.Join(" -> ", _pageStack.Reverse().Select(x => x.PageId)));

    private bool TryGetExistingPage(Type pageType, out VmBase? page)
    {
        page = _pageStack.FirstOrDefault(x => x.GetType() == pageType);
        //if (page != null)
        //{
        //    while (_pageStack.Peek().Id != page.Id) _pageStack.Pop();
        //    OnCurrentPageChanged();
        //    LogStack();
        //    return true;
        //}
        return false;
    }

    private VmBase PushPage(VmBase page)
    {
        _pageStack.Push(page);
        OnCurrentPageChanged();
        LogStack();
        return page;
    }
}