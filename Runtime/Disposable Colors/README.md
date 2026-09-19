# Disposable Colors
Simple disposable structs that you can use to make sure your GUI/Gizmos/Handles colors are properly reset.

## Example
```csharp
using (GUIColor.Override(Color.red))
{
    GUILayout.Box("Danger");
}
```