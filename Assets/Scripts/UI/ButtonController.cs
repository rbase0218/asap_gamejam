using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonController : GameFramework
{
    private DataManager _dataManager;
    private UnitSpawner _unitSpawner;
    
    private Button _spawnButton;
    private Button _upgradeButton;
    private Button _mineButton;
    private Button _firedoorButton;
    private Button _lottoButton;
    
    [field:SerializeField] public float MinGoldRange { get; private set; }
    [field:SerializeField] public float MaxGoldRange { get; private set; }

    
    public UnityEvent onClickMineButtonEvent;
    protected override void OnAwake()
    {
        base.OnAwake();

        _dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
        _unitSpawner = GameObject.Find("UnitSpawner").GetComponent<UnitSpawner>();
        
        _spawnButton = GameObject.Find("Spawn Button").GetComponent<Button>();
        _upgradeButton = GameObject.Find("Upgrade Button").GetComponent<Button>();
        _mineButton = GameObject.Find("Mine Button").GetComponent<Button>();
        _firedoorButton = GameObject.Find("FireDoor Button").GetComponent<Button>();
        _lottoButton = GameObject.Find("Lotto Button").GetComponent<Button>();
        
        _spawnButton.onClick.AddListener(OnClickSpawnButton);
        _upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        _mineButton.onClick.AddListener(OnClickMineButton);
        _firedoorButton.onClick.AddListener(OnClickFiredoorButton);
        _lottoButton.onClick.AddListener(OnClickLottoButton);
    }
    
    public void OnClickSpawnButton()
    {
        _unitSpawner.OnSpawn();
    }
    
    public void OnClickUpgradeButton()
    {
        Debug.Log("Upgrade Button 클릭");
    }

    public void OnClickMineButton()
    {
        float gold = Random.Range(MinGoldRange, MaxGoldRange) + _dataManager.GetGold();
        gold = Mathf.Round(gold);
        
        _dataManager.SetGold(gold);
        Debug.Log("Mine Button 클릭");
        
        onClickMineButtonEvent?.Invoke();
    }
    
    public void OnClickFiredoorButton()
    {
        Debug.Log("FireDoor Button 클릭");
    }
    
    public void OnClickLottoButton()
    {
        Debug.Log("Lotto Button 클릭");
    }
}
