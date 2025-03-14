using UnityEngine;

public class Projectile : GameFramework
{
    [SerializeField] private float _speed = 10f; // 투사체 속도
    [SerializeField] private float _lifetime = 5f; // 투사체 수명
    [SerializeField] private float _rotationSpeed = 200f; // 유도 회전 속도
    [SerializeField] private bool _isHoming = true; // 유도 기능 사용 여부
    [SerializeField] private GameObject _hitEffect; // 명중 효과 프리팹
    
    private float _damage; // 투사체 데미지
    private Transform _target; // 목표 타겟
    private UnitBase _owner; // 발사한 유닛
    private bool _hasHit = false; // 명중 여부
    
    private Rigidbody2D _rig;
    private SpriteRenderer _renderer;
    
    protected override void OnAwake()
    {
        base.OnAwake();
        
        _rig = GetComponent<Rigidbody2D>();
        if (_rig == null)
        {
            _rig = gameObject.AddComponent<Rigidbody2D>();
            _rig.gravityScale = 0f; // 중력 없음
        }
        
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    protected override void OnStart()
    {
        base.OnStart();
        
        // 수명 후 자동 파괴
        Destroy(gameObject, _lifetime);
    }
    
    // 투사체 초기화
    public void Initialize(float damage, Transform target, UnitBase owner)
    {
        _damage = damage;
        _target = target;
        _owner = owner;
        
        // 초기 방향 설정
        if (_target != null)
        {
            Vector2 direction = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            _rig.linearVelocity = direction * _speed;
            
            // 방향에 따라 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // 타겟이 없으면 오른쪽으로 발사
            _rig.linearVelocity = Vector2.right * _speed;
        }
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        
        // 이미 명중했거나 타겟이 없으면 리턴
        if (_hasHit || _target == null) return;
        
        if (_isHoming)
        {
            // 유도 기능 구현
            Vector2 direction = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            
            // 현재 속도 방향과 목표 방향 사이의 각도 계산
            float currentAngle = Mathf.Atan2(_rig.linearVelocity.y, _rig.linearVelocity.x) * Mathf.Rad2Deg;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // 각도 차이 계산 (최단 방향으로 회전)
            float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
            
            // 회전 각도 제한
            float maxRotation = _rotationSpeed * Time.deltaTime;
            float rotation = Mathf.Clamp(angleDifference, -maxRotation, maxRotation);
            
            // 새로운 각도 계산
            float newAngle = currentAngle + rotation;
            
            // 새로운 속도 벡터 계산
            Vector2 newDirection = new Vector2(
                Mathf.Cos(newAngle * Mathf.Deg2Rad),
                Mathf.Sin(newAngle * Mathf.Deg2Rad)
            );
            
            // 속도 업데이트
            _rig.linearVelocity = newDirection * _speed;
            
            // 투사체 회전
            transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit");
        if (_hasHit) return;
        
        if (other.CompareTag("Enemy"))
        {
            _hasHit = true;
            
            UnitBase targetUnit = other.GetComponent<UnitBase>();
            if (targetUnit != null)
            {
                targetUnit.OnHit(_damage);
            }
            
            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, transform.position, Quaternion.identity);
            }
            Debug.Log("투사체 명중");
            Destroy(gameObject);
        }
    }
}
