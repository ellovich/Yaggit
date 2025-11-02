using System.Collections.Concurrent;
using System.Reflection;

namespace Models.Extensions;

/// <summary>
/// Атрибут для хранения произвольного комментария для значения перечисления.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class CommentAttribute(string comment) : Attribute
{
    public string Comment { get; } = comment;
}

/// <summary>
/// Атрибут для привязки цвета к значению перечисления.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class AvaColorAttribute(eAvaColor color) : Attribute
{
    public eAvaColor Color { get; } = color;
}

public static class EnumExtensions
{
    // Кэш для ускорения повторных вызовов GetCustomAttribute
    private static readonly ConcurrentDictionary<(Type EnumType, string FieldName, Type AttributeType), Attribute?> AttributeCache = new();

    /// <summary>
    /// Универсальный метод получения атрибута для значения перечисления.
    /// </summary>
    private static TAttribute? GetEnumAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var enumType = value.GetType();
        var fieldName = value.ToString();

        var key = (enumType, fieldName, typeof(TAttribute));

        if (AttributeCache.TryGetValue(key, out var cached))
            return cached as TAttribute;

        var field = enumType.GetField(fieldName);
        if (field is null)
            return null;

        var attribute = field.GetCustomAttribute<TAttribute>();
        AttributeCache[key] = attribute;
        return attribute;
    }

    /// <summary>
    /// Получить текст описания (<see cref="DescriptionAttribute"/>).
    /// </summary>
    public static string GetDescription(this Enum value) =>
        value.GetEnumAttribute<DescriptionAttribute>()?.Description ?? value.ToString();

    /// <summary>
    /// Получить текст описания (<see cref="DescriptionAttribute"/>) с подстановкой аргументов.
    /// </summary>
    public static string GetDescription(this Enum value, params object[] args)
    {
        var desc = value.GetEnumAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        return args is { Length: > 0 } ? string.Format(desc, args) : desc;
    }

    /// <summary>
    /// Получить комментарий (<see cref="CommentAttribute"/>).
    /// </summary>
    public static string? GetComment(this Enum value) =>
        value.GetEnumAttribute<CommentAttribute>()?.Comment;

    /// <summary>
    /// Получить цвет (<see cref="AvaColorAttribute"/>).
    /// </summary>
    public static eAvaColor GetAvaColor(this Enum value) =>
        value.GetEnumAttribute<AvaColorAttribute>()?.Color ?? eAvaColor.None;

    /// <summary>
    /// Получить строку CSS-класса для цвета.
    /// </summary>
    public static string GetAvaColorClass(this Enum value) => value.GetAvaColor().ToString();

    /// <summary>
    /// Преобразовать целое значение в Enum указанного типа.
    /// </summary>
    public static T ToEnum<T>(this int constant) where T : Enum =>
        (T)Enum.ToObject(typeof(T), constant);

    /// <summary>
    /// Получить описание по nullable-интовому значению.
    /// </summary>
    public static string? GetDescription<T>(this int? constant) where T : Enum =>
        constant.HasValue ? constant.Value.ToEnum<T>().GetDescription() : null;
}
