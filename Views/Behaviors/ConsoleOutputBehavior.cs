using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaEdit;

namespace Views.Behaviors;

public class ConsoleOutputBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<string> TextProperty =
        AvaloniaProperty.RegisterAttached<ConsoleOutputBehavior, TextEditor, string>("Text");

    static ConsoleOutputBehavior()
    {
        TextProperty.Changed.AddClassHandler<TextEditor>(OnTextChanged);
    }

    private static void OnTextChanged(TextEditor editor, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string text)
        {
            editor.Text = text;
        }
    }

    public static string GetText(TextEditor editor) => editor.GetValue(TextProperty);
    public static void SetText(TextEditor editor, string value) => editor.SetValue(TextProperty, value);
}