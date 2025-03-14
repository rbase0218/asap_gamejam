using UnityEngine;

public class MapManager : GameFramework
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
    
    [field:SerializeField] public Vector2 FarSpawnPoint { get; private set; }
    [field:SerializeField] public Vector2 MidSpawnPoint { get; private set; }
    [field:SerializeField] public Vector2 NearSpawnPoint { get; private set; }

    private float _minWidth = .0f;
    private float _maxWidth = .0f;
    
    protected override void OnAwake()
    {
        if (!TryGetComponent(out _render))
        {
            Debug.Log("Renderer 컴포넌트가 없습니다.");
            Debug.Break();
        }
    
        float scaledWidth = _render.sprite.bounds.size.x * transform.localScale.x;
    
        _minWidth = -(scaledWidth / 2);
        _maxWidth = scaledWidth / 2;

        FarSpawnPoint = new Vector2(_minWidth + BaseWidth, _minWidth + BaseWidth + FarWidth);
        MidSpawnPoint = new Vector2(FarSpawnPoint.y, FarSpawnPoint.y + MidWidth);
        NearSpawnPoint = new Vector2(MidSpawnPoint.y, MidSpawnPoint.y + NearWidth);
    }

    public Vector2 GetBaseCenterPoint()
    {
        var basePointX = _minWidth + (BaseWidth / 2);
        var basePointY = transform.position.y;
    
        return new Vector2(basePointX, basePointY);
    }

    public Vector2 GetBaseRandPoint()
    {
        var basePointX = _minWidth + (BaseWidth / 2);
        
        var minRandY = transform.position.y - ((_render.sprite.bounds.size.y * transform.localScale.y) / 2);
        var maxRandY = transform.position.y + ((_render.sprite.bounds.size.y * transform.localScale.y) / 2);
        var basePointY = Random.Range(minRandY, maxRandY);
        return new Vector2(basePointX, basePointY);
    }
}