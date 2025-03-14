using UnityEngine;

public class UnitSpawner : GameFramework
{
    private int randNum = 0;
    private MapManager _mapManager;

    [SerializeField] private Transform unitSpawnObject;

    protected override void OnAwake()
    {
        _mapManager = GameObject.Find("Map").GetComponent<MapManager>();
    }
    
    public void OnSpawn()
    {
        SpawnPlayerUnit();
    }
    
    private void SpawnPlayerUnit()
    {
        // 랜덤으로 유닛을 생성한다.
        var unit = Resources.Load("Prefabs/Unit1001");
        if (unit == null)
        {
            Debug.Log("유닛이 없습니다.");
            return;
        }
        
        var spawnPoint = _mapManager.GetBaseRandPoint();
        var unitObj = Instantiate(unit, spawnPoint, Quaternion.identity) as GameObject;
        unitObj.transform.parent = unitSpawnObject;
        
        var playerUnit = unitObj.GetComponent<PlayerUnit>();
        Vector2 range = Vector2.zero;

        switch (playerUnit.attackRange)
        {
            case UnitAttackRange.Far:
                range = _mapManager.FarSpawnPoint;
                break;
            case UnitAttackRange.Mid:
                range = _mapManager.MidSpawnPoint;
                break;
            case UnitAttackRange.Near:
                range = _mapManager.NearSpawnPoint;
                break;
        }
        var arrivePoint = new Vector2(Random.Range(range.x, range.y), unitObj.transform.position.y);
        Debug.Log(arrivePoint);
        playerUnit.SetAttackPosition(arrivePoint);
    }
}
