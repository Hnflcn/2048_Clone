using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Pool : MonoBehaviour
    {
        public Transform parent;
        public static Pool Current;
        public int pooledAmount = 2;
        private bool _willGrow = true;

        public List<GameObject> prefabs;

        public List<List<GameObject>> PooledObjects;

        private void Awake()
        {
            Current = this;
            PooledObjects = new List<List<GameObject>>();
            for(var k = 0; k < prefabs.Count; k++)
            {
                PooledObjects.Add(new List<GameObject>());

                for(var i = 0; i < pooledAmount; i++)
                {
                    var obj = Instantiate(prefabs[k], parent, true);
                    obj.SetActive(false);
                    PooledObjects[k].Add(obj);
                }
            }
        }


        public GameObject GetPooledObject(int k)
        {
            for(var i = 0; i < PooledObjects[k].Count; i++)
            {
                if(!PooledObjects[k][i].activeInHierarchy)
                {
                    return PooledObjects[k][i];
                }
            }

            if (!_willGrow) return null;
            var obj = Instantiate(prefabs[k], parent, true);
            PooledObjects[k].Add(obj);
            return obj;

        }

    }
}