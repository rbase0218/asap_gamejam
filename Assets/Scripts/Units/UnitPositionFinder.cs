using UnityEngine;
using System.Collections.Generic;

public class UnitPositionFinder : MonoBehaviour
{
    [SerializeField] private LayerMask unitLayerMask; // PlayerUnit 레이어
    [SerializeField] private bool showDebug = true;
    [SerializeField] private int maxAttempts = 20; // 최대 시도 횟수
    
    // 디버그 시각화 설정
    [Header("Debug Visualization")]
    [SerializeField] private bool showLastAttempts = true;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private Color boundaryColor = Color.yellow;
    [SerializeField] private float debugDuration = 3f; // 디버그 표시 지속 시간
    
    // 디버그 정보 저장
    private List<DebugAttempt> _lastAttempts = new List<DebugAttempt>();
    private Vector2 _lastRangeBounds;
    private float _lastDebugTime;
    
    private class DebugAttempt
    {
        public Vector2 position;
        public Vector2 size;
        public bool success;
        
        public DebugAttempt(Vector2 pos, Vector2 sz, bool succ)
        {
            position = pos;
            size = sz;
            success = succ;
        }
    }

    private void Awake()
    {
        // PlayerUnit 레이어를 자동으로 설정
        unitLayerMask = LayerMask.GetMask("PlayerUnit");
    }

    /// <summary>
    /// 지정된 범위 내에서 다른 유닛과 겹치지 않는 위치를 찾습니다.
    /// </summary>
    /// <param name="rangeBounds">범위 경계 (x:최소값, y:최대값)</param>
    /// <param name="unitSize">유닛의 크기 (x:너비, y:높이)</param>
    /// <param name="yPosition">y 좌표 위치</param>
    /// <returns>겹치지 않는 위치</returns>
    public Vector2 FindNonOverlappingPosition(Vector2 rangeBounds, Vector2 unitSize, float yPosition)
    {
        if (showDebug)
        {
            Debug.Log($"범위 내에서 겹치지 않는 위치 찾기: 범위={rangeBounds}, 유닛 크기={unitSize}");
        }

        // 디버그 정보 초기화
        if (showLastAttempts)
        {
            _lastAttempts.Clear();
            _lastRangeBounds = rangeBounds;
            _lastDebugTime = Time.time;
        }

        // 충돌 검사에 사용할 박스 크기 (여유를 위해 약간 축소)
        Vector2 boxSize = new Vector2(unitSize.x * 0.9f, unitSize.y * 0.9f);
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // 1. 랜덤 좌표 생성
            float randomX = Random.Range(rangeBounds.x, rangeBounds.y);
            Vector2 testPosition = new Vector2(randomX, yPosition);
            
            // 2. 해당 위치에 다른 유닛이 있는지 확인
            bool isOverlapping = CheckOverlap(testPosition, boxSize);
            bool success = !isOverlapping;
            
            // 디버그 정보 저장
            if (showLastAttempts)
            {
                _lastAttempts.Add(new DebugAttempt(testPosition, boxSize, success));
            }
            
            // 3. 겹치지 않으면 위치 반환
            if (success)
            {
                if (showDebug)
                {
                    Debug.Log($"겹치지 않는 위치 찾음: {testPosition} (시도 횟수: {attempt + 1})");
                }
                return testPosition;
            }
            
            if (showDebug && attempt > 0 && attempt % 5 == 0)
            {
                Debug.Log($"아직 겹치지 않는 위치를 찾지 못했습니다. 시도 횟수: {attempt + 1}");
            }
        }
        
        // 최대 시도 횟수를 초과하면 범위 중앙 반환
        Vector2 fallbackPosition = new Vector2((rangeBounds.x + rangeBounds.y) / 2, yPosition);
        
        // 실패한 마지막 시도에 대한 디버그 정보 추가
        if (showLastAttempts)
        {
            _lastAttempts.Add(new DebugAttempt(fallbackPosition, boxSize, false));
        }
        
