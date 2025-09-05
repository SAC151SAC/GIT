using UnityEngine;

// 此脚本用于在编辑器中自动配置合理的对象池大小
[ExecuteInEditMode]
public class SoldierPoolConfiguration : MonoBehaviour
{
    [SerializeField] private SoldierPoolManager poolManager;

    private void Awake()
    {
        if (poolManager == null)
        {
            poolManager = FindObjectOfType<SoldierPoolManager>();
        }
    }

    // 在编辑器中点击此按钮自动配置合理的对象池大小
    [ContextMenu("Auto-Configure Pool Sizes")]
    public void AutoConfigurePoolSizes()
    {
        if (poolManager == null || poolManager.poolConfigs == null)
        {
            Debug.LogError("找不到有效的SoldierPoolManager或配置列表!");
            return;
        }

        // 清除现有配置
        poolManager.poolConfigs.Clear();

        // 为每种小兵类型配置合适的对象池大小
        foreach (SoldierType type in System.Enum.GetValues(typeof(SoldierType)))
        {
            SoldierPoolManager.SoldierPoolConfig config = new SoldierPoolManager.SoldierPoolConfig();
            config.type = type;

            // 为点赞小兵设置大对象池
            if (type == SoldierType.LikeSoldier)
            {
                config.initialPoolSize = 500;  // 初始500个
                config.maxPoolSize = 2000;     // 最大2000个
            }
            // 为礼物小兵设置小对象池
            else
            {
                config.initialPoolSize = 10;   // 初始10个
                config.maxPoolSize = 20;       // 最大20个
            }

            poolManager.poolConfigs.Add(config);
        }

        Debug.Log("已自动配置小兵对象池大小: 点赞小兵初始500/最大2000，礼物小兵初始10/最大20");
    }
}
