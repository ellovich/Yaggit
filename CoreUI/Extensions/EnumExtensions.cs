using System.ComponentModel;
using System.Reflection;

namespace CoreUI.Extensions;

internal static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());

        if (field == null)
            return value.ToString();

        DescriptionAttribute? attribute = Attribute
            .GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        return attribute == null
            ? value.ToString()
            : attribute.Description;
    }

    public static object GetValueByDescription(this Type targetType, string description)// where T : Enum
    {
        if (targetType == null || !targetType.IsEnum)
            throw new ArgumentException("Target type must be an Enum or Nullable<Enum>.");

        foreach (var field in targetType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null && attr.Description == description)
                return Enum.Parse(targetType, field.Name);
        }

        throw new ArgumentException($"Enum value with description '{description}' not found in {targetType}.");
    }
}