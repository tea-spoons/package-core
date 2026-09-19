
namespace TeaSpoons.PackageCore.Editor.Tests
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class SerializedPropertyExtensionsTest
    {
        private class TestObject : ScriptableObject
        {
            [System.Serializable]
            public struct Thing
            {
                public Vector2 Vector;
            }

            public Vector2 Vector;

            public Vector2[] VectorArray;

            public List<Vector2> VectorList;

            [SerializeField, Tooltip("A thing.")]
            private Thing thing;
            public Thing TheThing => thing;
            public Vector2 ThingVector
            {
                get => thing.Vector;
                set => thing.Vector = value;
            }

            [SerializeField]
            private Thing[] thingArray;
            public Thing FirstThing => thingArray[0];
            public Vector2 FirstThingVector
            {
                get => thingArray[0].Vector;
                set
                {
                    thingArray = new Thing[1];
                    thingArray[0].Vector = value;
                }
            }
        }

        private TestObject testObject;
        private SerializedObject serializedObject;

        [SetUp]
        public void SetUp()
        {
            testObject = ScriptableObject.CreateInstance<TestObject>();
            RefreshSerializedObject();
        }

        [TearDown]
        public void TearDown()
        {
            serializedObject?.Dispose();
            Object.DestroyImmediate(testObject);
        }

        #region Object Access
        [Test]
        public void DirectObjectAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.Vector = newVector;

            var property = serializedObject.FindProperty("Vector");
            Assert.IsTrue(property.TryGetTargetObject<Vector2>(out var foundVector));

            Assert.AreEqual(newVector, foundVector);
        }

        [Test]
        public void ObjectInStructAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.ThingVector = newVector;

            var property = serializedObject.FindProperty("thing.Vector");
            Assert.IsTrue(property.TryGetTargetObject<Vector2>(out var foundVector));

            Assert.AreEqual(newVector, foundVector);
        }

        [Test]
        public void ObjectInArrayAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.VectorArray = new Vector2[] { newVector };

            RefreshSerializedObject();

            var property = serializedObject.FindProperty("VectorArray").GetArrayElementAtIndex(0);
            Assert.IsTrue(property.TryGetTargetObject<Vector2>(out var foundVector));

            Assert.AreEqual(newVector, foundVector);
        }

        [Test]
        public void ObjectInListAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.VectorList = new List<Vector2>() { newVector };

            RefreshSerializedObject();

            var property = serializedObject.FindProperty("VectorList").GetArrayElementAtIndex(0);
            Assert.IsTrue(property.TryGetTargetObject<Vector2>(out var foundVector));

            Assert.AreEqual(newVector, foundVector);
        }

        [Test]
        public void ObjectInStructListAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.FirstThingVector = newVector;

            RefreshSerializedObject();

            var property = serializedObject.FindProperty("thingArray");
            property = property.GetArrayElementAtIndex(0);
            property = property.FindPropertyRelative("Vector");
            Assert.IsTrue(property.TryGetTargetObject<Vector2>(out var foundVector));

            Assert.AreEqual(newVector, foundVector);
        }
        #endregion

        #region Field Access
        [Test]
        public void DirectFieldAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.Vector = newVector;

            var property = serializedObject.FindProperty("Vector");
            Assert.IsTrue(property.TryGetField(out var field));

            Assert.AreEqual(newVector, field.GetValue(testObject));
        }

        [Test]
        public void DirectFieldAttributeAccess()
        {
            var property = serializedObject.FindProperty("thing");

            Assert.IsFalse(property.TryGetAttribute<MultilineAttribute>(out _));

            Assert.IsTrue(property.TryGetAttribute<TooltipAttribute>(out var attribute));
            Assert.AreEqual("A thing.", attribute.tooltip);
        }

        [Test]
        public void FieldInStructAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.ThingVector = newVector;

            RefreshSerializedObject();

            var property = serializedObject.FindProperty("thing.Vector");
            Assert.IsTrue(property.TryGetField(out var field));

            Assert.AreEqual(newVector, field.GetValue(testObject.TheThing));
        }

        [Test]
        public void FieldInArrayAccessDoesntWork()
        {
            testObject.VectorArray = new Vector2[1];

            RefreshSerializedObject();

            var property = serializedObject.FindProperty("VectorArray").GetArrayElementAtIndex(0);
            Assert.IsFalse(property.TryGetField(out _));
        }

        [Test]
        public void FieldInStructListAccess()
        {
            var newVector = new Vector2(12.34f, 56.78f);
            testObject.FirstThingVector = newVector;

            RefreshSerializedObject();

            var property = serializedObject.FindProperty("thingArray");
            property = property.GetArrayElementAtIndex(0);
            property = property.FindPropertyRelative("Vector");
            Assert.IsTrue(property.TryGetField(out var field));

            Assert.AreEqual(newVector, field.GetValue(testObject.FirstThing));
        }
        #endregion

        private void RefreshSerializedObject()
        {
            serializedObject = new SerializedObject(testObject);
        }
    }
}
