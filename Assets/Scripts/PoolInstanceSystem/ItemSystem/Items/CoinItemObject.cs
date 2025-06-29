using UnityEngine;

public class CoinItemObject : ItemObject
{
    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    private void OnEnable()
    {
        StartCoroutine(CinematicAnimation.WaitTime(Random.Range(0.02f, 0.5f), () =>
        SoundController.Instance.PlaySound(_gm.SoundLibrary.GetClip("Coin"))));
    }
}
