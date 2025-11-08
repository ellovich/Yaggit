using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ViewModels.UI.Yaggit.Messages;

public sealed class RepositoryChangedMessage(string repoPath) 
    : ValueChangedMessage<string>(repoPath)
{
}
