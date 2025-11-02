namespace CoreUI.Assets;

public partial class IconsViewer : UserControl
{
    public IconsViewer()
    {
        InitializeComponent();
        FillTabControl(Tabs);
    }

    private void FillTabControl(TabControl tabs)
    {
        var folders = Directory.GetDirectories(GetIconsPath()).ToList();
        folders.Insert(0, GetIconsPath());

        foreach (var path in folders)
        {
            var tabItem = new TabItem()
            {
                Header = Path.GetFileName(path),
                Content = new CoreUI.Controls.AutoGrid()
                {
                    ChildMargin = new Avalonia.Thickness(2),
                    ColumnCount = 8,
                    RowCount = 1000,
                }
            };
            FillGridWithIcons(path, (tabItem.Content as CoreUI.Controls.AutoGrid)!.Children);
            tabs.Items.Add(tabItem);
        }
    }

    private void FillGridWithIcons(string path, Avalonia.Controls.Controls children)
    {
        var files = Directory.GetFiles(path, "*.svg");

        foreach (var iconName in files)
        {
            var svgUri = new Uri(iconName);
            var svg = new Avalonia.Svg.Skia.Svg(svgUri)
            {
                Width = 40,
                Height = 40,
                Path = svgUri.OriginalString
            };

            var previewer = new StackPanel()
            {
                Margin = new Avalonia.Thickness(2, 4),
                Orientation = Avalonia.Layout.Orientation.Vertical,
                Children = {
                    svg,
                    new TextBlock
                    {
                        FontSize = 11,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Text = Path.GetFileNameWithoutExtension(iconName)
                    },
                },
            };

            children.Add(previewer);
        }
    }

    private string GetIconsPath()
    {
        string dir = AppContext.BaseDirectory;
        dir = GetIconsPath(dir, "Yaggit");
        dir = Path.Combine(dir, "CoreUI", "Assets", "Icons");
        return dir;
    }

    private string GetIconsPath(string path, string folderName)
    {
        int index = path.IndexOf(folderName, StringComparison.OrdinalIgnoreCase);
        if (index != -1)
        {
            int folderEnd = index + folderName.Length;
            string trimmedPath = path.Substring(0, folderEnd);
            if (!trimmedPath.EndsWith("\\"))
            {
                trimmedPath += "\\";
            }

            return trimmedPath;
        }

        return path;
    }
}