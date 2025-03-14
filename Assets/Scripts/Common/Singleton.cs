using UnityEngine;

/// <summary>
/// 제네릭 싱글톤 패턴 구현
/// </summary>
/// <typeparam name="T">싱글톤으로 구현할 클래스 타입</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 정적 인스턴스 변수
    private static T _instance;
    
    // 스레드 동기화를 위한 락 객체
    private static readonly object _lock = new object();
    
    // 싱글톤 인스턴스 속성
    public static T Instance
    {
        get
        {
            // 애플리케이션이 종료 중인지 확인
            if (ApplicationQuit)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit.");
                return null;
            }
            
            // 스레드 안전한 방식으로 인스턴스 접근
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 씬에서 모든 인스턴스 찾기
                    _instance = FindFirstObjectByType<T>();

                    // 씬에 인스턴스가 없는 경우
                    if (_instance == null)
                    {
                        // 새로운 게임 오브젝트 생성
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T)} (Singleton)";
                        
                        // 씬 전환시에도 유지
                        DontDestroyOnLoad(singletonObject);
                        
                        Debug.Log($"[Singleton] An instance of {typeof(T)} was created.");
                    }
                    else
                    {
                        Debug.Log($"[Singleton] Using existing instance of {typeof(T)}.");
                    }
                }
                
                return _instance;
            }
        }
    }
    
    // 애플리케이션 종료 플래그
    private static bool ApplicationQuit { get; set; } = false;
    
    protected virtual void Awake()
    {
        // 다른 인스턴스가 이미 존재하는지 확인
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] Another instance of {typeof(T)} already exists! Destroying this duplicate.");
            Destroy(gameObject);
            return;
        }

        // 인스턴스가 null인 경우 현재 인스턴스 할당
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[Singleton] {typeof(T)} instance initialized in Awake.");
        }
    }
    
    protected virtual void OnApplicationQuit()
    {
        ApplicationQuit = true;
    }
}