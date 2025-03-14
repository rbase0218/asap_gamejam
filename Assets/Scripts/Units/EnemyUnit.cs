using UnityEngine;

public class EnemyUnit : UnitBase
{
    private Rigidbody2D _rig;
    private BoxCollider2D _coll;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private Vector2 _moveDirection = Vector2.left; // 기본 이동 방향은 왼쪽
    
    protected override void OnAwake()
    {
        base.OnAwake();

        TryGetComponent(out _rig);
        TryGetComponent(out _coll);
        TryGetComponent(out _spriteRenderer);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        
        // 추가 업데이트 로직이 필요하면 여기에 구현
    }
    
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        // 지속적으로 왼쪽으로 이동
        MoveEnemy();
    }
    
    private void MoveEnemy()
    {
        // 유닛의 이동 속도를 가져와서 방향과 함께 속도 설정
        float speed = GetStatus().MoveSpeed;
        _rig.linearVelocity = _moveDirection.normalized * speed;
    }
    
    // 피격 시 처리 로직
    protected override void OnHit(float damage)
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
        // 사망 효과 재생, 점수 증가 등의 로직 추가
        
        // 오브젝트 제거
        Destroy(gameObject);
    }
}