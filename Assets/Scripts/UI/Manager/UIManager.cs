using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 관리를 위한 싱글톤 매니저 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static UIManager _instance;
    
    // 스레드 세이프한 인스턴스 접근자
    public static UIManager Instance
    {
        get
        {
            // 인스턴스가 없는 경우에만 찾거나 생성
            if (_instance == null)
            {
                // 씬에서 기존 인스턴스 찾기
                _instance = FindObjectOfType<UIManager>();
                
                // 씬에 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("UIManager");
                    _instance = managerObject.AddComponent<UIManager>();
                }
            }
            
            return _instance;
        }
    }
    
    // UI 캐싱용 딕셔너리
    private Dictionary<string, BaseUI> _uiDictionary = new Dictionary<string, BaseUI>();
    
    [SerializeField] private List<BaseUI> _registeredUIs = new List<BaseUI>();
    [SerializeField] private Transform _uiRoot; // UI가 생성될 부모 Transform
    
    private void Awake()
    {
        // 중복 인스턴스 확인 및 처리
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 싱글톤 설정
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // UI Root가 없으면 생성
        if (_uiRoot == null)
        {
            GameObject uiRootObject = new GameObject("UI_Root");
            uiRootObject.transform.SetParent(transform);
            _uiRoot = uiRootObject.transform;
            
            // Canvas가 필요한 경우 추가
            Canvas rootCanvas = uiRootObject.AddComponent<Canvas>();
            rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiRootObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            uiRootObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        // 등록된 UI 초기화
        InitializeRegisteredUIs();
    }
    
    /// <summary>
    /// 미리 등록된 UI들을 딕셔너리에 초기화
    /// </summary>
    private void InitializeRegisteredUIs()
    {
        foreach (BaseUI ui in _registeredUIs)
        {
            if (ui != null && !_uiDictionary.ContainsKey(ui.UIName))
            {
                _uiDictionary.Add(ui.UIName, ui);
            }
        }
    }
    
    /// <summary>
    /// UI 프리팹으로부터 새 UI 인스턴스 생성
    /// </summary>
    /// <param name="prefab">UI 프리팹</param>
    /// <returns>생성된 UI 컴포넌트</returns>
    public T CreateUI<T>(T prefab) where T : BaseUI
    {
        if (prefab == null) return null;
        
        // 이미 등록된 UI인지 확인
        if (_uiDictionary.TryGetValue(prefab.UIName, out BaseUI existingUI))
        {
            return existingUI as T;
        }
        
        // 새로운 UI 생성
        T newUI = Instantiate(prefab, _uiRoot);
        
        // 딕셔너리에 추가
        _uiDictionary.Add(newUI.UIName, newUI);
        
        return newUI;
    }
    
    /// <summary>
    /// 이름으로 UI 가져오기
    /// </summary>
    /// <param name="uiName">UI 이름</param>
    /// <returns>찾은 UI 컴포넌트</returns>
    public T GetUI<T>(string uiName) where T : BaseUI
    {
        if (_uiDictionary.TryGetValue(uiName, out BaseUI ui))
        {
            return ui as T;
        }
        
        Debug.LogWarning($"UI with name '{uiName}' not found.");
        return null;
    }
    
    /// <summary>
    /// UI 보이기
    /// </summary>
    /// <param name="uiName">표시할 UI 이름</param>
    public void ShowUI(string uiName)
    {
        if (_uiDictionary.TryGetValue(uiName, out BaseUI ui))
        {
            ui.Show();
        }
        else
        {
            Debug.LogWarning($"Cannot show UI with name '{uiName}'. UI not found.");
        }
    }
    
    /// <summary>
    /// UI 숨기기
    /// </summary>
    /// <param name="uiName">숨길 UI 이름</param>
    public void HideUI(string uiName)
    {
        if (_uiDictionary.TryGetValue(uiName, out BaseUI ui))
        {
            ui.Hide();
        }
        else
        {
            Debug.LogWarning($"Cannot hide UI with name '{uiName}'. UI not found.");
        }
    }
    
    /// <summary>
    /// 모든 UI 숨기기
    /// </summary>
    public void HideAllUI()
    {
        foreach (var ui in _uiDictionary.Values)
        {
            ui.Hide();
        }
    }
    
    /// <summary>
    /// UI 제거 및 정리
    /// </summary>
    /// <param name="uiName">제거할 UI 이름</param>
    public void DestroyUI(string uiName)
    {
        if (_uiDictionary.TryGetValue(uiName, out BaseUI ui))
        {
            _uiDictionary.Remove(uiName);
            Destroy(ui.gameObject);
        }
    }
    
    /// <summary>
    /// Unity 종료 시 정리
    /// </summary>
    private void OnDestroy()
    {
        // 싱글톤 인스턴스 정리
        if (_instance == this)
        {
            _instance = null;
        }
    }
}