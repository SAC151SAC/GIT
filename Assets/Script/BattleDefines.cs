using UnityEngine;

// 阵营类型
public enum CampType
{
    PartyA,
    PartyB
}

// 小兵类型
public enum SoldierType
{
    // 点赞召唤的小兵
    LikeSoldier,
    // 礼物按钮召唤的8种小兵
    GiftSoldier1,
    GiftSoldier2,
    GiftSoldier3,
    GiftSoldier4,
    GiftSoldier5,
    GiftSoldier6,
    GiftSoldier7,
    GiftSoldier8
}

// 战斗单位基类数据
[System.Serializable]
public class BattleUnitData
{
    public string unitName;
    public int maxHealth;
    public int attackPower;
    public float moveSpeed;
    public float attackRange;
    public float attackInterval;
    public GameObject prefab;
}

// 小兵数据
[System.Serializable]
public class SoldierData : BattleUnitData
{
    public SoldierType soldierType;
}

// 角色数据
[System.Serializable]
public class PlayerData : BattleUnitData
{
    public int splashAttackRange; // 范围攻击半径
}

