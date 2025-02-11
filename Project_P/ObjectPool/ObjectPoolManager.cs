using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public struct PoolParams
{
    public string Tag;
    public GameObject Owner;
    public Transform Parent;

    public Vector3 Position;
    public Quaternion Rotation;
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private Transform _poolParent;
    private readonly Dictionary<string, Pool> _poolMap = new();

    private const string PoolSystem = "PoolSystem_";
    public override void Initialize()
    {
        base.Initialize();

        _poolMap.Clear();
        GameObject NewPoolTr = new GameObject($"{PoolSystem}{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        NewPoolTr.transform.position = Vector3.zero;
        _poolParent = NewPoolTr.transform;
    }

    public PoolObjectBase GetFromPool(PoolParams poolParams)
    {
        if (!_poolMap.ContainsKey(poolParams.Tag)) CreatePool(poolParams.Tag);

        PoolObjectBase poolObject = _poolMap[poolParams.Tag].Pop(poolParams);
        if (poolObject == null) return null;
        else return poolObject;
    }

    private void CreatePool(string key)
    {
        var poolObject = DataManager.Instance.GetPoolObject(key);
        _poolMap.Add(key, new Pool(key, _poolParent, poolObject));
    }
}