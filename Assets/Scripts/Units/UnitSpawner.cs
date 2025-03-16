using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class UnitSpawner : GameFramework
{
    private int randNum = 0;
    private MapManager _mapManager;
    private UnitPositionFinder _positionFinder;

    [SerializeField] private int maxEnemyCode = 0;
    [SerializeField] private int maxUnitCode = 0;
    [SerializeField] private Transform unitSpawnObject;
    [SerializeField] private Transform enemySpawnObject;

    private UnitManager _unitManager;

    protected override void OnAwake()
    {
        _mapManager = GameObject.Find("Map").GetComponent<MapManager>();
        _unitManager = GetComponent<UnitManager>();
        
        // UnitPositionFinder 찾기 또는 생성
        _positionFinder = GetComponent<UnitPositionFinder>();
        if (_positionFinder == null)
        {
            _positionFinder = gameObject.AddComponent<UnitPositionFinder>();
            Debug.Log("UnitPositionFinder 컴포넌트가 자동으로 추가되었습니다.");
        }
        
        GameManager.Instance.onMouseClick.AddListener((x) =>
        {
            GameManager.Instance.StartRound(OnStartRound);
        });
    }
    
    public void OnSpawn()
    {
        SpawnPlayerUnit();
    }
    
    private void SpawnPlayerUnit(int code = -1)
    {
        // 랜덤으로 유닛을 생성한다.
        var randCode = (code == -1) ? Random.Range(1001, maxUnitCode + 1) : code;
        var unit = Resources.Load($"Prefabs/Unit{randCode.ToString()}");
        if (unit == null)
        {
            Debug.Log("유닛이 없습니다.");
            return;
        }
        
        // 기지에서 스폰
        var spawnPoint = _mapManager.GetBaseRandPoint();
        var unitObj = Instantiate(unit, spawnPoint, Quaternion.identity) as GameObject;
        unitObj.transform.parent = unitSpawnObject;
        
        var playerUnit = unitObj.GetComponent<PlayerUnit>();
        playerUnit.GetStatus().SetUnitCode(randCode);
        Vector2 rangeBounds = Vector2.zero;

        // 유닛의 공격 범위에 맞는 스폰 구간 설정
        switch (playerUnit.attackRange)
        {
            case UnitAttackRange.Far:
                rangeBounds = _mapManager.FarSpawnPoint;
                break;
            case UnitAttackRange.Mid:
                rangeBounds = _mapManager.MidSpawnPoint;
                break;
            case UnitAttackRange.Near:
                rangeBounds = _mapManager.NearSpawnPoint;
                break;
        }
        
        // 유닛의 스프라이트 크기 가져오기
        Vector2 unitSize = GetUnitSize(unitObj);
        
        // 겹치지 않는 위치 찾기
        Vector2 targetPosition = _positionFinder.FindNonOverlappingPosition(
            rangeBounds, 
            unitSize, 
            unitObj.transform.position.y
        );
        
        playerUnit.SetAttackPosition(targetPosition);
        _unitManager.AddPlayerUnit(playerUnit);
    }
    
    /// <summary>
    /// 유닛의 크기를 가져옵니다.
    /// </summary>
    private Vector2 GetUnitSize(GameObject unitObj)
    {
        // 스프라이트 렌더러가 있으면 스프라이트 크기 반환
        SpriteRenderer spriteRenderer = unitObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            return spriteRenderer.sprite.bounds.size;
        }
        
        // 스프라이트 렌더러가 없으면 콜라이더 크기 확인
        Collider2D collider = unitObj.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds.size;
        }
        
        // 둘 다 없으면 기본 크기 반환
        Debug.LogWarning("유닛의 크기를 확인할 수 없습니다. 기본 크기를 사용합니다.");
        return new Vector2(1f, 1f);
    }

    public void OnStartRound()
    {
        OnSpawnWithEnemy();
    }

    Coroutine _spawnCoroutine;
    public void OnSpawnWithEnemy()
    {
        if (_spawnCoroutine != null) return;
        _spawnCoroutine = StartCoroutine(StartSpawnUnit());
    }
    private void SpawnEnemyUnit()
    {
        var randCode = Random.Range(2001, maxEnemyCode + 1);
        var unit = Resources.Load($"Prefabs/Enemy{randCode.ToString()}");
        
        if (unit == null)
        {
            Debug.Log("유닛이 없습니다.");
            return;
        }
        
        var spawnPoint = _mapManager.GetEnemySpawnPoint();
        var unitObj = Instantiate(unit, spawnPoint, Quaternion.identity) as GameObject;
        unitObj.transform.parent = enemySpawnObject;
        
        var enemyUnit = unitObj.GetComponent<EnemyUnit>();
        enemyUnit.GetStatus().SetUnitCode(randCode);
        enemyUnit.Initialize(_unitManager);
        
        _unitManager.AddEnemyUnit(enemyUnit);
    }

    private void SpawnBossUnit()
    {
        var unit = Resources.Load("Prefabs/BossEnemy");
        if(unit == null)
        {
            Debug.Log("보스 유닛이 없습니다.");
            return;
        }

        var spawnPointX = _mapManager.GetEnemySpawnPoint().x;
        var spawnPointY = _mapManager.GetBaseCenterPoint().y;
        var unitObj = Instantiate(unit, new Vector2(spawnPointX, spawnPointY), Quaternion.identity) as GameObject;
        unitObj.transform.parent = enemySpawnObject;
        
        var enemyUnit = unitObj.GetComponent<EnemyUnit>();
        enemyUnit.Initialize(_unitManager);
    }

    private IEnumerator StartSpawnUnit()
    {
        GameManager.Instance.isEnemySpawning = true;
        SpawnBossUnit();
        
        for (int i = 0; i < 11; ++i)
        {
            SpawnEnemyUnit();
            float randNum = Random.Range(0.5f, 2.1f);

            if (i == 10)
                GameManager.Instance.isEnemySpawning = false;
            
            yield return new WaitForSeconds(randNum);
        }
        
        _spawnCoroutine = null;
    }
}