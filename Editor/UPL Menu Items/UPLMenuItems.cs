
namespace TeaSpoons.PackageCore.Editor
{
    using UnityEditor;
    using UnityEngine;

    internal static class UPLMenuItems
    {
        [MenuItem(Menus.RootItem + "UPL/Open UPL Index...", priority = 1_000_000)]
        private static void OpenUPLIndex()
        {
            Application.OpenURL("https://github.com/tea-spoons");
        }
    }
}
