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

    public void SpawnSoldier(SoldierType type, CampType camp)
    {
        if (IsBattleOver())
        {
            Debug.Log("战斗已结束，无法召唤小兵!");
            return;
        }

        Soldier soldier = SoldierPoolManager.Instance.GetSoldier(type, camp);
        if (soldier != null)
        {
            // 设置小兵生成位置
            Transform spawnPoint = camp == CampType.PartyA ? partyASpawnPoint : partyBSpawnPoint;
            soldier.transform.position = spawnPoint.position;
            soldier.transform.rotation = spawnPoint.rotation;

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
        // 点赞召唤特定小兵
        SpawnSoldier(SoldierType.LikeSoldier, camp);
        Debug.Log($"{camp} 点赞了，召唤了点赞小兵!");
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
}

