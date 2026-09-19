
namespace TeaSpoons.PackageCore
{
    using UnityEngine;

    /// <summary>
    /// A serializable class with a single value that triggers an event if the value is changed through the inspector.
    /// For updatable fields which reference objects that should be unsubscribed/unregistered from, this is more comfortable than using OnValidate.
    /// </summary>
    /// <typeparam name="T">The type of value being serialized.</typeparam>
    /// <example>
    /// [SerializeField]
    /// private PlayModeEditable<SomeGroup> group;
    /// 
    /// private void Awake()
    /// {
    ///     group.Value?.Add(this);
    /// #if UNITY_EDITOR
    ///     group.OnUpdate = (oldGroup, newGroup) =>
    ///     {
    ///         oldGroup?.Remove(this);
    ///         newGroup?.Add(this);
    ///     };
    /// #endif
    /// }
    /// 
    /// private void OnDestroy()
    /// {
    ///     group.Value?.Remove(this);
    /// #if UNITY_EDITOR
    ///     group.OnUpdate = null;
    /// #endif
    /// }
    /// </example>
    [System.Serializable]
    public sealed class PlayModeEditable<T>
    {
#if UNITY_EDITOR
        internal static class PropertyNames
        {
            public const string Value = nameof(value);
            public const string InvokeOnUpdateEvent = nameof(PlayModeEditable<object>.InvokeOnUpdateEvent);
        }
#endif

        public delegate void UpdateDelegate(T previousValue, T newValue);

        [SerializeField]
        private T value;
        public T Value => value;

#if UNITY_EDITOR
        public UpdateDelegate OnUpdate { get; set; }
#endif

        public PlayModeEditable(T value)
        {
            this.value = value;
        }

#if UNITY_EDITOR
        internal void InvokeOnUpdateEvent(object previousValue, object newValue)
        {
            OnUpdate?.Invoke((T)previousValue, (T)newValue);
        }
#endif

        public static implicit operator PlayModeEditable<T>(T value)
        {
            return new PlayModeEditable<T>(value);
        }

        public static implicit operator T(PlayModeEditable<T> editable)
        {
            return editable.value;
        }
    }
}
