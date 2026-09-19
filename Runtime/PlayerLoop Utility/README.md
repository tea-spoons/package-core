# PlayerLoop Utility
Simplifies updating Unity's PlayerLoop.

## Example
```csharp
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEngine;
using static TeaSpoons.PackageCore.PlayerLoopUtility;

public static class PlayerLoopUtilityExample
{
    // You need some type as an identifier for the system.
    private struct MySystem { }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        var newSystem = new PlayerLoopSystem
        {
            updateDelegate = MyCallback,
            type = typeof(MySystem)
        };

        // Example: Adds the new subsystem in the FixedUpdate step, before the MonoBehaviour FixedUpdate() broadcast.
        AddPlayerLoopSystem(new Path<FixedUpdate>(), new Before<FixedUpdate.ScriptRunBehaviourFixedUpdate>(), newSystem);

        // Example: Adds the new subsystem anywhere in the Update phase.
        AddPlayerLoopSystem(new Path<Update>(), new Append(), newSystem);
    }

    private static void MyCallback()
    {
        // ...
    }
}

```