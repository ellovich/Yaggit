namespace CoreUI.Assets;

public partial class IconsViewer : UserControl
{
    public IconsViewer()
    {
        InitializeComponent();
        FillTabControl(Tabs);
    }

    private static void FillTabControl(TabControl tabs)
    {
        var root = IconsPath;
        var allFolders = GetAllFolders(root);

        foreach (var path in allFolders)
        {
            var tabItem = new TabItem()
            {
                Header = GetTabHeader(root, path),
                Content = new CoreUI.Controls.AutoGrid()
                {
                    ChildMargin = new Avalonia.Thickness(2),
                    ColumnCount = 6,
                    RowCount = 1000,
                }
            };

            FillGridWithIcons(path, (tabItem.Content as CoreUI.Controls.AutoGrid)!.Children);
            tabs.Items.Add(tabItem);
        }
    }

    private static string GetTabHeader(string root, string fullPath)
    {
        if (string.Equals(root, fullPath, StringComparison.OrdinalIgnoreCase))
            return "Icons";

        return Path.GetRelativePath(root, fullPath)
                   .Replace(Path.DirectorySeparatorChar, '/');
    }

    private static void FillGridWithIcons(string path, Avalonia.Controls.Controls children)
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

    private static string IconsPath
    {
        get
        {
            string dir = AppContext.BaseDirectory;
            dir = GetIconsPath(dir, "Yaggit");
            dir = Path.Combine(dir, "CoreUI", "Assets", "Icons");
            return dir;
        }
    }

    private static IEnumerable<string> GetAllFolders(string root)
    {
        yield return root;

        foreach (var dir in Directory.GetDirectories(root, "*", SearchOption.AllDirectories))
            yield return dir;
    }

    private static string GetIconsPath(string path, string folderName)
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