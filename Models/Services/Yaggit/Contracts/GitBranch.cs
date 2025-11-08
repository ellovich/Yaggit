namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Ветка Git.
/// </summary>
/// <param name="Name">Имя ветки.</param>
/// <param name="IsCurrent">Признак, является ли ветка активной.</param>
public record GitBranch(string Name, bool IsCurrent)
{
    public override string ToString() => IsCurrent ? $"* {Name}" : Name;
}
