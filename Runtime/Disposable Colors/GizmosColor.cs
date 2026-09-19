
namespace TeaSpoons.PackageCore
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Changes <see cref="Gizmos.color"/> and reverts when it is disposed.
    /// </summary>
    /// <example>
    /// using (GizmosColor.Override(Color.red))
    /// {
    ///     Gizmos.DrawWireCube(position, size);
    /// }
    /// </example>
    public readonly struct GizmosColor : IDisposable
    {
        public static GizmosColor Override(Color color)
        {
            return new GizmosColor(color);
        }

        public static GizmosColor Multiply(Color color)
        {
            return new GizmosColor(Gizmos.color * color);
        }

        private readonly Color originalColor;

        private GizmosColor(Color color)
        {
            originalColor = Gizmos.color;
            Gizmos.color = color;
        }

        void IDisposable.Dispose()
        {
            Gizmos.color = originalColor;
        }
    }
}
