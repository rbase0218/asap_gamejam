using UnityEngine;

public class EnemyUnit : UnitBase
{
    private Rigidbody2D _rig;
    private BoxCollider2D _coll;
    
    protected override void OnAwake()
    {
        base.OnAwake();

        TryGetComponent(out _rig);
        TryGetComponent(out _coll);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
    
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }
}
