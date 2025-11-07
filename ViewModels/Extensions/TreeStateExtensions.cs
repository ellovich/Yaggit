using ViewModels.UI.Yaggit;

namespace ViewModels.Extensions;

public static class TreeStateExtensions
{
    // --------------------------
    // Сохранение expand-state
    // --------------------------
    public static Dictionary<string, bool> SaveExpandState(this IEnumerable<VmBranchNode> nodes)
    {
        var dict = new Dictionary<string, bool>();

        void dfs(VmBranchNode n)
        {
            if (n.FullName != null)
                dict[n.FullName] = n.IsExpanded;

            foreach (var c in n.Children)
                dfs(c);
        }

        foreach (var node in nodes)
            dfs(node);

        return dict;
    }


    // --------------------------
    // Восстановление expand-state
    // --------------------------
    public static void RestoreExpandState(this IEnumerable<VmBranchNode> nodes,
                                          Dictionary<string, bool> state)
    {
        void dfs(VmBranchNode n)
        {
            if (n.FullName != null &&
                state.TryGetValue(n.FullName, out bool expanded))
                n.IsExpanded = expanded;

            foreach (var c in n.Children)
                dfs(c);
        }

        foreach (var node in nodes)
            dfs(node);
    }


    // --------------------------
    // Авто-раскрытие пути до узла
    // --------------------------
    public static void ExpandPathTo(this IEnumerable<VmBranchNode> nodes,
                                    string? fullName)
    {
        if (fullName == null)
            return;

        VmBranchNode? FindRecursive(IEnumerable<VmBranchNode> list)
        {
            foreach (var n in list)
            {
                if (n.FullName == fullName)
                    return n;

                var f = FindRecursive(n.Children);
                if (f != null)
                {
                    n.IsExpanded = true;
                    return f;
                }
            }
            return null;
        }

        FindRecursive(nodes);
    }


    // --------------------------
    // Рекурсивный поиск узла
    // --------------------------
    public static VmBranchNode? FindNode(this IEnumerable<VmBranchNode> nodes,
                                         string? fullName)
    {
        if (fullName == null)
            return null;

        foreach (var n in nodes)
        {
            if (n.FullName == fullName)
                return n;

            var c = FindNode(n.Children, fullName);
            if (c != null)
                return c;
        }
        return null;
    }
}