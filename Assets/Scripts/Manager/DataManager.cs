using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private float gold = .0f;

    public void SetGold(float value)
    {
        gold = value;
    }
    
    public float GetGold() => gold;
}
