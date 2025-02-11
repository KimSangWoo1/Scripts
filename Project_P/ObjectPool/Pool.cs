using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private string _key;
    private Queue<PoolObjectBase> _objectQueue;

    PoolObjectBase _prefab;
    Transform _objectParent;
    int _capacity;

    private static string POOL = "Pool";

    public Pool(string key, Transform parent, PoolObjectBase prefab, int capacity = 1)
    {
        _objectParent = new GameObject(key + POOL).transform;
        _objectParent.position = Vector3.zero;
        _objectParent.SetParent(parent);

        _key = key;
        _objectQueue = new Queue<PoolObjectBase>(capacity);

        _prefab = prefab;
        _capacity = capacity;
    }

    private void CreateNewObject()
    {
        if (_prefab == null)
        {
            Debug.LogError("[Pool] Create Prefab Null");
            return;
        }

        for (int i = 0; i < _capacity; i++)
        {
            var newObj = GameObject.Instantiate<PoolObjectBase>(_prefab, Vector3.zero, Quaternion.identity, _objectParent);
            newObj.Tag = _key;
            newObj.SetPool(this);
            newObj.gameObject.SetActive(false);
            AddToPool(newObj);
        }
    }

    public virtual PoolObjectBase Pop(PoolParams poolParams)
    {
        if (IsEmpty())
        {
            CreateNewObject();
        }

        PoolObjectBase poolObject = _objectQueue.Dequeue();

        if (poolObject != null)
        {
            poolObject.transform.position = poolParams.Position;
            poolObject.transform.rotation = poolParams.Rotation;
            poolObject.Owner = poolParams.Owner;

            poolObject.gameObject.SetActive(true);
            poolObject.PoolOn();
        }
        else
        {
            Debug.LogError("[Pool] Pop Object Null");
        }
        return poolObject;
    }

    public virtual void Push(PoolObjectBase poolObject)
    {
        if (poolObject.transform.parent.GetComponent<PoolObjectBase>() != null) return;

        poolObject.gameObject.SetActive(false);
        poolObject.SetParent(_objectParent);
        _objectQueue.Enqueue(poolObject);
    }

    private void AddToPool(PoolObjectBase poolObject)
    {
        if (poolObject == null)
        {
            Debug.LogError("Cannot add a null object to the pool.");
            return;
        }

        if (!_objectQueue.Contains(poolObject))
        {
            _objectQueue.Enqueue(poolObject);
        }
    }

    public int GetPoolSize()
    {
        return _objectQueue.Count;
    }

    public bool IsEmpty()
    {
        return _objectQueue.Count == 0;
    }
}