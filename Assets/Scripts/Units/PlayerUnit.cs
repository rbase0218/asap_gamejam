using UnityEngine;
using System.Collections;

public class PlayerUnit : UnitBase
{
    private Vector2 _attackPosition;
    private Rigidbody2D _rig;
    private BoxCollider2D _coll;
    
    [SerializeField] private float _stoppingDistance = 0.1f;
    private bool _isMoving = false;

    public UnitAttackRange attackRange;
    
    [Header("Attack Settings")]
    [SerializeField] private GameObject _projectilePrefab; // 발사할 투사체 프리팹
    [SerializeField] private Transform _projectileSpawnPoint; // 투사체 발사 위치
    [SerializeField] private float _attackCooldown; // 공격 쿨다운 시간
    [SerializeField] private float _detectionRadius = 5f; // 적 감지 범위
    [SerializeField] private LayerMask _enemyLayer; // 적 레이어 마스크
    
    private bool _canAttack = true; // 공격 가능 여부
    private Transform _currentTarget; // 현재 공격 대상
    private float _attackTimer = 0f; // 공격 타이머
    
    protected override void OnAwake()
    {
        base.OnAwake();

        TryGetComponent(out _rig);
        TryGetComponent(out _coll);
        
        // 투사체 발사 위치가 지정되지 않았으면 자기 자신으로 설정
        if (_projectileSpawnPoint == null)
        {
            _projectileSpawnPoint = transform;
        }
        
        // 공격 쿨다운이 설정되지 않았으면 UnitStatus의 AttackSpeed 사용
        if (_attackCooldown <= 0)
        {
            _attackCooldown = GetStatus().AttackSpeed;
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        
        // 목표 위치에 도달했는지 확인
        if (_isMoving && HasReachedTargetPosition())
        {
            StopMoving();
        }
        
        // 적 탐지 및 공격
        if (!_isMoving)
        {
            // 이동 중이 아닐 때만 적을 탐지하고 공격
            DetectAndAttackEnemies();
        }
        
        // 공격 쿨다운 업데이트
        if (!_canAttack)
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= _attackCooldown)
            {
                _canAttack = true;
                _attackTimer = 0f;
            }
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
    
    // 적 탐지 및 공격
    private void DetectAndAttackEnemies()
    {
        // 공격 불가능 상태이면 리턴
        if (!_canAttack) return;
        
        // 현재 타겟이 있고 유효하고 범위 내에 있으면 그대로 공격
        if (_currentTarget != null && 
            IsValidTarget(_currentTarget.gameObject) && 
            IsTargetInRange(_currentTarget))
        {
            FireProjectile(_currentTarget);
            return;
        }
        
        // 현재 타겟이 없거나 유효하지 않으면 새로운 타겟 탐색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _enemyLayer);
        float closestDistance = float.MaxValue;
        Transform closestTarget = null;
        
        foreach (Collider2D collider in colliders)
        {
            if (IsValidTarget(collider.gameObject))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = collider.transform;
                }
            }
        }
        
        // 가장 가까운 타겟이 있으면 공격
        if (closestTarget != null)
        {
            _currentTarget = closestTarget;
            FireProjectile(_currentTarget);
        }
        else
        {
            _currentTarget = null;
        }
    }
    
    // 유효한 타겟인지 확인 (Enemy 태그)
    private bool IsValidTarget(GameObject target)
    {
        return target != null && target.CompareTag("Enemy");
    }
    
    // 타겟이 공격 범위 내에 있는지 확인
    private bool IsTargetInRange(Transform target)
    {
        if (target == null) return false;
        
        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= _detectionRadius;
    }
    
    // 투사체 발사
    private void FireProjectile(Transform target)
    {
        if (!_canAttack || _projectilePrefab == null) return;
        
        // 투사체 생성
        GameObject projectileObj = Instantiate(_projectilePrefab, _projectileSpawnPoint.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        
        if (projectile != null)
        {
            // 투사체 초기화
            projectile.Initialize(GetStatus().AttackDamage, target, this);
        }
        else
        {
            Debug.LogWarning("Projectile 컴포넌트가 없는 프리팹입니다.");
        }
        
        // 공격 쿨다운 설정
        _canAttack = false;
        _attackTimer = 0f;
    }
    
    // 피격 처리
    public override void OnHit(float damage)
    {
        base.OnHit(damage);
        
        // 현재 체력 감소
        int currentHealth = GetStatus().CurrentHealth;
        int defensePoint = GetStatus().DefensePoint;
        
        // 방어력을 고려한 데미지 계산
        int actualDamage = Mathf.Max(0, Mathf.RoundToInt(damage) - defensePoint);
        
        // 체력 감소
        int newHealth = Mathf.Max(0, currentHealth - actualDamage);
        GetStatus().SetHealth(newHealth);
        
        // 체력이 0이 되면 사망 처리
        if (newHealth <= 0)
        {
            Die();
        }
    }
    
    // 사망 처리
    private void Die()
    {
        // 사망 효과 재생, 점수 감소 등의 로직 추가
        
        // 오브젝트 제거
        Destroy(gameObject);
    }
    
    // 디버그용: 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}