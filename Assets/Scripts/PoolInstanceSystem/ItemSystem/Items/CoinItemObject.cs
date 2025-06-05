using UnityEngine;

public class CoinItemObject : ItemObject
{
    [SerializeField] private AudioClip coinClip;

    private void OnEnable()
    {
        StartCoroutine(CinematicAnimation.WaitTime(Random.Range(0f, 0.15f), () => SoundController.Instance.PlaySound(coinClip)));
    }
}
