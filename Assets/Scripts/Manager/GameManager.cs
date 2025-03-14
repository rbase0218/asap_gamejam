using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int mouseClickCount = 0;
    
    public UnityEvent<int> onMouseClick = new UnityEvent<int>();

    [SerializeField] private int money = 0;
    [SerializeField] private int securityGrade = 0;
    [SerializeField] private bool isFiredoorOpen = false;

    private float extraBossDamage = .0f;
    private int extraBossHealth = 0;
    private int extraBoosDefense = 0;
    
    private int extraNormalEnemyHealth = 0;
    private float extraNoramlEnemyMoveSpeed = .0f;
    private float extraNoramlEnemyDamage = .0f;
    
    

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
