
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace TeaSpoons.PackageCore
{
    /// <summary>
    /// Simplifies adding custom entries to the PlayerLoop system.
    /// </summary>
    public static class PlayerLoopUtility
    {
        #region Paths
        public abstract class Path : IEnumerable<Type>
        {
            public abstract IEnumerator<Type> GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public sealed class Path<T0> : Path
        {
            public override IEnumerator<Type> GetEnumerator()
            {
                yield return typeof(T0);
            }
        }

        public sealed class Path<T0, T1> : Path
        {
            public override IEnumerator<Type> GetEnumerator()
            {
                yield return typeof(T0);
                yield return typeof(T1);
            }
        }

        public sealed class Path<T0, T1, T2> : Path
        {
            public override IEnumerator<Type> GetEnumerator()
            {
                yield return typeof(T0);
                yield return typeof(T1);
                yield return typeof(T2);
            }
        }
        #endregion

        #region Positioning
        public interface IPositioning
        {
            internal abstract void Apply(ref PlayerLoopSystem[] subSystemList, PlayerLoopSystem newSystem);
        }

        public readonly struct Before<T> : IPositioning
        {
            void IPositioning.Apply(ref PlayerLoopSystem[] subSystemList, PlayerLoopSystem newSystem)
            {
                for (var index = 0; index < subSystemList.Length; index++)
                {
                    if (subSystemList[index].type == typeof(T))
                    {
                        InsertIntoArray(ref subSystemList, index, newSystem);
                        return;
                    }
                }

                throw new Exception($"PlayerLoopSystem to insert before was not found: {typeof(T).Name}");
            }
        }

        public readonly struct After<T> : IPositioning
        {
            void IPositioning.Apply(ref PlayerLoopSystem[] subSystemList, PlayerLoopSystem newSystem)
            {
                for (var index = 0; index < subSystemList.Length; index++)
                {
                    if (subSystemList[index].type == typeof(T))
                    {
                        InsertIntoArray(ref subSystemList, index + 1, newSystem);
                        return;
                    }
                }

                throw new Exception($"PlayerLoopSystem to insert after was not found: {typeof(T).Name}");
            }
        }

        public readonly struct Append : IPositioning
        {
            void IPositioning.Apply(ref PlayerLoopSystem[] subSystemList, PlayerLoopSystem newSystem)
            {
                Array.Resize(ref subSystemList, subSystemList.Length + 1);
                subSystemList[subSystemList.Length - 1] = newSystem;
            }
        }
        #endregion

        /// <summary>
        /// Adds the given <paramref name="newSystem"/> to the current PlayerLoop.
        /// </summary>
        /// <param name="parentPath">The path to the PlayerLoopSystem that will house the <paramref name="newSystem"/> as parent.</param>
        /// <param name="positioning">The positioning within the subsystem list of the parent defined by the <paramref name="parentPath"/>.</param>
        /// <exception cref="Exception">Thrown when the path is cannot be followed within the current PlayerLoop.</exception>
        /// <example>
        /// // shortens the following code
        /// using static TeaSpoons.PackageCore.PlayerLoopUtility;
        /// 
        /// // you need some type as an identifier
        /// private struct MySystem { }
        /// 
        /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        /// private static void Initialize()
        /// {
        ///     var newSystem = new PlayerLoopSystem
        ///     {
        ///         updateDelegate = MyCallback,
        ///         type = typeof(MySystem)
        ///     };
        ///     // Adds the new subsystem in the FixedUpdate step, before the MonoBehaviour FixedUpdate() broadcast.
        ///     AddSubSystem(new Path<FixedUpdate>(), new Before<FixedUpdate.ScriptRunBehaviourFixedUpdate>(), newSystem);
        /// 
        ///     // Adds the new subsystem anywhere in the Update phase.
        ///     AddSubSystem(new Path<Update>(), new Append(), newSystem);
        /// }
        /// </example>
        public static void AddPlayerLoopSystem(Path parentPath, IPositioning positioning, PlayerLoopSystem newSystem)
        {
            var rootSystem = PlayerLoop.GetCurrentPlayerLoop();

            // The subsystem list that contains the system that is being replaced
            var updatedSubsystemList = default(PlayerLoopSystem[]);
            // The index of the to-be-replaced system within its list
            var updatedSubSystemIndex = 0;
            // The system we're replacing because its subsystems will have changed
            var system = rootSystem;

            foreach (var pathType in parentPath)
            {
                updatedSubsystemList = null;
                for (updatedSubSystemIndex = 0; updatedSubSystemIndex < system.subSystemList.Length; updatedSubSystemIndex++)
                {
                    var subSystem = system.subSystemList[updatedSubSystemIndex];
                    if (subSystem.type == pathType)
                    {
                        updatedSubsystemList = system.subSystemList;
                        system = subSystem;
                        break;
                    }
                }

                if (updatedSubsystemList == null)
                {
                    break;
                }
            }

            if (updatedSubsystemList == null)
            {
                throw new Exception("Unable to traverse the given path in the current PlayerLoop system.");
            }

            positioning.Apply(ref system.subSystemList, newSystem);
            updatedSubsystemList[updatedSubSystemIndex] = system;

            PlayerLoop.SetPlayerLoop(rootSystem);
        }

        private static void InsertIntoArray<T>(ref T[] array, int index, T element)
        {
            if (array == null)
            {
                array = new T[] { element };
            }
            else
            {
                var list = new List<T>(array);
                list.Insert(index, element);
                array = list.ToArray();
            }
        }
    }
}
