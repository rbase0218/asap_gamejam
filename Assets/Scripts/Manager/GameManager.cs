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

    public int roundStartCount = 0;
    public bool isStartRound = false;
    public int GetMouseClickCount() => mouseClickCount;
    
    public void AddMouseClickCount()
    {
        mouseClickCount++;
        onMouseClick.Invoke(mouseClickCount);
    }
    
    public void ResetMouseClickCount()
    {
        mouseClickCount = 0;
    }

    public void StartRound(UnityAction action)
    {
        if (roundStartCount <= mouseClickCount)
        {
            isStartRound = true;
            action?.Invoke();
        }
    }

    public void StopRound()
    {
        isStartRound = false;
    }
}
