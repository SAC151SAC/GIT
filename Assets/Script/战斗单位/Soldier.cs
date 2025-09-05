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

    public void Initialize(CampType camp, SoldierType type, SoldierData data)
    {
        base.Initialize(camp, data);
        SoldierType = type;
        _soldierData = data;

        // 设置初始目标为敌方玩家
        SetTargetPlayer();
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

        // 使用多线程计算路径
        if (_pathfindingTask == null || _pathfindingTask.IsCompleted)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _pathfindingTask = CalculatePathAsync(_target.position, _cts.Token);
        }

        // 移动到计算好的目标位置
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _data.moveSpeed * Time.deltaTime);
        transform.LookAt(new Vector3(_targetPosition.x, transform.position.y, _targetPosition.z));
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
}