        Debug.LogWarning($"최대 시도 횟수({maxAttempts})를 초과했습니다. 범위 중앙 위치({fallbackPosition})를 반환합니다.");
        return fallbackPosition;
    }

    /// <summary>
    /// 지정된 위치에 다른 유닛이 있는지 확인합니다.
    /// </summary>
    /// <param name="position">검사할 위치</param>
    /// <param name="boxSize">검사할 박스 크기</param>
    /// <returns>겹치는 유닛이 있으면 true, 없으면 false</returns>
    private bool CheckOverlap(Vector2 position, Vector2 boxSize)
    {
        // OverlapBox로 지정된 위치에 유닛이 있는지 확인
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, boxSize, 0f, unitLayerMask);
        
        // 디버그 시각화를 위해 실시간으로 그리기
        if (showDebug)
        {
            Color debugColor = (colliders != null && colliders.Length > 0) ? Color.red : Color.green;
            Debug.DrawLine(
                new Vector3(position.x - boxSize.x/2, position.y - boxSize.y/2, 0),
                new Vector3(position.x + boxSize.x/2, position.y - boxSize.y/2, 0),
                debugColor, debugDuration);
            Debug.DrawLine(
                new Vector3(position.x + boxSize.x/2, position.y - boxSize.y/2, 0),
                new Vector3(position.x + boxSize.x/2, position.y + boxSize.y/2, 0),
                debugColor, debugDuration);
            Debug.DrawLine(
                new Vector3(position.x + boxSize.x/2, position.y + boxSize.y/2, 0),
                new Vector3(position.x - boxSize.x/2, position.y + boxSize.y/2, 0),
                debugColor, debugDuration);
            Debug.DrawLine(
                new Vector3(position.x - boxSize.x/2, position.y + boxSize.y/2, 0),
                new Vector3(position.x - boxSize.x/2, position.y - boxSize.y/2, 0),
                debugColor, debugDuration);
        }
        
        // 충돌하는 유닛이 있으면 true 반환
        bool hasOverlap = colliders != null && colliders.Length > 0;
        
        if (showDebug && hasOverlap)
        {
            Debug.Log($"위치 {position}에서 {colliders.Length}개의 유닛과 겹칩니다.");
        }
        
        return hasOverlap;
    }

    private void OnDrawGizmos()
    {
        if (!showDebug || !showLastAttempts) return;
        
        // 최근 시도가 지속 시간 내에 있는지 확인
        if (Time.time - _lastDebugTime > debugDuration) return;
        
        // 범위 경계 그리기
        Gizmos.color = boundaryColor;
        Gizmos.DrawLine(new Vector3(_lastRangeBounds.x, -10, 0), new Vector3(_lastRangeBounds.x, 10, 0));
        Gizmos.DrawLine(new Vector3(_lastRangeBounds.y, -10, 0), new Vector3(_lastRangeBounds.y, 10, 0));
        
        // 시도한 위치 그리기
        foreach (var attempt in _lastAttempts)
        {
            // 성공/실패에 따라 색상 설정
            Gizmos.color = attempt.success ? successColor : failColor;
            
            // 위치에 와이어 큐브 그리기
            Vector3 position = new Vector3(attempt.position.x, attempt.position.y, 0);
            Vector3 size = new Vector3(attempt.size.x, attempt.size.y, 0.1f);
            Gizmos.DrawWireCube(position, size);
            
            // 성공한 위치에는 추가 표시
            if (attempt.success)
            {
                Gizmos.DrawSphere(position, 0.1f);
            }
        }
        
        // 마지막 성공한 위치에 특별한 표시
        var lastSuccess = _lastAttempts.FindLast(a => a.success);
        if (lastSuccess != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 position = new Vector3(lastSuccess.position.x, lastSuccess.position.y, 0);
            Gizmos.DrawWireSphere(position, 0.3f);
        }
    }
    
    // 씬 뷰에서 현재 위치에 겹치는 유닛 확인 기능
    [ContextMenu("Check Overlap At Current Position")]
    private void CheckOverlapAtCurrentPosition()
    {
        // 유닛 평균 크기 가정
        Vector2 defaultSize = new Vector2(1f, 1f);
        bool isOverlapping = CheckOverlap((Vector2)transform.position, defaultSize);
        Debug.Log($"현재 위치 {transform.position}에서 겹치는 유닛: {(isOverlapping ? "있음" : "없음")}");
    }
}