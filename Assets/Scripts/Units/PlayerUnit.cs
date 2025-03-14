using UnityEngine;

public class PlayerUnit : UnitBase
{
    private Vector2 _attackPosition;
    private Rigidbody2D _rig;
    private BoxCollider2D _coll;
    
    [SerializeField] private float _stoppingDistance = 0.1f;
    private bool _isMoving = false;
    
    protected override void OnAwake()
    {
        base.OnAwake();

        TryGetComponent(out _rig);
        TryGetComponent(out _coll);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        
        if (_isMoving && HasReachedTargetPosition())
        {
            StopMoving();
        }
    }
    
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        // 이동 중이면 이동 처리
        if (_isMoving)
        {
            Move();
        }
    }
    
    public void SetAttackPosition(Vector2 targetPosition)
    {
        _attackPosition = targetPosition;
        _isMoving = true;
    }

    private void Move()
    {
        var moveDir = _attackPosition - (Vector2)transform.position;
        _rig.linearVelocity = moveDir.normalized * GetStatus().MoveSpeed;
    }
    
    private bool HasReachedTargetPosition()
    {
        float distanceToTarget = Vector2.Distance((Vector2)transform.position, _attackPosition);
        return distanceToTarget <= _stoppingDistance;
    }
    
    private void StopMoving()
    {
        _isMoving = false;
        _rig.linearVelocity = Vector2.zero;
    }
    
    public bool IsMoving()
    {
        return _isMoving;
    }
    
    public void ForceStopMoving()
    {
        StopMoving();
    }
}