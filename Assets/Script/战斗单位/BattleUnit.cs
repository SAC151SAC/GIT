using UnityEngine;

public abstract class BattleUnit : MonoBehaviour
{
    protected CampType _camp;
    protected BattleUnitData _data;
    protected int _currentHealth;
    protected bool _isDead = false;

    public CampType Camp => _camp;
    public bool IsDead => _isDead;

    public virtual void Initialize(CampType camp, BattleUnitData data)
    {
        _camp = camp;
        _data = data;
        _currentHealth = data.maxHealth;
        _isDead = false;
    }

    public virtual void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    // 保持protected访问修饰符
    protected virtual void Die()
    {
        _isDead = true;
        Debug.Log($"{_data.unitName} 已被击败!");
    }

    public virtual void Reset()
    {
        _currentHealth = _data.maxHealth;
        _isDead = false;
    }
}

