using UnityEngine;

/// <summary>
/// 모든 UI 요소의 기본 클래스
/// </summary>
public abstract class BaseUI : MonoBehaviour
{
    [SerializeField] private string _uiName;
    [SerializeField] protected CanvasGroup _canvasGroup;
    
    // UI 고유 이름 프로퍼티
    public string UIName => _uiName;
    
    protected virtual void Awake()
    {
        // 필요한 컴포넌트 자동 할당
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 초기 상태는 비활성화
        if (gameObject.activeSelf)
            Hide();
    }
    
    /// <summary>
    /// UI 표시
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        
        OnShow();
    }
    
    /// <summary>
    /// UI 숨기기
    /// </summary>
    public virtual void Hide()
    {
        OnHide();
        
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// UI가 표시될 때 호출되는 가상 메서드
    /// </summary>
    protected virtual void OnShow() { }
    
    /// <summary>
    /// UI가 숨겨질 때 호출되는 가상 메서드
    /// </summary>
    protected virtual void OnHide() { }
}
