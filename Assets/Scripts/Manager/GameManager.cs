using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public UnityEvent<int> onMouseClick = new UnityEvent<int>();

    [SerializeField] private int mouseClickCount = 0;
    
    public UserData userData;
    private UserData saveData = new UserData();

    [Header("Mining - Money")]
    [SerializeField] private int miningMinMoney = 0;
    [SerializeField] private int miningMaxMoney = 0;

    [Header("Spawn Money")]
    public int needMoneyToSpawn = 0;
    
    [Header("Firedoor Repair")]
    public int needMoneyToFiredoorRepair = 0;

    [Header("Round Control")]
    public int roundStartCount = 0;
    public bool isStartRound = false;
    public bool isEnemySpawning = false;


    private int saveMoney = 0;
    
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
            SaveRoundData();

            action?.Invoke();
        }
    }
    
    public void SaveRoundData()
    {
        saveData.money = userData.money;
        saveData.roundCount = userData.roundCount;
        saveData.unitDataList = userData.unitDataList;
    }

    public void ClearUnitData() => userData?.unitDataList.Clear();

    public void AddUnitData(UnitData data)
    {
        userData?.unitDataList.Add(data);
    }

    public void StopRound(UnityAction action)
    {
        isStartRound = false;
        mouseClickCount = 0;
        action?.Invoke();
        saveMoney = GetMoney();
    }

    public int GetMoney() => userData.money;

    public void AddMoney()
    {
        var result = Random.Range(miningMinMoney, miningMaxMoney);
        userData.money += result;
    }
    
    public void AddMoney(int value)
    {
        userData.money += value;
    }

    public void StartBackup()
    {
        userData.money = saveMoney;
        AddMoney();
        isEnemySpawning = false;
        isStartRound = false;
        mouseClickCount = 0;
    }
}
