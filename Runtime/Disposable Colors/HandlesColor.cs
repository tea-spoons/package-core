
#if UNITY_EDITOR
namespace TeaSpoons.PackageCore
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Changes <see cref="Handles.color"/> and reverts when it is disposed.
    /// </summary>
    /// <example>
    /// using (HandlesColor.Override(Color.red))
    /// {
    ///     Handles.DrawLine(a, b);
    /// }
    /// </example>
    public readonly struct HandlesColor : IDisposable
    {
        public static HandlesColor Override(Color color)
        {
            return new HandlesColor(color);
        }

        public static HandlesColor Multiply(Color color)
        {
            return new HandlesColor(Handles.color * color);
        }

        private readonly Color originalColor;

        private HandlesColor(Color color)
        {
            originalColor = Handles.color;
            Handles.color = color;
        }

        void IDisposable.Dispose()
        {
            Handles.color = originalColor;
        }
    }
}
#endif
