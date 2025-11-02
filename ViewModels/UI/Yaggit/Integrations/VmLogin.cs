namespace ViewModels.UI.Yaggit.Integrations;

public partial class VmLogin : VmBase
{
    private readonly ILogger<VmLogin> _logger;
    private readonly INavigationService _navigationService;
    //private readonly IMessengerService _messengerService;
    //private readonly IPopupService _popupService;
    //private readonly IAuthService _authService;
    //private readonly DataManager _dataManager;

 //   public WindowNotificationManager? NotificationManager { get; set; }

#pragma warning disable CS8618

    // Design-time constructor
    public VmLogin()
    { }

#pragma warning restore CS8618

    public VmLogin(
        ILogger<VmLogin> logger,
        INavigationService navigationService
        //,IMessengerService messengerService,
        //IAuthService authService,
        //DataManager dataManager
        )
    {
        _logger = logger;
        _navigationService = navigationService;
        //_messengerService = messengerService;
        //_authService = authService;
        //_dataManager = dataManager;
    }

    public void Dispose()
    {
        //_messengerService.Unregister<UserLoggedInMessage>(this);
        //_messengerService.Unregister<UserLoggedOutMessage>(this);
        _logger.LogInformation("VmEntry disposed");
    }

    #region FIELDS

    [ObservableProperty]
  //  [Required(ErrorMessageResourceName = nameof(AppDictionary.UsernameRequired), ErrorMessageResourceType = typeof(AppDictionary))]
  //  [MinLength(4, ErrorMessageResourceName = nameof(AppDictionary.UsernameLength), ErrorMessageResourceType = typeof(AppDictionary))]
    public partial string? Username { get; set; }

    [ObservableProperty]
  //  [Required(ErrorMessageResourceName = nameof(AppDictionary.PasswordRequired), ErrorMessageResourceType = typeof(AppDictionary))]
  //  [MinLength(6, ErrorMessageResourceName = nameof(AppDictionary.PasswordLength), ErrorMessageResourceType = typeof(AppDictionary))]
    public partial string? Password { get; set; }

    [ObservableProperty]
    private bool _isPasswordVisible;

    #endregion FIELDS

    #region COMMANDS

    [RelayCommand]
    private async Task Login()
    {
//        ValidateAllProperties();

//        if (HasErrors)
//            return;

//        try
//        {
//#if NO_SERVER
//            User user = new User() { Username = "Test User" };
//#else
//            User user = await _dataManager.TryLogin(Username!, Password!);
//#endif

//            _authService.DeleteToken();
//            _authService.SaveToken(user.Token);

//            // Отправляем сообщение о успешном входе
//            var vmUser = new VM_User(user.Username);
//            _messengerService.Send(new UserLoggedInMessage(vmUser));

//            _navigationService.NavigateTo(PageType.Dashboard);

//            _logger.LogInformation("LoggedIn successfully as {userName} (id: {userId})", user.Username, user.Id);
//            ShowSuccessfulLogin();

//            if (Platform.IsMobile)
//            {
//                _popupService.ShowPopup($"LoggedIn successfully as {user.Username} (id: {user.Id})");
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex.Message);
//            ShowErrorLogin();
//        }
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task GoogleAuth()
    {
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void GoToRegistration() => _navigationService.NavigateTo<VmRegister>();

    #endregion COMMANDS

    public void ShowSuccessfulLogin()
    {
        //NotificationManager?.Show(
        //    new Notification("Welcome", "Logged in successfully"),
        //    expiration: TimeSpan.FromSeconds(5),
        //    showIcon: true,
        //    showClose: true,
        //    type: NotificationType.Success,
        //    classes: ["Light"]);
    }

    public void ShowErrorLogin(string errorMessage = "")
    {
        //NotificationManager?.Show(
        //    new Notification("Error", $"Error in login or password"),
        //    expiration: TimeSpan.FromSeconds(5),
        //    showIcon: true,
        //    showClose: true,
        //    type: NotificationType.Error,
        //    classes: ["Light"]);
    }
}

//public class UserLoggedInMessage : ValueChangedMessage<VM_User>
//{
//    public UserLoggedInMessage(VM_User user) : base(user)
//    {
//    }
//}