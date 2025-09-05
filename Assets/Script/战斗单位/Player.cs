using UnityEngine;

public class Player : BattleUnit
{
    [SerializeField] private int splashAttackRange = 5; // 范围攻击半径
    [SerializeField] private float attackInterval = 2f; // 攻击间隔
    private float _lastAttackTime;

    public int CurrentHealth => _currentHealth;

    public void Initialize(CampType camp, PlayerData data)
    {
        base.Initialize(camp, data);
    }

    private void Update()
    {
        if (_isDead) return;

        // 自动攻击范围内的敌方小兵
        if (Time.time - _lastAttackTime >= attackInterval)
        {
            AttackNearbyEnemies();
            _lastAttackTime = Time.time;
        }
    }

    // 范围攻击附近的敌方小兵
    private void AttackNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, splashAttackRange);
        foreach (var collider in colliders)
        {
            Soldier enemySoldier = collider.GetComponent<Soldier>();
            if (enemySoldier != null && enemySoldier.Camp != _camp && !enemySoldier.IsDead)
            {
                enemySoldier.TakeDamage(_data.attackPower);
                Debug.Log($"{_data.unitName} 攻击了 {enemySoldier.name}，造成 {_data.attackPower} 点伤害!");
            }
        }
    }

    protected override void Die()
    {
        base.Die();
        Debug.Log($"{_data.unitName} 已被击败!");

        // 通知战斗系统检查游戏结束
        if (BattleSystem.Instance != null)
        {
            BattleSystem.Instance.CheckGameOverConditions();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制攻击范围 gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashAttackRange);
    }
}

