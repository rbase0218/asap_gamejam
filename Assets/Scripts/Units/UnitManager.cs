using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<UnitBase> enemyList = new List<UnitBase>();
    public List<UnitBase> playerList = new List<UnitBase>();

    private UnitData _currUnit;

    public void SaveUnit()
    {
        GameManager.Instance.ClearUnitData();
        
        foreach (var data in playerList)
        {
            _currUnit = new UnitData();

            _currUnit.unitCode = data.GetStatus().UnitCode;
            _currUnit.unitPosition = data.transform.position;
            GameManager.Instance.AddUnitData(_currUnit);
        }
    }

    public void AddPlayerUnit(UnitBase unit)
    {
        playerList.Add(unit);
    }

    public void AddEnemyUnit(UnitBase unit)
    {
        enemyList.Add(unit);
    }

    public void RemoveAllUnits()
    {
        foreach (var unit in playerList)
        {
            Destroy(unit.gameObject);
        }
        foreach (var unit in enemyList)
        {
            playerList.Remove(unit);
            Destroy(unit.gameObject);
        }
        
        playerList.Clear();
        enemyList.Clear();
    }
    
    public void RemoveEnemyUnit(UnitBase unit)
    {
        enemyList.Remove(unit);
        Destroy(unit.gameObject);
        
        UpdateEnemyUnit();
    }

    private void UpdateEnemyUnit()
    {
        if (enemyList.Count > 0 || GameManager.Instance.isEnemySpawning)
            return;
        
        GameManager.Instance.StopRound(SaveUnit);
    }
}
