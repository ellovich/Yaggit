using Avalonia.Controls.Notifications;
using System.ComponentModel.DataAnnotations;

namespace ViewModels.UI.Yaggit.Integrations;

public partial class VmRegister : VmBase
{
    private readonly ILogger<VmRegister> _logger;
    //private readonly DataManager _dataManager;
    //private readonly IAuthService _tokenService;
    private readonly INavigationService _navigationService;
    //private readonly VM_User _user;

    public WindowNotificationManager? NotificationManager { get; set; }

#pragma warning disable CS8618

    public VmRegister()
    { }

#pragma warning restore CS8618

    public VmRegister(
        ILogger<VmRegister> logger,
        //VM_User user,
        //DataManager dataManager,
        //IAuthService tokenService,
        INavigationService navigationService)
    {
        _logger = logger;
        //_dataManager = dataManager;
        //_tokenService = tokenService;
        //_user = user;
        _navigationService = navigationService;
    }

    #region FIELDS

    [ObservableProperty]
   // [Required(ErrorMessageResourceName = nameof(AppDictionary.UsernameRequired), ErrorMessageResourceType = typeof(AppDictionary))]
   // [MinLength(4, ErrorMessageResourceName = nameof(AppDictionary.UsernameLength), ErrorMessageResourceType = typeof(AppDictionary))]
    public partial string? Username { get; set; }

    [ObservableProperty]
  //  [Required(ErrorMessageResourceName = nameof(AppDictionary.PasswordRequired), ErrorMessageResourceType = typeof(AppDictionary))]
  //  [MinLength(6, ErrorMessageResourceName = nameof(AppDictionary.PasswordLength), ErrorMessageResourceType = typeof(AppDictionary))]
    public partial string? Password { get; set; }

    [ObservableProperty]
    //[Required(ErrorMessageResourceName = nameof(AppDictionary.PasswordRetypeRequired), ErrorMessageResourceType = typeof(AppDictionary))]
    //[CustomValidation(typeof(VmRegister), nameof(ValidatePasswordsMatch))]
    public partial string? PasswordRepeated { get; set; }

    [ObservableProperty]
    private bool _isPasswordVisible;

    [ObservableProperty]
  //  [Required(ErrorMessageResourceName = nameof(AppDictionary.EmailRequired), ErrorMessageResourceType = typeof(AppDictionary))]
  //  [EmailAddress(ErrorMessageResourceName = nameof(AppDictionary.EmailInvalid), ErrorMessageResourceType = typeof(AppDictionary))]
    private string? _email;

    public static ValidationResult? ValidatePasswordsMatch(string? passwordRepeated, ValidationContext context)
    {
        var instance = (VmRegister)context.ObjectInstance;

        if (instance.Password != passwordRepeated)
            return new ValidationResult(Auth.Auth_PasswordMissmatch, new[] { nameof(PasswordRepeated) });

        return ValidationResult.Success;
    }

    #endregion FIELDS

    #region COMMANDS

    [RelayCommand]
    private async Task Register()
    {
        //ValidateAllProperties();

        //if (HasErrors)
        //    return;

        //try
        //{
        //    User user = await _dataManager.SignupUser(Username!, Password!, Email!);

        //    _tokenService.DeleteToken();
        //    _tokenService.SaveToken(user.Token);

        //    NavigateBack();
        //    _logger.LogInformation("User {userName} (id: {userId}) Registered successful", user.Username, user.Id);
        //    ShowSuccessfulRegistration();
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Registration error: {er}", ex.Message);
        //    ShowErrorRegistration();
        //}
    }

    [RelayCommand]
    private void NavigateBack() => _navigationService.GoBack();

    #endregion COMMANDS

    public void ShowSuccessfulRegistration()
    {
        //NotificationManager?.Show(
        //    new Notification("Welcome", "Registered successfully"),
        //    expiration: TimeSpan.FromSeconds(5),
        //    showIcon: true,
        //    showClose: true,
        //    type: NotificationType.Success,
        //    classes: ["Light"]);
    }

    public void ShowErrorRegistration(string errorMessage = "")
    {
        //NotificationManager?.Show(
        //    new Notification("Error", $"Error in registration..."),
        //    expiration: TimeSpan.FromSeconds(5),
        //    showIcon: true,
        //    showClose: true,
        //    type: NotificationType.Error,
        //    classes: ["Light"]);
    }
}