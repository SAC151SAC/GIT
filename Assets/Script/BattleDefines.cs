using System;
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

// 保存小兵类型+阵营的组合键
public struct SoldierKey : IEquatable<SoldierKey>
{
    public SoldierType type;
    public CampType camp;

    public SoldierKey(SoldierType type, CampType camp)
    {
        this.type = type;
        this.camp = camp;
    }

    public bool Equals(SoldierKey other)
    {
        return type == other.type && camp == other.camp;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, camp);
    }
}


// 战斗单位基类数据
[System.Serializable]
public class BattleUnitData
{
    public string unitName;
    public int maxHealth;// 最大生命值
    public int attackPower;// 攻击力
    public float moveSpeed;// 移动速度
    public float attackRange;// 攻击范围
    public float attackInterval;// 攻击间隔
    public GameObject prefab;
}

// 小兵数据
[System.Serializable]
public class SoldierData : BattleUnitData
{
    public SoldierType soldierType;// 小兵类型
}

// 角色数据
[System.Serializable]
public class PlayerData : BattleUnitData
{
    public int splashAttackRange; // 范围攻击半径
}

