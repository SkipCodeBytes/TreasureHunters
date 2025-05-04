using UnityEngine;

[System.Serializable]
public class PlayableMapArea
{
    [SerializeField] private string mapName;
    [SerializeField] private string sceneName;
    [SerializeField] private Sprite mapImage;

    public string MapName { get => mapName;}
    public string SceneName { get => sceneName;}
    public Sprite MapImage { get => mapImage;}
}
