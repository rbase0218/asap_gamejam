using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UserData
{
    public int money;
    public int roundCount;
    
    public float extraBossDamage = .0f;
    public int extraBossHealth = 0;
    public int extraBoosDefense = 0;
    
    public int extraNormalEnemyHealth = 0;
    public float extraNormalEnemyMoveSpeed = .0f;
    public float extraNormalEnemyDamage = .0f;
    
    public List<UnitData> unitDataList = new List<UnitData>();
}

[System.Serializable]
public class UnitData
{
    public int unitCode;
    public Vector2 unitPosition;
}