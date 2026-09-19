namespace TeaSpoons.PackageCore
{
    using UnityEngine;

    [System.Serializable]
    public class ObjectAmount<T> where T : Object
    {
        public int Amount;
        public T Obj;

        public ObjectAmount(int amount, T obj)
        {
            this.Amount = amount;
            this.Obj = obj;
        }
    }
}
