using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Game/Unit")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public GameObject characterPrefab;
    public Sprite characterSprite;
    public int attackStat;
    public int defenseStat;
    public int evadeStat;
    public int lifeStat;
    public int reviveStat;
}
