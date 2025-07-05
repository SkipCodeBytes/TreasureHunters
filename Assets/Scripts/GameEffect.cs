using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effect")]
public class GameEffect : ScriptableObject
{
    public string EffectName;
    public string TargetMomentName;
    public EventMoment EventMoment;
    public EventMomentAction EventMomentAction;
}
