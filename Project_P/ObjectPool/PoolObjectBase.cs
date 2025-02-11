using System.Collections.Generic;
using UnityEngine;

public class PoolObjectBase : MonoBehaviour , IPoolable
{
    public string Tag { get; set; }

    public GameObject Owner { get; set; }

    protected Pool _pool;
    protected SpriteRenderer _spriteRenderer;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

    public virtual void Push()
    {
        if (_pool == null) Debug.LogError($"[Pool] Pool Null {this.name}");
        _pool.Push(this);
    }

    public void SetPool(Pool pool)
    {
        _pool = pool;
    }

    public virtual void PoolOn() { }

    public virtual void PoolOff() 
    {
        Push();
    }

    public virtual void SetParent(Transform parent)
    {
        this.transform.SetParent(parent);
    }
}

public interface IPoolable
{
    public void Push();
    public void PoolOn();
    public void PoolOff();
}