using UnityEngine;

public class MapManager : MonoBehaviour
{
    private SpriteRenderer _render;
    
    [field:SerializeField] public float BaseWidth { get; private set; }
    // 원거리
    [field:SerializeField] public float FarWidth { get; private set; }
    // 중거리
    [field:SerializeField] public float MidWidth { get; private set; }
    // 근거리
    [field:SerializeField] public float NearWidth { get; private set; }

    [field:SerializeField] public float FiredoorWidth { get; private set; }

    private float _minWidth = .0f;
    private float _maxWidth = .0f;
    
    private void Awake()
    {
        if (!TryGetComponent(out _render))
        {
            Debug.Log("Renderer 컴포넌트가 없습니다.");
            Debug.Break();
        }
    
        float scaledWidth = _render.sprite.bounds.size.x * transform.localScale.x;
    
        _minWidth = -(scaledWidth / 2);
        _maxWidth = scaledWidth / 2;
    }
}