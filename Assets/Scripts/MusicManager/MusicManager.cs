using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicList = new List<AudioClip>();
    private AudioSource _audioSource;
    [SerializeField] private float fadeDuration = 1.0f;

    private int currentTrackIndex = 0;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (musicList.Count > 0 && _audioSource != null)
        {
            _audioSource.clip = musicList[currentTrackIndex];
            _audioSource.Play();
        }
    }

    public void NextMusic()
    {
        if (musicList.Count <= 0 || _audioSource == null) return;

        currentTrackIndex++;
        if(currentTrackIndex >= musicList.Count) currentTrackIndex = 0;
        StartCoroutine(TransitionToNextTrack());
    }

    private IEnumerator TransitionToNextTrack()
    {
        // Fade out
        float startVolume = _audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        _audioSource.volume = 0f;
        _audioSource.Stop();

        _audioSource.clip = musicList[currentTrackIndex];
        _audioSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            _audioSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        _audioSource.volume = startVolume;
    }
}