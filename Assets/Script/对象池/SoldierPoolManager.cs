using System.Collections.Generic;
using UnityEngine;

public class SoldierPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class SoldierPoolConfig
    {
        public SoldierType type;
        public SoldierData data;
        public int initialPoolSize = 5;
        public int maxPoolSize = 20;
    }

    // 配置每种小兵的对象池参数
    public List<SoldierPoolConfig> poolConfigs;
    private Dictionary<SoldierType, ObjectPool<Soldier>> _pools = new Dictionary<SoldierType, ObjectPool<Soldier>>();
    private Transform _poolParent;

    private static SoldierPoolManager _instance;
    public static SoldierPoolManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 创建一个父对象来管理所有池对象
        _poolParent = new GameObject("SoldierPools").transform;
        _poolParent.parent = transform;

        // 初始化所有小兵对象池
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in poolConfigs)
        {
            if (config.data == null || config.data.prefab == null)
            {
                Debug.LogError($"小兵类型 {config.type} 没有指定数据或预制体!");
                continue;
            }

            Soldier prefab = config.data.prefab.GetComponent<Soldier>();
            if (prefab == null)
            {
                Debug.LogError($"小兵预制体 {config.data.prefab.name} 没有挂载 Soldier 组件!");
                continue;
            }

            // 创建该类型小兵的专用对象池
            var pool = new ObjectPool<Soldier>(
                prefab,
                _poolParent,
                config.initialPoolSize,
                config.maxPoolSize
            );

            _pools[config.type] = pool;
            Debug.Log($"初始化 {config.type} 对象池: 初始容量 {config.initialPoolSize}, 最大容量 {config.maxPoolSize}");
        }

        // 验证是否所有小兵类型都有对应的对象池配置
        ValidateAllSoldierTypesHavePool();
    }

    private void ValidateAllSoldierTypesHavePool()
    {
        foreach (SoldierType type in System.Enum.GetValues(typeof(SoldierType)))
        {
            if (!_pools.ContainsKey(type))
            {
                Debug.LogWarning($"小兵类型 {type} 没有配置对象池，将使用默认配置!");

                // 为未配置的类型创建默认对象池
                CreateDefaultPoolForType(type);
            }
        }
    }

    private void CreateDefaultPoolForType(SoldierType type)
    {
        // 为点赞小兵设置大对象池，礼物小兵设置小对象池
        int initialSize = type == SoldierType.LikeSoldier ? 500 : 10;
        int maxSize = type == SoldierType.LikeSoldier ? 2000 : 20;

        // 这里需要确保有默认的预制体，实际项目中应该在编辑器中配置
        // 这里只是演示逻辑
        Debug.LogWarning($"使用默认配置为 {type} 创建对象池: 初始容量 {initialSize}, 最大容量 {maxSize}");
    }

    public Soldier GetSoldier(SoldierType type, CampType camp)
    {
        if (_pools.TryGetValue(type, out var pool))
        {
            Soldier soldier = pool.Get();
            if (soldier != null)
            {
                soldier.Initialize(camp, type, GetSoldierData(type));
                return soldier;
            }
            else
            {
                Debug.LogWarning($"无法从 {type} 对象池获取小兵，可能已达到最大容量 {pool.TotalCount}/{pool.TotalCount}");
            }
        }
        else
        {
            Debug.LogError($"没有找到类型为 {type} 的小兵对象池!");
        }

        return null;
    }

    public void ReleaseSoldier(Soldier soldier)
    {
        if (soldier == null) return;

        if (_pools.TryGetValue(soldier.SoldierType, out var pool))
        {
            soldier.Reset();
            pool.Release(soldier);
        }
        else
        {
            Debug.LogError($"没有找到类型为 {soldier.SoldierType} 的小兵对象池，无法回收!");
            Destroy(soldier.gameObject);
        }
    }

    public SoldierData GetSoldierData(SoldierType type)
    {
        foreach (var config in poolConfigs)
        {
            if (config.type == type)
            {
                return config.data;
            }
        }
        return null;
    }

    // 打印所有对象池状态
    public void LogPoolStatus()
    {
        foreach (var kvp in _pools)
        {
            Debug.Log($"{kvp.Key} 对象池状态: 活跃 {kvp.Value.ActiveCount}, 闲置 {kvp.Value.InactiveCount}, 总容量 {kvp.Value.TotalCount}");
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}


