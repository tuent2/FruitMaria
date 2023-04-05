using System.Collections.Generic;
using dotmob.Scripts.LinqConstructor.AnyFieldInspector;
using UnityEngine;

namespace dotmob.Scripts.LinqConstructor
{
    [ExecuteInEditMode]
    public class Observer : MonoBehaviour
    {
        public List<AnyField> objectPath = new List<AnyField>();

        public Object obj;
        public object obj2;

        public static Observer THIS;

        void OnEnable()
        {
            if (THIS == null) THIS = this;
            else if (THIS != this)
                Destroy(gameObject);
        }
        public void StartObserve(GameObject g)
        {
            obj = g;
        }

        public void StartObserve(object g)
        {
            obj2 = g;
            objectPath.Clear();
            objectPath.Add(new AnyField(obj2, 0));
        }
    }
}


