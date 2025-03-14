using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int mouseClickCount = 0;
    
    public UnityEvent<int> onMouseClick = new UnityEvent<int>();

    public int GetMouseClickCount() => mouseClickCount;
    
    public void AddMouseClickCount()
    {
        mouseClickCount++;
        onMouseClick.Invoke(mouseClickCount);
        
        Debug.Log("Click Count : " + mouseClickCount);
    }

    public void ResetMouseClickCount()
    {
        mouseClickCount = 0;
    }
}
