using System.Collections;
using UnityEngine;

public class Player : BattleUnit
{
    // 基础属性（与 BattleUnitData 对应，暴露在 Inspector 中）
    [Header("基础属性")]
    [SerializeField] private string unitName = "Player";
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int attackPower = 50;// 攻击力
    [SerializeField] private float moveSpeed = 2f;// 移动速度
    [SerializeField] private float attackRange = 10f;// 攻击范围

    // 角色特有属性
    [Header("角色特有属性")]
    [SerializeField] private int splashAttackRange = 5; // 范围攻击半径
    [SerializeField] private float attackInterval = 2f; // 攻击间隔

    private float _lastAttackTime;
    private PlayerData _runtimeData; // 运行时数据容器

    public int CurrentHealth => _currentHealth;

    private Animator _animator;

    // 初始化时使用 Inspector 配置的数值
    public void Initialize(CampType camp)
    {
        // 构建运行时数据，将 Inspector 配置的值传入
        _runtimeData = new PlayerData
        {
            unitName = unitName,
            maxHealth = maxHealth,
            attackPower = attackPower,
            moveSpeed = moveSpeed,
            attackRange = attackRange,
            attackInterval = attackInterval,
            splashAttackRange = splashAttackRange
        };

        // 调用父类初始化，传入构建好的数据
        base.Initialize(camp, _runtimeData);

        _animator = GetComponent<Animator>(); // 获取动画组件
    }

    private void Update()
    {
        if (_isDead) return;

        // 自动攻击范围内的敌方小兵（使用配置的攻击间隔）
        if (Time.time - _lastAttackTime >= attackInterval)
        {
            AttackNearbyEnemies();
            _lastAttackTime = Time.time;
        }
    }

    // 范围攻击附近的敌方小兵（使用配置的攻击范围和攻击力）
    private void AttackNearbyEnemies()
    {
        bool hasTarget = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, splashAttackRange);
        foreach (var collider in colliders)
        {
            Soldier enemySoldier = collider.GetComponent<Soldier>();
            if (enemySoldier != null && enemySoldier.Camp != _camp && !enemySoldier.IsDead)
            {
                enemySoldier.TakeDamage(_data.attackPower);
                hasTarget = true;
            }
        }

        // 只有当有目标被攻击时才触发攻击动画
        if (hasTarget)
        {
            _animator.SetBool("Attack", true); // 使用BOOL类型控制攻击动画
                                               // 可以在这里添加一个延迟后重置攻击状态的逻辑
            StartCoroutine(ResetAttackState());
        }
    }

    // 重置攻击状态的协程
    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f); // 等待攻击动画播放一段时间
        _animator.SetBool("Attack", false);
    }

    protected override void Die()
    {
        base.Die();

        _animator.SetBool("Die", true); // 触发死亡动画
        Debug.Log($"{_data.unitName} 已被击败!");

        if (BattleSystem.Instance != null)
        {
            BattleSystem.Instance.CheckGameOverConditions();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashAttackRange);
    }
}

