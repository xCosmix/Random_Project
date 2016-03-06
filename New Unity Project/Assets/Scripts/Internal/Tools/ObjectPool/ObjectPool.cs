using UnityEngine;
using System.Collections.Generic;
namespace ObjectPool
{

    public static class Pool 
    {
        private class Object
        {
            public Object (GameObject reference, int averageInstances)
            {
                this.reference = reference;
                this.averageInstances = averageInstances;

                CreateInstances();
            }
            private void CreateInstances ()
            {
                instances = new GameObject[averageInstances];
                //instances[0] = reference;

                for (int i = 0; i < instances.Length; i++)
                {        
                    GameObject go = (GameObject.Instantiate(reference, spawnPosition, reference.transform.rotation) as GameObject);
                    instances[i] = go;
                    instances[i].transform.SetParent(poolGameObject.transform);
                    go.SetActive(false);
                }
            }

            public GameObject reference;
            public int averageInstances;

            public GameObject[] instances;
        }

        private static Vector3 spawnPosition = Vector3.one * -10000.0f;
        private static Dictionary <string, Object> objects = new Dictionary<string, Object>();
        private static GameObject poolGameObject;

        public static void Submit (GameObject gameObject, int averageInstances)
        {
            if (poolGameObject == null) { poolGameObject = new GameObject("Pool"); poolGameObject.hideFlags = HideFlags.HideInHierarchy; }

            if (objects.ContainsKey(gameObject.name)) return;
            objects.Add(gameObject.name, new Object(gameObject, averageInstances));
        }

        public static GameObject New (GameObject reference, Vector3 position, Quaternion rotation, int averageInstances = 10)
        {
            if (!objects.ContainsKey(reference.name))
            {
                Submit(reference, averageInstances);
            }

            Object target = objects[reference.name];
            for (int i = 0; i < target.instances.Length; i++)
            {
                if (!target.instances[i].activeSelf)
                {
                    GameObject instance = target.instances[i];
                    instance.SetActive(true);

                    instance.transform.SetParent(null);
                    instance.transform.rotation = rotation;
                    instance.transform.position = position;
                    return instance;
                }
            }
            Debug.LogWarning("Not enough instances!");
            return null;
        }
        public static void Destroy (GameObject instance)
        {
            instance.transform.position = spawnPosition;
            instance.transform.SetParent(poolGameObject.transform);
            instance.SetActive(false);
        }
    }
}