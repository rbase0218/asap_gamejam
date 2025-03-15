using UnityEngine;

public class UnitStatus : GameFramework
{
    // Unit의 Status를 관리하는 Script
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int CurrentHealth { get; private set; }
    
    [field: SerializeField] public int DefensePoint { get; private set; }
    [field: SerializeField] public int AttackDamage { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();

        SetFullHealth();
    }
    
    public void SetFullHealth()
    {
        CurrentHealth = MaxHealth;
    }

    public void SetHealth(int hp)
    {
        CurrentHealth = hp;
    }
    public void SetDefensePoint(int dp)
    {
        DefensePoint = dp;
    }

    public void SetDamage(int damage)
    {
        AttackDamage = damage;
    }
    
    public void SetAttackSpeed(float speed)
    {
        AttackSpeed = speed;
    }
    
    public void SetAttackRange(float range)
    {
        AttackRange = range;
    }
    
    public void SetMoveSpeed(float speed)
    {
        MoveSpeed = speed;
    }
}
