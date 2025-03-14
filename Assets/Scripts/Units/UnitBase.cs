using UnityEngine;

[RequireComponent(typeof(UnitStatus))]
public abstract class UnitBase : GameFramework
{
    private UnitStatus _status;
    
    protected override void OnAwake()
    {
        TryGetComponent(out _status);
    }

    protected virtual void OnAttack(UnitBase unit)
    {
        unit.OnHit(_status.AttackDamage);
    }

    protected virtual void OnHit(float damage)
    {
        
    }
    
    public UnitStatus GetStatus() => _status;
}
