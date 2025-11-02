using System.Globalization;
using System.Text.RegularExpressions;

namespace Models.Extensions;

/// <summary>
/// Набор расширений для форматирования и нормализации строк.
/// </summary>
public static partial class Formatting
{
    [GeneratedRegex("(?<=[a-z])([A-Z])", RegexOptions.Compiled)]
    private static partial Regex PascalSplitPattern();

    [GeneratedRegex("^[A-Z][a-z]*", RegexOptions.Compiled)]
    private static partial Regex PascalPrefixPattern();

    [GeneratedRegex(@"[\s_\-]+", RegexOptions.Compiled)]
    private static partial Regex SplitWordPattern();

    /// <summary>
    /// Нормализует имя, заменяя пробелы на подчёркивания, убирая недопустимые символы,
    /// и добавляя "_" в начало, если имя начинается с цифры.
    /// </summary>
    public static string NormalizeName(this string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        name = name.Replace(' ', '_');

        var validName = new string(name.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());

        if (string.IsNullOrEmpty(validName))
            return "_";

        if (char.IsDigit(validName[0]))
            validName = "_" + validName;

        return validName;
    }

    /// <summary>
    /// Преобразует строку в PascalCase (каждое слово с заглавной буквы, без пробелов и подчёркиваний).
    /// </summary>
    public static string ToPascalCase(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Разделяем по пробелам, дефисам, подчёркиваниям
        var words = SplitWordPattern().Split(text.Trim())
            .Where(w => !string.IsNullOrEmpty(w))
            .Select(w => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(w.ToLowerInvariant()));

        return string.Concat(words);
    }

    /// <summary>
    /// Делает первую букву строки строчной.
    /// </summary>
    public static string ToLowerFirstLetter(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return char.ToLowerInvariant(text[0]) + text[1..];
    }

    /// <summary>
    /// Делает первую букву строки заглавной.
    /// </summary>
    public static string ToUpperFirstLetter(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return char.ToUpperInvariant(text[0]) + text[1..];
    }

    /// <summary>
    /// Извлекает префикс из PascalCase-строки (например, "UserName" → "User").
    /// </summary>
    public static string GetPrefixFromPascalCase(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var match = PascalPrefixPattern().Match(input);
        return match.Success ? match.Value : input;
    }

    /// <summary>
    /// Заменяет первое вхождение подстроки на указанную строку.
    /// </summary>
    public static string ReplaceFirst(this string? text, string search, string replace)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(search))
            return text ?? string.Empty;

        var pos = text.IndexOf(search, StringComparison.Ordinal);
        if (pos < 0)
            return text;

        return text[..pos] + replace + text[(pos + search.Length)..];
    }

    /// <summary>
    /// Разбивает PascalCase или camelCase строку на слова.
    /// Пример: "UserName" → "User Name", "orderId" → "Order Id"
    /// </summary>
    public static string SplitPascalCase(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return PascalSplitPattern().Replace(text, " $1").Trim();
    }

    /// <summary>
    /// Проверяет, является ли строка валидным идентификатором C#.
    /// </summary>
    public static bool IsValidIdentifier(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        if (!char.IsLetter(text[0]) && text[0] != '_')
            return false;

        return text.All(c => char.IsLetterOrDigit(c) || c == '_');
    }
}
