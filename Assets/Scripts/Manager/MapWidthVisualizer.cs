using UnityEngine;

public class MapWidthVisualizer : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private bool showVisualization = true;
    [SerializeField] private float lineHeight = 1f;
    
    [Header("Colors")]
    [SerializeField] private Color baseColor = Color.blue;
    [SerializeField] private Color farColor = Color.red;
    [SerializeField] private Color midColor = Color.yellow;
    [SerializeField] private Color nearColor = Color.green;
    [SerializeField] private Color firedoorColor = Color.magenta;

    private SpriteRenderer _mapRenderer;
    private float _minWidth = .0f;
    private float _maxWidth = .0f;

    private void Awake()
    {
        InitializeMapBounds();
    }

    private void Start()
    {
        InitializeMapBounds();
    }

    private void InitializeMapBounds()
    {
        if (mapManager == null)
        {
            Debug.LogWarning("MapManager reference not set!");
            return;
        }

        _mapRenderer = mapManager.GetComponent<SpriteRenderer>();
        if (_mapRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on MapManager!");
            return;
        }

        // 맵 경계 계산
        float scaledWidth = _mapRenderer.sprite.bounds.size.x * transform.localScale.x;
        _minWidth = -(scaledWidth / 2);
        _maxWidth = scaledWidth / 2;
    }

    private void OnDrawGizmos()
    {
        if (!showVisualization || mapManager == null)
            return;
            
        var spriteRenderer = mapManager.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null)
            return;
            
        // 실행 모드가 아닐 때도 맵 정보를 가져옴
        float scaledWidth = spriteRenderer.sprite.bounds.size.x * spriteRenderer.transform.localScale.x;
        float scaledHeight = spriteRenderer.sprite.bounds.size.y * spriteRenderer.transform.localScale.y;
        Vector3 mapPosition = spriteRenderer.transform.position;
        float minWidth = mapPosition.x - (scaledWidth / 2);
        float maxWidth = mapPosition.x + (scaledWidth / 2);
        float yPos = mapPosition.y;
        
        // Base Width
        DrawHorizontalLine(
            minWidth, 
            minWidth + mapManager.BaseWidth, 
            yPos, 
            baseColor,
            "Base"
        );
        
        // Far Width
        DrawHorizontalLine(
            minWidth + mapManager.BaseWidth, 
            minWidth + mapManager.BaseWidth + mapManager.FarWidth, 
            yPos - lineHeight, 
            farColor,
            "Far (원거리)"
        );
        
        // Mid Width
        DrawHorizontalLine(
            minWidth + mapManager.BaseWidth + mapManager.FarWidth, 
            minWidth + mapManager.BaseWidth + mapManager.FarWidth + mapManager.MidWidth, 
            yPos - lineHeight * 2, 
            midColor,
            "Mid (중거리)"
        );
        
        // Near Width
        DrawHorizontalLine(
            minWidth + mapManager.BaseWidth + mapManager.FarWidth + mapManager.MidWidth, 
            minWidth + mapManager.BaseWidth + mapManager.FarWidth + mapManager.MidWidth + mapManager.NearWidth, 
            yPos - lineHeight * 3, 
            nearColor,
            "Near (근거리)"
        );
        
        // Firedoor Width
        DrawHorizontalLine(
            minWidth + mapManager.BaseWidth + mapManager.FarWidth + mapManager.MidWidth + mapManager.NearWidth, 
            maxWidth, 
            yPos - lineHeight * 4, 
            firedoorColor,
            "Firedoor"
        );
        
        // 전체 맵 표시
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(mapPosition, new Vector3(scaledWidth, scaledHeight, 0));
    }
    
    private void DrawHorizontalLine(float startX, float endX, float y, Color color, string label)
    {
        Gizmos.color = color;
        
        // 시작점과 끝점
        Vector3 start = new Vector3(startX, y, 0);
        Vector3 end = new Vector3(endX, y, 0);
        
        // 라인 그리기
        Gizmos.DrawLine(start, end);
        
        // 화살표 끝 표시
        float arrowSize = 0.2f;
        Vector3 arrowStart = end - new Vector3(arrowSize, 0, 0);
        Vector3 arrowUp = new Vector3(end.x, end.y + arrowSize/2, 0);
        Vector3 arrowDown = new Vector3(end.x, end.y - arrowSize/2, 0);
        Gizmos.DrawLine(arrowStart, arrowUp);
        Gizmos.DrawLine(arrowStart, arrowDown);
        
        // 시작점과 끝점에 수직 라인 추가
        Gizmos.DrawLine(start, start + new Vector3(0, arrowSize, 0));
        Gizmos.DrawLine(end, end + new Vector3(0, arrowSize, 0));
        
        // 레이블 표시 (Scene 뷰에서만 보임)
        Vector3 labelPos = new Vector3((startX + endX) / 2, y + 0.3f, 0);
        float width = endX - startX;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"{label}: {width:F2}");
#endif
    }

#if UNITY_EDITOR
    // OnValidate는 Inspector에서 값이 변경될 때마다 호출됩니다
    private void OnValidate()
    {
        // Inspector에서 값이 변경될 때 시각적 업데이트를 위해
        if (mapManager != null)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
    }
#endif
}