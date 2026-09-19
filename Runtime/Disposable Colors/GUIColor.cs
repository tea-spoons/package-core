
namespace TeaSpoons.PackageCore
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Changes <see cref="GUI.color"/> and reverts when it is disposed.
    /// </summary>
    /// <example>
    /// using (GUIColor.Override(Color.red))
    /// {
    ///   GUILayout.Box("Danger");
    /// }
    /// </example>
    public readonly struct GUIColor : IDisposable
    {
        public static GUIColor Override(Color color)
        {
            return new GUIColor(color);
        }

        public static GUIColor Multiply(Color color)
        {
            return new GUIColor(GUI.color * color);
        }

        private readonly Color originalColor;

        private GUIColor(Color color)
        {
            originalColor = GUI.color;
            GUI.color = color;
        }

        void IDisposable.Dispose()
        {
            GUI.color = originalColor;
        }
    }
}
