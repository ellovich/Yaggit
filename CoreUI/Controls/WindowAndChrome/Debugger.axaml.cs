using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace CoreUI.Controls;

public class Debugger : TemplatedControl
{
    private Button? _btn;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _btn = e.NameScope.Find<Button>("PART_Button");
        if (_btn != null)
            _btn.Click += OnBtnClick;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_btn != null)
        {
            _btn.Click -= OnBtnClick;
            _btn = null;
        }
    }

    private void OnBtnClick(object? sender, RoutedEventArgs e)
    {
#if DEBUG
        var mainWindow = GetMainWindow();
        if (mainWindow != null)
        {
            //
        }
#endif
    }

    private Window? GetMainWindow()
    {
        return Application.Current?.ApplicationLifetime
            switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            _ => null
        };
    }
}