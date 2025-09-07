using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class Soldier : BattleUnit
{
    public SoldierType SoldierType { get; private set; }
    private SoldierData _soldierData;
    private Transform _target;
    private float _lastAttackTime;
    private Vector3 _targetPosition;
    private bool _isMoving = false;

    // 多线程相关
    private CancellationTokenSource _cts;
    private Task _pathfindingTask;

    // 添加动画组件引用
    private Animator _animator;

    // 新增：显示在Inspector的配置信息（仅用于查看）
    [Header("配置信息（只读）")]
    [ReadOnly] public float configAttackRange;
    [ReadOnly] public string configUnitName;
    [ReadOnly] public int configMaxHealth;
    [ReadOnly] public int configAttackPower;
    [ReadOnly] public float configMoveSpeed;
    [ReadOnly] public float configAttackInterval;



    public void Initialize(CampType camp, SoldierType type, SoldierData data)
    {
        base.Initialize(camp, data);
        SoldierType = type;
        _soldierData = data;
        _animator = GetComponent<Animator>(); // 获取动画组件

        // 设置初始目标为敌方玩家
        SetTargetPlayer();

        // 新增：同步配置信息到显示字段
        SyncConfigData(data);
    }

    // 新增：同步配置数据到显示字段
    private void SyncConfigData(SoldierData data)
    {
        configAttackRange = data.attackRange;
        configUnitName = data.unitName;
        configMaxHealth = data.maxHealth;
        configAttackPower = data.attackPower;
        configMoveSpeed = data.moveSpeed;
        configAttackInterval = data.attackInterval;
    }

    // 新增方法：停止移动
    public void StopMovement()
    {
        _isMoving = false;
        _target = null;

        // 取消路径计算任务
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private void Update()
    {
        if (_isDead) return;

        // 更新动画参数
        _animator.SetBool("IsMoving", _isMoving);
        _animator.SetBool("IsAttacking", !_isMoving && _target != null &&
            Vector3.Distance(transform.position, _target.position) <= _data.attackRange);
        _animator.SetBool("IsDead", _isDead);

        // 如果没有目标，寻找目标
        if (_target == null || _target.GetComponent<BattleUnit>().IsDead)
        {
            FindTarget();
            return;
        }

        // 移动到目标
        if (Vector3.Distance(transform.position, _target.position) > _data.attackRange)
        {
            MoveTowardsTarget();
        }
        // 攻击目标
        else
        {
            _isMoving = false;
            if (Time.time - _lastAttackTime >= _data.attackInterval)
            {
                AttackTarget();
                _lastAttackTime = Time.time;
            }
        }
    }

    private void FindTarget()
    {
        // 优先寻找敌方小兵
        Soldier[] enemySoldiers = FindObjectsOfType<Soldier>();
        float closestDistance = Mathf.Infinity;
        Soldier closestSoldier = null;

        foreach (var soldier in enemySoldiers)
        {
            if (soldier.Camp != _camp && !soldier.IsDead)
            {
                float distance = Vector3.Distance(transform.position, soldier.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSoldier = soldier;
                }
            }
        }

        // 如果有敌方小兵，优先攻击
        if (closestSoldier != null)
        {
            _target = closestSoldier.transform;
        }
        else
        {
            // 否则攻击敌方玩家
            SetTargetPlayer();
        }
    }

    private void SetTargetPlayer()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player.Camp != _camp && !player.IsDead)
            {
                _target = player.transform;
                break;
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (_target == null) return;

        _isMoving = true;

        // 使用多线程计算2D路径
        if (_pathfindingTask == null || _pathfindingTask.IsCompleted)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            // 传递2D位置（忽略Z轴）
            _pathfindingTask = CalculatePathAsync(
                new Vector3(_target.position.x, _target.position.y, transform.position.z),
                _cts.Token
            );
        }

        // 计算移动后的位置（仅X和Y轴）
        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            new Vector3(_targetPosition.x, _targetPosition.y, transform.position.z), // 保持Z轴不变
            _data.moveSpeed * Time.deltaTime
        );

        // 限制位置在2D战场范围内
        if (BattleSystem.Instance != null)
        {
            newPosition = BattleSystem.Instance.battlefieldArea.Clamp(newPosition);
        }

        transform.position = newPosition;

        // 2D朝向目标（只旋转Z轴）
        Vector2 direction = new Vector2(
            _targetPosition.x - transform.position.x,
            _targetPosition.y - transform.position.y
        ).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private async Task CalculatePathAsync(Vector3 targetPos, CancellationToken cancellationToken)
    {
        // 模拟路径计算延迟
        await Task.Delay(100, cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
        {
            // 在主线程中更新目标位置
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                _targetPosition = targetPos;
            });
        }
    }

    private void AttackTarget()
    {
        BattleUnit targetUnit = _target.GetComponent<BattleUnit>();
        if (targetUnit != null && !targetUnit.IsDead)
        {
            targetUnit.TakeDamage(_data.attackPower);
            Debug.Log($"{_data.unitName} 攻击了 {targetUnit.gameObject.name}，造成 {_data.attackPower} 点伤害!");
        }
    }

    // 重写时保持protected访问修饰符
    protected override void Die()
    {
        base.Die();
        _isMoving = false;
        _animator.SetBool("IsDead", true); // 触发死亡动画

        // 取消路径计算任务
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        // 延迟回收，让死亡动画完成
        Invoke(nameof(ReleaseSelf), 1.0f);
    }

    private void ReleaseSelf()
    {
        SoldierPoolManager.Instance.ReleaseSoldier(this);
    }

    public override void Reset()
    {
        base.Reset();
        _target = null;
        _lastAttackTime = 0;
        _isMoving = false;

        // 取消路径计算任务
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        _pathfindingTask = null;
    }

    private void OnDestroy()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    // 绘制攻击范围Gizmos
    private void OnDrawGizmosSelected()
    {
        // 增加空值检查，防止预制体未初始化时出错
        if (_data == null) return;

        // 绘制攻击范围（圆形）
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, _data.attackRange);
    }

#if UNITY_EDITOR
    // 编辑器模式下手动同步配置数据（仅用于预览）
    [ContextMenu("Sync Config Data From Pool Manager")]
    public void SyncConfigFromPoolManager()
    {
        // 查找场景中的SoldierPoolManager
        var poolManager = UnityEngine.Object.FindObjectOfType<SoldierPoolManager>();
        if (poolManager == null)
        {
            Debug.LogError("未在场景中找到SoldierPoolManager");
            return;
        }

        // 尝试匹配当前预制体对应的配置（需手动指定类型和阵营用于预览）
        // 注意：这里需要手动设置类型和阵营，仅用于编辑器预览
        SoldierType previewType = SoldierType.LikeSoldier; // 手动修改为目标类型
        CampType previewCamp = CampType.PartyA; // 手动修改为目标阵营

        var data = poolManager.GetSoldierData(previewType, previewCamp);
        if (data != null)
        {
            SyncConfigData(data); // 同步到显示字段
            UnityEditor.EditorUtility.SetDirty(this); // 标记为脏数据，保存修改
        }
        else
        {
            Debug.LogError($"未找到 {previewCamp} 阵营 {previewType} 的配置数据");
        }
    }
#endif
}




