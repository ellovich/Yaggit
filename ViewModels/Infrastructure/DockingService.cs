using System.ComponentModel;
using System.Windows.Input;

namespace ViewModels.Infrastructure;

public interface IDockingService
{
    ObservableCollection<VmDockingDocument>? Tabs { get; }
    VmDockingDocument? CurrentTab { get; }

    void OpenPage<T>(string title = "", Action<T>? afterCreation = null) where T : VmBase;

    void OpenPage(Type pageType, string header = "", Action<VmBase>? afterCreation = null);

    Task OpenPageAsync<T>(string header = "", Func<T, Task>? afterCreation = null) where T : VmBase;

    Task OpenPageAsync(Type pageType, string header = "", Func<VmBase, Task>? afterCreation = null);
}

public partial class DockingService : ObservableObject, IDockingService
{
    private readonly INavigationService _navigationService;
    public ObservableCollection<VmDockingDocument> Tabs { get; } = [];
    [ObservableProperty] public partial VmDockingDocument? CurrentTab { get; set; }

    public DockingService(INavigationService navigationService)
    {
        _navigationService = navigationService;

        if (_navigationService is INotifyPropertyChanged nav)
        {
            nav.PropertyChanged += OnNavigationServicePropertyChanged;
        }
    }

    private void OnNavigationServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_navigationService.CurrentPage))
        {
            UpdateActiveTab();
        }
    }

    partial void OnCurrentTabChanged(VmDockingDocument? value)
    {
        if (value != null && value.Content is VmBase vm)
        {
            // Синхронизируем CurrentPage с содержимым активной вкладки
            _navigationService.NavigateToExisting(vm);
        }
    }

    private void UpdateActiveTab()
    {
        if (_navigationService.CurrentPage == null)
            return;

        var tab = Tabs.FirstOrDefault(t => t.Content == _navigationService.CurrentPage);
        if (tab != null)
        {
            CurrentTab = tab;
        }
    }

    private void OpenTab(VmBase vm, string header)
    {
        var existingTab = Tabs.FirstOrDefault(tab => tab.Header is string h && h == header);
        if (existingTab == null)
        {
            var newTab = new VmDockingDocument
            {
                Header = header,
                Content = vm,
                IsActive = true,
            };
            // Обработка закрытия вкладки // TODO: вернуть закрытие
            //newTab.Closed += (sender, e) =>
            //{
            //    Tabs.Remove(newTab);
            //    _navigationService.GoBack(); // Возвращаемся назад при закрытии
            //};
            Tabs.Add(newTab);
            CurrentTab = newTab; // Устанавливаем новую вкладку как активную
        }
        else
        {
            CurrentTab = existingTab; // Активируем существующую вкладку
        }
    }

    public void OpenPage<T>(string header = "", Action<T>? afterCreation = null) where T : VmBase
    {
        var vm = _navigationService.NavigateTo<T>(afterCreation);
        OpenTab(vm, header);
    }

    public void OpenPage(Type pageType, string header = "", Action<VmBase>? afterCreation = null)
    {
        var vm = _navigationService.NavigateTo(pageType, afterCreation);
        OpenTab(vm, header);
    }

    public async Task OpenPageAsync<T>(string header = "", Func<T, Task>? afterCreation = null) where T : VmBase
    {
        var vm = await _navigationService.NavigateToAsync<T>(afterCreation);
        OpenTab(vm, header);
    }

    public async Task OpenPageAsync(Type pageType, string header = "", Func<VmBase, Task>? afterCreation = null)
    {
        var vm = await _navigationService.NavigateToAsync(pageType, afterCreation);
        OpenTab(vm, header);
    }
}

public partial class VmDockingDocument : ObservableObject
{
    public string Header { get; set; } = string.Empty;
    public VmBase? Content { get; set; } = null;
    [ObservableProperty] public partial bool IsActive { get; set; }
    [ObservableProperty] public partial ICommand CloseCommand { get; set; }
}