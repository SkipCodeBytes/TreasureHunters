using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    [SerializeField] List<AudioClip> clipList;

    private Dictionary<string, AudioClip> clipDict;

    private void Awake()
    {
        clipDict = new Dictionary<string, AudioClip>();
        foreach (var clip in clipList)
        {
            if (clip != null && !clipDict.ContainsKey(clip.name))
            {
                clipDict.Add(clip.name, clip);
            }
        }
    }

    public AudioClip GetClip(string audioName)
    {
        if (clipDict.TryGetValue(audioName, out var clip))
            return clip;

        Debug.LogWarning($"AudioClip '{audioName}' no encontrado.");
        return null;
    }


    /*
    public AudioClip DiceResult;
    public AudioClip LosePanel;
    public AudioClip NewRound;
    public AudioClip OpenPanel;
    public AudioClip AddLife;*/
}
