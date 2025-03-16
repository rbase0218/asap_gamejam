using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonController : GameFramework
{
    private UnitSpawner _unitSpawner;
    
    private Button _spawnButton;
    private Button _upgradeButton;
    private Button _mineButton;
    private Button _backupButton;
    private Button _firedoorButton;
    private Button _lottoButton;

    private Button _enemyButton;
    
    [field:SerializeField] public float MinGoldRange { get; private set; }
    [field:SerializeField] public float MaxGoldRange { get; private set; }

    [SerializeField] private UnitBase firedoor;

    
    public UnityEvent onClickMineButtonEvent;
    protected override void OnAwake()
    {
        base.OnAwake();

        _unitSpawner = GameObject.Find("UnitSpawner").GetComponent<UnitSpawner>();
        
        _spawnButton = GameObject.Find("Spawn Button").GetComponent<Button>();
        _upgradeButton = GameObject.Find("Upgrade Button").GetComponent<Button>();
        _mineButton = GameObject.Find("Mine Button").GetComponent<Button>();
        _backupButton = GameObject.Find("Backup Button").GetComponent<Button>();
        _firedoorButton = GameObject.Find("FireDoor Button").GetComponent<Button>();
        _lottoButton = GameObject.Find("Lotto Button").GetComponent<Button>();
        _enemyButton = GameObject.Find("Enemy Button").GetComponent<Button>();
        
        _spawnButton.onClick.AddListener(OnClickSpawnButton);
        _upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        _mineButton.onClick.AddListener(OnClickMineButton);
        _backupButton.onClick.AddListener(OnClickBackupButton);
        _firedoorButton.onClick.AddListener(OnClickFiredoorButton);
        _lottoButton.onClick.AddListener(OnClickLottoButton);
        
        _enemyButton.onClick.AddListener(OnEnemySpawnButton);
    }

    public void OnEnemySpawnButton()
    {
        _unitSpawner.OnSpawnWithEnemy();
    }
    
    public void OnClickSpawnButton()
    {
        if (GameManager.Instance.isStartRound)
            return;

        var currMoney = GameManager.Instance.GetMoney();
        var needMoney = GameManager.Instance.needMoneyToSpawn;
        
        if(currMoney < needMoney)
            return;
        
        GameManager.Instance.AddMoney(-needMoney);
        _unitSpawner.OnSpawn();
        
        GameManager.Instance.AddMouseClickCount();
    }
    
    public void OnClickUpgradeButton()
    {
        Debug.Log("Upgrade Button 클릭");
        GameManager.Instance.AddMouseClickCount();
    }

    public void OnClickMineButton()
    {
        Debug.Log("Mine Button 클릭");
        
        GameManager.Instance.AddMoney();
        GameManager.Instance.AddMouseClickCount();
        
        onClickMineButtonEvent?.Invoke();
    }

    public void OnClickBackupButton()
    {
        GameManager.Instance.StartBackup();
        var data = GameManager.Instance.userData.unitDataList;
        
        _unitSpawner.CleanAllUnits();
        
        foreach (var d in data)
        {
            _unitSpawner.OnSpawn(d.unitCode, d.unitPosition);
        }
    }
    
    public void OnClickFiredoorButton()
    {
        if (GameManager.Instance.isStartRound)
            return;
        
        var currMoney = GameManager.Instance.GetMoney();
        var needMoney = GameManager.Instance.needMoneyToFiredoorRepair;
        
        if(currMoney < needMoney)
            return;
        
        GameManager.Instance.AddMoney(-needMoney);
        firedoor.GetStatus().SetFullHealth();
        
        GameManager.Instance.AddMouseClickCount();
        Debug.Log("FireDoor Button 클릭");
    }
    
    public void OnClickLottoButton()
    {
        GameManager.Instance.AddMouseClickCount();
        Debug.Log("Lotto Button 클릭");
    }
}
