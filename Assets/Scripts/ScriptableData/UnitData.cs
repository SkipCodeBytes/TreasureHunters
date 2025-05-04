using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class UnitData : ScriptableObject
{
    public string characterName;
    public GameObject characterPrefab;
    public int lifeStat;
    public int attackStat;
    public int defenseStat;
    public int evadeStat;
    public int reviveStat;
}
