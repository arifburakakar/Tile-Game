using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GenericObjectPool<T> where T : Component , IPoolable
{
    private readonly Stack<T> objects;
    private readonly HashSet<T> usedObjects;
    private T prefab;
    private Transform poolTransform;
    private int initialBufferSize;
    private int updateBufferSize = 5;
    
    public GenericObjectPool(T prefab, int initialBufferSize, Transform poolTransform)
    {
        objects = new Stack<T>(initialBufferSize);
        usedObjects = new HashSet<T>();
        this.prefab = prefab;
        this.poolTransform = poolTransform;
        this.initialBufferSize = initialBufferSize;
    }

    public void CreateInitialPoolObjects()
    {
        for (int i = 0; i < initialBufferSize; i++)
        {
            T newObject = Object.Instantiate(prefab);
            newObject.gameObject.SetActive(false);
            newObject.transform.SetParent(poolTransform);
            newObject.Create();
            objects.Push(newObject);
        }
    }

    public void UpdatePool()
    {
        for (int i = 0; i < updateBufferSize; i++)
        {
            T tempObj = Object.Instantiate(prefab);
            tempObj.gameObject.SetActive(false);
            tempObj.transform.SetParent(poolTransform);
            tempObj.Create();
            objects.Push(tempObj);
        }
    }
    
    public T Get()
    {
        if (objects.Count == 0)
        {
            UpdatePool();
        }

        T obj = objects.Pop();
        obj.gameObject.SetActive(true);
        obj.Spawn();
        usedObjects.Add(obj);
        return obj;
    }

    public void Release(T obj, bool clearAll = false)
    {
        if (!clearAll)
        {
            if (usedObjects.Remove(obj))
            {
                
            }
        }   

         
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(poolTransform);
        obj.OnDespawn();
        objects.Push(obj);
    }

    public void ReleaseAll()
    {
        List<T> tempUsedObjects = new List<T>(usedObjects);
        foreach (T obj in tempUsedObjects)
        {
            obj.Despawn();
        }
    
        usedObjects.Clear();
    }

    public void ClearAllPool()
    {
        foreach (T t in objects)
        {
            Object.Destroy(t);
        }
    }
}