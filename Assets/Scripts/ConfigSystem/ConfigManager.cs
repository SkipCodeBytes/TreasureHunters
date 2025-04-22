using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigManager : MonoBehaviour
{
    [HideInInspector] public static ConfigManager Instance;

    [SerializeField] private GameConfig _myGameConfig;

    public GameConfig MyGameConfig { get => _myGameConfig; set => _myGameConfig = value; }

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        _myGameConfig = LoadData();
        _myGameConfig.ApplyConfig();
        //StartCoroutine(setConfig(0.5f));
    }

    IEnumerator setConfig(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _myGameConfig.ApplyConfig();
    }

    public void SaveData()
    {
        Debug.Log("DatosGuardados");
        string json = JsonUtility.ToJson(_myGameConfig);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/datos.json", json);
    }

    private GameConfig LoadData()
    {
        string ruta = Application.persistentDataPath + "/datos.json";
        if (System.IO.File.Exists(ruta))
        {
            string json = System.IO.File.ReadAllText(ruta);
            return JsonUtility.FromJson<GameConfig>(json);
        } else
        {
            GameConfig gameConfig = new GameConfig();
            gameConfig.SetDefaultValues();
            return gameConfig;
        }
    }

    private void SetDefaultConfig()
    {
        _myGameConfig.SetDefaultValues();
        _myGameConfig.ApplyConfig();
        SaveData();
    }

    private void DebugConfig()
    {
        _myGameConfig.DebugValues();
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EventManager.StartListening("MSJ_DefaultConfig", SetDefaultConfig);
        EventManager.StartListening("MSJ_PrintConfig", DebugConfig);
    }

}
