using UnityEngine;

public class UnitStatus : MonoBehaviour
{
    // Unit의 Status를 관리하는 Script
    [SerializeField] private float maxHealth = .0f;
    [SerializeField] private float currentHealth = .0f;
    [SerializeField] private float attackDamage = .0f;
    [SerializeField] private float attackSpeed = .0f;
    [SerializeField] private float attackRange = .0f;
    [SerializeField] private float moveSpeed = .0f;

    public void SetHealth(int hp)
    {
        currentHealth = hp;
    }

    public void SetDamage(int damage)
    {
        attackDamage = damage;
    }
    
    public void SetAttackSpeed(float speed)
    {
        attackSpeed = speed;
    }
    
    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
    
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
