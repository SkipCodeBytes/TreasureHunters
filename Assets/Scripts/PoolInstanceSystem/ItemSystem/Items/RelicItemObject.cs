using UnityEngine;

public class RelicItemObject : ItemObject
{
    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    private void OnEnable()
    {
        StartCoroutine(CinematicAnimation.WaitTime(Random.Range(0.08f, 0.2f), () =>
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Relic"))));
    }
}
