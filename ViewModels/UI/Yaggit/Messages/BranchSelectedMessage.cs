using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ViewModels.UI.Yaggit.Messages;

public class BranchSelectedMessage(string branchName) 
    : ValueChangedMessage<string>(branchName)
{
}