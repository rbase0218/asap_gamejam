using UnityEngine;

[RequireComponent(typeof(UnitStatus))]
public abstract class UnitBase : GameFramework
{
    private UnitStatus _status;
    
    protected override void OnAwake()
    {
        TryGetComponent(out _status);
    }

    protected virtual void OnAttack(UnitBase unit, float damage = -1f)
    {
        if(damage < 0f)
            unit.OnHit(_status.AttackDamage);
        else
            unit.OnHit(damage);
    }

    public virtual void OnHit(float damage)
    {
        
    }
    
    public UnitStatus GetStatus() => _status;
}
