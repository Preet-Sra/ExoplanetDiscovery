using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class PoolItem
    {
        public string prefabName;
        public GameObject[] prefabs; // Array of prefab variants
        public int poolSize = 10;
    }

    public List<PoolItem> poolItems;
    public static ObjectPooler instance;
    private Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, GameObject[]> prefabDictionary = new Dictionary<string, GameObject[]>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        foreach (PoolItem item in poolItems)
        {
            prefabDictionary.Add(item.prefabName, item.prefabs); // Store prefab variants in dictionary

            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject prefabToInstantiate = item.prefabs[Random.Range(0, item.prefabs.Length)]; // Pick random variant
                GameObject obj = Instantiate(prefabToInstantiate, transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(item.prefabName, objectPool);
        }
    }

    // Function to get a random variant of an object from the pool
    public GameObject GetObject(string prefabName)
    {
        if (poolDictionary.ContainsKey(prefabName))
        {
            foreach (GameObject obj in poolDictionary[prefabName])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // If no inactive objects are found, create a new one with a random prefab variant
            if (prefabDictionary.ContainsKey(prefabName))
            {
                GameObject[] variants = prefabDictionary[prefabName];
                GameObject newObj = Instantiate(variants[Random.Range(0, variants.Length)]); // Pick random variant
                poolDictionary[prefabName].Add(newObj);
                return newObj;
            }
            else
            {
                Debug.LogWarning("Prefab name not found in object pool: " + prefabName);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Prefab name not found in object pool: " + prefabName);
            return null;
        }
    }

    // Function to return the object back to the pool
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    // Function to get an object from a specific prefab variant array (if you need to)
    public GameObject GetObjectFromVariant(string prefabName, int variantIndex)
    {
        if (poolDictionary.ContainsKey(prefabName))
        {
            foreach (GameObject obj in poolDictionary[prefabName])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // If no inactive objects are found, create a new one using a specific variant
            if (prefabDictionary.ContainsKey(prefabName) && variantIndex < prefabDictionary[prefabName].Length)
            {
                GameObject newObj = Instantiate(prefabDictionary[prefabName][variantIndex]);
                poolDictionary[prefabName].Add(newObj);
                return newObj;
            }
            else
            {
                Debug.LogWarning("Variant index out of range or prefab name not found in object pool: " + prefabName);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Prefab name not found in object pool: " + prefabName);
            return null;
        }
    }
}
