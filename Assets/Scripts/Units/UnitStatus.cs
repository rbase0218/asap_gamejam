using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    // Unit의 Status를 관리하는 Script
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float CurrentHealth { get; private set; }
    [field: SerializeField] public float AttackDamage { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }

    public void SetHealth(int hp)
    {
        CurrentHealth = hp;
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
