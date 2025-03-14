using UnityEngine;


[System.Serializable]
public class UnitStatusData
{
    public int code;
    public string name;
    public UnitGrade grade;
    public int hp;
    public int def;
    public int atk;
    public float atkSpd;
    public float atkRange;
    public float movSpd;
    public GameObject bulletEff;
}
public class UnitDatabase : MonoBehaviour
{
    public UnitStatusData[] unitStatusData;
}
