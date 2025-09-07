using TMPro;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public Player partyAPlayer;
    public Player partyBPlayer;
    public Transform partyASpawnPoint;
    public Transform partyBSpawnPoint;
    public float gameTimeMinutes = 5f; // 游戏时长（分钟）
    public TMP_Text timerText;        // 倒计时显示文本
    public GameOverUI gameOverUI;     // 游戏结束界面引用

    private float _remainingTime;
    private bool _isGameOver = false;
    private static BattleSystem _instance;
    public static BattleSystem Instance => _instance;

    [Header("战场设置")]
    public BattlefieldArea battlefieldArea = new BattlefieldArea();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始化倒计时
        _remainingTime = gameTimeMinutes * 60f;

        // 检查必要引用
        if (gameOverUI == null)
        {
            Debug.LogError("请分配GameOverUI引用！");
        }

        if (partyAPlayer == null || partyBPlayer == null)
        {
            Debug.LogError("请分配双方玩家引用！");
        }
        else
        {
            // 初始化玩家（传入阵营，使用 Inspector 配置的数值）
            partyAPlayer.Initialize(CampType.PartyA);
            partyBPlayer.Initialize(CampType.PartyB);
        }
    }

    private void Update()
    {
        if (_isGameOver) return;

        // 更新倒计时
        UpdateTimer();

        // 检查游戏结束条件
        CheckGameOverConditions();
    }

    // 更新倒计时显示
    private void UpdateTimer()
    {
        if (_remainingTime <= 0)
        {
            _remainingTime = 0;
            return;
        }

        _remainingTime -= Time.deltaTime;

        // 更新UI显示
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(_remainingTime / 60);
            int seconds = Mathf.FloorToInt(_remainingTime % 60);
            timerText.text = $"时间: {minutes:00}:{seconds:00}";
        }
    }

    // 检查游戏结束条件
    public void CheckGameOverConditions()
    {
        // 条件1: 任何一方角色血量为0
        if (partyAPlayer != null && partyAPlayer.IsDead)
        {
            EndGame("B方胜利！A方角色已被击败！");
            return;
        }

        if (partyBPlayer != null && partyBPlayer.IsDead)
        {
            EndGame("A方胜利！B方角色已被击败！");
            return;
        }

        // 条件2: 倒计时结束，比较双方血量
        if (_remainingTime <= 0)
        {
            if (partyAPlayer == null || partyBPlayer == null) return;

            if (partyAPlayer.CurrentHealth < partyBPlayer.CurrentHealth)
            {
                EndGame("B方胜利！A方角色血量更低！");
            }
            else if (partyBPlayer.CurrentHealth < partyAPlayer.CurrentHealth)
            {
                EndGame("A方胜利！B方角色血量更低！");
            }
            else
            {
                EndGame("平局！双方角色血量相同！");
            }
        }
    }

    // 结束游戏
    public void EndGame(string resultMessage)
    {
        if (_isGameOver) return;

        _isGameOver = true;
        Debug.Log($"游戏结束: {resultMessage}");

        // 停止所有小兵行动
        StopAllSoldiers();

        // 显示游戏结束界面
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver(resultMessage);
        }
    }

    // 停止所有小兵行动
    private void StopAllSoldiers()
    {
        Soldier[] allSoldiers = FindObjectsOfType<Soldier>();
        foreach (var soldier in allSoldiers)
        {
            soldier.StopMovement();
        }
    }

    public void SpawnSoldier(SoldierType type, CampType camp, Vector3? offset = null)
    {
        if (IsBattleOver())
        {
            Debug.Log("战斗已结束，无法召唤小兵!");
            return;
        }

        Soldier soldier = SoldierPoolManager.Instance.GetSoldier(type, camp);
        if (soldier != null)
        {
            // 设置小兵生成位置（支持偏移量）
            Transform spawnPoint = camp == CampType.PartyA ? partyASpawnPoint : partyBSpawnPoint;
            Vector3 spawnPosition = spawnPoint.position;
            if (offset.HasValue)
            {
                spawnPosition += offset.Value;
            }

            // 确保生成位置在战场范围内
            spawnPosition = battlefieldArea.Clamp(spawnPosition);

            soldier.transform.position = spawnPosition;

            // 确保Y轴旋转为0（修正2D游戏旋转问题）
            Quaternion spawnRotation = soldier.transform.rotation;
            spawnRotation = Quaternion.Euler(0, 0, 0); // 完全重置旋转
            soldier.transform.rotation = spawnRotation;

            Debug.Log($"{camp} 召唤了 {type} 小兵!");
        }
    }

    public bool IsBattleOver()
    {
        return (partyAPlayer != null && partyAPlayer.IsDead) ||
               (partyBPlayer != null && partyBPlayer.IsDead);
    }

    public void OnLikeButtonPressed(CampType camp)
    {
        // 点赞召唤3×3队列的三组点赞小兵（由上到下生成）
        const int groupCount = 3;    // 3组队列
        const int rowCount = 3;      // 每组3行
        const int colCount = 3;      // 每行3列
        const float spacing = 1.5f;  // 单位间距

        // 计算阵营初始偏移（区分A/B方生成方向）
        float campOffset = camp == CampType.PartyA ? 0 : 5f; // 避免双方生成位置重叠

        for (int g = 0; g < groupCount; g++)
        {
            // 组与组之间的垂直间距（由上到下的方向）
            float groupYOffset = g * spacing * 4; // 每组间隔4个单位

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    // 计算单个小兵的位置偏移
                    Vector3 offset = new Vector3(
                        (c - 1) * spacing + campOffset,  // X轴：左右排列（居中）
                        groupYOffset + r * spacing,      // Y轴：由上到下（组内+组间偏移）
                        0                                // Z轴：前后排列（如需可调整）
                    );

                    // 生成带偏移的点赞小兵
                    SpawnSoldier(SoldierType.LikeSoldier, camp, offset);
                }
            }
        }

        Debug.Log($"{camp} 点赞了，召唤了3×3队列的三组点赞小兵!");
    }


    public void OnGiftButtonPressed(CampType camp, int giftIndex)
    {
        // 礼物按钮召唤对应类型的小兵 (1-8)
        if (giftIndex < 1 || giftIndex > 8)
        {
            Debug.LogError("无效的礼物索引，必须在1-8之间!");
            return;
        }

        SoldierType soldierType = (SoldierType)((int)SoldierType.GiftSoldier1 + giftIndex - 1);
        SpawnSoldier(soldierType, camp);
        Debug.Log($"{camp} 使用了礼物 {giftIndex}，召唤了对应小兵!");
    }

    public void OnCommentButtonPressed(CampType camp, string comment)
    {
        Debug.Log($"{camp} 评论: {comment}");
        // 可以在这里添加评论相关的游戏逻辑
    }

    // 在Scene视图中绘制Gizmos
    private void OnDrawGizmos()
    {
        // 绘制战场范围
        battlefieldArea.DrawGizmos();

        // 绘制生成点（2D风格）
        if (partyASpawnPoint != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.7f); // 半透明绿色
            DrawCircle(partyASpawnPoint.position, 0.5f, 32); // 使用自定义圆形绘制
        }

        if (partyBSpawnPoint != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.7f); // 半透明红色
            DrawCircle(partyBSpawnPoint.position, 0.5f, 32); // 使用自定义圆形绘制
        }
    }

    // 在Scene视图中选中物体时绘制额外Gizmos
    private void OnDrawGizmosSelected()
    {
        // 绘制战场中心点（2D）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            new Vector3(battlefieldArea.center.x, battlefieldArea.center.y, battlefieldArea.gizmoZPosition),
            0.3f
        );
    }

    // 自定义圆形绘制方法
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        // 保存当前矩阵
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // 设置矩阵以忽略Z轴（2D）
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);

        Vector3 from = new Vector3(radius, 0, 0);
        Vector3 to = Vector3.zero;

        // 绘制多边形来模拟圆形
        for (int i = 0; i < segments + 1; i++)
        {
            float angle = (i * 2.0f * Mathf.PI) / segments;
            to.x = radius * Mathf.Cos(angle);
            to.y = radius * Mathf.Sin(angle);

            Gizmos.DrawLine(from, to);
            from = to;
        }

        // 恢复原始矩阵
        Gizmos.matrix = originalMatrix;
    }
}

