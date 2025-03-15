using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyUnit : UnitBase
{
    private Rigidbody2D _rig;
    private BoxCollider2D _coll;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private Vector2 _moveDirection = Vector2.left; // 기본 이동 방향은 왼쪽
    
    [Header("Attack Settings")]
    [SerializeField] private LayerMask _targetLayers; // 공격 가능한 레이어 마스크
    [SerializeField] private string _targetTag = "Unit"; // 공격 가능한 태그들
    [SerializeField] private float _targetCheckInterval = 0.2f; // 주기적인 타겟 체크 간격
    
    private bool _isAttacking = false; // 공격 중 상태
    private bool _canAttack = true;    // 공격 가능 상태
    private Transform _currentTarget;   // 현재 공격 대상
    private float _attackTimer = 0f;   // 공격 타이머
    private float _targetCheckTimer = 0f; // 타겟 체크 타이머
    private bool _hasAttackedTarget = false; // 현재 타겟을 공격했는지 여부
    
    // 이동 및 공격 상태
    private enum EnemyState
    {
        Moving,
        Attacking
    }
    
    private EnemyState _currentState = EnemyState.Moving;
    
    protected override void OnAwake()
    {
        base.OnAwake();

        TryGetComponent(out _rig);
        TryGetComponent(out _coll);
        TryGetComponent(out _spriteRenderer);
        
        // 공격 가능한 레이어 자동 설정 (필요시 인스펙터에서 변경 가능)
        if (_targetLayers == 0)
        {
            _targetLayers = LayerMask.GetMask("Default"); // 타겟이 사용하는 레이어로 변경
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        
        // 주기적으로 타겟 체크
        _targetCheckTimer += Time.deltaTime;
        if (_targetCheckTimer >= _targetCheckInterval)
        {
            // 공격 범위 내 타겟 찾기 - 항상 체크하도록 수정
            FindTargetInRange();
            _targetCheckTimer = 0f;
        }
        
        // 상태에 따른 행동 처리
        switch (_currentState)
        {
            case EnemyState.Moving:
                // 이동 상태일 때는 별도 업데이트 로직 없음
                // 타겟 체크는 위에서 주기적으로 수행
                break;
                
            case EnemyState.Attacking:
                // 공격 중일 때는 이동을 멈춤 (확실하게 속도를 0으로 유지)
                StopMoving();
                
                // 공격 상태일 때 공격 타이머 업데이트
                if (!_canAttack)
                {
                    _attackTimer += Time.deltaTime;
                    if (_attackTimer >= GetStatus().AttackSpeed)
                    {
                        _canAttack = true;
                        _attackTimer = 0f;
                    }
                }
                
                // 타겟이 없어지거나 공격 범위를 벗어나면 다시 이동 상태로 전환
                if (_currentTarget == null || !IsTargetInAttackRange(_currentTarget))
                {
                    _currentState = EnemyState.Moving;
                    _currentTarget = null;
                    _hasAttackedTarget = false; // 타겟을 잃었으므로 플래그 리셋
                }
                else if (_canAttack)
                {
                    // 공격 가능하면 공격 실행
                    StartCoroutine(PerformAttack());
                }
                break;
        }
    }
    
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        // 이동 상태일 때만 이동
        if (_currentState == EnemyState.Moving)
        {
            MoveEnemy();
        }
    }
    
    private void MoveEnemy()
    {
        float speed = GetStatus().MoveSpeed;
        _rig.linearVelocity = _moveDirection.normalized * speed;
    }
    
    // 이동 멈추기
    private void StopMoving()
    {
        // 확실하게 이동을 중지하기 위해 velocity를 0으로 설정
        if (_rig != null && _rig.linearVelocity != Vector2.zero)
        {
            _rig.linearVelocity = Vector2.zero;
        }
    }
    
    // 공격 범위 내에 타겟이 있는지 확인
    private void FindTargetInRange()
    {
        // 공격 범위를 원형으로 검사
        float attackRange = GetStatus().AttackRange;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, _targetLayers);
        
        if (colliders.Length == 0)
        {
            // 범위 내에 타겟이 없으면 이동 상태로 전환
            if (_currentState == EnemyState.Attacking)
            {
                _currentState = EnemyState.Moving;
                _currentTarget = null;
                _hasAttackedTarget = false;
            }
            return;
        }
        
        // 공격 가능한 모든 대상을 거리 순으로 정렬
        var validTargets = new List<Transform>();
        
        foreach (Collider2D collider in colliders)
        {
            if (IsValidTarget(collider.gameObject))
            {
                validTargets.Add(collider.transform);
            }
        }
        
        // 유효한 타겟이 없으면 리턴
        if (validTargets.Count == 0) return;
        
        // 거리순으로 정렬
        validTargets = validTargets.OrderBy(t => 
            Vector2.Distance(transform.position, t.position)).ToList();
        
        // 가장 가까운 타겟이 있으면 공격 시작
        // 이미 타겟이 있고 범위 내에 있으면 그 타겟 유지, 아니면 새 타겟 선택
        Transform newTarget = validTargets[0];
        
        // 현재 타겟이 없거나, 범위를 벗어났거나, 파괴되었으면 새 타겟으로 교체
        if (_currentTarget == null || 
            !IsTargetInAttackRange(_currentTarget) ||
            validTargets.Contains(_currentTarget) == false)
        {
            StartAttackingTarget(newTarget);
        }
    }
    
    // 유효한 공격 대상인지 확인
    private bool IsValidTarget(GameObject target)
    {
        if (target.CompareTag(_targetTag))
            return true;
        return false;
    }
    
    // 공격 시작
    private void StartAttackingTarget(Transform target)
    {
        // 타겟 변경
        _currentTarget = target;
        _currentState = EnemyState.Attacking;
        _hasAttackedTarget = false; // 새로운 타겟이므로 공격 플래그 리셋
        
        StopMoving(); // 이동 즉시 중지
        
        // 즉시 첫 공격 실행
        if (_canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }
    
    // 대상이 공격 범위 내에 있는지 확인
    private bool IsTargetInAttackRange(Transform target)
    {
        if (target == null) return false;
        
        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= GetStatus().AttackRange;
    }
    
    // 공격 실행 코루틴
    private IEnumerator PerformAttack()
    {
        if (_isAttacking || !_canAttack || _currentTarget == null) yield break;
        
        _isAttacking = true;
        _canAttack = false;
        
        // 공격 전에 이동 중지 확인
        StopMoving();
        
        UnitBase targetUnit = _currentTarget.GetComponent<UnitBase>();
        if (targetUnit != null)
        {
            OnAttack(targetUnit);
            _hasAttackedTarget = true; // 타겟을 공격했음을 표시
        }
        
        // 공격 후 짧은 딜레이
        yield return new WaitForSeconds(0.2f);
        
        _isAttacking = false;
        // _canAttack은 업데이트에서 쿨다운 타이머로 관리됨
    }
    
    // 피격 시 처리 로직
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
        // 오브젝트 제거
        Destroy(gameObject);
    }
}