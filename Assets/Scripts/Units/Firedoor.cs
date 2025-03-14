using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Firedoor : UnitBase
{
    [Header("Events")]
    [SerializeField] private UnityEvent<float> onDamaged;
    [SerializeField] private UnityEvent onDestroyed;
    
    protected override void OnStart()
    {
        base.OnStart();
        
        // 시작 시 최대 체력으로 설정
        GetStatus().SetHealth(GetStatus().MaxHealth);
    }
    
    // UnitBase에서 상속받은 OnHit 메서드를 오버라이드
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
        
        // 데미지 이벤트 발생
        onDamaged?.Invoke(actualDamage);
        
        // 데미지를 입었을 때 시각적 피드백
        StartCoroutine(DamageEffect());
        
        // 체력이 0이 되면 파괴
        if (newHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator DamageEffect()
    {
        // 스프라이트 렌더러가 있으면 색상 변경으로 피드백 제공
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
        else
        {
            yield return null;
        }
    }
    
    // 파괴 처리를 위한 메서드
    private void Die()
    {
        onDestroyed?.Invoke();
        Destroy(gameObject);
    }
    
    // 현재 체력 비율 반환 (UI 등에서 사용)
    public float GetHealthPercentage()
    {
        return (float)GetStatus().CurrentHealth / GetStatus().MaxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        OnHit(damage);
    }
}