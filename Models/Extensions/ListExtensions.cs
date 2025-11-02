namespace Models.Extensions;

/// <summary>
/// Расширения для удобного преобразования списков и коллекций в строку.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Преобразует коллекцию в строку с разделителем.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="source">Коллекция.</param>
    /// <param name="separator">Разделитель между элементами (по умолчанию "; ").</param>
    /// <param name="nullPlaceholder">Текст для элементов, равных null (по умолчанию пустая строка).</param>
    /// <returns>Строка, содержащая элементы через разделитель, или пустая строка, если коллекция пуста.</returns>
    public static string ToSeparatedString<T>(
        this IEnumerable<T>? source,
        string separator = "; ",
        string nullPlaceholder = "")
    {
        if (source == null)
            return string.Empty;

        return string.Join(separator, source.Select(x => x?.ToString() ?? nullPlaceholder));
    }

    /// <summary>
    /// Преобразует коллекцию в строку, где каждый элемент — с новой строки.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="source">Коллекция.</param>
    /// <param name="addSeparator">Добавлять ли разделитель (например, ";") в конце строки каждого элемента.</param>
    /// <param name="separator">Разделитель (по умолчанию ";").</param>
    /// <returns>Строка с элементами построчно.</returns>
    public static string ToMultilineString<T>(
        this IEnumerable<T>? source,
        bool addSeparator = true,
        string separator = ";")
    {
        if (source == null)
            return string.Empty;

        var sep = addSeparator ? separator : string.Empty;
        return string.Join(Environment.NewLine, source.Select(x => $"{x}{sep}"));
    }

    /// <summary>
    /// Преобразует коллекцию байтов в строку HEX-значений.
    /// </summary>
    public static string ToHexString(this IEnumerable<byte>? bytes, string separator = " ")
    {
        if (bytes == null)
            return string.Empty;

        return string.Join(separator, bytes.Select(b => b.ToString("X2")));
    }

    /// <summary>
    /// Преобразует коллекцию в JSON-подобный список (для логов, отладки).
    /// </summary>
    public static string ToDebugString<T>(this IEnumerable<T>? source)
    {
        if (source == null)
            return "[]";

        var items = source.Select(x => $"\"{x}\"");
        return $"[{string.Join(", ", items)}]";
    }
}
