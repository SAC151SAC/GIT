using System;
using System.Collections.Generic;
using UnityEngine;

public class SoldierPoolManager : MonoBehaviour
{
    // 在SoldierPoolManager.cs中修改配置类：
    [System.Serializable]
    public class SoldierPoolConfig
    {
        public SoldierType type;         // 小兵类型（点赞/礼物）
        public CampType camp;            // 新增：所属阵营
        public SoldierData data;         // 小兵数据（包含预制体）
        public int initialPoolSize;  // 初始池大小
        public int maxPoolSize;     // 最大池大小
    }

    // 配置每种小兵的对象池参数
    public List<SoldierPoolConfig> poolConfigs;// 小兵对象池配置                                               
    private Dictionary<SoldierKey, ObjectPool<Soldier>> _pools = new Dictionary<SoldierKey, ObjectPool<Soldier>>();
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
                Debug.LogError($"阵营{config.camp}的{config.type}未配置数据或预制体!");
                continue;
            }

            Soldier prefab = config.data.prefab.GetComponent<Soldier>();
            if (prefab == null)
            {
                Debug.LogError($"预制体{config.data.prefab.name}未挂载Soldier组件!");
                continue;
            }

            // 用类型+阵营作为键创建对象池
            var key = new SoldierKey(config.type, config.camp);
            var pool = new ObjectPool<Soldier>(
                prefab,
                _poolParent,
                config.initialPoolSize,
                config.maxPoolSize
            );

            _pools[key] = pool;
            Debug.Log($"初始化 {config.camp}的{config.type} 对象池: 初始{config.initialPoolSize}, 最大{config.maxPoolSize}");
        }

        // 验证所有组合是否都有配置（可选）
        ValidateAllSoldierCombinations();
    }

    // 验证所有类型+阵营的组合是否都有配置
    private void ValidateAllSoldierCombinations()
    {
        foreach (SoldierType type in Enum.GetValues(typeof(SoldierType)))
        {
            foreach (CampType camp in Enum.GetValues(typeof(CampType)))
            {
                var key = new SoldierKey(type, camp);
                if (!_pools.ContainsKey(key))
                {
                    Debug.LogWarning($"阵营{camp}的{type}未配置对象池，使用默认配置!");
                    CreateDefaultPoolForType(type, camp);
                }
            }
        }
    }

    // 创建默认池（按阵营+类型）
    private void CreateDefaultPoolForType(SoldierType type, CampType camp)
    {
        int initialSize = type == SoldierType.LikeSoldier ? 500 : 10;
        int maxSize = type == SoldierType.LikeSoldier ? 2000 : 20;
        var key = new SoldierKey(type, camp);
        // 注意：实际项目中需要为默认池指定预制体
        Debug.LogWarning($"为阵营{camp}的{type}创建默认池: 初始{initialSize}, 最大{maxSize}");
    }

    //private void ValidateAllSoldierTypesHavePool()
    //{
    //    foreach (SoldierType type in System.Enum.GetValues(typeof(SoldierType)))
    //    {
    //        if (!_pools.ContainsKey(type))
    //        {
    //            Debug.LogWarning($"小兵类型 {type} 没有配置对象池，将使用默认配置!");

    //            // 为未配置的类型创建默认对象池
    //            CreateDefaultPoolForType(type);
    //        }
    //    }
    //}

    //private void CreateDefaultPoolForType(SoldierType type)
    //{
    //    // 为点赞小兵设置大对象池，礼物小兵设置小对象池
    //    int initialSize = type == SoldierType.LikeSoldier ? 500 : 10;
    //    int maxSize = type == SoldierType.LikeSoldier ? 2000 : 20;

    //    // 这里需要确保有默认的预制体，实际项目中应该在编辑器中配置
    //    // 这里只是演示逻辑
    //    Debug.LogWarning($"使用默认配置为 {type} 创建对象池: 初始容量 {initialSize}, 最大容量 {maxSize}");
    //}

    // 获取小兵：按类型+阵营
    public Soldier GetSoldier(SoldierType type, CampType camp)
    {
        var key = new SoldierKey(type, camp);
        if (_pools.TryGetValue(key, out var pool))
        {
            Soldier soldier = pool.Get();
            if (soldier != null)
            {
                soldier.Initialize(camp, type, GetSoldierData(type, camp));
                return soldier;
            }
        }
        Debug.LogError($"未找到阵营{camp}的{type}对象池!");
        return null;
    }

    // 回收小兵：根据小兵自身的类型和阵营
    public void ReleaseSoldier(Soldier soldier)
    {
        if (soldier == null) return;
        var key = new SoldierKey(soldier.SoldierType, soldier.Camp);
        if (_pools.TryGetValue(key, out var pool))
        {
            soldier.Reset();
            pool.Release(soldier);
        }
        else
        {
            Debug.LogError($"无法回收阵营{soldier.Camp}的{soldier.SoldierType}，未找到对应池!");
            Destroy(soldier.gameObject);
        }
    }

    // 获取小兵数据（按类型+阵营）
    public SoldierData GetSoldierData(SoldierType type, CampType camp)
    {
        return poolConfigs.Find(c => c.type == type && c.camp == camp)?.data;
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


