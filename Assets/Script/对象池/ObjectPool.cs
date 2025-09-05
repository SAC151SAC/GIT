using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private T _prefab;
    private Transform _parent;
    private Queue<T> _pool = new Queue<T>();
    private int _initialSize;
    private int _maxSize;
    private int _activeCount = 0;

    public int ActiveCount => _activeCount;
    public int InactiveCount => _pool.Count;
    public int TotalCount => ActiveCount + InactiveCount;

    public ObjectPool(T prefab, Transform parent, int initialSize, int maxSize = int.MaxValue)
    {
        _prefab = prefab;
        _parent = parent;
        _initialSize = initialSize;
        _maxSize = maxSize;

        // 预初始化对象池
        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNewObject();
            _pool.Enqueue(obj);
        }
    }

    private T CreateNewObject()
    {
        T obj = GameObject.Instantiate(_prefab, _parent);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public T Get()
    {
        T obj;

        // 如果池中有可用对象，直接取出
        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        // 如果没有可用对象且未达到最大容量，创建新对象
        else if (TotalCount < _maxSize)
        {
            obj = CreateNewObject();
        }
        // 达到最大容量，无法创建新对象
        else
        {
            Debug.LogWarning($"对象池已达到最大容量 {_maxSize}，无法获取新对象！");
            return null;
        }

        _activeCount++;
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        if (obj == null) return;

        // 如果已经达到最大容量，直接销毁而不是放回池
        if (TotalCount >= _maxSize)
        {
            GameObject.Destroy(obj.gameObject);
        }
        else
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parent);
            _pool.Enqueue(obj);
        }

        _activeCount--;
        if (_activeCount < 0) _activeCount = 0;
    }

    // 清理超出初始容量的对象
    public void TrimExcess()
    {
        while (_pool.Count > _initialSize)
        {
            T obj = _pool.Dequeue();
            GameObject.Destroy(obj.gameObject);
        }
    }
}

