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

    protected virtual void Die()
    {
        _isDead = true;
        Debug.Log($"{_data?.unitName ?? "Unknown unit"} 已被击败!");
    }

    public virtual void Reset()
    {
        // 添加空值检查，避免在编辑器中报错
        if (_data != null)
        {
            _currentHealth = _data.maxHealth;
        }
        else
        {
            _currentHealth = 0; // 或给一个默认值
        }
        _isDead = false;
    }
}
